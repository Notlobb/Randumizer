using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class Level
    {
        public enum StartOffset
        {
            Eolis = 0x2C242,
            Trunk = 0x2C2F0,
            Mist = 0x2C538,
            Branch = 0x2C7AA,
            DartmoorArea = 0x2C8F2,
            EvilOnesLair = 0x2C9F4,
            Buildings = 0x2CB12,
            Towns = 0x2CCB9,
        }

        public enum EndOffset
        {
            Eolis = 0x2C26F,
            Trunk = 0x2C491,
            Mist = 0x2C759,
            Branch = 0x2C8B1,
            DartmoorArea = 0x2C9CB,
            EvilOnesLair = 0x2CA85,
            Buildings = 0x2CC9C,
            Towns = 0x2CCED,
        }

        public const int glitchedOffset = 0x2C31F;
        public static readonly List<StartOffset> offsets = new List<StartOffset>
        {
            StartOffset.Eolis,
            StartOffset.Trunk,
            StartOffset.Mist,
            StartOffset.Branch,
            StartOffset.DartmoorArea,
            StartOffset.EvilOnesLair,
            StartOffset.Buildings,
            StartOffset.Towns,
        };

        public static readonly List<EndOffset> ends = new List<EndOffset>
        {
            EndOffset.Eolis,
            EndOffset.Trunk,
            EndOffset.Mist,
            EndOffset.Branch,
            EndOffset.DartmoorArea,
            EndOffset.EvilOnesLair,
            EndOffset.Buildings,
            EndOffset.Towns,
        };

        public static Dictionary<StartOffset, Level> LevelDict { get; set; } = new Dictionary<StartOffset, Level>();

        public StartOffset Start { get; set; }
        public EndOffset End { get; set; }
        public List<Screen> Screens { get; set; } = new List<Screen>();
        public List<SubLevel> SubLevels { get; set; } = new List<SubLevel>();
        public WorldNumber Number { get; set; }

        public Level(StartOffset startOffset, EndOffset endOffset, WorldNumber number)
        {
            Start = startOffset;
            End = endOffset;
            Number = number;
        }

        public void AddScreens(byte[] content)
        {
            int offset = (int)Start;
            bool text = false;
            Screen screen = new Screen();

            while (offset <= (int)End && offset < content.Length)
            {
                if (content[offset] == 0xFF)
                {
                    if (text)
                    {
                        Screens.Add(screen);
                        screen = new Screen();
                    }

                    text = !text;
                    offset++;
                    continue;
                }

                if (offset == glitchedOffset)
                {
                    // special case needed as ROM is inconsistent
                    Screens.Add(screen);
                    screen = new Screen();
                    text = false;
                }

                if (text)
                {
                    screen.Text.Add(content[offset]);
                    offset++;
                }
                else
                {
                    var sprite = new Sprite((Sprite.SpriteId)content[offset], content[offset + 1]);
                    screen.Sprites.Add(sprite);
                    offset += 2;
                }
            }
        }

        public bool IsEolis()
        {
            return Start == StartOffset.Eolis;
        }

        public void RandomizeEnemies(Random random)
        {
            foreach (var screen in Screens)
            {
                if (EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Easy ||
                    EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Normal ||
                    EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Hard)
                {
                    screen.RandomizeEnemies(random, IsEolis());
                }
                else if (EnemyOptions.EnemySet == EnemyOptions.EnemySetType.NonMixed)
                {
                    screen.RandomizeEnemiesNonMixed(random, IsEolis());
                }
                else
                {
                    int evilOneProbability = 0;
                    int grieveProbability = 0;
                    int bossProbability = 0;
                    int hardProbability = 0;

                    if (EnemyOptions.EnemySet == EnemyOptions.EnemySetType.VeryHard)
                    {
                        hardProbability = 30;
                        bossProbability = hardProbability + 30;
                    }
                    else if (EnemyOptions.EnemySet == EnemyOptions.EnemySetType.ExtremelyHard)
                    {
                        evilOneProbability = 50;
                        hardProbability = 45;
                        grieveProbability = hardProbability + 10;
                        bossProbability = grieveProbability + 35;
                    }
                    else
                    {
                        if (Number == WorldNumber.Mist)
                        {
                            hardProbability = 5;
                            bossProbability = hardProbability + 10;
                        }
                        else if (Number == WorldNumber.Branch)
                        {
                            hardProbability = 10;
                            bossProbability = hardProbability + 20;
                        }
                        else if (Number == WorldNumber.Dartmoor)
                        {
                            hardProbability = 30;
                            bossProbability = hardProbability + 30;
                        }
                        else if (Number == WorldNumber.EvilOnesLair)
                        {
                            hardProbability = 45;
                            bossProbability = hardProbability + 45;
                        }
                        else
                        {
                            hardProbability = 3;
                            bossProbability = hardProbability + 5;
                        }
                    }

                    screen.RandomizeEnemiesWithProbabilities(random, IsEolis(), evilOneProbability,
                                                             grieveProbability, bossProbability, hardProbability);
                }
            }
        }

        public void WriteToContent(byte[] content)
        {
            int offset = (int)Start;
            foreach (var screen in Screens)
            {
                offset = screen.WriteToContent(content, offset);
            }
        }

        public void AddSubLevel(SubLevel.Id id, int start, int end)
        {
            var screens = new List<Screen>();
            for (int i = start; i <= end; i++)
            {
                screens.Add(Screens[i]);
            }
            var sublevel = new SubLevel(id, screens);
            SubLevels.Add(sublevel);
            SubLevel.SubLevelDict[sublevel.SubLevelId] = sublevel;
        }

        public void AddSubLevelScreens(SubLevel sublevel, int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                sublevel.Screens.Add(Screens[i]);
            }
        }
    }
}
