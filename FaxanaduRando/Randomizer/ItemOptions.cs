using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Data.Converters;

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

        public enum WeaponStatRandomization
        {
            Unchanged,
            Swap,
            Subtle,
            Mild,
            Moderate,
            Strong,
            Extreme,
        };

        public enum MagicStatRandomization
        {
            Unchanged,
            Swap,
            Subtle,
            Mild,
            Moderate,
            Strong,
            Extreme,
        };

        public enum ArmorStatRandomization
        {
            Unchanged,
            Swap,
            PlusMinus1,
            PlusMinus2,
        };

        [Flag(16)]
        public static bool GuaranteeElixirNearFortress { get; set; } = true;

        [Flag(17)]
        public static bool FixPendantBug { get; set; } = true;

        [Flag(18)]
        public static bool BuffGloves { get; set; } = true;

        [Flag(19)]
        public static bool BuffHourglass { get; set; } = true;

        [Flag(FlagsCodec.NonBoolFlagBase + 41)]
        public static WingBootDurations WingbootDurationSetting { get; set; } = WingBootDurations.ScalesUpFrom40;

        [Flag(FlagsCodec.NonBoolFlagBase + 42)]
        [FlagMaxValue(2)]
        public static int ShieldSetting { get; set; } = 1;

        [Flag(FlagsCodec.NonBoolFlagBase + 40)]
        public static StartingWeaponOptions StartingWeapon { get; set; } = StartingWeaponOptions.LongSword;

        [Flag(20)]
        public static bool RandomizeBarRank { get; set; } = true;

        [Flag(22)]
        public static bool GuaranteeMattock { get; set; } = true;

        [Flag(21)]
        public static bool GuaranteeStartingSpell { get; set; } = true;

        [Flag(23)]
        public static bool ReplacePoison { get; set; } = true;

        [Flag(FlagsCodec.NonBoolFlagBase + 39)]
        public static MattockUsages MattockUsage { get; set; } = MattockUsages.AnywhereExceptBannedScreens;

        [Flag(24)]
        public static bool AlwaysSpawnSmallItems { get; set; } = true;

        [Flag(25)]
        public static bool RandomizeItemNames { get; set; } = false;

        [Flag(30)]
        public static bool IncludeSomeEolisDoors { get; set; } = false;

        [Flag(FlagsCodec.NonBoolFlagBase + 43)]
        public static BigItemSpawning BigItemSpawns { get; set; } = BigItemSpawning.AlwaysSpawn;

        [Flag(FlagsCodec.NonBoolFlagBase + 44)]
        public static ItemShuffle ShuffleItems { get; set; } = ItemShuffle.MixOnlyShopsAndGifts;

        [Flag(FlagsCodec.NonBoolFlagBase + 45)]
        public static KeyRandomization RandomizeKeys { get; set; } = KeyRandomization.Unchanged;

        [Flag(FlagsCodec.NonBoolFlagBase + 47)]
        public static MultipleGiftOptions MultipleGifts { get; set; } = MultipleGiftOptions.AllGurusExceptConflateAndEolis;

        [Flag(FlagsCodec.NonBoolFlagBase + 50)]
        public static KeyLimit SmallKeyLimit { get; set; } = KeyLimit.NoLimit;

        [Flag(FlagsCodec.NonBoolFlagBase + 53)]
        public static KeyLimit BigKeyLimit { get; set; } = KeyLimit.NoLimit;

        [Flag(FlagsCodec.NonBoolFlagBase + 54)]
        public static WeaponStatRandomization WeaponStatSetting { get; set; } = WeaponStatRandomization.Unchanged;

        [Flag(FlagsCodec.NonBoolFlagBase + 55)]
        public static MagicStatRandomization MagicStatSetting { get; set; } = MagicStatRandomization.Unchanged;

        [Flag(FlagsCodec.NonBoolFlagBase + 56)]
        public static ArmorStatRandomization ArmorStatSetting { get; set; } = ArmorStatRandomization.Unchanged;
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

    public class WeaponStatSettings : ObservableCollection<string>
    {
        public WeaponStatSettings()
        {
            Add("Unchanged");
            Add("Swap");
            Add("Subtle");
            Add("Mild");
            Add("Moderate");
            Add("Strong");
            Add("Extreme");
        }
    }

    public class MagicStatSettings : ObservableCollection<string>
    {
        public MagicStatSettings()
        {
            Add("Unchanged");
            Add("Swap");
            Add("Subtle");
            Add("Mild");
            Add("Moderate");
            Add("Strong");
            Add("Extreme");
        }
    }

    public class ArmorStatSettings : ObservableCollection<string>
    {
        public ArmorStatSettings()
        {
            Add("Unchanged");
            Add("Swap");
            Add("+/- 1");
            Add("+/- 2");
        }
    }

    public class WeaponStatConverter : EnumConverter<ItemOptions.WeaponStatRandomization>
    {
    }

    public class MagicStatConverter : EnumConverter<ItemOptions.MagicStatRandomization>
    {
    }

    public class ArmorStatConverter : EnumConverter<ItemOptions.ArmorStatRandomization>
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
