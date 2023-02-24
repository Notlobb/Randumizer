using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class EnemyRandomizer
    {
        public void RandomizeEnemyHP(Table enemyHPTable, Random random)
        {
            if (EnemyOptions.EnemyHPSetting == EnemyOptions.EnemyHP.PlusMinus20)
            {
                RandomizeTableCeiling(enemyHPTable, 20, random);
            }
            else if (EnemyOptions.EnemyHPSetting == EnemyOptions.EnemyHP.PlusMinus40)
            {
                RandomizeTableCeiling(enemyHPTable, 40, random);
            }
            else if (EnemyOptions.EnemyHPSetting == EnemyOptions.EnemyHP.PlusMinus60)
            {
                RandomizeTableCeiling(enemyHPTable, 60, random);
            }
            else if (EnemyOptions.EnemyHPSetting == EnemyOptions.EnemyHP.PlusMinus80)
            {
                RandomizeTableCeiling(enemyHPTable, 80, random);
            }
            else if (EnemyOptions.EnemyHPSetting == EnemyOptions.EnemyHP.PlusMinus50Percent)
            {
                RandomizeTableFraction(enemyHPTable, 0.5, random);
            }
            else if (EnemyOptions.EnemyHPSetting == EnemyOptions.EnemyHP.PlusMinus100Percent)
            {
                RandomizeTableFraction(enemyHPTable, 1, random);
            }
            else if (EnemyOptions.EnemyHPSetting == EnemyOptions.EnemyHP.AlwaysPlus50Percent)
            {
                IncreaseTableEntries(enemyHPTable, 0.5, random);
            }
            else if (EnemyOptions.EnemyHPSetting == EnemyOptions.EnemyHP.AlwaysPlus100Percent)
            {
                IncreaseTableEntries(enemyHPTable, 1, random);
            }
        }

        public void RandomizeEnemyDamage(Table enemyDamageTable, Random random)
        {
            if (EnemyOptions.EnemyDamageSetting == EnemyOptions.EnemyDamage.PlusMinus10)
            {
                RandomizeTableCeiling(enemyDamageTable, 10, random);
            }
            else if (EnemyOptions.EnemyDamageSetting == EnemyOptions.EnemyDamage.PlusMinus20)
            {
                RandomizeTableCeiling(enemyDamageTable, 20, random);
            }
            else if (EnemyOptions.EnemyDamageSetting == EnemyOptions.EnemyDamage.PlusMinus30)
            {
                RandomizeTableCeiling(enemyDamageTable, 30, random);
            }
            else if (EnemyOptions.EnemyDamageSetting == EnemyOptions.EnemyDamage.PlusMinus40)
            {
                RandomizeTableCeiling(enemyDamageTable, 40, random);
            }
            else if (EnemyOptions.EnemyDamageSetting == EnemyOptions.EnemyDamage.PlusMinus50Percent)
            {
                RandomizeTableFraction(enemyDamageTable, 0.5, random);
            }
            else if (EnemyOptions.EnemyDamageSetting == EnemyOptions.EnemyDamage.PlusMinus100Percent)
            {
                RandomizeTableFraction(enemyDamageTable, 1, random);
            }
            else if (EnemyOptions.EnemyDamageSetting == EnemyOptions.EnemyDamage.AlwaysPlus50Percent)
            {
                IncreaseTableEntries(enemyDamageTable, 0.5, random);
            }
            else if (EnemyOptions.EnemyDamageSetting == EnemyOptions.EnemyDamage.AlwaysPlus100Percent)
            {
                IncreaseTableEntries(enemyDamageTable, 1, random);
            }
        }

        public void RandomizeExperience(Table enemyExperienceTable, Random random)
        {
            for (int i = 0; i < enemyExperienceTable.Entries.Count; i++)
            {
                if (Sprite.enemies.Contains((Sprite.SpriteId)i))
                {
                    int newExperience;
                    if (random.Next(0, 4) == 0)
                    {
                        newExperience = random.Next(0, 256);
                    }
                    else
                    {
                        newExperience = random.Next(0, 81);
                    }

                    enemyExperienceTable.Entries[i][0] = (byte)newExperience;
                }
            }
        }

        public void RandomizeRewards(Table enemyRewardTypeTable, Table enemyRewardQuantityTable, Random random)
        {
            for (int i = 0; i < enemyRewardTypeTable.Entries.Count; i++)
            {
                int newRewardType;
                if (random.Next(0, 20) == 0)
                {
                    newRewardType = 0xFF;
                }
                else
                {
                    newRewardType = random.Next(0, 63);
                }

                enemyRewardTypeTable.Entries[i][0] = (byte)newRewardType;
            }
            for (int i = 0; i < enemyRewardQuantityTable.Entries.Count; i++)
            {
                int ceiling = 255;
                if (i >= 48)
                {
                    ceiling = 40;
                }
                int newRewardQuantity = random.Next(1, ceiling + 1);
                enemyRewardQuantityTable.Entries[i][0] = (byte)newRewardQuantity;
            }
        }

        public void RandomizeMagicImmunities(Table magicResistanceTable, Random random, byte[] content)
        {
            foreach (var entry in magicResistanceTable.Entries)
            {
                entry[0] = (byte)random.Next(0, 5);
            }

            var resistSection = new Section();
            resistSection.Bytes.Add(OpCode.JMPAbsolute);
            resistSection.Bytes.Add(0xD2);
            resistSection.Bytes.Add(0x81);
            resistSection.AddToContent(content, Section.GetOffset(14, 0x8AFF, 0x8000));

            resistSection = new Section();
            resistSection.Bytes.Add(OpCode.JMPAbsolute);
            resistSection.Bytes.Add(0xFD);
            resistSection.Bytes.Add(0x81);
            resistSection.Bytes.Add(OpCode.STYAbsolute);
            resistSection.Bytes.Add(0x01);
            resistSection.Bytes.Add(0x00);
            resistSection.Bytes.Add(OpCode.LDYAbsoluteX);
            resistSection.Bytes.Add(0xCC);
            resistSection.Bytes.Add(0x02);
            resistSection.Bytes.Add(OpCode.LDAAbsoluteY);
            resistSection.Bytes.Add(0x3B);
            resistSection.Bytes.Add(0xB7);
            resistSection.Bytes.Add(OpCode.CMPAbsolute);
            resistSection.Bytes.Add(0x01);
            resistSection.Bytes.Add(0x00);
            resistSection.Bytes.Add(OpCode.BNE);
            resistSection.Bytes.Add(0x01);
            resistSection.Bytes.Add(OpCode.RTS);
            resistSection.Bytes.Add(OpCode.LDYAbsolute);
            resistSection.Bytes.Add(0x01);
            resistSection.Bytes.Add(0x00);
            resistSection.Bytes.Add(OpCode.LDAAbsoluteY);
            resistSection.Bytes.Add(0x73);
            resistSection.Bytes.Add(0x8B);
            resistSection.Bytes.Add(OpCode.JMPAbsolute);
            resistSection.Bytes.Add(0x02);
            resistSection.Bytes.Add(0x8B);
            resistSection.AddToContent(content, Section.GetOffset(14, 0x81CF, 0x8000));
        }

        public void RandomizeBehaviour(int id, Table spriteBehaviourTable, Random random)
        {
            var possibleBehaviours = new List<Sprite.SpriteId>
            {
                Sprite.SpriteId.Mario,
                Sprite.SpriteId.Snowman,
                Sprite.SpriteId.Jason,
                Sprite.SpriteId.Cricket,
                Sprite.SpriteId.Raisin,
            };

            var newBehaviour = possibleBehaviours[random.Next(0, possibleBehaviours.Count)];
            spriteBehaviourTable.Entries[id] = spriteBehaviourTable.Entries[(int)newBehaviour];
        }

        public void RandomizeBehaviours(Table spriteBehaviourTable, Random random, Dictionary<Sprite.SpriteId, Sprite.SpriteId> enemyBehaviourDict)
        {
            var enemyBehaviours = new List<byte[]>();
            var enemyBaseBehaviourDict = new Dictionary<byte[], Sprite.SpriteId>();
            var excludedEnemies = new List<Sprite.SpriteId>() { Sprite.SpriteId.BurrowingCyclops };
            if (EnemyOptions.AISetting == EnemyOptions.AIShuffle.Partial)
            {
                excludedEnemies.Add(Sprite.SpriteId.RockLobster);
                excludedEnemies.Add(Sprite.SpriteId.KingGrieve);
                excludedEnemies.Add(Sprite.SpriteId.EvilOne);
            }

            for (int i = 0; i < spriteBehaviourTable.Entries.Count; i++)
            {
                var id = (Sprite.SpriteId)i;
                if (Sprite.enemies.Contains(id))
                {
                    if (excludedEnemies.Contains(id))
                    {
                        continue;
                    }

                    enemyBehaviours.Add(spriteBehaviourTable.Entries[i]);
                    enemyBaseBehaviourDict[spriteBehaviourTable.Entries[i]] = id;
                }
            }

            Util.ShuffleList(enemyBehaviours, 0, enemyBehaviours.Count - 1, random);
            int enemyCount = 0;

            for (int i = 0; i < spriteBehaviourTable.Entries.Count; i++)
            {
                if (Sprite.enemies.Contains((Sprite.SpriteId)i))
                {
                    var id = (Sprite.SpriteId)i;
                    if (excludedEnemies.Contains(id))
                    {
                        continue;
                    }

                    spriteBehaviourTable.Entries[i] = enemyBehaviours[enemyCount];
                    enemyBehaviourDict[id] = enemyBaseBehaviourDict[enemyBehaviours[enemyCount]];
                    enemyCount++;
                }
            }

            foreach (var oldId in excludedEnemies)
            {
                var newId = enemyBehaviours[random.Next(enemyBehaviours.Count)];
                spriteBehaviourTable.Entries[(int)oldId] = newId;
                enemyBehaviourDict[oldId] = enemyBaseBehaviourDict[newId];
            }
        }

        public void UpdateSpriteValues(int id, byte hp, byte damage, byte experience, byte rewardType,
                                       Table enemyHPTable,
                                       Table enemyDamageTable,
                                       Table enemyExperienceTable,
                                       Table enemyRewardTypeTable)
        {
            enemyHPTable.Entries[id][0] = hp;
            enemyDamageTable.Entries[id][0] = damage;
            enemyExperienceTable.Entries[id][0] = experience;
            enemyRewardTypeTable.Entries[id][0] = rewardType;
        }

        private void RandomizeTableCeiling(Table table, int ceiling, Random random)
        {
            for (int i = 0; i < table.Entries.Count; i++)
            {
                byte value = table.Entries[i][0];
                value = GetNewValue(value, ceiling, random);
                table.Entries[i][0] = value;
            }
        }

        private void RandomizeTableFraction(Table table, double fraction, Random random)
        {
            for (int i = 0; i < table.Entries.Count; i++)
            {
                byte value = table.Entries[i][0];
                value = GetNewValue(value, (int)(value * fraction), random);
                table.Entries[i][0] = value;
            }
        }

        private void IncreaseTableEntries(Table table, double fraction, Random random)
        {
            for (int i = 0; i < table.Entries.Count; i++)
            {
                byte value = table.Entries[i][0];
                value = (byte)Math.Min(255, value + value * fraction);
                table.Entries[i][0] = value;
            }
        }

        private byte GetNewValue(int value, int ceiling, Random random)
        {
            int modification = random.Next(0, ceiling);
            if (random.Next(0, 2) == 0)
            {
                value = Math.Min(value + modification, 255);
            }
            else
            {
                value = Math.Max(value - modification, 0);
            }

            if (value < 4)
            {
                if (random.Next(0, 4) != 0)
                {
                    value = 4;
                }
            }

            return (byte)value;
        }
    }
}
