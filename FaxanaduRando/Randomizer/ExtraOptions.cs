using System.Collections.ObjectModel;

namespace FaxanaduRando.Randomizer
{
    public enum Music
    {
        Random,
        None,
        Unchanged,
        Community,
        CommunityAndOriginal,
        CommunityChaos,
        CommunityAndOriginalChaos,
    }

    public class ExtraOptions
    {
        public static bool RandomizePalettes { get; set; } = false;
        public static bool RandomizeSounds { get; set; } = false;
        public static bool AppendSuffix { get; set; } = false;
        public static Music MusicSetting { get; set; } = Music.Unchanged;
    }

    public class MusicSettings : ObservableCollection<string>
    {
        public MusicSettings()
        {
            Add("Random");
            Add("None");
            Add("Unchanged");
            Add("Community");
            Add("Community + Original");
            Add("Community (Chaos)");
            Add("Community + Original (Chaos)");
        }
    }

    public class MusicConverter : EnumConverter<Music>
    {
    }
}
