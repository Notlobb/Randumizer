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

        public static Dictionary<T1, T2> Reverse<T1, T2>(Dictionary<T2, T1> dictionary)
        {
            var reverse = new Dictionary<T1, T2>();
            foreach (var key in dictionary.Keys)
            {
                reverse.Add(dictionary[key], key);
            }

            return reverse;
        }
    }
}
