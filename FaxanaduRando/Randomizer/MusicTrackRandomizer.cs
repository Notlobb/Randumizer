using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Reflection;

namespace FaxanaduRando.Randomizer
{
    public class MusicTrackRandomizer
    {
        // high level constants for music data
        private const int MUSIC_BANK = 5;
        private const ushort TRACK_POINTER_TABLE = 0x8efb;
        private const int TRACK_COUNT = 16;
        // music engine opcodes that ends instruction stream
        private static readonly HashSet<byte> OpcodesEnding = [0xf4, 0xf5, 0xfe, 0xff];
        // opcodes that take a byte parameter
        private static readonly HashSet<byte> OpcodesParam = [0xee, 0xef, 0xf0, 0xf1, 0xf2, 0xf3, 0xf6, 0xf7, 0xfb, 0xfd];
        // JSR opcode
        private const byte OpcodeJSR = 0xf8;

        // publicly available info on which music was in fact assigned to each slot
        public static IReadOnlyList<string> AssignedMusic { get; private set; } = Array.Empty<string>();
        // publicly available info on how many times the recursion was called (debug only)
#if DEBUG
        public static long solveCalls { get; private set; } = 0;
#endif

        private static readonly string[] VanillaTrackNames = [
            "Intro",
            "Land of Dwarf (Dartmoor Castle)",
            "Trunk",
            "Branches",
            "Mist",
            "Towers",
            "Eolis",
            "Mantra/Death",
            "Towns",
            "Boss Music",
            "Hour Glass",
            "Outro",
            "King",
            "Guru",
            "Shops/House",
            "Zenis (Evil Lair)"
        ];

        // Fallback metadata for the original Faxanadu soundtrack
        // Tracks extracted directly from the ROM do not carry JSON metadata, so we
        // conservatively allow each vanilla track to replace only its original slot
        // This table can be expanded later as suitable replacements are identified
        private static readonly int[][] VanillaAllowedSlots = [
            [1],  // Intro
            [2],  // Land of Dwarf (Dartmoor Castle)
            [3],  // Trunk
            [4],  // Branches
            [5],  // Mist
            [6],  // Towers
            [7],  // Eolis
            [8],  // Mantra/Death
            [9],  // Towns
            [10],  // Boss Music
            [11], // Hour Glass
            [12], // Outro
            [13], // King
            [14], // Guru
            [15], // Shops/House
            [16], // Zenis (Evil Lair)
        ];

        public static void RandomizeMusicTracks(byte[] rom, Random random, bool includeOriginal)
        {
            var mods = LoadEmbeddedMusicModules();
            if (includeOriginal)
            {
                mods.AddRange(ExtractVanillaMusic(rom));
            }

            // precompute sizes
            List<int> moduleSizes = mods.Select(module => module.Size()).ToList();

            // build up the total list of candidates per slot
            List<List<int>> candidates = [];

            for (int slot = 0; slot < TRACK_COUNT; slot++)
            {
                candidates.Add([]);

                for (int mod = 0; mod < mods.Count; mod++)
                {
                    if (mods[mod].AllowedSlots.Contains(slot))
                    {
                        candidates[slot].Add(mod);
                    }
                }
            }

            // randomize the order of each candidate list - this is the only randomization we need
            foreach (List<int> list in candidates)
            {
                for (int i = list.Count - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    (list[i], list[j]) = (list[j], list[i]);
                }
            }

            int bankLimit = 0xc000 - (TRACK_POINTER_TABLE + TRACK_COUNT * 8);

            // sanity check
#if DEBUG
            int optimisticMinimumSize = 0;

            for (int slot = 0; slot < TRACK_COUNT; slot++)
            {
                if (candidates[slot].Count == 0)
                    throw new InvalidOperationException($"No music modules can be placed in slot {slot}.");

                optimisticMinimumSize += candidates[slot]
                    .Select(mod => moduleSizes[mod])
                    .Min();
            }

            if (optimisticMinimumSize > bankLimit)
            {
                throw new InvalidOperationException(
                    $"Music library cannot possibly fit in bank 5. " +
                    $"Optimistic minimum size is {optimisticMinimumSize} bytes, " +
                    $"but only {bankLimit} bytes are available");
            }
#endif

            // recurse on most to least contrained slot index for less backtracking
            // TODO: Future optimization
            // Recompute the "most constrained remaining slot" after each assignment
            // in the recursion, instead of using a fixed slot order
            var slotOrder = Enumerable.Range(0, TRACK_COUNT)
                .OrderBy(slot => candidates[slot].Count)
                .ToList();

            // which modules have already been used
            bool[] usedModules = new bool[mods.Count];
            // which module a certain slot uses (-1 means unassigned)
            int[] chosenModules = Enumerable.Repeat(-1, TRACK_COUNT).ToArray();

#if DEBUG
            solveCalls = 0;
#endif

            // local recursive function, captures all outside variables defined above
            // "depth" here means index in slotOrder (most to least constrained)
            bool Solve(int depth, int currentSize)
            {
#if DEBUG
                ++solveCalls;
#endif

                // every slot has been assigned - success!
                if (depth == TRACK_COUNT)
                    return true;

                int slot = slotOrder[depth];

                foreach (int module in candidates[slot])
                {
                    if (usedModules[module])
                        continue;

                    int newSize = currentSize + moduleSizes[module];
                    if (newSize > bankLimit)
                        continue;

                    // TODO: Future optimization
                    // Compute a lower bound on the minimum additional bytes required to
                    // fill the remaining slots using the remaining unused modules
                    // If newSize + lowerBound > bankLimit, prune this branch

                    // assign
                    usedModules[module] = true;
                    chosenModules[slot] = module;

                    if (Solve(depth + 1, newSize))
                        return true;

                    // unassign, this path was no good
                    usedModules[module] = false;
                    chosenModules[slot] = -1;
                }

                return false;
            }

            if (!Solve(0, 0))
            {
                throw new InvalidOperationException("Unable to build up music tracks within bank 5 free space");
            }

            // record which tracks were in fact chosen
            AssignedMusic = chosenModules
                .Select(index => mods[index].Description)
                .ToArray();

            WriteModulesToRom(rom, chosenModules.Select(i => mods[i]).ToList());
        }

        private static void WriteModulesToRom(byte[] rom, List<MusicModule> modules)
        {
            int pointerTableOffset = Section.GetOffset(MUSIC_BANK, TRACK_POINTER_TABLE, 0x8000);
            int writeOffset = pointerTableOffset + TRACK_COUNT * 8;

            int ptr = pointerTableOffset;

            foreach (MusicModule module in modules)
            {
                foreach (MusicChannel channel in module.Channels)
                {
                    ushort cpuAddress = GetCpuAddress(writeOffset);

                    // Write channel pointer.
                    rom[ptr++] = (byte)cpuAddress;
                    rom[ptr++] = (byte)(cpuAddress >> 8);

                    // Relocate and emit channel.
                    byte[] bytes = channel.ToBytes(cpuAddress);

                    Array.Copy(bytes, 0, rom, writeOffset, bytes.Length);

                    writeOffset += bytes.Length;
                }
            }
        }

        private static ushort GetCpuAddress(int fileOffset)
        {
            int bankOffset = Section.GetOffset(MUSIC_BANK, 0x8000, 0x8000);
            return (ushort)(0x8000 + (fileOffset - bankOffset));
        }

        // developer function for extracting all music from a rom (input file-path)
        // to json files, which will be written to an output-folder
#if DEBUG
        public static void ExtractVanillaMusicToFiles(string romFileName, string outputDirectory)
        {
            byte[] rom = File.ReadAllBytes(romFileName);
            string baseName = Path.GetFileNameWithoutExtension(romFileName);
            Directory.CreateDirectory(outputDirectory);
            List<MusicModule> modules = ExtractVanillaMusic(rom);
            JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };

            for (int i = 0; i < modules.Count; i++)
            {
                MusicModuleDto dto = MusicModuleDto.FromModule(modules[i]);

                string fileName = Path.Combine(
                    outputDirectory,
                    $"{baseName}-track{i + 1:D2}.json"); // 1-indexed for convention

                File.WriteAllText(
                    fileName,
                    JsonSerializer.Serialize(dto, options));
            }
        }
#endif

        // load all music already present in ROM as modules
        private static List<MusicModule> ExtractVanillaMusic(byte[] rom)
        {
            List<MusicModule> modules = [];

            int ptr = Section.GetOffset(MUSIC_BANK, TRACK_POINTER_TABLE, 0x8000);

            for (int i = 0; i < GetTrackCount(rom); i++)
            {
                var mod = DisasmTrack(rom, ptr);
                if (i < VanillaTrackNames.Length)
                {
                    mod.Description = $"Ripped from ROM (Track {i + 1}, vanilla slot: {VanillaTrackNames[i]})";
                    mod.AllowedSlots.AddRange(VanillaAllowedSlots[i].Select(slot => slot - 1));
                }
                else
                {
                    mod.Description = $"Ripped from ROM (Track {i + 1})";
                    mod.AllowedSlots.Add(i); // unknown track: keep it in its own slot for now
                }
                modules.Add(mod);
                ptr += 8; // four 16-bit channel pointers
            }

            return modules;
        }

        // load all embedded music module files
        private static List<MusicModule> LoadEmbeddedMusicModules()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string prefix = assembly.GetName().Name + ".Resources.MusicModules.";
            List<MusicModule> modules = [];
            foreach (string resource in assembly.GetManifestResourceNames()
                .Where(r => r.StartsWith(prefix) && r.EndsWith(".json")))
            {
                using Stream stream = assembly.GetManifestResourceStream(resource)!;
                using StreamReader reader = new(stream);

                MusicModuleDto dto = JsonSerializer.Deserialize<MusicModuleDto>(
                    reader.ReadToEnd())!;

                modules.Add(dto.ToModule());
            }
            return modules;
        }

        private static int GetTrackCount(byte[] rom)
        {
            int pointerTableOffset = Section.GetOffset(MUSIC_BANK, TRACK_POINTER_TABLE, 0x8000);

            int result = 0;
            int lowestTarget = 0x10000;

            for (int i = 0; ; i += 2)
            {
                int nextPtrCpu = TRACK_POINTER_TABLE + i + 2;

                // vanilla Faxanadu has one unused byte between the pointer table
                // and the music data, custom roms may not - so handle both cases
                if (nextPtrCpu > lowestTarget + 1)
                    break;

                result++;

                ushort ptr = BitConverter.ToUInt16(rom, pointerTableOffset + i);
                lowestTarget = Math.Min(lowestTarget, ptr);
            }

            return result / 4;
        }

        private static MusicModule DisasmTrack(byte[] rom, int pointerTableFileOffset)
        {
            MusicModule track = new();

            for (int i = 0; i < 4; i++)
            {
                ushort cpuAddress = BitConverter.ToUInt16(rom, pointerTableFileOffset + i * 2);
                int fileOffset = Section.GetOffset(MUSIC_BANK, cpuAddress, 0x8000);

                track.Channels[i] = DisasmChannel(rom, fileOffset);
            }

            return track;
        }

        private static MusicChannel DisasmChannel(byte[] rom, int channelFileOffset)
        {
            var romToInstr = DisasmInstructions(rom, channelFileOffset);
            Dictionary<int, int> romToNormalized = [];
            int normalizedOffset = 0;

            foreach (var kv in romToInstr.OrderBy(x => x.Key))
            {
                romToNormalized.Add(kv.Key, normalizedOffset);
                normalizedOffset += kv.Value.Size;
            }

            MusicChannel chan = new();

            foreach (var kv in romToInstr.OrderBy(x => x.Key))
            {
                Instruction instruction = kv.Value;
                chan.Code.Add(instruction.Opcode);
                if (instruction.ByteArg.HasValue)
                {
                    chan.Code.Add(instruction.ByteArg.Value);
                }
                else if (instruction.JumpTarget.HasValue)
                {
                    chan.Relocations.Add(chan.Code.Count);
                    int targetFile = Section.GetOffset(MUSIC_BANK, instruction.JumpTarget.Value, 0x8000);
                    ushort normalizedTarget = (ushort)romToNormalized[targetFile];
                    chan.Code.Add((byte)normalizedTarget);
                    chan.Code.Add((byte)(normalizedTarget >> 8));
                }
            }

            return chan;
        }

        private class MusicChannel
        {
            public List<byte> Code = [];
            public List<int> Relocations = [];

            // returns the channel as bytecode from a given cpu-address
            // caller needs to construct the ptr table
            public byte[] ToBytes(ushort cpuAddress)
            {
                byte[] bytes = [.. Code];

                foreach (int relocation in Relocations)
                {
                    ushort target = BitConverter.ToUInt16(bytes, relocation);
                    target += cpuAddress;

                    bytes[relocation] = (byte)target;
                    bytes[relocation + 1] = (byte)(target >> 8);
                }

                return bytes;
            }
        }

        private class MusicModule
        {
            public string Description { get; set; } = "";
            // the music slots this track is allowed to occupy
            public List<int> AllowedSlots { get; set; } = [];

            public MusicChannel[] Channels =
            [
                new(), // sq1
                new(), // sq2
                new(), // tri
                new(), // noise
            ];

            public int Size()
            {
                int size = 0;

                foreach (MusicChannel channel in Channels)
                {
                    size += channel.Code.Count;
                }

                return size;
            }
        }

        private class Instruction
        {
            public byte Opcode { get; init; }
            // Present for opcodes taking a single byte parameter.
            public byte? ByteArg { get; set; }
            // Present only for JSR
            public ushort? JumpTarget { get; set; }
            public int Size => 1 + (ByteArg.HasValue ? 1 : 0) + (JumpTarget.HasValue ? 2 : 0);
        }

        private static Dictionary<int, Instruction> DisasmInstructions(byte[] rom, int entryOffset)
        {
            Dictionary<int, Instruction> instructions = [];
            HashSet<int> visited = [];
            Queue<int> work = [];

            work.Enqueue(entryOffset);

            while (work.Count > 0)
            {
                int offset = work.Dequeue();
                if (!visited.Add(offset))
                    continue;

                while (true)
                {
                    int instructionOffset = offset;
                    byte opcode = rom[offset++];

                    Instruction instruction = new()
                    {
                        Opcode = opcode
                    };

                    if (opcode == OpcodeJSR)
                    {
                        ushort targetCpu = BitConverter.ToUInt16(rom, offset);
                        instruction.JumpTarget = targetCpu;
                        offset += 2;

                        int targetOffset = Section.GetOffset(MUSIC_BANK, targetCpu, 0x8000);

                        if (!visited.Contains(targetOffset))
                            work.Enqueue(targetOffset);
                    }
                    else if (OpcodesParam.Contains(opcode))
                    {
                        instruction.ByteArg = rom[offset++];
                    }

                    instructions.Add(instructionOffset, instruction);

                    if (OpcodesEnding.Contains(opcode))
                        break;
                }
            }

            return instructions;
        }

        private sealed class MusicChannelDto
        {
            public byte[] Code { get; set; } = [];
            public List<int> Relocations { get; set; } = [];

            public static MusicChannelDto FromChannel(MusicChannel channel)
            {
                return new MusicChannelDto
                {
                    Code = [.. channel.Code],
                    Relocations = [.. channel.Relocations]
                };
            }

            public MusicChannel ToChannel()
            {
                return new MusicChannel
                {
                    Code = [.. Code],
                    Relocations = [.. Relocations]
                };
            }
        }

        private sealed class MusicModuleDto
        {
            public string Description { get; set; } = "";
            public List<int> AllowedSlots { get; set; } = [];
            public MusicChannelDto[] Channels { get; set; } = [];

            public static MusicModuleDto FromModule(MusicModule module)
            {
                return new MusicModuleDto
                {
                    Description = module.Description,
                    AllowedSlots = module.AllowedSlots.Select(slot => slot + 1).ToList(),
                    Channels = module.Channels
                        .Select(MusicChannelDto.FromChannel)
                        .ToArray()
                };
            }

            public MusicModule ToModule()
            {
                MusicModule module = new()
                {
                    Description = Description,
                    AllowedSlots = AllowedSlots.Select(slot => slot - 1).ToList()
                };

                for (int i = 0; i < 4; i++)
                    module.Channels[i] = Channels[i].ToChannel();

                return module;
            }
        }

    }
}
