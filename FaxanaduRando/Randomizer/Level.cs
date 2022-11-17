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

        protected Dictionary<byte, byte> startToSpecial = new Dictionary<byte, byte>();

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
                        evilOneProbability = 30;
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
                sublevel.AddScreen(Screens[i]);
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
                                             Random random, uint attempts);

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

                result = CreateSublevels(startScreens, endScreens, candidates, specialScreens, random, attempts);
            }

            if (result)
            {
                foreach (var sublevel in SubLevels)
                {
                    var friends = new List<Screen>();
                    CheckFriends(sublevel.Screens, friends);

                    var temp = new List<Screen>(friends);
                    var temp2 = new List<Screen>();
                    while (temp.Count > 0)
                    {
                        CheckFriends(temp, temp2);
                        friends.AddRange(temp2);
                        temp = temp2;
                        temp2 = new List<Screen>();
                    }

                    foreach (var screen in friends)
                    {
                        sublevel.AddScreen(screen);
                    }
                }
            }

            return result;
        }

        protected bool CreateSublevel(Screen start, Screen end, List<Screen> candidates, List<Screen> specialScreens,
                                      int specialProbability, int endProbability, Random random, SubLevel.Id sublevelId, uint attempts, bool newSublevelScreens=true)
        {
            if (!GeneralOptions.ShuffleTowers)
            {
                if (startToSpecial.ContainsKey(start.Number))
                {
                    if (specialScreens.Contains(Screens[startToSpecial[start.Number]]))
                    {
                        return false;
                    }
                }
            }

            bool swapped = false;
            if (random.Next(2) == 0)
            {
                var tmp = start;
                start = end;
                end = tmp;
                swapped = true;
            }

            var current = start;
            var sublevel = SubLevel.SubLevelDict[sublevelId];
            if (newSublevelScreens)
            {
                sublevel.Screens = new List<Screen>();
                sublevel.AddScreen(current);
            }
            else if (swapped)
            {
                sublevel.AddScreen(current);
            }

            bool keepGoing = true;
            bool attemptedEnd;
            while (keepGoing)
            {
                bool found = false;
                attemptedEnd = false;
                if (candidates.Count < 8)
                {
                    endProbability = 100;
                }

                if (attempts < 100000)
                {
                    if (specialProbability > 0 && specialProbability < 100)
                    {
                        specialProbability = random.Next(1, 100);
                    }

                    if (endProbability > 0 && endProbability < 100)
                    {
                        endProbability = random.Next(1, 100);
                    }
                }

                foreach (var friendDirection in current.FriendConnections.Keys)
                {
                    var friend = current.FriendConnections[friendDirection];
                    current.Connect(friendDirection, friend);
                    sublevel.AddScreen(friend);
                    current = friend;
                    found = true;
                    break;
                }

                if (found)
                {
                    continue;
                }

                var directions = new List<Direction>(current.AvailableDirections);
                if (specialScreens.Count > 0 && random.Next(0, 100) < specialProbability)
                {
                    found = TryAddingSpecial(specialScreens, directions, sublevel, ref current);
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
                            current.Connect(direction, end);
                            AddEnd(sublevel, newSublevelScreens, swapped, end);
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
                                end.Connect(direction, candidate);
                                AddEnd(sublevel, newSublevelScreens, swapped, end);
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
                            current.Connect(direction, end);
                            sublevel.AddScreen(end);
                            return true;
                        }
                    }

                    if (!foundEnd)
                    {
                        attemptedEnd = true;
                    }

                    endProbability += 2;
                }

                while (directions.Count > 0 && !found)
                {
                    var direction = directions[random.Next(0, directions.Count)];
                    directions.Remove(direction);

                    for (int i = 0; i < candidates.Count; i++)
                    {
                        var candidate = candidates[i];
                        if (current.CanConnect(direction, candidate))
                        {
                            current.Connect(direction, candidate);
                            sublevel.AddScreen(candidate);
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
                    found = TryAddingSpecial(specialScreens, directions, sublevel, ref current);
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

        public void GetScrollingData(byte[] content)
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

        private void AddEnd(SubLevel sublevel, bool newSublevelScreens, bool swapped, Screen end)
        {
            if (newSublevelScreens)
            {
                sublevel.AddScreen(end);
            }
            else if (!swapped)
            {
                sublevel.AddScreen(end);
            }
        }

        private void CheckFriends(List<Screen> screens, List<Screen> friends)
        {
            foreach (var screen in screens)
            {
                foreach (var direction in screen.FriendEnds.Keys)
                {
                    var friend = screen.FriendEnds[direction];
                    screen.Connect(direction, friend);
                    friends.Add(friend);
                }
            }
        }

        private bool TryAddingSpecial(List<Screen> specialScreens, List<Direction> directions, SubLevel sublevel, ref Screen current)
        {
            foreach (var direction in directions)
            {
                foreach (var special in specialScreens)
                {
                    if (current.CanConnect(direction, special))
                    {
                        current.Connect(direction, special);
                        sublevel.AddScreen(special);
                        current = special;
                        specialScreens.Remove(special);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
