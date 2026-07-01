using System.Collections.ObjectModel;

namespace FaxanaduRando.Randomizer
{
    public enum Music
    {
        Random,
        None,
        Unchanged,
    }

    public class ExtraOptions
    {
        [FlagNone]
        public static bool RandomizePalettes { get; set; } = false;

        [FlagNone]
        public static bool RandomizeSounds { get; set; } = false;

        [FlagNone]
        public static bool AppendSuffix { get; set; } = false;

        [FlagNone]
        public static Music MusicSetting { get; set; } = Music.Unchanged;
    }

    public class MusicSettings : ObservableCollection<string>
    {
        public MusicSettings()
        {
            Add("Random");
            Add("None");
            Add("Unchanged");
        }
    }

    public class MusicConverter : EnumConverter<Music>
    {
    }
}
