using System.Collections.ObjectModel;

namespace FaxanaduRando.Randomizer
{
    public class EnemyOptions
    {
        public enum EnemySetType
        {
            NonMixed,
            Easy,
            Normal,
            Hard,
            VeryHard,
            ExtremelyHard,
            Scaling,
            Unchanged,
        };

        public enum EnemyHP
        {
            Unchanged,
            PlusMinus20,
            PlusMinus40,
            PlusMinus60,
            PlusMinus80,
            PlusMinus50Percent,
            PlusMinus100Percent,
            AlwaysPlus50Percent,
            AlwaysPlus100Percent,
        };

        public enum EnemyDamage
        {
            Unchanged,
            PlusMinus10,
            PlusMinus20,
            PlusMinus30,
            PlusMinus40,
            PlusMinus50Percent,
            PlusMinus100Percent,
            AlwaysPlus50Percent,
            AlwaysPlus100Percent,
        };

        public static EnemySetType EnemySet { get; set; } = EnemySetType.NonMixed;
        public static EnemyHP EnemyHPSetting { get; set; } = EnemyHP.Unchanged;
        public static EnemyDamage EnemyDamageSetting { get; set; } = EnemyDamage.Unchanged;
        public static bool RandomizeExperience { get; set; } = true;
        public static bool RandomizeRewards { get; set; } = true;
        public static bool RandomizeMagicImmunities { get; set; } = true;
        public static bool TryToMoveBosses { get; set; } = true;
    }

    public class EnemySetSettings : ObservableCollection<string>
    {
        public EnemySetSettings()
        {
            Add("No mixed enemy types");
            Add("Easy");
            Add("Normal");
            Add("Hard");
            Add("Very hard");
            Add("Extremely hard");
            Add("Scaling");
            Add("Unchanged");
        }
    }

    public class EnemyHPSettings : ObservableCollection<string>
    {
        public EnemyHPSettings()
        {
            Add("Unchanged");
            Add("+/- up to 20");
            Add("+/- up to 40");
            Add("+/- up to 60");
            Add("+/- up to 80");
            Add("+/- up to 50%");
            Add("+/- up to 100%");
            Add("Always +50%");
            Add("Always +100%");
        }
    }

    public class EnemyDamageSettings : ObservableCollection<string>
    {
        public EnemyDamageSettings()
        {
            Add("Unchanged");
            Add("+/- up to 10");
            Add("+/- up to 20");
            Add("+/- up to 30");
            Add("+/- up to 40");
            Add("+/- up to 50%");
            Add("+/- up to 100%");
            Add("Always +50%");
            Add("Always +100%");
        }
    }

    public class EnemyHPConverter : EnumConverter<EnemyOptions.EnemyHP>
    {
    }

    public class EnemyDamageConverter : EnumConverter<EnemyOptions.EnemyDamage>
    {
    }

    public class EnemySetConverter : EnumConverter<EnemyOptions.EnemySetType>
    {
    }
}
