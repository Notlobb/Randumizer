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

        public void RandomizeBehaviourProperties(byte[] content, Random random)
        {
            // Behaviour for 0x2 (Coin)
            if (random.Next(0, 2) == 0)
            {
                //Coin vertical dropping height
                content[Section.GetOffset(14, 0x8D1B, 0x8000)] = 0;
            }

            //Coin vertical bouncing height
            content[Section.GetOffset(14, 0x8D55, 0x8000)] = (byte)random.Next(2, 8);
            if (random.Next(0, 2) == 0)
            {
                //Invert coin floating
                content[Section.GetOffset(14, 0x8D3F, 0x8000)] = 0;
            }

            // Behaviour for 0x3 (Rock)
            content[Section.GetOffset(14, 0x9E97, 0x8000)] = (byte)random.Next(1, 7);

            // Behaviour for 0x4 (Luigi)
            // First jump
            content[Section.GetOffset(14, 0xAE20, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAE21, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAE22, 0x8000)] = (byte)random.Next(0, 5);
            content[Section.GetOffset(14, 0xAA76, 0x8000)] = (byte)random.Next(0, 65);

            // Second jump
            content[Section.GetOffset(14, 0xAE38, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAE39, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAE3A, 0x8000)] = (byte)random.Next(0, 5);

            // Horizontal movement
            content[Section.GetOffset(14, 0xAE1B, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAE1C, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAE26, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAE27, 0x8000)] = (byte)random.Next(0, 3);

            // Behaviour for 0x5 (NecronAides)
            if (random.Next(0, 2) == 0)
            {
                //Change climbing tile type to 0 (air)
                content[Section.GetOffset(14, 0x8DD9, 0x8000)] = 0;
            }

            // Horizontal movement
            content[Section.GetOffset(14, 0x8E2E, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x8E33, 0x8000)] = (byte)random.Next(0, 3);

            // Behaviour for 0x6 (Zombie)
            // Horizontal movement speeds
            content[Section.GetOffset(14, 0xAF09, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAF0A, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAF13, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAF14, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAF1D, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAF1E, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAF27, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAF28, 0x8000)] = (byte)random.Next(0, 3);

            // Horizontal movement durations
            content[Section.GetOffset(14, 0xAF08, 0x8000)] = GetRandomDuration(2, 100, random);
            content[Section.GetOffset(14, 0xAF12, 0x8000)] = GetRandomDuration(2, 100, random);
            content[Section.GetOffset(14, 0xAF1C, 0x8000)] = GetRandomDuration(2, 100, random);
            content[Section.GetOffset(14, 0xAF26, 0x8000)] = GetRandomDuration(2, 100, random);

            // Pause durations
            content[Section.GetOffset(14, 0xAF0D, 0x8000)] = GetRandomDuration(2, 60, random);
            content[Section.GetOffset(14, 0xAF17, 0x8000)] = GetRandomDuration(2, 60, random);
            content[Section.GetOffset(14, 0xAF21, 0x8000)] = GetRandomDuration(2, 60, random);
            content[Section.GetOffset(14, 0xAF2B, 0x8000)] = GetRandomDuration(2, 60, random);

            // Behaviour for 0x7 (Hornet)
            // Horizontal movement speed
            content[Section.GetOffset(14, 0x8E88, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x8E8D, 0x8000)] = (byte)random.Next(0, 3);

            // Vertical movement
            content[Section.GetOffset(14, 0x8EB0, 0x8000)] = GetRandomBitSet(random);
            content[Section.GetOffset(14, 0x8EB1, 0x8000)] = GetRandomBitSet(random);
            content[Section.GetOffset(14, 0x8EB2, 0x8000)] = GetRandomBitSet(random);
            content[Section.GetOffset(14, 0x8EB3, 0x8000)] = GetRandomBitSet(random);
            content[Section.GetOffset(14, 0x8EB4, 0x8000)] = GetRandomBitSet(random);
            content[Section.GetOffset(14, 0x8EB5, 0x8000)] = GetRandomBitSet(random);
            content[Section.GetOffset(14, 0x8EB6, 0x8000)] = GetRandomBitSet(random);
            content[Section.GetOffset(14, 0x8EB7, 0x8000)] = GetRandomBitSet(random);

            content[Section.GetOffset(14, 0x8EB8, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x8EB9, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x8EBA, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x8EBB, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x8EBC, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x8EBD, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x8EBE, 0x8000)] = (byte)random.Next(0, 3);

            // Behaviour for 0x8 (Metroid)
            content[Section.GetOffset(14, 0x8EE5, 0x8000)] = (byte)random.Next(2, 7);
            content[Section.GetOffset(14, 0x8EEA, 0x8000)] = (byte)random.Next(2, 7);
            content[Section.GetOffset(14, 0x8F00, 0x8000)] = (byte)random.Next(2, 7);
            content[Section.GetOffset(14, 0x8F05, 0x8000)] = (byte)random.Next(2, 7);

            // Behaviour for 0x9 (Spawned ghost)
            content[Section.GetOffset(14, 0x8F5A, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x8F5F, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x8F67, 0x8000)] = (byte)random.Next(2, 7);
            content[Section.GetOffset(14, 0x8F6F, 0x8000)] = (byte)random.Next(2, 7);
            content[Section.GetOffset(14, 0x8F9E, 0x8000)] = (byte)random.Next(2, 7);
            content[Section.GetOffset(14, 0x8FA3, 0x8000)] = (byte)random.Next(2, 7);

            // Behaviour for 0xB (Ghost)
            content[Section.GetOffset(14, 0x8FFD, 0x8000)] = (byte)random.Next(2, 7);
            content[Section.GetOffset(14, 0x9002, 0x8000)] = (byte)random.Next(2, 7);

            // Behaviour for 0x0C (Snowman)
            // Jump
            content[Section.GetOffset(14, 0xAEFD, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAEFD, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAEFE, 0x8000)] = (byte)random.Next(0, 5);

            // Horizontal speed
            content[Section.GetOffset(14, 0xAEEF, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAEF0, 0x8000)] = (byte)random.Next(0, 3);
            if (random.Next(0, 2) == 0)
            {
                // Swap far/close patterns
                byte temp1 = content[Section.GetOffset(14, 0xAEF4, 0x8000)];
                byte temp2 = content[Section.GetOffset(14, 0xAEF5, 0x8000)];
                content[Section.GetOffset(14, 0xAEF4, 0x8000)] = content[Section.GetOffset(14, 0xAEF6, 0x8000)];
                content[Section.GetOffset(14, 0xAEF5, 0x8000)] = content[Section.GetOffset(14, 0xAEF7, 0x8000)];
                content[Section.GetOffset(14, 0xAEF6, 0x8000)] = temp1;
                content[Section.GetOffset(14, 0xAEF7, 0x8000)] = temp2;
            }

            // Behaviour for 0x0D (Nash)
            // Randomize projectile
            content[Section.GetOffset(14, 0xA0AE, 0x8000)] = (byte)GetRandomProjectile(random);
            // Randomize pause
            content[Section.GetOffset(14, 0x9066, 0x8000)] = GetRandomDuration(2, 150, random);

            // Behaviour for 0x0E (Fire giant)
            content[Section.GetOffset(14, 0xAF4E, 0x8000)] = GetRandomDuration(2, 100, random);
            content[Section.GetOffset(14, 0xAF4F, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAF50, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAF44, 0x8000)] = (byte)random.Next(4, 150);

            // Behaviour for 0x0F (Blue mage)
            content[Section.GetOffset(14, 0x9148, 0x8000)] = (byte)random.Next(2, 120);
            content[Section.GetOffset(14, 0x9171, 0x8000)] = (byte)random.Next(24, 60);
            content[Section.GetOffset(14, 0x914F, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x9154, 0x8000)] = (byte)random.Next(0, 3);

            // Behaviour for 0x10 (Execution hood)
            content[Section.GetOffset(14, 0x91CD, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x91D2, 0x8000)] = (byte)random.Next(0, 3);

            // Behaviour for 0x11 (Table)
            content[Section.GetOffset(14, 0xB0B0, 0x8000)] = (byte)random.Next(2, 80);
            content[Section.GetOffset(14, 0xB0B1, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB0B2, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB0B9, 0x8000)] = (byte)random.Next(2, 80);
            content[Section.GetOffset(14, 0xB0BA, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB0BB, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB0C5, 0x8000)] = (byte)random.Next(2, 120);
            content[Section.GetOffset(14, 0xB0CD, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB0CE, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB0CF, 0x8000)] = (byte)random.Next(0, 5);

            // Behaviour for 0x15 (Cloaked mage)
            content[Section.GetOffset(14, 0x9254, 0x8000)] = (byte)GetRandomProjectile(random);
            content[Section.GetOffset(14, 0xAF6C, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xAF6F, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xAF70, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAF71, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAF76, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xAF79, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xAF7A, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAF7B, 0x8000)] = (byte)random.Next(0, 3);

            // Behaviour for 0x17 (Raisin)
            content[Section.GetOffset(14, 0xAF93, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAF94, 0x8000)] = (byte)random.Next(0, 3);

            // Behaviour for 0x18 (Psychic)
            if (EnemyOptions.AIPropertySetting == EnemyOptions.AIProperrtyRandomization.Full)
            {
                content[Section.GetOffset(14, 0xAF9C, 0x8000)] = GetRandomDuration(4, 60, random);
                content[Section.GetOffset(14, 0xAFA1, 0x8000)] = GetRandomDuration(4, 60, random);
                content[Section.GetOffset(14, 0xAFA4, 0x8000)] = GetRandomDuration(4, 60, random);
                content[Section.GetOffset(14, 0xAFA9, 0x8000)] = GetRandomDuration(4, 60, random);
                content[Section.GetOffset(14, 0xAFAC, 0x8000)] = GetRandomDuration(4, 60, random);

                content[Section.GetOffset(14, 0xAF9D, 0x8000)] = (byte)random.Next(0, 200);
                content[Section.GetOffset(14, 0xAF9E, 0x8000)] = (byte)random.Next(0, 3);

                content[Section.GetOffset(14, 0xAFA5, 0x8000)] = (byte)random.Next(0, 200);
                content[Section.GetOffset(14, 0xAFA6, 0x8000)] = (byte)random.Next(0, 3);

                content[Section.GetOffset(14, 0xAFAD, 0x8000)] = (byte)random.Next(0, 200);
                content[Section.GetOffset(14, 0xAFAE, 0x8000)] = (byte)random.Next(0, 3);
            }

            // Behaviour for 0x19 (Mario)
            content[Section.GetOffset(14, 0xAFB9, 0x8000)] = GetRandomDuration(2, 30, random);
            content[Section.GetOffset(14, 0xAFC0, 0x8000)] = GetRandomDuration(2, 30, random);
            content[Section.GetOffset(14, 0xAFC5, 0x8000)] = GetRandomDuration(2, 30, random);
            content[Section.GetOffset(14, 0xAFCC, 0x8000)] = GetRandomDuration(2, 30, random);
            content[Section.GetOffset(14, 0xAFD1, 0x8000)] = GetRandomDuration(2, 30, random);
            content[Section.GetOffset(14, 0xAFD8, 0x8000)] = GetRandomDuration(2, 30, random);
            content[Section.GetOffset(14, 0xAFE8, 0x8000)] = GetRandomDuration(2, 60, random);
            content[Section.GetOffset(14, 0xAFE9, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAFEA, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAFF9, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAFFA, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAFFB, 0x8000)] = (byte)random.Next(0, 5);

            // Behaviour for 0x1A (Giant bees)
            content[Section.GetOffset(14, 0x92F9, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x92FE, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x9338, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x933D, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x9364, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x9369, 0x8000)] = (byte)random.Next(0, 3);

            // Behaviour for 0x1B (Myconid)
            content[Section.GetOffset(14, 0xB023, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xB024, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB025, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB028, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xB02D, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xB02E, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB02F, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB032, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xB035, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xB036, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB037, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB03B, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB03C, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB03D, 0x8000)] = (byte)random.Next(0, 5);
            content[Section.GetOffset(14, 0xB040, 0x8000)] = GetRandomDuration(2, 40, random);

            // Behaviour for 0x1C (Naga)
            content[Section.GetOffset(14, 0x93FC, 0x8000)] = (byte)random.Next(2, 7);
            content[Section.GetOffset(14, 0x9401, 0x8000)] = (byte)random.Next(2, 7);

            // Behaviour for 0x1E (Giant strider)
            content[Section.GetOffset(14, 0xB04C, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xB04D, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB04E, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB052, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB053, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB054, 0x8000)] = (byte)random.Next(0, 5);

            // Behaviour for 0x1F (Sir Gawaine)
            content[Section.GetOffset(14, 0x94B5, 0x8000)] = (byte)random.Next(0, 50);
            content[Section.GetOffset(14, 0x94BE, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x94C3, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x94FC, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x9501, 0x8000)] = (byte)random.Next(0, 200);

            // Behaviour for 0x20 (Jason)
            content[Section.GetOffset(14, 0xB082, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xB083, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB084, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB088, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB089, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB08A, 0x8000)] = (byte)random.Next(0, 5);
            content[Section.GetOffset(14, 0xB08F, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xB090, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB091, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB095, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB096, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB097, 0x8000)] = (byte)random.Next(0, 5);
            content[Section.GetOffset(14, 0xB09C, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xB09D, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB09E, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB0A2, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB0A3, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB0A4, 0x8000)] = (byte)random.Next(0, 5);

            // Behaviour for 0x22 (Yareeka)
            content[Section.GetOffset(14, 0x9566, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x956B, 0x8000)] = (byte)random.Next(0, 200);
            byte value = (byte)random.Next(0, 100);
            content[Section.GetOffset(14, 0x9559, 0x8000)] = value;
            content[Section.GetOffset(14, 0x9560, 0x8000)] = value;
            content[Section.GetOffset(14, 0x9587, 0x8000)] = (byte)random.Next(0, 200);

            // Behaviour for 0x23 (TeleCreature)
            content[Section.GetOffset(14, 0x95C6, 0x8000)] = (byte)random.Next(0, 120);
            content[Section.GetOffset(14, 0x95D0, 0x8000)] = (byte)random.Next(0, 120);
            content[Section.GetOffset(14, 0x95E9, 0x8000)] = (byte)random.Next(0, 120);

            // Behaviour for 0x24 (Jouster)
            content[Section.GetOffset(14, 0x9677, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x967C, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x96B9, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x96BE, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x9638, 0x8000)] = (byte)random.Next(0, 80);

            // Behaviour for 0x26 (Cricket)
            content[Section.GetOffset(14, 0xAE90, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xAE91, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAE92, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAE9E, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAE9F, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAEA0, 0x8000)] = (byte)random.Next(0, 5);
            content[Section.GetOffset(14, 0xAEA3, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xAEA4, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAEA5, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAEAB, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAEAC, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAEAD, 0x8000)] = (byte)random.Next(0, 5);

            // Behaviour for 0x27 (Slug)
            content[Section.GetOffset(14, 0x9752, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x9757, 0x8000)] = (byte)random.Next(0, 3);
            value = (byte)random.Next(0, 50);
            content[Section.GetOffset(14, 0x973C, 0x8000)] = value;
            value += (byte)random.Next(0, 50);
            content[Section.GetOffset(14, 0x9740, 0x8000)] = value;
            value += (byte)random.Next(0, 50);
            content[Section.GetOffset(14, 0x9744, 0x8000)] = value;
            value += (byte)random.Next(0, 50);
            content[Section.GetOffset(14, 0x9748, 0x8000)] = value;

            // Behaviour for 0x28 (Lamprey)
            content[Section.GetOffset(14, 0xB06C, 0x8000)] = GetRandomDuration(2, 100, random);
            content[Section.GetOffset(14, 0xB06D, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB06E, 0x8000)] = (byte)random.Next(0, 3);

            // Behaviour for 0x2A (Monodron)
            content[Section.GetOffset(14, 0xAED0, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xAED4, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAED5, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAED6, 0x8000)] = (byte)random.Next(0, 5);
            content[Section.GetOffset(14, 0xAEDB, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0xAEDF, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAEE0, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xAEE1, 0x8000)] = (byte)random.Next(0, 5);

            // Behaviour for 0x2B (Bat)
            content[Section.GetOffset(14, 0x97C5, 0x8000)] = (byte)random.Next(1, 6);
            content[Section.GetOffset(14, 0x97CA, 0x8000)] = (byte)random.Next(1, 6);
            content[Section.GetOffset(14, 0x9815, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x9810, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x9845, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x9840, 0x8000)] = (byte)random.Next(0, 3);

            // Behaviour for 0x2C (BurrowingCyclops)
            content[Section.GetOffset(14, 0x988B, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0x9893, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0x989B, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0x98A3, 0x8000)] = GetRandomDuration(2, 40, random);

            // Behaviour for 0x2D (Wyvern)
            // Don't update projectile due to graphical glitches
            //content[Section.GetOffset(14, 0xA0FC, 0x8000)] = (byte)GetRandomProjectile(random);
            content[Section.GetOffset(14, 0x9BDF, 0x8000)] = (byte)random.Next(2, 7);
            content[Section.GetOffset(14, 0x9BE4, 0x8000)] = (byte)random.Next(2, 7);
            content[Section.GetOffset(14, 0x9C1D, 0x8000)] = (byte)random.Next(2, 6);
            content[Section.GetOffset(14, 0x9C44, 0x8000)] = (byte)random.Next(2, 6);
            content[Section.GetOffset(14, 0x9C4B, 0x8000)] = (byte)random.Next(0, 200);

            // Behaviour for 0x2E (DogBoss)
            content[Section.GetOffset(14, 0xB0E3, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB0E4, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB0E5, 0x8000)] = (byte)random.Next(0, 5);
            content[Section.GetOffset(14, 0xB102, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB103, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB104, 0x8000)] = (byte)random.Next(0, 5);

            // Behaviour for 0x2F (BigSnake)
            content[Section.GetOffset(14, 0x9CAB, 0x8000)] = GetRandomDuration(2, 40, random);
            content[Section.GetOffset(14, 0x9CB9, 0x8000)] = (byte)random.Next(0, 4);
            content[Section.GetOffset(14, 0x9CBE, 0x8000)] = (byte)random.Next(0, 4);

            // Behaviour for 0x30 (Nest)
            value = (byte)GetRandomEnemy(random);
            content[Section.GetOffset(14, 0x9D67, 0x8000)] = value;
            content[Section.GetOffset(14, 0xC2B8, 0x8000)] = value;
            content[Section.GetOffset(14, 0x9D82, 0x8000)] = value;

            // Behaviour for 0x31 (RockLobster)
            content[Section.GetOffset(14, 0x9E42, 0x8000)] = (byte)random.Next(10, 60);
            content[Section.GetOffset(14, 0x9DE8, 0x8000)] = (byte)random.Next(5, 50);
            //TODO morph rocks into something else?
            //if (random.Next(0, 2) == 0)
            //{
            //    content[Section.GetOffset(14, 0x9E19, 0x8000)] = (byte)Sprite.SpriteId.EvilOneProjectile;
            //    content[Section.GetOffset(14, 0x9E01, 0x8000)] = (byte)Sprite.SpriteId.EvilOneProjectile;
            //}

            // Behaviour for 0x32 (King Grieve)
            content[Section.GetOffset(14, 0x9F2F, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x9F34, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x9FA9, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0x9FA7, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0x9FBF, 0x8000)] = GetRandomDuration(2, 100, random);
            content[Section.GetOffset(14, 0x9F69, 0x8000)] = (byte)random.Next(10, 65);

            // Behaviour for 0x33 (Evil One)
            content[Section.GetOffset(14, 0xA133, 0x8000)] = (byte)GetRandomProjectile(random);
            content[Section.GetOffset(14, 0xA064, 0x8000)] = (byte)random.Next(0, 20);
            content[Section.GetOffset(14, 0xA065, 0x8000)] = (byte)random.Next(0, 20);
            content[Section.GetOffset(14, 0xA066, 0x8000)] = (byte)random.Next(0, 20);
            content[Section.GetOffset(14, 0xA067, 0x8000)] = (byte)random.Next(0, 20);
            content[Section.GetOffset(14, 0xA068, 0x8000)] = (byte)random.Next(0, 20);
            content[Section.GetOffset(14, 0xA069, 0x8000)] = (byte)random.Next(0, 20);
            content[Section.GetOffset(14, 0xA06A, 0x8000)] = (byte)random.Next(0, 20);
            content[Section.GetOffset(14, 0xA06B, 0x8000)] = (byte)random.Next(0, 20);
            content[Section.GetOffset(14, 0xA06C, 0x8000)] = (byte)random.Next(0, 20);
            content[Section.GetOffset(14, 0xA06D, 0x8000)] = (byte)random.Next(0, 20);

            // Behaviour for 0x46 (Eye)
            content[Section.GetOffset(14, 0xAE48, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE49, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE4A, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE4B, 0x8000)] = (byte)random.Next(0, 2);

            content[Section.GetOffset(14, 0xAE5C, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE5D, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE5E, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE5F, 0x8000)] = (byte)random.Next(0, 2);

            content[Section.GetOffset(14, 0xAE70, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE71, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE72, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE73, 0x8000)] = (byte)random.Next(0, 2);

            content[Section.GetOffset(14, 0xAE79, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE7A, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE7B, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE7C, 0x8000)] = (byte)random.Next(0, 2);

            content[Section.GetOffset(14, 0xAE85, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE86, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE87, 0x8000)] = (byte)random.Next(0, 2);
            content[Section.GetOffset(14, 0xAE88, 0x8000)] = (byte)random.Next(0, 2);

            // Behaviour for 0x47 (Spiky)
            content[Section.GetOffset(14, 0xAEB5, 0x8000)] = GetRandomDuration(2, 80, random);
            content[Section.GetOffset(14, 0xAEB6, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xAEB7, 0x8000)] = (byte)random.Next(0, 3);

            // Behaviour for 0x34 (Citizen 1)
            content[Section.GetOffset(14, 0xB126, 0x8000)] = GetRandomDuration(2, 100, random);
            content[Section.GetOffset(14, 0xB127, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB128, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB12B, 0x8000)] = GetRandomDuration(2, 100, random);

            // Behaviour for 0x41 (Citizen 2)
            content[Section.GetOffset(14, 0xB1CE, 0x8000)] = GetRandomDuration(2, 100, random);
            content[Section.GetOffset(14, 0xB1CF, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB1D0, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB1D3, 0x8000)] = GetRandomDuration(2, 100, random);
            content[Section.GetOffset(14, 0xB1D6, 0x8000)] = GetRandomDuration(2, 100, random);
            content[Section.GetOffset(14, 0xB1D7, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB1D8, 0x8000)] = (byte)random.Next(0, 3);

            // Behaviour for 0x42 (Guard)
            content[Section.GetOffset(14, 0xB1E0, 0x8000)] = GetRandomDuration(2, 100, random);
            content[Section.GetOffset(14, 0xB1E1, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB1E2, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB1E5, 0x8000)] = GetRandomDuration(2, 100, random);

            // Behaviour for 0x44 (Nurse)
            content[Section.GetOffset(14, 0xB1ED, 0x8000)] = GetRandomDuration(2, 100, random);
            content[Section.GetOffset(14, 0xB1EE, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB1EF, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB1F2, 0x8000)] = GetRandomDuration(2, 100, random);

            // Behaviour for 0x45 (Citizen 3)
            content[Section.GetOffset(14, 0xB1FC, 0x8000)] = GetRandomDuration(2, 100, random);
            content[Section.GetOffset(14, 0xB1FD, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB1FE, 0x8000)] = (byte)random.Next(0, 3);
            content[Section.GetOffset(14, 0xB203, 0x8000)] = GetRandomDuration(2, 100, random);
            content[Section.GetOffset(14, 0xB204, 0x8000)] = (byte)random.Next(0, 200);
            content[Section.GetOffset(14, 0xB205, 0x8000)] = (byte)random.Next(0, 3);
        }

        private byte GetRandomBitSet(Random random)
        {
            byte value = (byte)random.Next(0, 8);
            if (value < 3)
            {
                return value;
            }
            else if (value == 3)
            {
                return 0x4;
            }
            else if (value == 4)
            {
                return 0x8;
            }
            else if (value == 5)
            {
                return 0x10;
            }
            else if (value == 6)
            {
                return 0x20;
            }
            else if (value == 6)
            {
                return 0x40;
            }
            else if (value == 7)
            {
                return 0x80;
            }

            return value;
        }

        byte GetRandomDuration(int min, int max, Random random)
        {
            if (random.Next(0, 2) == 0)
            {
                max /= 2;
            }

            byte duration = (byte)random.Next(min, max);
            return duration;
        }

        private Sprite.SpriteId GetRandomProjectile(Random random)
        {
            var candidates = new List<Sprite.SpriteId>(Sprite.Projectiles);
            candidates.Remove(Sprite.SpriteId.Fireball);
            if (EnemyOptions.AIPropertySetting == EnemyOptions.AIProperrtyRandomization.Partial)
            {
                candidates.Remove(Sprite.SpriteId.EvilOneProjectile);
            }

            return candidates[random.Next(candidates.Count)];
        }

        private Sprite.SpriteId GetRandomEnemy(Random random)
        {
            var candidates = new List<Sprite.SpriteId>(Sprite.tallIds);
            candidates.Remove(Sprite.SpriteId.Jason);
            candidates.AddRange(Sprite.shortIds);
            candidates.AddRange(Sprite.flyingIds);
            return candidates[random.Next(candidates.Count)];
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
