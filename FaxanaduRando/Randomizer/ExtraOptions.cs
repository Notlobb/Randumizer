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
        public static bool RandomizePalettes { get; set; } = false;
        public static bool RandomizeSounds { get; set; } = false;
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
