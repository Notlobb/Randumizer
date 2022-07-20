using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
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
                nonBossIdList.Remove(Sprite.SpriteId.EvilCreature);
                idList.AddRange(nonBossIdList);
                idList.AddRange(nonBossIdList);
            }
            else if (EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Normal)
            {
                nonBossIdList.Remove(Sprite.SpriteId.Psychic);
                nonBossIdList.Remove(Sprite.SpriteId.EvilCreature);
                nonBossIdList.AddRange(sourceIds);
                nonBossIdList.Remove(Sprite.SpriteId.Psychic);
            }

            idList.AddRange(nonBossIdList);
            idList.AddRange(Sprite.bossIds);
            sourceIds.UnionWith(Sprite.bossIds);

            easyIdList.Add(Sprite.SpriteId.Bee);
            easyIdList.Add(Sprite.SpriteId.BipedCyclops);
            easyIdList.Add(Sprite.SpriteId.Ghost);
            easyIdList.Add(Sprite.SpriteId.JumpingCyclops);
            easyIdList.Add(Sprite.SpriteId.JumpingCreature);
            easyIdList.Add(Sprite.SpriteId.TeleCreature);
            easyIdList.Add(Sprite.SpriteId.Spiky);
            easyIdList.Add(Sprite.SpriteId.FloatingFangs);
            easyIdList.Add(Sprite.SpriteId.StillKnight);
            easyIdList.Add(Sprite.SpriteId.Goat);

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
            hardIdList.Add(Sprite.SpriteId.EvilCreature);
            hardIdList.Add(Sprite.SpriteId.StillKnight);
        }

        public List<Sprite> Sprites { get; set; } = new List<Sprite>();
        public List<byte> Text { get; set; } = new List<byte>();
        public SubLevel ParentSubLevel { get; set; } = null;
        public SubLevel.Id ParentSubLevelId { get { return ParentSubLevel != null ? ParentSubLevel.SubLevelId : SubLevel.Id.Other; } }
        public bool SkipBosses { get; set; } = false;

        public void RandomizeEnemiesWithProbabilities(Random random,
                                                      bool eolis,
                                                      int evilOneProbability,
                                                      int grieveProbability,
                                                      int bossProbability,
                                                      int hardProbability)
        {
            bool skipBosses = ShouldSkipBosses();
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
                    if (eolis && EnemyOptions.EnemySet != EnemyOptions.EnemySetType.Hard)
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

        public void RandomizeEnemiesNonMixed(Random random, bool eolis)
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
                    if (newId == Sprite.SpriteId.EvilCreature)
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
                        sprite.Id = Sprite.SpriteId.JumpingCreature;
                    }
                }
            }
        }

        public int WriteToContent(byte[] content, int offset)
        {
            foreach (var sprite in Sprites)
            {
                sprite.WriteToContent(content, offset);
                offset += 2;
            }
            offset++;
            if (offset == Level.glitchedOffset)
            {
                return offset;
            }
            foreach (var text in Text)
            {
                content[offset] = text;
                offset++;
            }
            offset++;
            return offset;
        }

    }
}
