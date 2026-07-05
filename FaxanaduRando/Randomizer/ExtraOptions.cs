using System.Collections.ObjectModel;

namespace FaxanaduRando.Randomizer
{
    public enum Music
    {
        Random,
        None,
        Unchanged
    }

    public enum Soundtrack
    {
        Original,
        Community,
        Mix
    }

    public class ExtraOptions
    {
        public static bool RandomizePalettes { get; set; } = false;
        public static bool RandomizeSounds { get; set; } = false;
        public static bool AppendSuffix { get; set; } = false;
        public static Music MusicSetting { get; set; } = Music.Unchanged;
        public static Soundtrack SoundtrackSetting { get; set; } = Soundtrack.Original;
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

    public class SoundtrackSettings : ObservableCollection<string>
    {
        public SoundtrackSettings()
        {
            Add("Original");
            Add("Community");
            Add("Mix");
        }
    }

    public class MusicConverter : EnumConverter<Music>
    {
    }
}
