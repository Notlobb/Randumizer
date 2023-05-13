using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public enum Direction : byte
    {
        Left,
        Right,
        Up,
        Down,
    }

    public class Screen
    {
        private static List<Sprite.SpriteId> nonBossIdList = new List<Sprite.SpriteId>();
        private static List<Sprite.SpriteId> idList = new List<Sprite.SpriteId>();
        private static List<Sprite.SpriteId> easyIdList = new List<Sprite.SpriteId>();
        private static HashSet<Sprite.SpriteId> smallIds = new HashSet<Sprite.SpriteId>();
        private static List<Sprite.SpriteId> flyingIdList = new List<Sprite.SpriteId>();
        private static List<Sprite.SpriteId> smallIdList = new List<Sprite.SpriteId>();
        private static Dictionary<Sprite.SpriteId, List<Sprite.SpriteId>> bossTargets = new Dictionary<Sprite.SpriteId, List<Sprite.SpriteId>>();
        private static List<Sprite.SpriteId> largeIdList = new List<Sprite.SpriteId>();
        private static List<Sprite.SpriteId> hardIdList = new List<Sprite.SpriteId>();
        private static HashSet<Sprite.SpriteId> sourceIds = new HashSet<Sprite.SpriteId>();

        public Screen(byte number, WorldNumber parentWorld)
        {
            Number = number;
            ParentWorld = parentWorld;
        }

        public static void SetupEnemyIds()
        {
            idList = new List<Sprite.SpriteId>();
            nonBossIdList = new List<Sprite.SpriteId>();
            easyIdList = new List<Sprite.SpriteId>();
            smallIds = new HashSet<Sprite.SpriteId>();
            flyingIdList = new List<Sprite.SpriteId>();
            smallIdList = new List<Sprite.SpriteId>();
            bossTargets = new Dictionary<Sprite.SpriteId, List<Sprite.SpriteId>>();
            largeIdList = new List<Sprite.SpriteId>();
            hardIdList = new List<Sprite.SpriteId>();
            sourceIds = new HashSet<Sprite.SpriteId>();

            sourceIds.UnionWith(Sprite.tallIds);
            sourceIds.UnionWith(Sprite.shortIds);
            sourceIds.UnionWith(Sprite.flyingIds);
            nonBossIdList.AddRange(sourceIds);

            if (EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Easy)
            {
                nonBossIdList.Remove(Sprite.SpriteId.Psychic);
                nonBossIdList.Remove(Sprite.SpriteId.ExecutionHood);
                idList.AddRange(nonBossIdList);
                idList.AddRange(nonBossIdList);
            }
            else if (EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Normal)
            {
                nonBossIdList.Remove(Sprite.SpriteId.Psychic);
                nonBossIdList.Remove(Sprite.SpriteId.ExecutionHood);
                nonBossIdList.AddRange(sourceIds);
                nonBossIdList.Remove(Sprite.SpriteId.Psychic);
            }

            idList.AddRange(nonBossIdList);
            idList.AddRange(Sprite.bossIds);
            sourceIds.UnionWith(Sprite.bossIds);

            easyIdList.Add(Sprite.SpriteId.Hornet);
            easyIdList.Add(Sprite.SpriteId.Zombie);
            easyIdList.Add(Sprite.SpriteId.Ghost);
            easyIdList.Add(Sprite.SpriteId.Monodron);
            easyIdList.Add(Sprite.SpriteId.Cricket);
            easyIdList.Add(Sprite.SpriteId.TeleCreature);
            easyIdList.Add(Sprite.SpriteId.Spiky);
            easyIdList.Add(Sprite.SpriteId.Metroid);
            easyIdList.Add(Sprite.SpriteId.StillKnight);
            easyIdList.Add(Sprite.SpriteId.Snowman);

            bossTargets[Sprite.SpriteId.DogBoss] = new List<Sprite.SpriteId>{
                                                    Sprite.SpriteId.DogBoss,
                                                    Sprite.SpriteId.Wyvern,
                                                    Sprite.SpriteId.Table,
                                                    };
            bossTargets[Sprite.SpriteId.Wyvern] = new List<Sprite.SpriteId>{
                                                    Sprite.SpriteId.DogBoss,
                                                    Sprite.SpriteId.Wyvern,
                                                    Sprite.SpriteId.Table,
                                                    };
            bossTargets[Sprite.SpriteId.Table] = new List<Sprite.SpriteId>{
                                                    Sprite.SpriteId.DogBoss,
                                                    Sprite.SpriteId.Wyvern,
                                                    Sprite.SpriteId.Table,
                                                    Sprite.SpriteId.BigSnake,
                                                    Sprite.SpriteId.Nest,
                                                    Sprite.SpriteId.RockLobster,
                                                    };
            bossTargets[Sprite.SpriteId.BigSnake] = new List<Sprite.SpriteId>{
                                                    Sprite.SpriteId.DogBoss,
                                                    Sprite.SpriteId.Wyvern,
                                                    Sprite.SpriteId.Table,
                                                    Sprite.SpriteId.BigSnake,
                                                    Sprite.SpriteId.Nest,
                                                    Sprite.SpriteId.RockLobster,
                                                    };
            bossTargets[Sprite.SpriteId.Nest] = new List<Sprite.SpriteId>{
                                                    Sprite.SpriteId.DogBoss,
                                                    Sprite.SpriteId.Wyvern,
                                                    Sprite.SpriteId.Table,
                                                    Sprite.SpriteId.Nest,
                                                    Sprite.SpriteId.RockLobster,
                                                    };
            bossTargets[Sprite.SpriteId.RockLobster] = new List<Sprite.SpriteId>{
                                                    Sprite.SpriteId.DogBoss,
                                                    Sprite.SpriteId.Table,
                                                    Sprite.SpriteId.Nest,
                                                    Sprite.SpriteId.RockLobster,
                                                    };

            smallIds.UnionWith(Sprite.shortIds);
            smallIds.UnionWith(Sprite.tallIds);
            flyingIdList.AddRange(Sprite.flyingIds);
            smallIdList.AddRange(smallIds);
            smallIdList.Remove(Sprite.SpriteId.Psychic);

            largeIdList.AddRange(Sprite.bossIds);
            hardIdList.Add(Sprite.SpriteId.Psychic);
            hardIdList.Add(Sprite.SpriteId.ExecutionHood);
            hardIdList.Add(Sprite.SpriteId.StillKnight);
        }

        public List<Sprite> Sprites { get; set; } = new List<Sprite>();
        public List<TextObject> Text { get; set; } = new List<TextObject>();
        public WorldNumber ParentWorld { get; set; } = WorldNumber.Unknown;
        public SubLevel.Id ParentSublevel { get; set; } = SubLevel.Id.Other;
        public bool SkipBosses { get; set; } = false;
        public ScrollingData ScrollData { get; set; }
        public HashSet<Direction> Directions { get; set; } = new HashSet<Direction>();
        public HashSet<Direction> AvailableDirections { get; set; } = new HashSet<Direction>();
        public byte Number { get; set; }
        public Transition Transition { get; set; } = null;
        public Screen ConnectedScreen { get; set; } = null;
        public HashSet<byte> OpenTilesLeft { get; set; } = new HashSet<byte>();
        public HashSet<byte> OpenTilesRight { get; set; } = new HashSet<byte>();
        public HashSet<byte> OpenTilesUp { get; set; } = new HashSet<byte>();
        public HashSet<byte> OpenTilesDown { get; set; } = new HashSet<byte>();
        public HashSet<byte> SecondOpenTilesLeft { get; set; } = new HashSet<byte>();
        public HashSet<byte> SecondOpenTilesRight { get; set; } = new HashSet<byte>();
        public HashSet<byte> SecondOpenTilesUp { get; set; } = new HashSet<byte>();
        public HashSet<byte> SecondOpenTilesDown { get; set; } = new HashSet<byte>();
        public List<DoorId> Doors { get; set; } = new List<DoorId>();
        public List<GiftItem.Id> Gifts { get; set; } = new List<GiftItem.Id>();
        public Dictionary<Direction, Screen> FriendEnds { get; set; } = new Dictionary<Direction, Screen>();
        public Dictionary<Direction, Screen> FriendConnections { get; set; } = new Dictionary<Direction, Screen>();

        public static Direction GetReverse(Direction direction)
        {
            if (direction == Direction.Left)
            {
                return Direction.Right;
            }
            else if (direction == Direction.Right)
            {
                return Direction.Left;
            }
            else if (direction == Direction.Up)
            {
                return Direction.Down;
            }

            return Direction.Up;
        }

        public HashSet<byte> GetTiles(Direction direction)
        {
            if (direction == Direction.Left)
            {
                return OpenTilesLeft;
            }
            else if (direction == Direction.Right)
            {
                return OpenTilesRight;
            }
            else if (direction == Direction.Up)
            {
                return OpenTilesUp;
            }

            return OpenTilesDown;
        }

        public HashSet<byte> GetSecondTiles(Direction direction)
        {
            if (direction == Direction.Left)
            {
                return SecondOpenTilesLeft;
            }
            else if (direction == Direction.Right)
            {
                return SecondOpenTilesRight;
            }
            else if (direction == Direction.Up)
            {
                return SecondOpenTilesUp;
            }

            return SecondOpenTilesDown;
        }

        public bool CanConnect(Direction direction, Screen other)
        {
            if (!AvailableDirections.Contains(direction))
            {
                return false;
            }

            var reverse = GetReverse(direction);
            if (!other.AvailableDirections.Contains(reverse))
            {
                return false;
            }

            var tiles = GetTiles(direction);
            var otherTiles = other.GetTiles(reverse);
            var secondTiles = GetSecondTiles(direction);
            var secondOtherTiles = other.GetSecondTiles(reverse);

            bool result = CanConnect(direction, tiles, otherTiles);
            if (secondTiles.Count == 0 && secondOtherTiles.Count == 0)
            {
                return result;
            }

            if (secondTiles.Count > 0 && secondOtherTiles.Count > 0)
            {
                return false;
            }

            if (secondTiles.Count > 0)
            {
                result &= CanConnect(direction, secondTiles, otherTiles);
            }

            if (secondOtherTiles.Count > 0)
            {
                result &= other.CanConnect(reverse, secondOtherTiles, tiles);
            }

            return result; 
        }

        private bool CanConnect(Direction direction, HashSet<byte> tiles, HashSet<byte> otherTiles)
        {
            if (direction == Direction.Up || direction == Direction.Down)
            {
                return tiles.Overlaps(otherTiles);
            }

            tiles = new HashSet<byte>(tiles);
            tiles.IntersectWith(otherTiles);
            var values = new List<byte>(tiles);
            values.Sort();
            for (int i = 0; i < values.Count - 1; i++)
            {
                if (values[i] == values[i + 1] - 1)
                {
                    return true;
                }
            }

            return false;
        }

        public void Connect(Direction direction, Screen other)
        {
            Connect(direction, other.Number);
            var reverse = GetReverse(direction);
            other.Connect(reverse, Number);
        }

        public void Connect(Direction direction, byte number)
        {
            if (direction == Direction.Left)
            {
                ScrollData.Left = number;
            }
            else if (direction == Direction.Right)
            {
                ScrollData.Right = number;
            }
            else if (direction == Direction.Up)
            {
                ScrollData.Up = number;
            }
            else if (direction == Direction.Down)
            {
                ScrollData.Down = number;
            }

            AvailableDirections.Remove(direction);
        }

        public void RandomizeEnemiesWithProbabilities(Random random,
                                                      int evilOneProbability,
                                                      int grieveProbability,
                                                      int bossProbability,
                                                      int hardProbability)
        {
            bool skipBosses = ShouldSkipBosses();

            if (Doors.Count > 0 &&
                Sprites.Count > 1)
            {
                grieveProbability = 0;
            }

            foreach (var sprite in Sprites)
            {
                if (sourceIds.Contains(sprite.Id))
                {
                    Sprite.SpriteId newId;
                    int next = random.Next(100);
                    if (!skipBosses && Sprites.Count < 2)
                    {
                        if (next < evilOneProbability)
                        {
                            newId = Sprite.SpriteId.EvilOne;
                            sprite.Update(newId, random);
                            continue;
                        }
                    }

                    if (next < hardProbability)
                    {
                        newId = hardIdList[random.Next(hardIdList.Count)];
                    }
                    else if (!skipBosses && Sprites.Count < 3 && next < grieveProbability)
                    {
                        newId = Sprite.SpriteId.KingGrieve;
                        skipBosses = true;
                    }
                    else if (!skipBosses && next < bossProbability)
                    {
                        newId = largeIdList[random.Next(largeIdList.Count)];
                        skipBosses = true;
                    }
                    else
                    {
                        newId = nonBossIdList[random.Next(nonBossIdList.Count)];
                    }

                    sprite.Update(newId, random);
                }
            }

            PreventGraphicalGlitches();
        }

        public void RandomizeEnemies(Random random, bool eolis)
        {
            bool skipBosses = ShouldSkipBosses();
            foreach (var sprite in Sprites)
            {
                if (sourceIds.Contains(sprite.Id))
                {
                    Sprite.SpriteId newId;
                    bool easy = eolis && EnemyOptions.EnemySet != EnemyOptions.EnemySetType.Hard;
                    easy |= (EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Easy && random.Next(0, 3) != 0);
                    if (easy)
                    {
                        newId = easyIdList[random.Next(easyIdList.Count)];
                    }
                    else if (skipBosses)
                    {
                        newId = nonBossIdList[random.Next(nonBossIdList.Count)];
                    }
                    else
                    {
                        newId = idList[random.Next(idList.Count)];
                        if (Sprite.bossIds.Contains(newId))
                        {
                            skipBosses = true;
                        }
                    }

                    sprite.Update(newId, random);
                }
            }

            PreventGraphicalGlitches();
        }

        public void RandomizeEnemiesNonMixed(Random random)
        {
            foreach (var sprite in Sprites)
            {
                var newId = Sprite.SpriteId.Undefined;
                if (sprite.Id == Sprite.SpriteId.Spiky ||
                    Sprite.flyingIds.Contains(sprite.Id))
                {
                    newId = flyingIdList[random.Next(flyingIdList.Count)];
                }
                else if (smallIds.Contains(sprite.Id))
                {
                    newId = smallIdList[random.Next(smallIdList.Count)];
                    if (newId == Sprite.SpriteId.ExecutionHood)
                    {
                        newId = smallIdList[random.Next(smallIdList.Count)];
                    }
                }
                else if (Sprite.bossIds.Contains(sprite.Id))
                {
                    var list = bossTargets[sprite.Id];
                    newId = list[random.Next(list.Count)];
                }

                if (newId != Sprite.SpriteId.Undefined)
                {
                    sprite.Update(newId, random);
                }
            }
        }

        public bool ShouldSkipBosses()
        {
            if (SkipBosses)
            {
                return true;
            }

            if (Sprites.Count > 3)
            {
                return true;
            }

            if (ItemOptions.BigItemSpawns == ItemOptions.BigItemSpawning.AlwaysSpawn)
            {
                return false;
            }

            bool skipBosses = false;
            if (!ContainsBoss())
            {
                foreach (var sprite in Sprites)
                {
                    if (Sprite.KeyItems.Contains(sprite.Id))
                    {
                        skipBosses = true;
                        break;
                    }
                }
            }

            return skipBosses;
        }

        private bool ContainsBoss()
        {
            foreach (var sprite in Sprites)
            {
                if (Sprite.bossIds.Contains(sprite.Id) ||
                    Sprite.superBossIds.Contains(sprite.Id))
                {
                    return true;
                }
            }

            return false;
        }

        public void PreventGraphicalGlitches()
        {
            if (Sprites.Count > 1 && ContainsBoss())
            {
                foreach (var sprite in Sprites)
                {
                    if (sprite.Id == Sprite.SpriteId.Jason)
                    {
                        sprite.Id = Sprite.SpriteId.Cricket;
                    }
                }
            }
        }

        public void AddToContent(byte[] content)
        {
            ScrollData.AddToContent(content);
            foreach (var sprite in Sprites)
            {
                sprite.AddToContent(content);
            }
            foreach (var text in Text)
            {
                text.AddToContent(content);
            }
            if (Transition != null)
            {
                Transition.AddToContent(content);
            }
        }
    }
}
