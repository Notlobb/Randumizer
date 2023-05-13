using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace FaxanaduRando.Randomizer
{
    public class ItemOptions
    {
        public enum WingBootDurations
        {
            Random,
            Always40,
            Always20,
            ScalesUpFrom40,
            ScalesUpFrom30,
            ScalesUpFrom20,
            ScalesUpFrom10,
            Permanent,
            Unchanged,
        };

        public enum StartingWeaponOptions
        {
            Random,
            Dagger,
            LongSword,
            GuaranteeOnlyWithNoSpells,
            NoGuaranteed,
        };

        public enum MattockUsages
        {
            AnywhereExceptBannedScreens,
            AnywhereExceptBannedScreensAllowMattockLockedItems,
            Anywhere,
            AnywhereUpdateLogic,
            Unchanged,
        };

        public enum BigItemSpawning
        {
            AlwaysSpawn,
            AlwaysLockBehindBosses,
            Unchanged,
        };

        public enum ItemShuffle
        {
            Mixed,
            MixOnlyShopsAndGifts,
            NoMixed,
            Unchanged,
        };

        public enum KeyRandomization
        {
            Randomized,
            Shuffled,
            Unchanged,
        };

        public enum MultipleGiftOptions
        {
            AllGurusExceptConflate,
            AllGurusExceptConflateAndEolis,
            None,
        };

        public enum KeyLimit
        {
            Zero,
            One,
            Two,
            Three,
            NoLimit
        };

        public static bool GuaranteeElixirNearFortress { get; set; } = true;
        public static bool FixPendantBug { get; set; } = true;
        public static bool BuffGloves { get; set; } = true;
        public static bool BuffHourglass { get; set; } = true;
        public static WingBootDurations WingbootDurationSetting { get; set; } = WingBootDurations.ScalesUpFrom40;
        public static int ShieldSetting { get; set; } = 1;
        public static StartingWeaponOptions StartingWeapon { get; set; } = StartingWeaponOptions.LongSword;
        public static bool RandomizeBarRank { get; set; } = true;
        public static bool GuaranteeMattock { get; set; } = true;
        public static bool GuaranteeStartingSpell { get; set; } = true;
        public static bool ReplacePoison { get; set; } = true;
        public static MattockUsages MattockUsage { get; set; } = MattockUsages.AnywhereExceptBannedScreens;
        public static bool AlwaysSpawnSmallItems { get; set; } = true;
        public static bool IncludeSomeEolisDoors { get; set; } = false;
        public static BigItemSpawning BigItemSpawns { get; set; } = BigItemSpawning.AlwaysSpawn;
        public static ItemShuffle ShuffleItems { get; set; } = ItemShuffle.MixOnlyShopsAndGifts;
        public static KeyRandomization RandomizeKeys { get; set; } = KeyRandomization.Unchanged;
        public static MultipleGiftOptions MultipleGifts { get; set; } = MultipleGiftOptions.AllGurusExceptConflateAndEolis;
        public static KeyLimit SmallKeyLimit { get; set; } = KeyLimit.NoLimit;
        public static KeyLimit BigKeyLimit { get; set; } = KeyLimit.NoLimit;
    }

    public class WingBootSettings : ObservableCollection<string>
    {
        public WingBootSettings()
        {
            Add("Random");
            Add("Always 40");
            Add("Always 20");
            Add("40, scales up with rank");
            Add("30, scales up with rank");
            Add("20, scales up with rank");
            Add("10, scales up with rank");
            Add("Permanent");
            Add("Unchanged");
        }
    }

    public class StartingWeaponSettings : ObservableCollection<string>
    {
        public StartingWeaponSettings()
        {
            Add("Random");
            Add("Dagger");
            Add("Longsword");
            Add("Guarantee only if Eolis has no spells");
            Add("No guaranteed");
        }
    }

    public class MattockUsageSettings : ObservableCollection<string>
    {
        public MattockUsageSettings()
        {
            Add("Anywhere except banned screens (Trunk exit or Branch double item screen)");
            Add("Anywhere except banned screens, allow mattock-locked items");
            Add("Anywhere, allow mattock-locked items, spring quest will still be considered required");
            Add("Anywhere, allow mattock-locked items, spring quest won't be considered required");
            Add("Unchanged");
        }
    }

    public class ShieldSettings : ObservableCollection<string>
    {
        public ShieldSettings()
        {
            Add("Shields work with ointment");
            Add("Shields work with ointment + shields are stronger");
            Add("Unchanged");
        }
    }

    public class BigItemSpawnsSettings : ObservableCollection<string>
    {
        public BigItemSpawnsSettings()
        {
            Add("Always spawn");
            Add("Always lock behind bosses");
            Add("Unchanged");
        }
    }

    public class ShuffleItemsSettings : ObservableCollection<string>
    {
        public ShuffleItemsSettings()
        {
            Add("Shuffle, mix item types");
            Add("Shuffle, mix only shop and gift items");
            Add("Shuffle, don't mix most gifts with shops");
            Add("Unchanged");
        }
    }

    public class RandomizeKeysSettings : ObservableCollection<string>
    {
        public RandomizeKeysSettings()
        {
            Add("Randomize");
            Add("Shuffle");
            Add("Unchanged");
        }
    }

    public class MultipleGiftSettings : ObservableCollection<string>
    {
        public MultipleGiftSettings()
        {
            Add("All gift locations except Conflate guru");
            Add("All gift locations except Conflate and Eolis gurus");
            Add("None");
        }
    }

    public class KeyLimitSettings : ObservableCollection<string>
    {
        public KeyLimitSettings()
        {
            Add("Max 0");
            Add("Max 1");
            Add("Max 2");
            Add("Max 3");
            Add("No limit");
        }
    }

    public class StartingWeaponConverter : EnumConverter<ItemOptions.StartingWeaponOptions>
    {
    }

    public class MattockUsageConverter : EnumConverter<ItemOptions.MattockUsages>
    {
    }

    public class WingBootSettingsConverter : EnumConverter<ItemOptions.WingBootDurations>
    {
    }

    public class BigItemSpawnsSettingsConverter : EnumConverter<ItemOptions.BigItemSpawning>
    {
    }

    public class ShuffleItemsConverter : EnumConverter<ItemOptions.ItemShuffle>
    {
    }

    public class RandomizeKeysConverter : EnumConverter<ItemOptions.KeyRandomization>
    {
    }

    public class MultipleGiftsConverter : EnumConverter<ItemOptions.MultipleGiftOptions>
    {
    }

    public class KeyLimitConverter : EnumConverter<ItemOptions.KeyLimit>
    {
    }

    public class EnumConverter<T> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (T)value;
        }
    }
}
