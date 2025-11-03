using System.Collections.ObjectModel;

namespace FaxanaduRando.Randomizer
{
    public enum TileRandomizationMode
    {
        Unchanged,
        DecorativeOnly,
        PerWorld,
        AllWorlds,
        Chaos,
    }

    public class TileOptions
    {
        public static TileRandomizationMode RandomizationMode { get; set; } = TileRandomizationMode.Unchanged;
        public static bool PreserveCollision { get; set; } = true;
        public static bool RandomizeColors { get; set; } = false;
        public static int RandomizationIntensity { get; set; } = 50; // 0-100 percentage
    }

    public class TileRandomizationSettings : ObservableCollection<string>
    {
        public TileRandomizationSettings()
        {
            Add("Unchanged");
            Add("Decorative tiles only");
            Add("Per-world tile sets");
            Add("All worlds mixed");
            Add("Chaos mode");
        }
    }

    public class TileRandomizationModeConverter : EnumConverter<TileRandomizationMode>
    {
    }
}
