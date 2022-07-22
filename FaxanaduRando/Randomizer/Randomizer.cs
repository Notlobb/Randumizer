using System;
using System.Collections.Generic;
using System.IO;

namespace FaxanaduRando.Randomizer
{
    public class Randomizer
    {
        public bool Randomize(string inputFile, string flags, int seed, out string message)
        {
            message = "Failed to find a valid randomization for this seed";
            Random random = new Random(seed);

            #if !DEBUG
            if (GeneralOptions.GenerateSpoilerLog)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (random.Next(0, 2) > 2)
                    {
                        return false;
                    }
                }
            }
            #endif

            byte[] content = File.ReadAllBytes(inputFile);
            AddMiscHacks(content, random);

            var levels = GetLevels(content);
            var doorRandomizer = new DoorRandomizer(content, random);
            var shopRandomizer = new ShopRandomizer(content, doorRandomizer);
            var giftRandomizer = new GiftRandomizer(content);
            doorRandomizer.UpdateBuildings(giftRandomizer, shopRandomizer);

            var spriteBehaviourTable = new Table(Section.GetOffset(14, 0xAD2D, 0x8000), 100, 2, content);
            if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Mixed)
            {
                UpdateGiftItemLogic(levels, content, spriteBehaviourTable);
            }

            var itemRandomizer = new ItemRandomizer(random);
            if (!itemRandomizer.ShuffleItems(levels, shopRandomizer, giftRandomizer, doorRandomizer, content, out uint attempts))
            {
                return false;
            }

            doorRandomizer.FinalizeWorlds(levels, random);

            if (ItemOptions.AlwaysSpawnSmallItems)
            {
                spriteBehaviourTable.Entries[(int)Sprite.SpriteId.HourGlass] =
                    spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Wingboots];
                spriteBehaviourTable.Entries[(int)Sprite.SpriteId.RedPotion2] =
                    spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Wingboots];
                spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Ointment2] =
                    spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Wingboots];
                spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Poison2] =
                    spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Wingboots];

                if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Unchanged)
                {
                    spriteBehaviourTable.Entries[(int)Sprite.SpriteId.MattockOrRingRuby] =
                        spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Wingboots];
                    spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Glove2OrKeyJoker] =
                        spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Wingboots];
                }

                var section = new Section();
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.AddToContent(content, Section.GetOffset(14, 0xA506, 0x8000));
            }

            if (ItemOptions.BigItemSpawns == ItemOptions.BigItemSpawning.AlwaysLockBehindBosses)
            {
                var section = new Section();
                section.Bytes.Add(OpCode.JSR);
                section.Bytes.Add(0x00);
                section.Bytes.Add(0xBF);
                section.AddToContent(content, Section.GetOffset(14, 0xA379, 0x8000));
                section.AddToContent(content, Section.GetOffset(14, 0xA3A9, 0x8000));
                section.AddToContent(content, Section.GetOffset(14, 0xA3E4, 0x8000));
                section.AddToContent(content, Section.GetOffset(14, 0xA408, 0x8000));
                section.AddToContent(content, Section.GetOffset(14, 0xA42C, 0x8000));
                section.AddToContent(content, Section.GetOffset(14, 0xA450, 0x8000));
                section.AddToContent(content, Section.GetOffset(14, 0xA474, 0x8000));

                section = new Section();
                section.Bytes.Add(OpCode.TXA);
                section.Bytes.Add(OpCode.PHA);
                section.Bytes.Add(OpCode.LDYImmediate);
                section.Bytes.Add(0x07);
                section.Bytes.Add(OpCode.LDAAbsoluteY);
                section.Bytes.Add(0xCC);
                section.Bytes.Add(0x02);
                section.Bytes.Add(OpCode.TAX);
                section.Bytes.Add(OpCode.LDAAbsoluteX);
                section.Bytes.Add(0x44);
                section.Bytes.Add(0xB5);
                section.Bytes.Add(OpCode.CMPImmediate);
                section.Bytes.Add(0x07);
                section.Bytes.Add(OpCode.BNE);
                section.Bytes.Add(0x04);
                section.Bytes.Add(OpCode.PLA);
                section.Bytes.Add(OpCode.TAX);
                section.Bytes.Add(OpCode.CLC);
                section.Bytes.Add(OpCode.RTS);
                section.Bytes.Add(OpCode.DEY);
                section.Bytes.Add(OpCode.TYA);
                section.Bytes.Add(OpCode.BPL);
                section.Bytes.Add(0x04);
                section.Bytes.Add(OpCode.PLA);
                section.Bytes.Add(OpCode.TAX);
                section.Bytes.Add(OpCode.SEC);
                section.Bytes.Add(OpCode.RTS);
                section.Bytes.Add(OpCode.JMPAbsolute);
                section.Bytes.Add(0x04);
                section.Bytes.Add(0xBF);
                section.AddToContent(content, Section.GetOffset(14, 0xBF00, 0x8000));

                spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Rod] =
                    spriteBehaviourTable.Entries[(int)Sprite.SpriteId.WingbootsBossLocked];
            }
            else if (ItemOptions.BigItemSpawns == ItemOptions.BigItemSpawning.AlwaysSpawn)
            {
                var section = new Section();
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.AddToContent(content, Section.GetOffset(14, 0xA379, 0x8000));
                section.AddToContent(content, Section.GetOffset(14, 0xA3A9, 0x8000));
                section.AddToContent(content, Section.GetOffset(14, 0xA3E4, 0x8000));
                section.AddToContent(content, Section.GetOffset(14, 0xA408, 0x8000));
                section.AddToContent(content, Section.GetOffset(14, 0xA42C, 0x8000));
                section.AddToContent(content, Section.GetOffset(14, 0xA450, 0x8000));
                section.AddToContent(content, Section.GetOffset(14, 0xA474, 0x8000));
            }

            if (ItemOptions.BigItemSpawns != ItemOptions.BigItemSpawning.Unchanged)
            {
                spriteBehaviourTable.Entries[(int)Sprite.SpriteId.MattockBossLocked] =
                    spriteBehaviourTable.Entries[(int)Sprite.SpriteId.WingbootsBossLocked];

                UpdateWingBootsQuest(content);

                var section = new Section();
                section.Bytes.Add(OpCode.JMPAbsolute);
                section.Bytes.Add(0x23);
                section.Bytes.Add(0xA5);
                section.Bytes.Add(OpCode.RTS);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.AddToContent(content, Section.GetOffset(14, 0xA3B4, 0x8000));
            }

            if (ItemOptions.RandomizeBarRank)
            {
                byte rank = (byte)random.Next(6, 11);
                var section = new Section();
                section.Bytes.Add(rank);
                section.AddToContent(content, Section.GetOffset(12, 0xA254, 0x8000));
                giftRandomizer.BarRank = rank;
            }

            var ememyRandomizer = new EnemyRandomizer();
            Screen.SetupEnemyIds();
            var enemyHPTable = new Table(Section.GetOffset(14, 0xB5A9, 0x8000), 100, 1, content);
            var enemyDamageTable = new Table(Section.GetOffset(14, 0xB6D7, 0x8000), 100, 1, content);
            var enemyExperienceTable = new Table(Section.GetOffset(14, 0xB60E, 0x8000), 100, 1, content);
            var enemyRewardTypeTable = new Table(Section.GetOffset(14, 0xB672, 0x8000), 100, 1, content);
            var enemyRewardQuantityTable = new Table(Section.GetOffset(14, 0xACED, 0x8000), 63, 1, content);
            var magicResistanceTable = new Table(Section.GetOffset(14, 0xB73B, 0x8000), 100, 1, content);

            #if DEBUG
            var spriteTypes = new Dictionary<Sprite.SpriteId, SpriteType>();
            for (int i = 0; i < 100; i++)
            {
                var id = (Sprite.SpriteId)i;
                var hp = enemyHPTable.Entries[i][0];
                var damage = enemyDamageTable.Entries[i][0];
                spriteTypes[id] = new SpriteType(id, hp, damage);
            }

            var spells = new Dictionary<Spell.Id, Spell>();
            spells[Spell.Id.Deluge] = new Spell(Spell.Id.Deluge, content[Section.GetOffset(14, 0xB7A9, 0x8000)],
                                                content[Section.GetOffset(14, 0xB7A0, 0x8000)]);
            spells[Spell.Id.Thunder] = new Spell(Spell.Id.Thunder, content[Section.GetOffset(14, 0xB7AA, 0x8000)],
                                                 content[Section.GetOffset(14, 0xB7A1, 0x8000)]);
            spells[Spell.Id.Fire] = new Spell(Spell.Id.Fire, content[Section.GetOffset(14, 0xB7AB, 0x8000)],
                                              content[Section.GetOffset(14, 0xB7A2, 0x8000)]);
            spells[Spell.Id.Death] = new Spell(Spell.Id.Death, content[Section.GetOffset(14, 0xB7AC, 0x8000)],
                                               content[Section.GetOffset(14, 0xB7A3, 0x8000)]);
            spells[Spell.Id.Tilte] = new Spell(Spell.Id.Tilte, content[Section.GetOffset(14, 0xB7AD, 0x8000)],
                                               content[Section.GetOffset(14, 0xB7A4, 0x8000)]);
            #endif
            if (EnemyOptions.EnemySet != EnemyOptions.EnemySetType.Unchanged)
            {
                foreach (var level in levels)
                {
                    level.RandomizeEnemies(random);
                }

                if (EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Hard ||
                    EnemyOptions.EnemySet == EnemyOptions.EnemySetType.VeryHard ||
                    EnemyOptions.EnemySet == EnemyOptions.EnemySetType.ExtremelyHard ||
                    EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Scaling)
                {
                    ememyRandomizer.RandomizeBehaviour((int)Sprite.SpriteId.StillKnight, spriteBehaviourTable, random);
                    ememyRandomizer.UpdateSpriteValues((int)Sprite.SpriteId.StillKnight, 40, 20, 100, 50, enemyHPTable, enemyDamageTable, enemyExperienceTable, enemyRewardTypeTable);
                }
            }

            if (EnemyOptions.EnemyHPSetting != EnemyOptions.EnemyHP.Unchanged)
            {
                ememyRandomizer.RandomizeEnemyHP(enemyHPTable, random);
            }

            if (EnemyOptions.EnemyDamageSetting != EnemyOptions.EnemyDamage.Unchanged)
            {
                ememyRandomizer.RandomizeEnemyDamage(enemyDamageTable, random);
            }

            if (EnemyOptions.RandomizeExperience)
            {
                ememyRandomizer.RandomizeExperience(enemyExperienceTable, random);
            }

            if (EnemyOptions.RandomizeRewards)
            {
                ememyRandomizer.RandomizeRewards(enemyRewardTypeTable, enemyRewardQuantityTable, random);
            }

            if (EnemyOptions.RandomizeMagicImmunities)
            {
                ememyRandomizer.RandomizeMagicImmunities(magicResistanceTable, random);
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

            doorRandomizer.AddToContent(content, random);

            if (GeneralOptions.DarkTowers)
            {
                content[Section.GetOffset(15, 0xDF53, 0xC000)] = PaletteRandomizer.DarkPalette;
                content[Section.GetOffset(15, 0xE56A, 0xC000)] = PaletteRandomizer.DarkPalette;

                if (!(GeneralOptions.ShuffleTowers && GeneralOptions.IncludeEvilOnesFortress))
                {
                    doorRandomizer.Doors[DoorId.FinalDoor].Requirement.palette = PaletteRandomizer.DarkPalette;
                    doorRandomizer.Doors[DoorId.FinalDoor].AddToContent(content);
                }
            }

            foreach (var level in levels)
            {
                level.WriteToContent(content);
            }

            spriteBehaviourTable.AddToContent(content);
            enemyHPTable.AddToContent(content);
            enemyDamageTable.AddToContent(content);
            enemyExperienceTable.AddToContent(content);
            enemyRewardTypeTable.AddToContent(content);
            enemyRewardQuantityTable.AddToContent(content);
            magicResistanceTable.AddToContent(content);

            var textRandomizer = new TextRandomizer(content, random);
            if (GeneralOptions.RandomizeTitles)
            {
                textRandomizer.RandomizeTitles(content, random);
            }

            if (!textRandomizer.UpdateText(shopRandomizer, giftRandomizer, doorRandomizer, content))
            {
                return false;
            }

            var titleText = Text.GetAllTitleText(content, Section.GetOffset(12, 0x9DCC, 0x8000),
                                                 Section.GetOffset(12, 0x9E0D, 0x8000));
            Text.AddTitleText(0, "RANDUMIZER V23", titleText);
            var hash = ((uint)flags.GetHashCode()).ToString();
            if (hash.Length > 8)
            {
                hash = hash.Substring(0, 8);
            }

            Text.AddTitleText(1, $"FLAG HASH {hash}", titleText);
            Text.AddTitleText(2, $"SEED {seed}", titleText);
            Text.SetAllTitleText(content, titleText, Section.GetOffset(12, 0x9DCC, 0x8000));

            int dotIndex = inputFile.IndexOf(".nes");
            string outputFile;
            #if DEBUG
            outputFile = inputFile.Insert(dotIndex, "_" + seed.ToString());
            #else
            outputFile = inputFile.Insert(dotIndex, "_" + seed.ToString() + "_" + flags);
            #endif

            RandomizeExtras(content, random, doorRandomizer, out byte finalPalette, out bool addSection);

            if (GeneralOptions.ShuffleTowers)
            {
                var newSection = new Section();
                newSection.Bytes.Add(OpCode.JMPAbsolute);
                newSection.Bytes.Add(0x00);
                newSection.Bytes.Add(0xFE);
                newSection.AddToContent(content, Section.GetOffset(15, 0xE565, 0xC000));

                newSection = new Section();
                newSection.Bytes.Add(OpCode.LDAImmediate);
                newSection.Bytes.Add(0x01);
                newSection.Bytes.Add(OpCode.STAAbsolute);
                newSection.Bytes.Add(0xFF);
                newSection.Bytes.Add(0x1F);
                newSection.Bytes.Add(OpCode.LDAAbsolute);
                newSection.Bytes.Add(0xFE);
                newSection.Bytes.Add(0x1F);
                newSection.Bytes.Add(OpCode.STAAbsolute);
                newSection.Bytes.Add(0x35);
                newSection.Bytes.Add(0x04);
                newSection.Bytes.Add(OpCode.JSR);
                newSection.Bytes.Add(0xDC);
                newSection.Bytes.Add(0xDA);
                newSection.Bytes.Add(OpCode.RTS);
                newSection.AddToContent(content, Section.GetOffset(15, 0xFE00, 0xC000));

                newSection = new Section();
                newSection.Bytes.Add(OpCode.JMPAbsolute);
                newSection.Bytes.Add(0x20);
                newSection.Bytes.Add(0xFE);
                newSection.Bytes.Add(OpCode.NOP);
                newSection.Bytes.Add(OpCode.NOP);
                newSection.AddToContent(content, Section.GetOffset(15, 0xDF1D, 0xC000));

                newSection = new Section();
                newSection.Bytes.Add(OpCode.LDAImmediate);
                newSection.Bytes.Add(0x00);
                newSection.Bytes.Add(OpCode.JSR);
                newSection.Bytes.Add(0x62);
                newSection.Bytes.Add(0xD0);
                newSection.Bytes.Add(OpCode.LDAAbsolute);
                newSection.Bytes.Add(0xFF);
                newSection.Bytes.Add(0x1F);
                newSection.Bytes.Add(OpCode.CMPImmediate);
                newSection.Bytes.Add(0x01);
                newSection.Bytes.Add(OpCode.BEQ);
                newSection.Bytes.Add(0x03);
                newSection.Bytes.Add(OpCode.JMPAbsolute);
                newSection.Bytes.Add(0x22);
                newSection.Bytes.Add(0xDF);
                newSection.Bytes.Add(OpCode.LDAImmediate);
                newSection.Bytes.Add(0x00);
                newSection.Bytes.Add(OpCode.STAAbsolute);
                newSection.Bytes.Add(0xFF);
                newSection.Bytes.Add(0x1F);
                newSection.Bytes.Add(OpCode.JMPAbsolute);
                newSection.Bytes.Add(0x46);
                newSection.Bytes.Add(0xDD);
                newSection.AddToContent(content, Section.GetOffset(15, 0xFE20, 0xC000));

                newSection = new Section();
                newSection.Bytes.Add(OpCode.JSR);
                newSection.Bytes.Add(0x40);
                newSection.Bytes.Add(0xFE);
                newSection.AddToContent(content, Section.GetOffset(15, 0xE84C, 0xC000));

                newSection = new Section();
                newSection.Bytes.Add(OpCode.TAY);
                newSection.Bytes.Add(OpCode.LSR);
                newSection.Bytes.Add(OpCode.LSR);
                newSection.Bytes.Add(OpCode.LSR);
                newSection.Bytes.Add(OpCode.LSR);
                newSection.Bytes.Add(OpCode.STAAbsolute);
                newSection.Bytes.Add(0xFE);
                newSection.Bytes.Add(0x1F);
                newSection.Bytes.Add(OpCode.TYA);
                newSection.Bytes.Add(OpCode.ANDImmediate);
                newSection.Bytes.Add(0x0F);
                newSection.Bytes.Add(OpCode.STAAbsolute);
                newSection.Bytes.Add(0x2B);
                newSection.Bytes.Add(0x04);
                newSection.Bytes.Add(OpCode.RTS);
                newSection.AddToContent(content, Section.GetOffset(15, 0xFE40, 0xC000));

                if (!addSection)
                {
                    newSection = new Section();
                    newSection.Bytes.Add(OpCode.JSR);
                    newSection.Bytes.Add(0x60);
                    newSection.Bytes.Add(0xFE);
                    newSection.AddToContent(content, Section.GetOffset(15, 0xE54E, 0xC000));

                    newSection = new Section();
                    newSection.Bytes.Add(OpCode.CMPImmediate);
                    newSection.Bytes.Add(PaletteRandomizer.BranchPalette);
                    newSection.Bytes.Add(OpCode.BNE);
                    newSection.Bytes.Add(0x04);
                    newSection.Bytes.Add(OpCode.LDAImmediate);
                    newSection.Bytes.Add(0x04);
                    newSection.Bytes.Add(OpCode.BNE);
                    newSection.Bytes.Add(0x0A);
                    newSection.Bytes.Add(OpCode.CMPImmediate);
                    newSection.Bytes.Add(finalPalette);
                    newSection.Bytes.Add(OpCode.BEQ);
                    newSection.Bytes.Add(0x04);
                    newSection.Bytes.Add(OpCode.CMPAbsoluteX);
                    newSection.Bytes.Add(0x69);
                    newSection.Bytes.Add(0xE5);
                    newSection.Bytes.Add(OpCode.RTS);
                    newSection.Bytes.Add(OpCode.LDAImmediate);
                    newSection.Bytes.Add(0x10);
                    newSection.Bytes.Add(OpCode.STAAbsolute);
                    newSection.Bytes.Add(0xFA);
                    newSection.Bytes.Add(0x00);
                    newSection.Bytes.Add(OpCode.STAAbsolute);
                    newSection.Bytes.Add(0xD1);
                    newSection.Bytes.Add(0x03);
                    newSection.Bytes.Add(OpCode.RTS);
                    newSection.AddToContent(content, Section.GetOffset(15, 0xFE60, 0xC000));
                }

                newSection = new Section();
                newSection.Bytes.Add(OpCode.JMPAbsolute);
                newSection.Bytes.Add(0xA0);
                newSection.Bytes.Add(0xFD);
                newSection.AddToContent(content, Section.GetOffset(15, 0xE5D7, 0xC000));

                newSection = new Section();
                newSection.Bytes.Add(OpCode.LDAImmediate);
                newSection.Bytes.Add(0x00);
                newSection.Bytes.Add(OpCode.STAAbsolute);
                newSection.Bytes.Add(0xFF);
                newSection.Bytes.Add(0x1F);
                newSection.Bytes.Add(OpCode.JMPAbsolute);
                newSection.Bytes.Add(0xDC);
                newSection.Bytes.Add(0xDA);
                newSection.AddToContent(content, Section.GetOffset(15, 0xFDA0, 0xC000));
            }

            File.WriteAllBytes(outputFile, content);
            if (GeneralOptions.GenerateSpoilerLog)
            {
                var spoilers = new List<string>();
                spoilers.Add("Randumizer v0.23");
                spoilers.Add($"Seed {seed}");
                spoilers.Add($"Flags {flags}");
                #if DEBUG
                spoilers.Add($"Randomization attempts: {attempts}");
                #endif

                var hints = textRandomizer.GetHints(shopRandomizer, giftRandomizer, doorRandomizer, true);
                foreach (var hint in hints)
                {
                    string spoiler = hint.Replace('�', ' ');
                    spoiler = hint.Replace('�', ' ');
                    spoiler = spoiler.Replace('�', ' ');
                    spoiler = spoiler.Replace('�', ' ');
                    spoiler = spoiler.Replace('�', ' ');
                    spoiler = spoiler.Replace('�', ' ');
                    spoilers.Add(spoiler);
                }

                File.WriteAllLines(outputFile.Replace(".nes", ".txt"), spoilers);
            }

            message = "Randomized ROM created at " + outputFile;
            return true;
        }

        private List<Level> GetLevels(byte[] content)
        {
            int index = 0;
            var levels = new List<Level>();
            while (index < Level.offsets.Count)
            {
                var offset = Level.offsets[index];
                var end = Level.ends[index];
                var level = new Level(offset, end, (WorldNumber)index);
                level.AddScreens(content);
                levels.Add(level);
                Level.LevelDict[level.Start] = level;
                index++;
            }

            AddSublevels(levels);
            SubLevel.SubLevelDict[SubLevel.Id.EastTrunk].Screens[2].Sprites[0].RequiresWingBoots = true;
            var branchHiddenItem = SubLevel.SubLevelDict[SubLevel.Id.EastBranch].Screens[7].Sprites[0];
            branchHiddenItem.RequiresWingBoots = true;
            SubLevel.SubLevelDict[SubLevel.Id.MasconTower].Screens[0].SkipBosses = true;
            SubLevel.SubLevelDict[SubLevel.Id.Dartmoor].Screens[11].SkipBosses = true;
            SubLevel.SubLevelDict[SubLevel.Id.CastleFraternal].Screens[3].SkipBosses = true;
            SubLevel.SubLevelDict[SubLevel.Id.CastleFraternal].Screens[4].SkipBosses = true;
            SubLevel.SubLevelDict[SubLevel.Id.EvilOnesLair].Screens[14].SkipBosses = true;
            if (ItemOptions.BigItemSpawns != ItemOptions.BigItemSpawning.AlwaysSpawn &&
                (EnemyOptions.EnemySet == EnemyOptions.EnemySetType.NonMixed ||
                 EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Unchanged))
            {
                SubLevel.SubLevelDict[SubLevel.Id.MasconTower].Screens[0].Sprites[0].RequiresWingBoots = true;
                SubLevel.SubLevelDict[SubLevel.Id.EvilOnesLair].Screens[14].Sprites[0].RequiresMattock = true;
            }

            if (ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Unchanged)
            {
                branchHiddenItem.SetX((byte)(branchHiddenItem.GetX() + 1));
            }

            return levels;
        }

        private void AddSublevels(List<Level> levels)
        {
            foreach (var level in levels)
            {
                if (level.Start == Level.StartOffset.Trunk)
                {
                    level.AddSubLevel(SubLevel.Id.TowerOfTrunk, 13, 21);
                    level.AddSubLevel(SubLevel.Id.TowerOfFortress, 42, level.Screens.Count - 3);
                    level.AddSubLevel(SubLevel.Id.JokerHouse, level.Screens.Count - 2, level.Screens.Count - 1);

                    var screens = new List<Screen>();
                    screens.Add(level.Screens[12]);
                    screens.Add(level.Screens[24]);
                    var sublevel = new SubLevel(SubLevel.Id.EarlyTrunk, screens);
                    level.SubLevels.Add(sublevel);
                    SubLevel.SubLevelDict[sublevel.SubLevelId] = sublevel;

                    screens = new List<Screen>();
                    screens.Add(level.Screens[28]);
                    screens.Add(level.Screens[32]);
                    screens.Add(level.Screens[33]);
                    sublevel = new SubLevel(SubLevel.Id.EastTrunk, screens);
                    level.SubLevels.Add(sublevel);
                    SubLevel.SubLevelDict[sublevel.SubLevelId] = sublevel;
                }
                else if (level.Start == Level.StartOffset.Mist)
                {
                    level.AddSubLevel(SubLevel.Id.EarlyMist, 15, 16);
                    level.AddSubLevel(SubLevel.Id.LateMist, 26, 30);
                    level.AddSubLevel(SubLevel.Id.TowerOfSuffer, 46, 60);
                    level.AddSubLevel(SubLevel.Id.TowerOfMist, 61, 74);
                    level.AddSubLevel(SubLevel.Id.MasconTower, 75, 77);
                    level.AddSubLevel(SubLevel.Id.VictimTower, 78, level.Screens.Count - 1);
                }
                else if (level.Start == Level.StartOffset.Branch)
                {
                    level.AddSubLevel(SubLevel.Id.EarlyBranch, 3, 6);
                    level.SubLevels[0].Screens.Add(level.Screens[11]);
                    level.AddSubLevel(SubLevel.Id.BattleHelmetWing, 7, 9);
                    level.AddSubLevel(SubLevel.Id.MiddleBranch, 15, 19);
                    level.AddSubLevel(SubLevel.Id.DropDownWing, 22, 24);
                    level.AddSubLevel(SubLevel.Id.EastBranch, 26, 34);
                }
                else if (level.Start == Level.StartOffset.DartmoorArea)
                {
                    level.AddSubLevel(SubLevel.Id.Dartmoor, 0, 14);
                    level.AddSubLevel(SubLevel.Id.CastleFraternal, 16, 20);
                    level.AddSubLevelScreens(level.SubLevels[1], 22, level.Screens.Count - 1);
                    level.AddSubLevel(SubLevel.Id.KingGrieve, 21, 21);

                }
                else if (level.Start == Level.StartOffset.EvilOnesLair)
                {
                    level.AddSubLevel(SubLevel.Id.EvilOnesLair, 0, level.Screens.Count - 1);
                }
                else if (level.Start == Level.StartOffset.Buildings)
                {
                    var screens = new List<Screen>();
                    screens.Add(level.Screens[31]);
                    var sublevel = new SubLevel(SubLevel.Id.BirdHospital, screens);
                    level.SubLevels.Add(sublevel);
                    SubLevel.SubLevelDict[sublevel.SubLevelId] = sublevel;

                    screens = new List<Screen>();
                    screens.Add(level.Screens[55]);
                    sublevel = new SubLevel(SubLevel.Id.DartmoorHouse, screens);
                    level.SubLevels.Add(sublevel);
                    SubLevel.SubLevelDict[sublevel.SubLevelId] = sublevel;

                    screens = new List<Screen>();
                    screens.Add(level.Screens[67]);
                    sublevel = new SubLevel(SubLevel.Id.FraternalHouse, screens);
                    level.SubLevels.Add(sublevel);
                    SubLevel.SubLevelDict[sublevel.SubLevelId] = sublevel;
                }
            }
        }

        private void AddMiscHacks(byte[] content, Random random)
        {
            if (ItemOptions.FixPendantBug)
            {
                content[Section.GetOffset(14, 0x8879, 0x8000)] = OpCode.BEQ;
            }

            if (GeneralOptions.AllowEquippingIndoors)
            {
                content[Section.GetOffset(12, 0x8B87, 0x8000)] = 0xFF;
            }

            if (GeneralOptions.AllowLoweringRespawn)
            {
                if (GeneralOptions.MiscDoorSetting == GeneralOptions.MiscDoors.Unchanged ||
                    GeneralOptions.MiscDoorSetting == GeneralOptions.MiscDoors.ShuffleExcludeTowns ||
                    GeneralOptions.MiscDoorSetting == GeneralOptions.MiscDoors.ShuffleIncludeTownsExceptGurusAndKeyshops ||
                    GeneralOptions.MiscDoorSetting == GeneralOptions.MiscDoors.ShuffleIncludeTownsExceptGurus)
                {
                    var section = new Section();
                    section.Bytes.Add(OpCode.NOP);
                    section.Bytes.Add(OpCode.NOP);
                    section.Bytes.Add(OpCode.NOP);
                    section.Bytes.Add(OpCode.NOP);
                    section.Bytes.Add(OpCode.NOP);
                    section.AddToContent(content, Section.GetOffset(12, 0x8394, 0x8000));
                }
                else
                {
                    var section = new Section();
                    section.Bytes.Add(OpCode.CMPImmediate);
                    section.Bytes.Add(0x0);
                    section.Bytes.Add(OpCode.BEQ);
                    section.Bytes.Add(0x4);
                    section.Bytes.Add(OpCode.NOP);
                    section.AddToContent(content, Section.GetOffset(12, 0x8394, 0x8000));
                }
            }

            if (GeneralOptions.PreventKnockbackOnLadders)
            {
                var section = new Section();
                section.Bytes.Add(OpCode.JSR);
                section.Bytes.Add(0xD0);
                section.Bytes.Add(0xFE);
                section.Bytes.Add(OpCode.NOP);
                section.AddToContent(content, Section.GetOffset(15, 0xE28C, 0xC000));

                section = new Section();
                section.Bytes.Add(OpCode.JSR);
                section.Bytes.Add(0x52);
                section.Bytes.Add(0xE7);
                section.Bytes.Add(OpCode.LDAZeroPage);
                section.Bytes.Add(0xA4);
                section.Bytes.Add(OpCode.ANDImmediate);
                section.Bytes.Add(0x08);
                section.Bytes.Add(OpCode.BEQ);
                section.Bytes.Add(0x05);
                section.Bytes.Add(OpCode.LDAImmediate);
                section.Bytes.Add(0x00);
                section.Bytes.Add(OpCode.STAZeroPage);
                section.Bytes.Add(0xAA);
                section.Bytes.Add(OpCode.RTS);
                section.Bytes.Add(OpCode.LDAImmediate);
                section.Bytes.Add(0x08);
                section.Bytes.Add(OpCode.STAZeroPage);
                section.Bytes.Add(0xAA);
                section.Bytes.Add(OpCode.RTS);
                section.AddToContent(content, Section.GetOffset(15, 0xFED0, 0xC000));
            }

            if (GeneralOptions.MoveSpringQuestRequirement)
            {
                var springSection = new Section();
                springSection.Bytes.Add(OpCode.NOP);
                springSection.Bytes.Add(OpCode.NOP);
                springSection.AddToContent(content, Section.GetOffset(15, 0xE971, 0xC000));
            }

            var times = new List<byte>();
            if (ItemOptions.WingbootDurationSetting == ItemOptions.WingBootDurations.Random)
            {
                var duration = (byte)random.Next(10, 46);
                times.Add(duration);
                duration += (byte)random.Next(1, 11);
                times.Add(duration);
                duration += (byte)random.Next(1, 11);
                times.Add(duration);
                duration += (byte)random.Next(1, 11);
                times.Add(duration);
                UpdateWingBootsTimer(times, content);
            }
            else if (ItemOptions.WingbootDurationSetting == ItemOptions.WingBootDurations.Always40)
            {
                times.Add(40);
                times.Add(40);
                times.Add(40);
                times.Add(40);
                UpdateWingBootsTimer(times, content);
            }
            else if (ItemOptions.WingbootDurationSetting == ItemOptions.WingBootDurations.Always20)
            {
                times.Add(20);
                times.Add(20);
                times.Add(20);
                times.Add(20);
                UpdateWingBootsTimer(times, content);
            }
            else if (ItemOptions.WingbootDurationSetting == ItemOptions.WingBootDurations.ScalesUpFrom40)
            {
                times.Add(40);
                times.Add(45);
                times.Add(50);
                times.Add(55);
                UpdateWingBootsTimer(times, content);
            }
            else if (ItemOptions.WingbootDurationSetting == ItemOptions.WingBootDurations.ScalesUpFrom30)
            {
                times.Add(30);
                times.Add(35);
                times.Add(40);
                times.Add(45);
                UpdateWingBootsTimer(times, content);
            }
            else if (ItemOptions.WingbootDurationSetting == ItemOptions.WingBootDurations.ScalesUpFrom20)
            {
                times.Add(20);
                times.Add(30);
                times.Add(40);
                times.Add(50);
                UpdateWingBootsTimer(times, content);
            }
            else if (ItemOptions.WingbootDurationSetting == ItemOptions.WingBootDurations.ScalesUpFrom10)
            {
                times.Add(10);
                times.Add(20);
                times.Add(30);
                times.Add(40);
                UpdateWingBootsTimer(times, content);
            }

            if (ItemOptions.BuffGloves)
            {
                var section = new Section();
                section.Bytes.Add(40);
                section.AddToContent(content, Section.GetOffset(15, 0xC7DD, 0xC000));
            }

            if (ItemOptions.BuffHourglass)
            {
                var section = new Section();
                for (int i = 0; i < 9; i++)
                {
                    section.Bytes.Add(OpCode.NOP);
                }
                section.AddToContent(content, Section.GetOffset(15, 0xC5D8, 0xC000));
            }

            if (ItemOptions.ShieldSetting == 0 || ItemOptions.ShieldSetting == 1)
            {
                var section = new Section();
                section.Bytes.Add(OpCode.JSR);
                section.Bytes.Add(0xE0);
                section.Bytes.Add(0xBF);
                section.AddToContent(content, Section.GetOffset(14, 0x877C, 0x8000));

                section = new Section();
                section.Bytes.Add(OpCode.LDAAbsolute);
                section.Bytes.Add(0x27);
                section.Bytes.Add(0x04);
                section.Bytes.Add(OpCode.BPL);
                section.Bytes.Add(0x04);
                section.Bytes.Add(OpCode.LDAAbsolute);
                section.Bytes.Add(0xBF);
                section.Bytes.Add(0x03);
                section.Bytes.Add(OpCode.RTS);
                section.Bytes.Add(OpCode.LDAImmediate);
                section.Bytes.Add(0x03);
                section.Bytes.Add(OpCode.RTS);
                section.AddToContent(content, Section.GetOffset(14, 0xBFE0, 0x8000));
            }

            if (ItemOptions.ShieldSetting == 1)
            {
                var section = new Section();
                section.Bytes.Add(OpCode.JSR);
                section.Bytes.Add(0xC0);
                section.Bytes.Add(0xBF);
                section.Bytes.Add(OpCode.NOP);
                section.AddToContent(content, Section.GetOffset(14, 0x87E8, 0x8000));

                section = new Section();
                section.Bytes.Add(OpCode.CPY);
                section.Bytes.Add(0x2);
                section.Bytes.Add(OpCode.BEQ);
                section.Bytes.Add(0x4);
                section.Bytes.Add(OpCode.LSR);
                section.Bytes.Add(OpCode.LSR);
                section.Bytes.Add(OpCode.LSR);
                section.Bytes.Add(OpCode.RTS);
                section.Bytes.Add(OpCode.LDAImmediate);
                section.Bytes.Add(0x0);
                section.Bytes.Add(OpCode.RTS);
                section.AddToContent(content, Section.GetOffset(14, 0xBFC0, 0x8000));
            }

            if (GeneralOptions.FastText)
            {
                var newSection = new Section();
                newSection.Bytes.Add(OpCode.JMPAbsolute);
                newSection.Bytes.Add(0x00);
                newSection.Bytes.Add(0xFF);
                newSection.AddToContent(content, Section.GetOffset(15, 0xF4A2, 0xC000));

                newSection = new Section();
                newSection.Bytes.Add(OpCode.JSR);
                newSection.Bytes.Add(0x90);
                newSection.Bytes.Add(0xFF);
                newSection.Bytes.Add(OpCode.JSR);
                newSection.Bytes.Add(0x90);
                newSection.Bytes.Add(0xFF);
                newSection.Bytes.Add(OpCode.JSR);
                newSection.Bytes.Add(0x90);
                newSection.Bytes.Add(0xFF);
                newSection.Bytes.Add(OpCode.LDAAbsolute);
                newSection.Bytes.Add(0x1C);
                newSection.Bytes.Add(0x02);
                newSection.Bytes.Add(OpCode.JMPAbsolute);
                newSection.Bytes.Add(0x57);
                newSection.Bytes.Add(0xF5);
                newSection.AddToContent(content, Section.GetOffset(15, 0xFF00, 0xC000));

                newSection = new Section();
                newSection.Bytes.Add(OpCode.LDAAbsolute);
                newSection.Bytes.Add(0x13);
                newSection.Bytes.Add(0x02);
                newSection.Bytes.Add(OpCode.BEQ);
                newSection.Bytes.Add(0x06);
                newSection.Bytes.Add(OpCode.LDAAbsolute);
                newSection.Bytes.Add(0x1C);
                newSection.Bytes.Add(0x02);
                newSection.Bytes.Add(OpCode.JSR);
                newSection.Bytes.Add(0xA5);
                newSection.Bytes.Add(0xF4);
                newSection.Bytes.Add(OpCode.RTS);
                newSection.AddToContent(content, Section.GetOffset(15, 0xFF90, 0xC000));
            }

            if (GeneralOptions.FastStart)
            {
                var healthSection = new Section();
                healthSection.Bytes.Add(0x50);
                healthSection.AddToContent(content, Section.GetOffset(15, 0xDB30, 0xC000));

                healthSection = new Section();
                healthSection.Bytes.Add(0x50);
                healthSection.AddToContent(content, Section.GetOffset(15, 0xDEAF, 0xC000));

                var newSection = new Section();
                newSection.Bytes.Add(OpCode.JSR);
                newSection.Bytes.Add(0xC0);
                newSection.Bytes.Add(0xBD);
                newSection.AddToContent(content, Section.GetOffset(14, 0xDB2C, 0x8000));

                newSection = new Section();
                newSection.Bytes.Add(OpCode.LDAImmediate);
                newSection.Bytes.Add(0xDC);
                newSection.Bytes.Add(OpCode.STAAbsolute);
                newSection.Bytes.Add(0x92);
                newSection.Bytes.Add(0x03);

                newSection.Bytes.Add(OpCode.LDAImmediate);
                newSection.Bytes.Add(0x05);
                newSection.Bytes.Add(OpCode.STAAbsolute);
                newSection.Bytes.Add(0x93);
                newSection.Bytes.Add(0x03);

                if (ItemOptions.RandomizeKeys == ItemOptions.KeyRandomization.Unchanged)
                {
                    newSection.Bytes.Add(OpCode.LDAImmediate);
                    newSection.Bytes.Add(0x80);
                    newSection.Bytes.Add(OpCode.STAAbsolute);
                    newSection.Bytes.Add(0x2C);
                    newSection.Bytes.Add(0x04);
                }

                newSection.Bytes.Add(OpCode.JSR);
                newSection.Bytes.Add(0xA7);
                newSection.Bytes.Add(0xDE);
                newSection.Bytes.Add(OpCode.RTS);

                newSection.AddToContent(content, Section.GetOffset(14, 0xBDC0, 0x8000));
            }

            if (GeneralOptions.DragonSlayerRequired ||
                GeneralOptions.PendantRodRubyRequired ||
                GeneralOptions.MoveSpringQuestRequirement)
            {
                var newSection = new Section();
                newSection.Bytes.Add(OpCode.JMPAbsolute);
                newSection.Bytes.Add(0x80);
                newSection.Bytes.Add(0xFE);
                newSection.AddToContent(content, Section.GetOffset(15, 0xEBC1, 0xC000));
                newSection = new Section();
                if (GeneralOptions.DragonSlayerRequired)
                {
                    newSection.Bytes.Add(OpCode.LDAAbsolute);
                    newSection.Bytes.Add(0xBD);
                    newSection.Bytes.Add(0x03);
                    newSection.Bytes.Add(OpCode.CMPImmediate);
                    newSection.Bytes.Add(0x03);
                    newSection.Bytes.Add(OpCode.BEQ);
                    if (GeneralOptions.UpdateMiscText && ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Unchanged)
                    {
                        newSection.Bytes.Add(0x09);
                        newSection.Bytes.Add(OpCode.LDAImmediate);
                        newSection.Bytes.Add(0x0);
                        newSection.Bytes.Add(OpCode.JSR);
                        newSection.Bytes.Add(0x59);
                        newSection.Bytes.Add(0xF8);
                        newSection.Bytes.Add(0x0c);
                        newSection.Bytes.Add(0x41);
                        newSection.Bytes.Add(0x82);
                        newSection.Bytes.Add(OpCode.RTS);
                    }
                    else
                    {
                        newSection.Bytes.Add(0x01);
                        newSection.Bytes.Add(OpCode.RTS);
                    }
                }

                if (GeneralOptions.PendantRodRubyRequired)
                {
                    newSection.Bytes.Add(OpCode.LDAAbsolute);
                    newSection.Bytes.Add(0x2C);
                    newSection.Bytes.Add(0x04);
                    newSection.Bytes.Add(OpCode.ANDImmediate);
                    newSection.Bytes.Add(0x02);
                    newSection.Bytes.Add(OpCode.BNE);
                    newSection.Bytes.Add(0x01);
                    newSection.Bytes.Add(OpCode.RTS);
                    newSection.Bytes.Add(OpCode.LDAAbsolute);
                    newSection.Bytes.Add(0x2C);
                    newSection.Bytes.Add(0x04);
                    newSection.Bytes.Add(OpCode.ANDImmediate);
                    newSection.Bytes.Add(0x04);
                    newSection.Bytes.Add(OpCode.BNE);
                    newSection.Bytes.Add(0x01);
                    newSection.Bytes.Add(OpCode.RTS);
                    newSection.Bytes.Add(OpCode.LDAAbsolute);
                    newSection.Bytes.Add(0x2C);
                    newSection.Bytes.Add(0x04);
                    newSection.Bytes.Add(OpCode.ANDImmediate);
                    newSection.Bytes.Add(0x40);
                    newSection.Bytes.Add(OpCode.BNE);
                    newSection.Bytes.Add(0x01);
                    newSection.Bytes.Add(OpCode.RTS);
                }

                if (GeneralOptions.MoveSpringQuestRequirement)
                {
                    newSection.Bytes.Add(OpCode.LDAAbsolute);
                    newSection.Bytes.Add(0x2D);
                    newSection.Bytes.Add(0x04);
                    newSection.Bytes.Add(OpCode.ANDImmediate);
                    newSection.Bytes.Add(0x07);
                    newSection.Bytes.Add(OpCode.CMPImmediate);
                    newSection.Bytes.Add(0x07);
                    newSection.Bytes.Add(OpCode.BEQ);
                    newSection.Bytes.Add(0x01);
                    newSection.Bytes.Add(OpCode.RTS);
                }

                newSection.Bytes.Add(OpCode.LDAAbsolute);
                newSection.Bytes.Add(0x2C);
                newSection.Bytes.Add(0x04);
                newSection.Bytes.Add(OpCode.JMPAbsolute);
                newSection.Bytes.Add(0xC4);
                newSection.Bytes.Add(0xEB);
                newSection.AddToContent(content, Section.GetOffset(15, 0xFE80, 0xC000));
            }

            if (ItemOptions.MattockUsage == ItemOptions.MattockUsages.AnywhereExceptMistEntrance ||
                ItemOptions.MattockUsage == ItemOptions.MattockUsages.AnywhereExceptMistEntranceNoFraternalItemShuffle)
            {
                var mattocksection = new Section();
                mattocksection.Bytes.Add(OpCode.JSR);
                mattocksection.Bytes.Add(0xD0);
                mattocksection.Bytes.Add(0xFD);
                mattocksection.Bytes.Add(OpCode.NOP);
                mattocksection.Bytes.Add(OpCode.NOP);
                mattocksection.Bytes.Add(OpCode.NOP);
                mattocksection.AddToContent(content, Section.GetOffset(15, 0xC637, 0xC000));

                mattocksection = new Section();
                mattocksection.Bytes.Add(OpCode.LDAAbsolute);
                mattocksection.Bytes.Add(0x24);
                mattocksection.Bytes.Add(0x00);
                mattocksection.Bytes.Add(OpCode.CMPImmediate);
                mattocksection.Bytes.Add(0x01);
                mattocksection.Bytes.Add(OpCode.BNE);
                mattocksection.Bytes.Add(0x0A);
                mattocksection.Bytes.Add(OpCode.LDAAbsolute);
                mattocksection.Bytes.Add(0x63);
                mattocksection.Bytes.Add(0x00);
                mattocksection.Bytes.Add(OpCode.CMPImmediate);
                mattocksection.Bytes.Add(0x28);
                mattocksection.Bytes.Add(OpCode.BNE);
                mattocksection.Bytes.Add(0x03);
                mattocksection.Bytes.Add(OpCode.LDAImmediate);
                mattocksection.Bytes.Add(0x1);
                mattocksection.Bytes.Add(OpCode.RTS);
                mattocksection.Bytes.Add(OpCode.LDAImmediate);
                mattocksection.Bytes.Add(0x0);
                mattocksection.Bytes.Add(OpCode.RTS);
                mattocksection.AddToContent(content, Section.GetOffset(15, 0xFDD0, 0xC000));
            }
            else if (ItemOptions.MattockUsage != ItemOptions.MattockUsages.Unchanged)
            {
                var mattocksection = new Section();
                mattocksection.Bytes.Add(OpCode.LDAImmediate);
                mattocksection.Bytes.Add(0x0);
                mattocksection.Bytes.Add(OpCode.NOP);
                mattocksection.Bytes.Add(OpCode.NOP);
                mattocksection.Bytes.Add(OpCode.NOP);
                mattocksection.Bytes.Add(OpCode.NOP);
                mattocksection.AddToContent(content, Section.GetOffset(15, 0xC637, 0xC000));
            }

            if (ItemOptions.ReplacePoison)
            {
                var jumpSection = new Section();
                jumpSection.Bytes.Add(OpCode.JSR);
                jumpSection.Bytes.Add(0x70);
                jumpSection.Bytes.Add(0xFD);
                jumpSection.AddToContent(content, Section.GetOffset(15, 0xC48B, 0xC000));

                var manaPotionSection = new Section();
                manaPotionSection.Bytes.Add(OpCode.LDAAbsolute);
                manaPotionSection.Bytes.Add(0xC1);
                manaPotionSection.Bytes.Add(0x03);
                manaPotionSection.Bytes.Add(OpCode.CMPImmediate);
                manaPotionSection.Bytes.Add(0x11);
                manaPotionSection.Bytes.Add(OpCode.BNE);
                manaPotionSection.Bytes.Add(0x06);
                manaPotionSection.Bytes.Add(OpCode.JSR);
                manaPotionSection.Bytes.Add(0x06);
                manaPotionSection.Bytes.Add(0xC5);
                manaPotionSection.Bytes.Add(OpCode.JSR);
                manaPotionSection.Bytes.Add(0xBF);
                manaPotionSection.Bytes.Add(0xC4);
                manaPotionSection.Bytes.Add(OpCode.LDAAbsolute);
                manaPotionSection.Bytes.Add(0xC1);
                manaPotionSection.Bytes.Add(0x03);
                manaPotionSection.Bytes.Add(OpCode.RTS);
                manaPotionSection.AddToContent(content, Section.GetOffset(15, 0xFD70, 0xC000));

                var convertPoisonSection = new Section();
                convertPoisonSection.Bytes.Add(0x08);
                convertPoisonSection.Bytes.Add(OpCode.JSR);
                convertPoisonSection.Bytes.Add(0xE4);
                convertPoisonSection.Bytes.Add(0xD0);
                convertPoisonSection.Bytes.Add(OpCode.LDAImmediate);
                convertPoisonSection.Bytes.Add(0x11);
                convertPoisonSection.Bytes.Add(OpCode.JSR);
                convertPoisonSection.Bytes.Add(0xCD);
                convertPoisonSection.Bytes.Add(0xC8);
                convertPoisonSection.Bytes.Add(OpCode.LDXAbsolute);
                convertPoisonSection.Bytes.Add(0x03);
                convertPoisonSection.Bytes.Add(0x78);
                convertPoisonSection.Bytes.Add(OpCode.RTS);
                convertPoisonSection.Bytes.Add(OpCode.NOP);
                convertPoisonSection.Bytes.Add(OpCode.NOP);
                convertPoisonSection.AddToContent(content, Section.GetOffset(15, 0xC845, 0xC000));
            }
        }

        private void RandomizeExtras(byte[] content, Random random, DoorRandomizer doorRandomizer, out byte finalPalette,
                                     out bool addSection)
        {
            finalPalette = PaletteRandomizer.FinalPalette;
            addSection = false;
            var paletteRandomizer = new PaletteRandomizer();
            if (ExtraOptions.RandomizePalettes)
            {
                paletteRandomizer.RandomizePalettes(content, random);
                doorRandomizer.RandomizeTowerPalettes(content, random, out finalPalette);
            }

            if (ExtraOptions.MusicSetting != Music.Unchanged)
            {
                paletteRandomizer.RandomizeMusic(content, random);
            }

            var section = new Section();
            if (ExtraOptions.MusicSetting == Music.None)
            {
                section.Bytes.Add(OpCode.LDAImmediate);
                section.Bytes.Add(0x0);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                addSection = true;
            }
            else if (ExtraOptions.MusicSetting == Music.Random && ExtraOptions.RandomizePalettes)
            {
                section.Bytes.Add(OpCode.LSRA);
                section.Bytes.Add(OpCode.TAX);
                section.Bytes.Add(OpCode.INX);
                section.Bytes.Add(OpCode.TXA);
                addSection = true;
            }

            if (addSection)
            {
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.Bytes.Add(OpCode.NOP);
                section.AddToContent(content, Section.GetOffset(15, 0xE54E, 0xC000));
            }

            if (ExtraOptions.RandomizeSounds)
            {
                var soundRandomizer = new SoundRandomizer();
                soundRandomizer.RandomizeSounds(content, random);
            }
        }

        private void UpdateGiftItemLogic(List<Level> levels, byte[] content, Table spriteBehaviourTable)
        {
            if (ItemOptions.BigItemSpawns == ItemOptions.BigItemSpawning.Unchanged)
            {
                UpdateWingBootsQuest(content);
            }

            foreach (var level in levels)
            {
                foreach (var screen in level.Screens)
                {
                    foreach (var sprite in screen.Sprites)
                    {
                        if (sprite.Id == Sprite.SpriteId.MattockOrRingRuby)
                        {
                            sprite.Id = Sprite.SpriteId.MattockBossLocked;
                        }
                        else if (sprite.Id == Sprite.SpriteId.Glove2OrKeyJoker)
                        {
                            sprite.Id = Sprite.SpriteId.Glove;
                        }
                        else if (sprite.Id == Sprite.SpriteId.KeyAce)
                        {
                            sprite.Id = (Sprite.SpriteId)59;
                        }
                    }
                }
            }

            spriteBehaviourTable.Entries[(int)Sprite.SpriteId.KeyAce] =
                spriteBehaviourTable.Entries[(int)Sprite.SpriteId.WingbootsBossLocked];
            spriteBehaviourTable.Entries[(int)Sprite.SpriteId.RingDworf] =
                spriteBehaviourTable.Entries[(int)Sprite.SpriteId.WingbootsBossLocked];
            spriteBehaviourTable.Entries[(int)Sprite.SpriteId.RingDemon] =
                spriteBehaviourTable.Entries[(int)Sprite.SpriteId.WingbootsBossLocked];
            spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Glove2OrKeyJoker] =
                spriteBehaviourTable.Entries[(int)Sprite.SpriteId.WingbootsBossLocked];
            spriteBehaviourTable.Entries[(int)Sprite.SpriteId.MattockOrRingRuby] =
                spriteBehaviourTable.Entries[(int)Sprite.SpriteId.WingbootsBossLocked];

            var newSection = new Section();
            newSection.Bytes.Add(OpCode.JMPAbsolute);
            newSection.Bytes.Add(0xD0);
            newSection.Bytes.Add(0xFC);
            newSection.Bytes.Add(OpCode.NOP);
            newSection.AddToContent(content, Section.GetOffset(15, 0xC764, 0xC000));

            newSection = new Section();
            newSection.Bytes.Add(OpCode.NOP);
            newSection.Bytes.Add(OpCode.NOP);
            newSection.Bytes.Add(OpCode.NOP);
            newSection.Bytes.Add(OpCode.NOP);
            newSection.AddToContent(content, Section.GetOffset(15, 0xC7A5, 0xC000));

            newSection = new Section();
            newSection.Bytes.Add(OpCode.TAX);
            newSection.Bytes.Add(OpCode.CMPImmediate);
            newSection.Bytes.Add(0x35);
            newSection.Bytes.Add(OpCode.BNE);
            newSection.Bytes.Add(0x0E);
            newSection.Bytes.Add(OpCode.LDAAbsolute);
            newSection.Bytes.Add(0x2C);
            newSection.Bytes.Add(0x04);
            newSection.Bytes.Add(OpCode.ORAImmediate);
            newSection.Bytes.Add(0x10);
            newSection.Bytes.Add(OpCode.STAAbsolute);
            newSection.Bytes.Add(0x2C);
            newSection.Bytes.Add(0x04);
            newSection.Bytes.Add(OpCode.LDAImmediate);
            newSection.Bytes.Add(0x08);
            newSection.Bytes.Add(OpCode.JSR);
            newSection.Bytes.Add(0xE4);
            newSection.Bytes.Add(0xD0);
            newSection.Bytes.Add(OpCode.TXA);
            newSection.Bytes.Add(OpCode.CMPImmediate);
            newSection.Bytes.Add(0x36);
            newSection.Bytes.Add(OpCode.BNE);
            newSection.Bytes.Add(0x0E);
            newSection.Bytes.Add(OpCode.LDAAbsolute);
            newSection.Bytes.Add(0x2C);
            newSection.Bytes.Add(0x04);
            newSection.Bytes.Add(OpCode.ORAImmediate);
            newSection.Bytes.Add(0x20);
            newSection.Bytes.Add(OpCode.STAAbsolute);
            newSection.Bytes.Add(0x2C);
            newSection.Bytes.Add(0x04);
            newSection.Bytes.Add(OpCode.LDAImmediate);
            newSection.Bytes.Add(0x08);
            newSection.Bytes.Add(OpCode.JSR);
            newSection.Bytes.Add(0xE4);
            newSection.Bytes.Add(0xD0);
            newSection.Bytes.Add(OpCode.TXA);
            newSection.Bytes.Add(OpCode.CMPImmediate);
            newSection.Bytes.Add(0x38);
            newSection.Bytes.Add(OpCode.BNE);
            newSection.Bytes.Add(0x0B);
            newSection.Bytes.Add(OpCode.LDAImmediate);
            newSection.Bytes.Add(0x04);
            newSection.Bytes.Add(OpCode.JSR);
            newSection.Bytes.Add(0xCD);
            newSection.Bytes.Add(0xC8);
            newSection.Bytes.Add(OpCode.LDAImmediate);
            newSection.Bytes.Add(0x08);
            newSection.Bytes.Add(OpCode.JSR);
            newSection.Bytes.Add(0xE4);
            newSection.Bytes.Add(0xD0);
            newSection.Bytes.Add(OpCode.TXA);
            newSection.Bytes.Add(OpCode.CMPImmediate);
            newSection.Bytes.Add(0x50);
            newSection.Bytes.Add(OpCode.BNE);
            newSection.Bytes.Add(0x0E);
            newSection.Bytes.Add(OpCode.LDAAbsolute);
            newSection.Bytes.Add(0x2C);
            newSection.Bytes.Add(0x04);
            newSection.Bytes.Add(OpCode.ORAImmediate);
            newSection.Bytes.Add(0x40);
            newSection.Bytes.Add(OpCode.STAAbsolute);
            newSection.Bytes.Add(0x2C);
            newSection.Bytes.Add(0x04);
            newSection.Bytes.Add(OpCode.LDAImmediate);
            newSection.Bytes.Add(0x08);
            newSection.Bytes.Add(OpCode.JSR);
            newSection.Bytes.Add(0xE4);
            newSection.Bytes.Add(0xD0);
            newSection.Bytes.Add(OpCode.TXA);
            newSection.Bytes.Add(OpCode.CMPImmediate);
            newSection.Bytes.Add(0x5F);
            newSection.Bytes.Add(OpCode.BNE);
            newSection.Bytes.Add(0x0A);
            newSection.Bytes.Add(OpCode.LDAImmediate);
            newSection.Bytes.Add(0x08);
            newSection.Bytes.Add(OpCode.JSR);
            newSection.Bytes.Add(0xE4);
            newSection.Bytes.Add(0xD0);
            newSection.Bytes.Add(OpCode.LDAImmediate);
            newSection.Bytes.Add(0x08);
            newSection.Bytes.Add(OpCode.JMPAbsolute);
            newSection.Bytes.Add(0xCD);
            newSection.Bytes.Add(0xC8);
            newSection.Bytes.Add(OpCode.TXA);
            newSection.Bytes.Add(OpCode.JMPAbsolute);
            newSection.Bytes.Add(0x68);
            newSection.Bytes.Add(0xC7);
            newSection.AddToContent(content, Section.GetOffset(15, 0xFCD0, 0xC000));

            newSection = new Section();
            newSection.Bytes.Add(OpCode.JSR);
            newSection.Bytes.Add(0x50);
            newSection.Bytes.Add(0xFD);
            newSection.AddToContent(content, Section.GetOffset(15, 0xCD8F, 0xC000));

            newSection = new Section();
            newSection.Bytes.Add(OpCode.LDAAbsolute);
            newSection.Bytes.Add(0x8B);
            newSection.Bytes.Add(0x03);
            newSection.Bytes.Add(OpCode.CMPImmediate);
            newSection.Bytes.Add((byte)Sprite.SpriteId.MattockOrRingRuby);
            newSection.Bytes.Add(OpCode.BNE);
            newSection.Bytes.Add(0x02);
            newSection.Bytes.Add(OpCode.LDAImmediate);
            newSection.Bytes.Add(0x04);
            newSection.Bytes.Add(OpCode.CMPImmediate);
            newSection.Bytes.Add((byte)Sprite.SpriteId.Glove2OrKeyJoker);
            newSection.Bytes.Add(OpCode.BNE);
            newSection.Bytes.Add(0x02);
            newSection.Bytes.Add(OpCode.LDAImmediate);
            newSection.Bytes.Add(0x61);
            newSection.Bytes.Add(OpCode.RTS);
            newSection.AddToContent(content, Section.GetOffset(15, 0xFD50, 0xC000));

            var spriteTypeTable = new Table(Section.GetOffset(14, 0xB544, 0x8000), 100, 1, content);
            spriteTypeTable.Entries[(int)Sprite.SpriteId.RingDemon][0] = 5;
            spriteTypeTable.Entries[(int)Sprite.SpriteId.RingDworf][0] = 5;
            spriteTypeTable.Entries[(int)Sprite.SpriteId.KeyAce][0] = 5;
            spriteTypeTable.AddToContent(content);
        }

        private void UpdateWingBootsQuest(byte[] content)
        {
            var newSection = new Section();
            newSection.Bytes.Add(OpCode.NOP);
            newSection.Bytes.Add(OpCode.NOP);
            newSection.Bytes.Add(OpCode.NOP);
            newSection.Bytes.Add(OpCode.NOP);
            newSection.Bytes.Add(OpCode.NOP);
            newSection.Bytes.Add(OpCode.NOP);
            newSection.Bytes.Add(OpCode.NOP);
            newSection.Bytes.Add(OpCode.NOP);
            newSection.Bytes.Add(OpCode.NOP);
            newSection.Bytes.Add(OpCode.NOP);
            newSection.AddToContent(content, Section.GetOffset(14, 0xA418, 0x8000));
        }

        private void UpdateWingBootsTimer(List<byte> times, byte[] content)
        {
            var section = new Section();
            foreach (var time in times)
            {
                section.Bytes.Add(time);
            }

            section.AddToContent(content, Section.GetOffset(15, 0xC599, 0xC000));
        }
    }
}
