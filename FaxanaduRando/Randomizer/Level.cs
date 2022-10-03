using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public abstract class Level
    {
        public const int glitchedOffset = 0x2C31F;

        public static Dictionary<WorldNumber, Level> LevelDict { get; set; } = new Dictionary<WorldNumber, Level>();

        public List<Screen> Screens { get; set; } = new List<Screen>();
        public List<SubLevel> SubLevels { get; set; } = new List<SubLevel>();
        public WorldNumber Number { get; set; }
        public WorldNumber AdjustedNumber { get; set; }

        public Level(WorldNumber number, byte[] content)
        {
            Number = number;
            AdjustedNumber = Number;

            AddScreens(content);
            GetScrollingData(content);
            CorrectScreenSprites();
        }

        public abstract int GetStartOffset();
        public abstract int GetEndOffset();

        public void AddScreens(byte[] content)
        {
            int offset = GetStartOffset();
            bool text = false;
            byte number = 0;
            Screen screen = new Screen(number);

            while (offset <= GetEndOffset() && offset < content.Length)
            {
                if (content[offset] == 0xFF)
                {
                    if (text)
                    {
                        Screens.Add(screen);
                        number++;
                        screen = new Screen(number);
                    }

                    text = !text;
                    offset++;
                    continue;
                }

                if (offset == glitchedOffset)
                {
                    // special case needed as ROM is inconsistent
                    Screens.Add(screen);
                    number++;
                    screen = new Screen(number);
                    text = false;
                }

                if (text)
                {
                    screen.Text.Add(new TextObject(content, offset));
                    offset++;
                }
                else
                {
                    var sprite = new Sprite(content, offset);
                    screen.Sprites.Add(sprite);
                    offset += 2;
                }
            }
        }

        public virtual bool IsEolis()
        {
            return false;
        }

        public virtual bool ShouldRandomizeScreens()
        {
            return Util.AllWorldScreensRandomized();
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
                    int hardProbability = 3;
                    int bossProbability = hardProbability + 5;

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
                        if (screen.ParentWorld == WorldNumber.Mist)
                        {
                            hardProbability = 5;
                            bossProbability = hardProbability + 10;
                        }
                        else if (screen.ParentWorld == WorldNumber.Branch)
                        {
                            hardProbability = 10;
                            bossProbability = hardProbability + 20;
                        }
                        else if (screen.ParentWorld == WorldNumber.Dartmoor)
                        {
                            hardProbability = 30;
                            bossProbability = hardProbability + 30;
                        }
                        else if (screen.ParentWorld == WorldNumber.EvilOnesLair)
                        {
                            hardProbability = 45;
                            bossProbability = hardProbability + 45;
                        }
                    }

                    screen.RandomizeEnemiesWithProbabilities(random, IsEolis(), evilOneProbability,
                                                             grieveProbability, bossProbability, hardProbability);
                }
            }
        }

        public void AddToContent(byte[] content)
        {
            foreach (var screen in Screens)
            {
                screen.AddToContent(content);
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
            for (int i = start; i <= end; i++)
            {
                sublevel.Screens.Add(Screens[i]);
            }
        }

        public virtual void CorrectScreenSprites()
        {
        }

        public abstract void SetupScreens();
        public abstract void GetEnds(ref List<Screen> startScreens, ref List<Screen> endScreens, Random random);
        public abstract List<Screen> GetCandidates(Random random);
        public abstract List<Screen> GetSpecialScreens(Random random);
        public abstract bool CreateSublevels(List<Screen> startScreens, List<Screen> endScreens,
                                             List<Screen> candidates, List<Screen> specialScreens,
                                             Random random);

        public virtual bool RandomizeScreens(Random random, ref uint attempts)
        {
            if (!ShouldRandomizeScreens())
            {
                return true;
            }

            SetupScreens();
            var startScreens = new List<Screen>();
            var endScreens = new List<Screen>();
            GetEnds(ref startScreens, ref endScreens, random);

            bool result = false;
            while (!result && attempts < 200000)
            {
                attempts++;
                foreach (var screen in Screens)
                {
                    screen.ScrollData.Left = 0xFF;
                    screen.ScrollData.Right = 0xFF;
                    screen.ScrollData.Up = 0xFF;
                    screen.ScrollData.Down = 0xFF;

                    screen.AvailableDirections = new HashSet<Direction>(screen.Directions);
                }

                if (attempts % 10 == 0)
                {
                    startScreens = new List<Screen>();
                    endScreens = new List<Screen>();
                    GetEnds(ref startScreens, ref endScreens, random);
                }

                var candidates = GetCandidates(random);
                var specialScreens = GetSpecialScreens(random);

                result = CreateSublevels(startScreens, endScreens, candidates, specialScreens, random);
            }

            return result;
        }

        protected bool CreateSublevel(Screen start, Screen end, List<Screen> candidates, List<Screen> specialScreens,
                                      int specialProbability, int endProbability, Random random, SubLevel.Id sublevelId)
        {
            var current = start;
            var sublevel = SubLevel.SubLevelDict[sublevelId];
            sublevel.Screens = new List<Screen>();
            sublevel.Screens.Add(current);
            bool keepGoing = true;
            bool attemptedEnd;
            while (keepGoing)
            {
                bool found = false;
                attemptedEnd = false;
                if (candidates.Count < 6)
                {
                    endProbability = 100;
                }

                var directions = new List<Direction>(current.AvailableDirections);
                if (specialScreens.Count > 0 && random.Next(0, 100) < specialProbability)
                {
                    foreach (var direction in directions)
                    {
                        foreach (var special in specialScreens)
                        {
                            if (current.CanConnect(direction, special))
                            {
                                current.Connect(direction, special.Number);
                                var reverse = Screen.GetReverse(direction);
                                special.Connect(reverse, current.Number);
                                sublevel.Screens.Add(special);
                                current = special;
                                specialScreens.Remove(special);
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            break;
                        }
                    }
                }

                if (found)
                {
                    continue;
                }

                if (random.Next(0, 100) < endProbability)
                {
                    foreach (var direction in directions)
                    {
                        if (current.CanConnect(direction, end))
                        {
                            current.Connect(direction, end.Number);
                            var reverse = Screen.GetReverse(direction);
                            end.Connect(reverse, current.Number);
                            sublevel.Screens.Add(end);
                            return true;
                        }
                    }

                    var endDirections = new HashSet<Direction>(end.AvailableDirections);
                    bool foundEnd = false;
                    foreach (var direction in endDirections)
                    {
                        for (int i = 0; i < candidates.Count; i++)
                        {
                            var candidate = candidates[i];
                            if (end.CanConnect(direction, candidate))
                            {
                                end.Connect(direction, candidate.Number);
                                var reverse = Screen.GetReverse(direction);
                                candidate.Connect(reverse, end.Number);
                                sublevel.Screens.Add(end);
                                candidates.Remove(candidate);
                                end = candidate;
                                foundEnd = true;
                                break;
                            }
                        }

                        if (foundEnd)
                        {
                            break;
                        }
                    }

                    foreach (var direction in directions)
                    {
                        if (current.CanConnect(direction, end))
                        {
                            current.Connect(direction, end.Number);
                            var reverse = Screen.GetReverse(direction);
                            end.Connect(reverse, current.Number);
                            sublevel.Screens.Add(end);
                            return true;
                        }
                    }

                    if (!foundEnd)
                    {
                        attemptedEnd = true;
                    }

                    endProbability += 10;
                }

                while (directions.Count > 0 && !found)
                {
                    var direction = directions[random.Next(0, directions.Count)];
                    directions.Remove(direction);

                    for (int i = 0; i < candidates.Count; i++)
                    {
                        if (current.CanConnect(direction, candidates[i]))
                        {
                            var candidate = candidates[i];
                            current.Connect(direction, candidate.Number);
                            var reverse = Screen.GetReverse(direction);
                            candidate.Connect(reverse, current.Number);
                            sublevel.Screens.Add(candidate);
                            current = candidate;
                            candidates.Remove(candidate);
                            found = true;
                            endProbability += 2;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    endProbability = 100;
                    if (attemptedEnd)
                    {
                        keepGoing = false;
                    }
                }
            }

            return false;
        }

        public void GetScrollingData( byte[] content)
        {
            int bankOffset = Section.GetOffset(3, 0x8000, 0x8000);
            int pointer = Util.GetPointer((byte)Door.OtherWorldDict[Number], content, 3);
            byte b1 = content[bankOffset + pointer + 4];
            byte b2 = content[bankOffset + pointer + 5];
            var bytes = new byte[] { b1, b2 };
            var newPointer = BitConverter.ToUInt16(bytes, 0);

            for (int i = 0; i < Screens.Count; i++)
            {
                var data = new ScrollingData(content, bankOffset + newPointer + i * 4);
                Screens[i].ScrollData = data;
            }
        }
    }
}
