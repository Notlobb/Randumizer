using FaxanaduRando.Randomizer;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FaxanaduRando.RegressionTests
{
    internal static class Program
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

#if DEBUG
        private static readonly string DatasetFileName = "dataset-debug.json";
#else
        private static readonly string DatasetFileName = "dataset-release.json";
#endif

        // implemented in new api
        public class RandomizationException : Exception { public RandomizationException(string message) : base(message) { } }
        // implemented in new api
        public readonly record struct RandomizationResult(
            byte[] Rom,
            List<string> SpoilerLog,
            string FileNameSuffix);

        private static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return 1;
            }

            string command = args[0].ToLowerInvariant();
            string romFile = args[1];
            int count = args.Length >= 3 ? int.Parse(args[2]) : 100;
            int metaSeed = args.Length >= 4 ? int.Parse(args[3]) : 0;

            return command switch
            {
                "generate" => Generate(romFile, count, metaSeed),
                "verify" => Verify(romFile),
                _ => InvalidCommand(command)
            };
        }

        private static int Generate(string romFile, int count, int metaSeed)
        {
            Console.WriteLine($"Generating dataset using '{romFile}'");

            byte[] inputRom = File.ReadAllBytes(romFile);

            var customTextFiles = Directory
                .GetFiles(Path.GetDirectoryName(romFile)!, "*.txt")
                .OrderBy(Path.GetFileName, StringComparer.Ordinal)
                .ToArray();

            var dataset = new RegressionDataset
            {
                Date = DateTime.UtcNow,
                InputRomSha256 = Sha256(inputRom),
                MetaSeed = metaSeed
            };

            var metaRandom = new Random(metaSeed);

            for (int i = 0; i < count; ++i)
            {

                if (i == 0 || (i + 1) % 10 == 0 || i + 1 == count)
                {
                    Console.WriteLine($"[{i + 1}/{count}]");
                }

                bool useCustomText = customTextFiles.Length > 0 && metaRandom.Next(4) == 0;

                var test = new RegressionTest
                {
                    Seed = metaRandom.Next(),
                    Flags = FlagsCodec.Serialize(
                        FlagsCodec.GenerateRandom(metaRandom, useCustomText)
                        ),
                    CustomTextFile = useCustomText
                        ? Path.GetFileName(customTextFiles[metaRandom.Next(customTextFiles.Length)])
                        : null,
                    RandomizePalettes = metaRandom.Next(2) == 0,
                    RandomizeSounds = metaRandom.Next(2) == 0,
                    AppendSuffix = metaRandom.Next(2) == 0,
                    MusicSetting = RandomEnum<Music>(metaRandom),
                    SoundtrackSetting = RandomEnum<Soundtrack>(metaRandom),
                };

                dataset.Tests.Add(test);

                try
                {
                    ApplySettings(test);
                    var randomResult = RandomizeWrapper(inputRom,
                        ReadCustomText(romFile, test),
                        test.Flags, test.Seed);
                    test.OutputRomSha256 = Sha256(randomResult.Rom);
                    if (randomResult.SpoilerLog.Count > 0)
                    {
                        test.OutputSpoilerLogSha256 = Sha256SpoilerLog(randomResult.SpoilerLog);
                    }
                    if (!string.IsNullOrEmpty(randomResult.FileNameSuffix))
                    {
                        test.OutputFileSuffix = randomResult.FileNameSuffix;
                    }
                }
                catch (Exception ex)
                {
                    test.OutputError = ex.Message;
                }
            }

            string datasetFile = Path.Combine(
                Path.GetDirectoryName(romFile)!,
                DatasetFileName);

            string json = JsonSerializer.Serialize(dataset, JsonOptions);
            File.WriteAllText(datasetFile, json);

            Console.WriteLine($"Generated {dataset.Tests.Count} regression tests.");
            return 0;
        }

        public static RandomizationResult RandomizeWrapper(
            byte[] inputFileContents,
            string[] customTextFileContents,
            string flags,
            int seed)
        {
            string workDir = Path.Combine(Path.GetTempPath(),
                "FaxanaduRando.RegressionTests",
                Guid.NewGuid().ToString("N"));

            Directory.CreateDirectory(workDir);

            try
            {
                string inputRom = Path.Combine(workDir, "input.nes");
                File.WriteAllBytes(inputRom, inputFileContents);

                string? customText = null;
                if (customTextFileContents.Length > 0)
                {
                    customText = Path.Combine(workDir, "custom.txt");
                    File.WriteAllLines(customText, customTextFileContents);
                }

                var randomizer = new FaxanaduRando.Randomizer.Randomizer();
                if (!randomizer.Randomize(inputRom, customText, flags, seed, out string message))
                {
                    throw new RandomizationException(message);
                }

                var outputRom = Directory.GetFiles(workDir, "*.nes")
                    .Single(f => Path.GetFileName(f) != "input.nes");

                string? spoilerLog = Directory
                    .GetFiles(workDir, "input*.txt")
                    .SingleOrDefault();

                string outputName = Path.GetFileNameWithoutExtension(outputRom);

#if DEBUG
                // input_12345_foobar -> _foobar
                string prefix = $"input_{seed}";
#else
                // input_12345_flags_foobar -> _foobar
                string prefix = $"input_{seed}_{flags}";
#endif

                string fileNameSuffix = outputName.StartsWith(prefix)
                    ? outputName.Substring(prefix.Length)
                    : "";

                return new RandomizationResult
                {
                    Rom = File.ReadAllBytes(outputRom),
                    SpoilerLog = spoilerLog != null
                        ? File.ReadAllLines(spoilerLog).ToList()
                        : new List<string>(),
                    FileNameSuffix = fileNameSuffix
                };
            }
            finally
            {
                Directory.Delete(workDir, true);
            }
        }

        private static int Verify(string romFile)
        {
            Console.WriteLine($"Verifying dataset using '{romFile}'");

            byte[] inputRom = File.ReadAllBytes(romFile);

            string datasetFile = Path.Combine(
                Path.GetDirectoryName(romFile)!,
                DatasetFileName);

            var dataset = JsonSerializer.Deserialize<RegressionDataset>(
                File.ReadAllText(datasetFile),
                JsonOptions)!;

            if (Sha256(inputRom) != dataset.InputRomSha256)
                throw new InvalidOperationException("Input ROM does not match dataset.");

            int passed = 0;
            int processed = 0;

            foreach (var test in dataset.Tests)
            {
                ++processed;

                if (processed == 1 ||
                    processed % 10 == 0 ||
                    processed == dataset.Tests.Count)
                {
                    Console.WriteLine($"[{processed}/{dataset.Tests.Count}]");
                }

                ApplySettings(test);

                try
                {
                    var result = RandomizeWrapper(
                        inputRom,
                        ReadCustomText(romFile, test),
                        test.Flags,
                        test.Seed);

                    // compare ROM hash
                    if (Sha256(result.Rom) != test.OutputRomSha256)
                        throw new Exception("ROM hash mismatch.");

                    // compare spoiler log
                    string? spoilerHash = result.SpoilerLog.Count == 0
                        ? null
                        : Sha256SpoilerLog(result.SpoilerLog);

                    if (spoilerHash != test.OutputSpoilerLogSha256)
                        throw new Exception("Spoiler log mismatch.");

                    // compare suffix
                    if ((string.IsNullOrEmpty(result.FileNameSuffix) ? null : result.FileNameSuffix)
                        != test.OutputFileSuffix)
                    {
                        throw new Exception("Filename suffix mismatch.");
                    }

                    // expected success?
                    if (test.OutputError != null)
                        throw new Exception("Expected randomization to fail.");

                    ++passed;
                }
                catch (Exception ex)
                {
                    if (test.OutputError == null || ex.Message != test.OutputError)
                    {
                        Console.WriteLine($"Seed {test.Seed}: FAILED");
                        Console.WriteLine($"Expected: {test.OutputError ?? "<success>"}");
                        Console.WriteLine($"Actual:   {ex.Message}");
                    }
                    else
                    {
                        ++passed;
                    }
                }
            }

            Console.WriteLine($"{passed}/{dataset.Tests.Count} tests passed.");
            return passed == dataset.Tests.Count ? 0 : 1;
        }

        private static int InvalidCommand(string command)
        {
            Console.Error.WriteLine($"Unknown command '{command}'.");
            PrintUsage();
            return 1;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Faxanadu Regression Test Suite");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  RegressionTests generate <rom.nes> [count] [meta-seed]");
            Console.WriteLine("  RegressionTests verify  <rom.nes>");
        }

        public sealed class RegressionDataset
        {
            public DateTime Date { get; set; }

            // Ensures the correct base ROM is being used.
            public required string InputRomSha256 { get; set; }

            // Seed used to generate the regression dataset itself.
            public int MetaSeed { get; set; }

            public List<RegressionTest> Tests { get; set; } = new();
        }

        public sealed class RegressionTest
        {
            public int Seed { get; set; }
            public required string Flags { get; set; }
            public string? CustomTextFile { get; set; }

            // extra options not captured by the flags serialization
            public bool RandomizePalettes { get; set; }
            public bool RandomizeSounds { get; set; }
            public bool AppendSuffix { get; set; }
            public Music MusicSetting { get; set; }
            public Soundtrack SoundtrackSetting { get; set; }

            public string? OutputRomSha256 { get; set; }
            public string? OutputSpoilerLogSha256 { get; set; }
            public string? OutputFileSuffix { get; set; }
            public string? OutputError { get; set; }
        }

        private static void ApplySettings(RegressionTest test)
        {
            ExtraOptions.RandomizePalettes = test.RandomizePalettes;
            ExtraOptions.RandomizeSounds = test.RandomizeSounds;
            ExtraOptions.AppendSuffix = test.AppendSuffix;
            ExtraOptions.MusicSetting = test.MusicSetting;
            ExtraOptions.SoundtrackSetting = test.SoundtrackSetting;

            FlagsCodec.ApplySettings(FlagsCodec.Deserialize(test.Flags));
        }

        private static string[] ReadCustomText(string romFile, RegressionTest test)
        {
            if (test.CustomTextFile == null)
                return Array.Empty<string>();

            return File.ReadAllLines(
                Path.Combine(Path.GetDirectoryName(romFile)!, test.CustomTextFile));
        }

        // returns a random valid enum entry
        private static T RandomEnum<T>(Random random) where T : struct, Enum
        {
            var values = Enum.GetValues<T>();
            return values[random.Next(values.Length)];
        }

        private static string Sha256(byte[] bytes)
        {
            return Convert.ToHexString(SHA256.HashData(bytes));
        }

        private static string Sha256SpoilerLog(List<string> lines)
        {
            string text = string.Join(Environment.NewLine, lines) + Environment.NewLine;
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text)));
        }
    }
}
