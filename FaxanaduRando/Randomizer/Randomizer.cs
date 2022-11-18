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

            var levels = GetLevels(content, random);
            uint screenAttempts = 0;
            if (GeneralOptions.RandomizeScreens != GeneralOptions.ScreenRandomization.Unchanged)
            {
                foreach (var level in levels)
                {
                    bool result = level.RandomizeScreens(random, ref screenAttempts);
                    if (!result)
                    {
                        return result;
                    }
                }
            }

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
                level.AddToContent(content);
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
                textRandomizer.RandomizeTitles(content);
            }

            if (!textRandomizer.UpdateText(shopRandomizer, giftRandomizer, doorRandomizer, content))
            {
                return false;
            }

            var titleText = Text.GetAllTitleText(content, Section.GetOffset(12, 0x9DCC, 0x8000),
                                                 Section.GetOffset(12, 0x9E0D, 0x8000));
            Text.AddTitleText(0, "RANDUMIZER V25B15", titleText);
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
            string suffix = "";

            if (ExtraOptions.AppendSuffix)
            {
                suffix = GetSuffix(random);
            }
#if DEBUG
            outputFile = inputFile.Insert(dotIndex, "_" + seed.ToString() + suffix);
#else
            outputFile = inputFile.Insert(dotIndex, "_" + seed.ToString() + "_" + flags + suffix);
#endif

            RandomizeExtras(content, random, doorRandomizer, out byte finalPalette, out bool addSection);

            if (GeneralOptions.ShuffleTowers)
            {
                AddTowerShuffleModifications(content, addSection, finalPalette);
            }

            File.WriteAllBytes(outputFile, content);
            if (GeneralOptions.GenerateSpoilerLog)
            {
                var spoilers = new List<string>();
                spoilers.Add("Randumizer v0.25 Beta 15");
                spoilers.Add($"Seed {seed}");
                spoilers.Add($"Flags {flags}");
#if DEBUG
                spoilers.Add($"Randomization attempts: {attempts}");
                spoilers.Add($"Screen randomization attempts: {screenAttempts}");
#endif
                var hints = textRandomizer.GetHints(shopRandomizer, giftRandomizer, doorRandomizer, true);
                foreach (var hint in hints)
                {
                    string spoiler = hint.Replace(Text.spaceChar, ' ');
                    spoiler = hint.Replace(Text.lineBreakChar, ' ');
                    spoiler = spoiler.Replace(Text.endOfTextChar, ' ');
                    spoiler = spoiler.Replace(Text.lineBreakWithPauseChar, ' ');
                    spoiler = spoiler.Replace(Text.secondSpaceChar, ' ');
                    spoilers.Add(spoiler);
                }
#if DEBUG
                spoilers.Add($"Title Experience Gold");
                foreach (var data in textRandomizer.GetTitleData())
                {
                    spoilers.Add(data);
                }

                var enemyData = new Dictionary<Sprite.SpriteId, SpriteType>();
                for (int i = 0; i < 100; i++)
                {
                    if (Sprite.enemies.Contains((Sprite.SpriteId)i))
                    {
                        var id = (Sprite.SpriteId)i;
                        var hp = enemyHPTable.Entries[i][0];
                        var damage = enemyDamageTable.Entries[i][0];
                        enemyData[id] = new SpriteType(id, hp, damage);
                    }
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


                spoilers.Add($"Id Hp Damage");
                foreach (var data in enemyData.Values)
                {
                    spoilers.Add($"{data.Id} {data.Hp} {data.Damage}");
                }

                spoilers.Add($"Id Manacost Damage");
                foreach (var spell in spells.Values)
                {
                    spoilers.Add($"{spell.SpellId} {spell.ManaCost} {spell.Damage}");
                }
#endif

                File.WriteAllLines(outputFile.Replace(".nes", ".txt"), spoilers);
            }

            message = "Randomized ROM created at " + outputFile;
            return true;
        }

        private string GetSuffix(Random random)
        {
            var beginningStrings = new List<string>()
            {
                "Red",
                "Blue",
                "Yellow",
                "Green",
                "White",
                "Black",
                "Gray",
                "Icy",
                "Orange",
                "Purple",
                "Amazing",
                "Awesome",
                "Attractive",
                "Bald",
                "Beautiful",
                "Chubby",
                "Clean",
                "Dazzling",
                "Drab",
                "Elegant",
                "Fancy",
                "Fit",
                "Flabby",
                "Glamorous",
                "Gorgeous",
                "Handsome",
                "Long",
                "Magnificent",
                "Muscular",
                "Plain",
                "Plump",
                "Quaint",
                "Scruffy",
                "Shapely",
                "Short",
                "Skinny",
                "Stocky",
                "Careful",
                "Clever",
                "Famous",
                "Gifted",
                "Hallowed",
                "Helpful",
                "Important",
                "Inexpensive",
                "Odd",
                "Poor",
                "Powerful",
                "Rich",
                "Shy",
                "Aggressive",
                "Ambitious",
                "Brave",
                "Calm",
                "Delightful",
                "Eager",
                "Faithful",
                "Gentle",
                "Happy",
                "Jolly",
                "Kind",
                "Nice",
                "Obedient",
                "Polite",
                "Proud",
                "Silly",
                "Victorious",
                "Witty",
                "Wonderful",
                "Angry",
                "Bewildered",
                "Clumsy",
                "Embarrassed",
                "Embarrassing",
                "Fierce",
                "Grumpy",
                "Itchy",
                "Jealous",
                "Lazy",
                "Mysterious",
                "Nervous",
                "Obnoxious",
                "Pitiful",
                "Repulsive",
                "Scary",
                "Thoughtless",
                "Worried",
                "Big",
                "Colossal",
                "Gigantic",
                "Great",
                "Huge",
                "Immense",
                "Large",
                "Massive",
                "Microscopic",
                "Short",
                "Small",
                "Tall",
                "Tiny",
                "Ancient",
                "Early",
                "Fast",
                "Long",
                "Modern",
                "Old",
                "Prehistoric",
                "Quick",
                "Slow",
                "Swift",
                "Young",
                "Acidic",
                "Bitter",
                "Delicious",
                "Fresh",
                "Greasy",
                "Juicy",
                "Hot",
                "Moldy",
                "Salty",
                "Sour",
                "Spicy",
                "Sweet",
                "Tasteless",
                "Tasty",
                "Yummy",
                "Chilly",
                "Cold",
                "Cool",
                "Damaged",
                "Damp",
                "Dirty",
                "Dry",
                "Fluffy",
                "Freezing",
                "Greasy",
                "Icy",
                "Sharp",
                "Slimy",
                "Sticky",
                "Strong",
                "Warm",
                "Weak",
                "Wet",
                "Wooden",
                "Metal",
                "Metallic",
                "Smart",
                "Wise",
                "Negative",
                "Positive",
                "Chaotic",
                "Shining",
                "Evil",
                "Good",
                "Neutral",
                "Dark",
                "Shadow",
                "Royal",
                "Poisonous",
                "Confused",
                "Surprised",
                "Bizarre",
                "Heavenly",
                "Holy",
            };

            var middleStrings = new List<string>()
            {
                "Eolis",
                "Trunk",
                "Mist",
                "Branch",
                "Dartmoor",
                "Zenis",
                "Apolune",
                "Forepaw",
                "Mascon",
                "Victim",
                "Conflate",
                "Daybreak",
                "Elven",
                "Dwarven",
                "Guru",
                "Smoker",
                "Tundra",
                "Chaos",
                "Cardinals",
                "Zelda",
                "Link",
                "Mario",
                "Luigi",
                "Dragon",
                "Duck",
                "Metroid",
                "MonoDron",
                "Wyvern",
                "Grieve",
                "King",
                "Queen",
                "One",
                "ShadowEura",
                "Clown",
                "American",
                "European",
                "Canadian",
                "Stardust",
                "Time",
                "Mega",
                "Killer",
                "Skeleton",
                "Mantra",
                "Fencer",
                "Titan",
                "Alien",
                "Cheese",
                "Onion",
                "Moogle",
                "Chocobo",
            };

            var endingStrings = new List<string>()
            {
                "Poison",
                "Posion",
                "Elixir",
                "Ointment",
                "Thoughts",
                "Smoke",
                "Tundra",
                "Beer",
                "Duck",
                "MonoDron",
                "Wyvern",
                "Grieve",
                "King",
                "Queen",
                "Prince",
                "Clown",
                "Slime",
                "Adventure",
                "Diver",
                "Crusader",
                "Hero",
                "Stopper",
                "Man",
                "Knight",
                "Pokemon",
                "Charmander",
                "Squirtle",
                "Bulbasaur",
                "Mudkip",
                "Piplup",
                "Skeleton",
                "Mantra",
                "Key",
                "Fencer",
                "Titan",
                "Alien",
                "Cheese",
                "Onion",
                "Moogle",
                "Chocobo",
                "Warrior",
                "Quest",
            };

            foreach (var id in Enum.GetValues(typeof(ShopRandomizer.Id)))
            {
                endingStrings.Add(id.ToString());
            }

            string suffix = "_";
            suffix += beginningStrings[random.Next(beginningStrings.Count)];
            suffix += middleStrings[random.Next(middleStrings.Count)];
            suffix += endingStrings[random.Next(endingStrings.Count)];
            return suffix;
        }

        private void AddTowerShuffleModifications(byte[] content, bool addSection, byte finalPalette)
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

        private List<Level> GetLevels(byte[] content, Random random)
        {
            var levels = new List<Level>();
            levels.Add(new Eolis(WorldNumber.Eolis, content));
            levels.Add(new Trunk(WorldNumber.Trunk, content));
            levels.Add(new Mist(WorldNumber.Mist, content));
            levels.Add(new Branch(WorldNumber.Branch, content));
            levels.Add(new Dartmoor(WorldNumber.Dartmoor, content));
            levels.Add(new Zenis(WorldNumber.EvilOnesLair, content));
            levels.Add(new Buildings(WorldNumber.Buildings, content));
            levels.Add(new Towns(WorldNumber.Towns, content));

            foreach (var level in levels)
            {
                Level.LevelDict[level.Number] = level;
            }

            AddSublevels(levels);
            SubLevel.SubLevelDict[SubLevel.Id.MiddleTrunk].Screens[4].Sprites[0].RequiresMattock = true;
            if (ItemOptions.GuaranteeMattock)
            {
                SubLevel.SubLevelDict[SubLevel.Id.MiddleTrunk].Screens[4].Sprites[0].ShouldBeShuffled = false;
                SubLevel.SubLevelDict[SubLevel.Id.MiddleTrunk].Screens[4].Sprites[0].Id = Sprite.SpriteId.MattockBossLocked;
            }

            if (Util.AllWorldScreensRandomized())
            {
                var topItem = Level.LevelDict[WorldNumber.Branch].Screens[30].Sprites[2];
                topItem.ShouldBeShuffled = false;
                var possibleItems = new List<Sprite.SpriteId>
                {
                    Sprite.SpriteId.HourGlass,
                    Sprite.SpriteId.Poison,
                    Sprite.SpriteId.Poison2,
                    Sprite.SpriteId.RedPotion,
                    Sprite.SpriteId.RedPotion2,
                    Sprite.SpriteId.Glove,
                    Sprite.SpriteId.Ointment,
                    Sprite.SpriteId.Ointment2,
                    Sprite.SpriteId.Wingboots,
                    Sprite.SpriteId.WingbootsBossLocked,
                };
                topItem.Id = possibleItems[random.Next(0, possibleItems.Count)];
            }

            SubLevel.SubLevelDict[SubLevel.Id.MiddleTrunk].RequiresMattock = true;
            SubLevel.SubLevelDict[SubLevel.Id.EastTrunk].Screens[6].Sprites[0].RequiresWingBoots = true;
            var branchHiddenItem = SubLevel.SubLevelDict[SubLevel.Id.EastBranch].Screens[8].Sprites[0];
            branchHiddenItem.RequiresWingBoots = true;
            SubLevel.SubLevelDict[SubLevel.Id.MasconTower].Screens[0].SkipBosses = true;
            SubLevel.SubLevelDict[SubLevel.Id.Dartmoor].Screens[11].SkipBosses = true;
            SubLevel.SubLevelDict[SubLevel.Id.CastleFraternal].Screens[3].SkipBosses = true;
            SubLevel.SubLevelDict[SubLevel.Id.CastleFraternal].Screens[4].SkipBosses = true;
            SubLevel.SubLevelDict[SubLevel.Id.CastleFraternal].Screens[3].Sprites[2].RequiresMattock = true;
            if (ItemOptions.MattockUsage == ItemOptions.MattockUsages.Unchanged ||
                ItemOptions.MattockUsage == ItemOptions.MattockUsages.AnywhereExceptBannedScreens)
            {
                SubLevel.SubLevelDict[SubLevel.Id.CastleFraternal].Screens[3].Sprites[2].ShouldBeShuffled = false;
            }

            SubLevel.SubLevelDict[SubLevel.Id.EvilOnesLair].Screens[14].SkipBosses = true;
            if (ItemOptions.BigItemSpawns != ItemOptions.BigItemSpawning.AlwaysSpawn &&
                (EnemyOptions.EnemySet == EnemyOptions.EnemySetType.NonMixed ||
                 EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Unchanged))
            {
                SubLevel.SubLevelDict[SubLevel.Id.MasconTower].Screens[0].Sprites[1].RequiresWingBoots = true;
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
                if (level.Number == WorldNumber.Eolis)
                {
                    var sublevel = new SubLevel(SubLevel.Id.Eolis, level.Screens.GetRange(1, level.Screens.Count - 1));
                    SubLevel.SubLevelDict[SubLevel.Id.Eolis] = sublevel;
                    level.SubLevels.Add(sublevel);
                }
                else if (level.Number == WorldNumber.Trunk)
                {
                    level.AddSubLevel(SubLevel.Id.EarlyTrunk, 0, 7);
                    level.AddSubLevel(SubLevel.Id.MiddleTrunk, 8, 12);
                    level.AddSubLevel(SubLevel.Id.LateTrunk, 22, 26);
                    level.AddSubLevel(SubLevel.Id.TowerOfTrunk, 13, 21);
                    level.AddSubLevel(SubLevel.Id.EastTrunk, 28, 40);
                    level.AddSubLevel(SubLevel.Id.TowerOfFortress, 41, level.Screens.Count - 3);
                    level.AddSubLevel(SubLevel.Id.JokerHouse, level.Screens.Count - 2, level.Screens.Count - 1);

                    SubLevel.SkySpringSublevel = SubLevel.Id.EastTrunk;
                    SubLevel.FortressSpringSublevel = SubLevel.Id.TowerOfFortress;
                    SubLevel.JokerSpringSublevel = SubLevel.Id.JokerHouse;
                }
                else if (level.Number == WorldNumber.Mist)
                {
                    level.AddSubLevel(SubLevel.Id.EarlyMist, 16, 17);
                    level.SubLevels[0].AddScreen(level.Screens[1]);
                    level.SubLevels[0].AddScreen(level.Screens[9]);
                    level.AddSubLevel(SubLevel.Id.MiddleMist, 12, 13);
                    level.SubLevels[1].AddScreen(level.Screens[0]);
                    level.SubLevels[1].AddScreen(level.Screens[6]);
                    level.SubLevels[1].AddScreen(level.Screens[13]);
                    level.SubLevels[1].AddScreen(level.Screens[22]);
                    level.AddSubLevel(SubLevel.Id.LateMist, 23, 32);
                    level.SubLevels[2].AddScreen(level.Screens[Mist.VictimRightScreen]);
                    level.SubLevels[2].AddScreen(level.Screens[Mist.FireMageScreen]);
                    level.AddSubLevel(SubLevel.Id.TowerOfSuffer, 47, 61);
                    level.AddSubLevel(SubLevel.Id.TowerOfMist, 62, 76);
                    level.AddSubLevel(SubLevel.Id.MasconTower, 77, 79);
                    level.AddSubLevel(SubLevel.Id.VictimTower, 80, level.Screens.Count - 1);
                }
                else if (level.Number == WorldNumber.Branch)
                {
                    level.AddSubLevel(SubLevel.Id.EarlyBranch, 3, 6);
                    level.AddSubLevelScreens(level.SubLevels[0], 10, 14);
                    level.AddSubLevel(SubLevel.Id.BattleHelmetWing, 7, 9);
                    level.AddSubLevel(SubLevel.Id.MiddleBranch, 15, 18);
                    level.AddSubLevel(SubLevel.Id.DropDownWing, 22, 24);
                    level.AddSubLevel(SubLevel.Id.EastBranch, 25, 35);
                    level.AddSubLevel(SubLevel.Id.BackFromEastBranch, 19, 19);
                }
                else if (level.Number == WorldNumber.Dartmoor)
                {
                    level.AddSubLevel(SubLevel.Id.Dartmoor, 0, 14);
                    level.AddSubLevel(SubLevel.Id.CastleFraternal, 16, 20);
                    level.AddSubLevelScreens(level.SubLevels[1], 22, level.Screens.Count - 1);
                    level.AddSubLevel(SubLevel.Id.KingGrieve, 21, 21);

                }
                else if (level.Number == WorldNumber.EvilOnesLair)
                {
                    level.AddSubLevel(SubLevel.Id.EvilOnesLair, 0, level.Screens.Count - 1);
                }
                else if (level.Number == WorldNumber.Buildings)
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

            if (GeneralOptions.FlexibleItems)
            {
                content[Section.GetOffset(12, 0x8B87, 0x8000)] = 0xFF;
                content[Section.GetOffset(15, 0xC47C, 0xC000)] = OpCode.NOP;
                content[Section.GetOffset(15, 0xC47D, 0xC000)] = OpCode.NOP;
            }

            if (GeneralOptions.AddKillSwitch)
            {
                var switchSection = new Section();
                switchSection.Bytes.Add(OpCode.JSR);
                switchSection.Bytes.Add(0xE4);
                switchSection.Bytes.Add(0xFE);
                switchSection.AddToContent(content, Section.GetOffset(15, 0xE039, 0xC000));

                switchSection = new Section();
                switchSection.Bytes.Add(OpCode.JSR);
                switchSection.Bytes.Add(0xA8);
                switchSection.Bytes.Add(0xCB);
                switchSection.Bytes.Add(OpCode.LDAZeroPage);
                switchSection.Bytes.Add(0x19);
                switchSection.Bytes.Add(OpCode.ANDImmediate);
                switchSection.Bytes.Add(0x20);
                switchSection.Bytes.Add(OpCode.BEQ);
                switchSection.Bytes.Add(0x05);
                switchSection.Bytes.Add(OpCode.LDAImmediate);
                switchSection.Bytes.Add(0x01);
                switchSection.Bytes.Add(OpCode.STAAbsolute);
                switchSection.Bytes.Add(0x38);
                switchSection.Bytes.Add(0x04);
                switchSection.Bytes.Add(OpCode.RTS);
                switchSection.AddToContent(content, Section.GetOffset(15, 0xFEE4, 0xC000));
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
            if (ItemOptions.WingbootDurationSetting == ItemOptions.WingBootDurations.Permanent)
            {
                times.Add(1);
                times.Add(1);
                times.Add(1);
                times.Add(1);
                UpdateWingBootsTimer(times, content);
                var bootSection = new Section();
                bootSection.Bytes.Add(OpCode.NOP);
                bootSection.Bytes.Add(OpCode.NOP);
                bootSection.Bytes.Add(OpCode.NOP);
                bootSection.AddToContent(content, Section.GetOffset(15, 0xC5AE, 0xC000));
            }
            else if (ItemOptions.WingbootDurationSetting == ItemOptions.WingBootDurations.Random)
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

            if (ItemOptions.MattockUsage == ItemOptions.MattockUsages.AnywhereExceptBannedScreensAllowMattockLockedItems ||
                ItemOptions.MattockUsage == ItemOptions.MattockUsages.AnywhereExceptBannedScreens)
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
                mattocksection.Bytes.Add(OpCode.LDAAbsolute);
                mattocksection.Bytes.Add(0x24);
                mattocksection.Bytes.Add(0x00);
                mattocksection.Bytes.Add(OpCode.CMPImmediate);
                mattocksection.Bytes.Add(0x05);
                mattocksection.Bytes.Add(OpCode.BNE);
                mattocksection.Bytes.Add(0x0A);
                mattocksection.Bytes.Add(OpCode.LDAAbsolute);
                mattocksection.Bytes.Add(0x63);
                mattocksection.Bytes.Add(0x00);
                mattocksection.Bytes.Add(OpCode.CMPImmediate);
                mattocksection.Bytes.Add(0x1E);
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
