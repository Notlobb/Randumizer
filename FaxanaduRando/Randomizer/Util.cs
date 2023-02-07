using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
   public class Util
    {
        public static void ShuffleList<T>(List<T> items, int startIndex, int endIndex, Random random)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                int index = random.Next(startIndex, endIndex + 1);
                var temp = items[i];
                items[i] = items[index];
                items[index] = temp;
            }
        }

        public static bool EarlyFinishPossible()
        {
            return GeneralOptions.QuickSeed && (GeneralOptions.ShuffleWorlds || (GeneralOptions.ShuffleTowers && GeneralOptions.IncludeEvilOnesFortress));
        }

        public static bool GurusShuffled()
        {
            return GeneralOptions.MiscDoorSetting == GeneralOptions.MiscDoors.ShuffleIncludeTowns ||
                GeneralOptions.MiscDoorSetting == GeneralOptions.MiscDoors.ShuffleIncludeTownsExceptKeyShops;
        }

        public static bool KeyShopsShuffled()
        {
            return GeneralOptions.MiscDoorSetting == GeneralOptions.MiscDoors.ShuffleIncludeTowns ||
                GeneralOptions.MiscDoorSetting == GeneralOptions.MiscDoors.ShuffleIncludeTownsExceptGurus;
        }

        public static bool AllCoreWorldScreensRandomized()
        {
            return GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorlds ||
                   GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorldExceptEolisAndZenis;
        }

        public static bool AllEndWorldScreensRandomized()
        {
            return GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorlds ||
                   GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.EolisAndZenisOnly;
        }

        public static Dictionary<T1, T2> Reverse<T1, T2>(Dictionary<T2, T1> dictionary)
        {
            var reverse = new Dictionary<T1, T2>();
            foreach (var key in dictionary.Keys)
            {
                reverse.Add(dictionary[key], key);
            }

            return reverse;
        }

        public static int GetPointer(byte world, byte[] content, byte bank)
        {
            int bankOffset = Section.GetOffset(bank, 0x8000, 0x8000);
            byte b1 = content[bankOffset];
            byte b2 = content[bankOffset + 1];
            int levelOffset = world * 2;
            var bytes = new byte[] { b1, b2 };
            int pointer = BitConverter.ToUInt16(bytes, 0);

            b1 = content[bankOffset + pointer + levelOffset];
            b2 = content[bankOffset + pointer + levelOffset + 1];
            bytes = new byte[] { b1, b2 };
            pointer = BitConverter.ToUInt16(bytes, 0);

            return pointer;
        }

        public static int GetPointer(byte[] content, int offset)
        {
            byte b1 = content[offset];
            byte b2 = content[offset + 1];
            var bytes = new byte[] { b1, b2 };
            var pointer = BitConverter.ToUInt16(bytes, 0);

            return pointer;
        }
    }
}
