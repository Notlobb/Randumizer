using System.Collections.ObjectModel;

namespace FaxanaduRando.Randomizer
{
    public class GeneralOptions
    {
        public enum Hints
        {
            Strong,
            Weak,
            None,
            Community,
        };

        public enum MiscDoors
        {
            ShuffleIncludeTowns,
            ShuffleIncludeTownsExceptGurus,
            ShuffleIncludeTownsExceptKeyShops,
            ShuffleIncludeTownsExceptGurusAndKeyshops,
            ShuffleExcludeTowns,
            Unchanged,
        };

        public enum DoorTypeShuffle
        {
            ShuffleMoveKeys,
            ShuffleDontMoveKeys,
            Unchanged,
        };

        public enum ScreenRandomization
        {
            AllWorlds,
            AllWorldExceptEolisAndZenis,
            EolisAndZenisOnly,
            Unchanged,
        };

        public enum SegmentShuffle
        {
            AllSegments,
            TownsOnly,
            Unchanged,
        };

        [Flag(0)]
        public static bool FastText { get; set; } = true;

        [Flag(1)]
        public static bool FastStart { get; set; } = true;

        [Flag(2)]
        public static bool DragonSlayerRequired { get; set; } = false;

        [Flag(3)]
        public static bool PendantRodRubyRequired { get; set; } = false;

        [Flag(4)]
        public static bool MoveSpringQuestRequirement { get; set; } = false;

        [Flag(5)]
        public static bool ShuffleTowers { get; set; } = false;

        [Flag(6)]
        public static bool ShuffleWorlds { get; set; } = false;

        [Flag(7)]
        public static bool UpdateMiscText { get; set; } = true;

        [Flag(8)]
        public static bool GenerateSpoilerLog { get; set; } = true;

        [Flag(9)]
        public static bool QuickSeed { get; set; } = false;

        [Flag(10)]
        public static bool AllowLoweringRespawn { get; set; } = true;

        [Flag(11)]
        public static bool PreventKnockbackOnLadders { get; set; } = true;

        [Flag(26)]
        public static bool FlexibleItems { get; set; } = true;

        [Flag(27)]
        public static bool IncludeEvilOnesFortress { get; set; } = false;

        [Flag(28)]
        public static bool DarkTowers { get; set; } = false;

        [Flag(29)]
        public static bool RandomizeTitles { get; set; } = true;

        [Flag(31)]
        public static bool AddKillSwitch { get; set; } = false;

        [Flag(FlagsCodec.NonBoolFlagBase + 33)]
        public static Hints HintSetting { get; set; } = Hints.Strong;

        [Flag(FlagsCodec.NonBoolFlagBase + 34)]
        public static MiscDoors MiscDoorSetting { get; set; } = MiscDoors.Unchanged;

        [Flag(FlagsCodec.NonBoolFlagBase + 35)]
        public static DoorTypeShuffle DoorTypeSetting { get; set; } = DoorTypeShuffle.Unchanged;

        [Flag(FlagsCodec.NonBoolFlagBase + 46)]
        public static ScreenRandomization RandomizeScreens { get; set; } = ScreenRandomization.Unchanged;

        [Flag(FlagsCodec.NonBoolFlagBase + 49)]
        public static SegmentShuffle ShuffleSegments { get; set; } = SegmentShuffle.Unchanged;
    }

    public class FlagPresets : ObservableCollection<string>
    {
        public FlagPresets()
        {
            Add("Beginner friendly");
            Add("Standard");
            Add("Race (typical)");
            Add("Race (classic)");
            Add("Challenge mode");
            Add("Chaos mode");
            Add("Extra fast");
        }
    }

    public class HintSettings : ObservableCollection<string>
    {
        public HintSettings()
        {
            Add("Strong hints");
            Add("Weak hints");
            Add("No hints");
            Add("Community hints only");
        }
    }

    public class MiscDoorSettings : ObservableCollection<string>
    {
        public MiscDoorSettings()
        {
            Add("Shuffle, include most buildings in towns");
            Add("Shuffle, include most buildings in towns except Gurus");
            Add("Shuffle, include most buildings in towns except key shops");
            Add("Shuffle, include most buildings in towns except Gurus and key shops");
            Add("Shuffle, exclude towns");
            Add("Unchanged");
        }
    }

    public class DoorTypeSettings : ObservableCollection<string>
    {
        public DoorTypeSettings()
        {
            Add("Shuffle, also move key requirements");
            Add("Shuffle, don't move key requirements");
            Add("Unchanged");
        }
    }

    public class ScreenRandomizationSettings : ObservableCollection<string>
    {
        public ScreenRandomizationSettings()
        {
            Add("Randomize for all worlds");
            Add("Randomize for all worlds except Eolis and Zenis");
            Add("Randomize for Eolis and Zenis only");
            Add("Unchanged");
        }
    }

    public class SegmentShuffleSettings : ObservableCollection<string>
    {
        public SegmentShuffleSettings()
        {
            Add("Shuffle all segments");
            Add("Shuffle towns only");
            Add("Unchanged");
        }
    }

    public class HintConverter : EnumConverter<GeneralOptions.Hints>
    {
    }

    public class MiscDoorConverter : EnumConverter<GeneralOptions.MiscDoors>
    {
    }

    public class DoorTypeConverter : EnumConverter<GeneralOptions.DoorTypeShuffle>
    {
    }

    public class ScreenRandomizationConverter : EnumConverter<GeneralOptions.ScreenRandomization>
    {
    }

    public class SegmentShuffleConverter : EnumConverter<GeneralOptions.SegmentShuffle>
    {
    }
}
