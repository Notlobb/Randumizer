using System;

namespace FaxanaduRando.RegressionTests
{
    internal static class Program
    {
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
            Console.WriteLine($"Generate dataset using '{romFile}'");

            return 0;
        }

        private static int Verify(string romFile)
        {
            Console.WriteLine($"Verify dataset using '{romFile}'");
            return 0;
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
            Console.WriteLine("  RegressionTests generate <rom.nes>");
            Console.WriteLine("  RegressionTests verify  <rom.nes>");
        }
    }
}
