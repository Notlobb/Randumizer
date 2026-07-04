using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace FaxanaduRando.Randomizer
{
    public class MusicTrackRandomizer
    {
        // high level constants for music data
        private const int MUSIC_BANK = 5;
        private const ushort TRACK_POINTER_TABLE = 0x8efb;
        private const ushort TRACK_COUNT = 16;
        // music engine opcodes that ends instruction stream
        private static readonly HashSet<byte> OpcodesEnding = [0xf4, 0xf5, 0xfe, 0xff];
        // opcodes that take a byte parameter
        private static readonly HashSet<byte> OpcodesParam = [0xee, 0xef, 0xf0, 0xf1, 0xf2, 0xf3, 0xf6, 0xf7, 0xfb, 0xfd];
        // JSR opcode
        private const byte OpcodeJSR = 0xf8;

        public static void RandomizeMusicTracks(byte[] rom, bool includeOriginal, bool chaosMode)
        {
            // TODO: pull in embedded modules and add actual randomization logic
            var mods = ExtractVanillaMusic(rom);
            WriteModulesToRom(rom, mods);
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

        private static List<MusicModule> ExtractVanillaMusic(byte[] rom)
        {
            List<MusicModule> modules = [];

            int ptr = Section.GetOffset(MUSIC_BANK, TRACK_POINTER_TABLE, 0x8000);

            for (int i = 0; i < TRACK_COUNT; i++)
            {
                var mod = DisasmTrack(rom, ptr);
                mod.Description = "Ripped from ROM";
                mod.AllowedSlots.Add(i + 1);
                modules.Add(mod);
                ptr += 8; // four 16-bit channel pointers
            }

            return modules;
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
                    AllowedSlots = [.. module.AllowedSlots],
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
                    AllowedSlots = [.. AllowedSlots]
                };

                for (int i = 0; i < 4; i++)
                    module.Channels[i] = Channels[i].ToChannel();

                return module;
            }
        }

    }
}
