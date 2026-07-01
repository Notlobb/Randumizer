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
        [Flag(33)]
        public static bool RandomizePalettes { get; set; } = false;

        [Flag(34)]
        public static bool RandomizeSounds { get; set; } = false;

        [Flag(35)]
        public static bool AppendSuffix { get; set; } = false;

        [Flag(FlagsCodec.NonBoolFlagBase + 53)]
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
