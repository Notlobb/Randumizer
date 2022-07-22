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

        public enum WorldDoors
        {
            ShuffleMoveKeys,
            ShuffleDontMoveKeys,
            Unchanged,
        };

        public static bool FastText { get; set; } = true;
        public static bool FastStart { get; set; } = true;
        public static bool DragonSlayerRequired { get; set; } = false;
        public static bool PendantRodRubyRequired { get; set; } = false;
        public static bool MoveFinalRequirements { get; set; } = false;
        public static bool MoveSpringQuestRequirement { get; set; } = false;
        public static bool ShuffleTowers { get; set; } = false;
        public static bool ShuffleWorlds { get; set; } = false;
        public static bool UpdateMiscText { get; set; } = true;
        public static bool GenerateSpoilerLog { get; set; } = true;
        public static bool QuickSeed { get; set; } = false;
        public static bool AllowLoweringRespawn { get; set; } = true;
        public static bool PreventKnockbackOnLadders { get; set; } = true;
        public static bool AllowEquippingIndoors { get; set; } = true;
        public static bool IncludeEvilOnesFortress { get; set; } = false;
        public static bool DarkTowers { get; set; } = false;
        public static bool RandomizeTitles { get; set; } = true;
        public static Hints HintSetting { get; set; } = Hints.Strong;
        public static MiscDoors MiscDoorSetting { get; set; } = MiscDoors.Unchanged;
        public static WorldDoors WorldDoorSetting { get; set; } = WorldDoors.Unchanged;
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

    public class WorldDoorSettings : ObservableCollection<string>
    {
        public WorldDoorSettings()
        {
            Add("Shuffle, also move key requirements");
            Add("Shuffle, don't move key requirements");
            Add("Unchanged");
        }
    }

    public class HintConverter : EnumConverter<GeneralOptions.Hints>
    {
    }

    public class MiscDoorConverter : EnumConverter<GeneralOptions.MiscDoors>
    {
    }

    public class WorldDoorConverter : EnumConverter<GeneralOptions.WorldDoors>
    {
    }
}
