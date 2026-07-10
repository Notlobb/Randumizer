using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{

    public readonly record struct RandomizationResult(
        byte[] Rom,
        List<string> SpoilerLog,
        string FileNameSuffix);

    public class RandomizationException : Exception
    {
        public RandomizationException(string message)
            : base(message)
        {
        }
    }

    public class Randomizer
    {
        public RandomizationResult Randomize(byte[] inputFileContents, string[] customTextFileContents, string flags, int seed)
        {
            Random random = new Random(seed);

#if !DEBUG
            if (GeneralOptions.GenerateSpoilerLog)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (random.Next(0, 2) > 2)
                    {
                        throw new RandomizationException("Unexpected error");
                    }
                }
            }
#endif

            byte[] content = (byte[])inputFileContents.Clone();
            AddMiscHacks(content, random);

            var levels = GetLevels(content, random);
            uint screenAttempts = 0;
            var attemptDictionary = new Dictionary<WorldNumber, uint>();
            if (GeneralOptions.RandomizeScreens != GeneralOptions.ScreenRandomization.Unchanged)
            {
                foreach (var level in levels)
                {
                    uint localAttempts = screenAttempts;
                    bool screenResult = level.RandomizeScreens(random, ref screenAttempts);
                    localAttempts = screenAttempts - localAttempts;
                    attemptDictionary[level.Number] = localAttempts;
                    if (!screenResult)
                    {
                        throw new RandomizationException("Screen randomization failed");
                    }
                }
            }

            var segmentRandomizer = new SegmentRandomizer(content);
            var doorRandomizer = new DoorRandomizer(content, random);
            var shopRandomizer = new ShopRandomizer(content, doorRandomizer);
            var giftRandomizer = new GiftRandomizer(content);
            doorRandomizer.UpdateBuildings(giftRandomizer, shopRandomizer);
            doorRandomizer.LimitKeys(random);

            var spriteBehaviourTable = new Table(Section.GetOffset(14, ROM.SpriteBehaviorScriptTable), 100, 2, content);

            if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Mixed)
            {
                AsmHacks.DynamicHackConfigureGiftItemSprites(levels, content, ROM.HackItemPickup, spriteBehaviourTable,
                    ItemOptions.BigItemSpawns == ItemOptions.BigItemSpawning.Unchanged);
            }

            var itemRandomizer = new ItemRandomizer(random);
            if (!itemRandomizer.ShuffleItems(levels, shopRandomizer, giftRandomizer, doorRandomizer, segmentRandomizer, content, out uint attempts))
            {
                throw new RandomizationException("Item randomization failed");
            }

            doorRandomizer.FinalizeWorlds(levels, random, content);

            if (ItemOptions.AlwaysSpawnSmallItems)
            {
                // route several item behaviors to the Wing Boots behavior
                spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Hourglass] =
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
                    spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Glove2] =
                        spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Wingboots];
                }

                // make the Wing Boots and all other items routed to its action handler
                // always show, not only 1/4 of the time
                var section = new Section();
                section.NOP(3);
                section.AddToContent(content, Section.GetOffset(14, ROM.ItemWingBootsRandom_JMP_Sprites_Remove));
            }

            if (ItemOptions.BigItemSpawns == ItemOptions.BigItemSpawning.AlwaysLockBehindBosses)
            {
                AsmHacks.DynamicHackBossLockedItemsCheckNoBossesRemaining(content, 14,
                    ROM.HackBossLockedItemsCheckNoBossesRemaining);

                spriteBehaviourTable.Entries[(int)Sprite.SpriteId.Rod] =
                    spriteBehaviourTable.Entries[(int)Sprite.SpriteId.WingbootsBossLocked];
            }
            else if (ItemOptions.BigItemSpawns == ItemOptions.BigItemSpawning.AlwaysSpawn)
            {
                var section = new Section();
                section.NOP(5);
                section.AddToContent(content, Section.GetOffset(14, ROM.SpriteBehavior_BattleSuit_CheckForBosses));
                section.AddToContent(content, Section.GetOffset(14, ROM.SpriteBehavior_BattleHelmet_CheckForBosses));
                section.AddToContent(content, Section.GetOffset(14, ROM.SpriteBehavior_DragonSlayer_CheckForBosses));
                section.AddToContent(content, Section.GetOffset(14, ROM.SpriteBehavior_QMattock_CheckForBosses));
                section.AddToContent(content, Section.GetOffset(14, ROM.SpriteBehavior_QWingBoots_CheckForBosses));
                section.AddToContent(content, Section.GetOffset(14, ROM.SpriteBehavior_BlackOnyx_CheckForBosses));
                section.AddToContent(content, Section.GetOffset(14, ROM.SpriteBehavior_Pendant_CheckForBosses));
            }

            if (ItemOptions.BigItemSpawns != ItemOptions.BigItemSpawning.Unchanged)
            {
                spriteBehaviourTable.Entries[(int)Sprite.SpriteId.MattockBossLocked] =
                    spriteBehaviourTable.Entries[(int)Sprite.SpriteId.WingbootsBossLocked];

                AsmHacks.StaticHackIgnoreWingBootsQuestRequirement(content);

                var section = new Section();
                section.JMP(ROM.Sprite_Remove);
                section.RTS();
                section.NOP(6);
                section.AddToContent(content, Section.GetOffset(14, ROM.Sprites_RemoveAll));
            }

            if (ItemOptions.RandomizeBarRank)
            {
                byte rank = (byte)random.Next(6, 11);
                var section = new Section();
                section.Db(rank);
                section.AddToContent(content, Section.GetOffset(12, ROM.ScriptVictimBarCheckRankOperand));
                giftRandomizer.BarRank = rank;
            }

            var enemyRandomizer = new EnemyRandomizer();
            Screen.SetupEnemyIds();
            var enemyHPTable = new Table(Section.GetOffset(14, ROM.SpriteHPTable), 100, 1, content);
            var enemyDamageTable = new Table(Section.GetOffset(14, ROM.SpriteDamageTable), 100, 1, content);
            var enemyExperienceTable = new Table(Section.GetOffset(14, ROM.SpriteXPTable), 100, 1, content);
            var enemyRewardTypeTable = new Table(Section.GetOffset(14, ROM.SpriteDropIndexTable), 100, 1, content);
            var enemyRewardQuantityTable = new Table(Section.GetOffset(14, ROM.SpriteDropValueTable), 63, 1, content);
            var magicResistanceTable = new Table(Section.GetOffset(14, ROM.SpriteMagicDefenseTable), 100, 1, content);

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
                    enemyRandomizer.RandomizeBehaviour((int)Sprite.SpriteId.StillKnight, spriteBehaviourTable, random);
                    enemyRandomizer.UpdateSpriteValues((int)Sprite.SpriteId.StillKnight, 40, 20, 100, 50, enemyHPTable, enemyDamageTable, enemyExperienceTable, enemyRewardTypeTable);
                }
            }

            if (EnemyOptions.EnemyHPSetting != EnemyOptions.EnemyHP.Unchanged)
            {
                enemyRandomizer.RandomizeEnemyHP(enemyHPTable, random);
            }

            if (EnemyOptions.EnemyDamageSetting != EnemyOptions.EnemyDamage.Unchanged)
            {
                enemyRandomizer.RandomizeEnemyDamage(enemyDamageTable, random);
            }

            if (EnemyOptions.RandomizeExperience)
            {
                enemyRandomizer.RandomizeExperience(enemyExperienceTable, random);
            }

            if (EnemyOptions.RandomizeRewards)
            {
                enemyRandomizer.RandomizeRewards(enemyRewardTypeTable, enemyRewardQuantityTable, random);
            }

            if (EnemyOptions.RandomizeMagicImmunities)
            {
                enemyRandomizer.RandomizeMagicImmunities(magicResistanceTable, random, content);
            }

            var enemyBehaviourDict = new Dictionary<Sprite.SpriteId, Sprite.SpriteId>();
            if (EnemyOptions.AISetting != EnemyOptions.AIShuffle.Unchanged)
            {
                enemyRandomizer.RandomizeBehaviours(spriteBehaviourTable, random, enemyBehaviourDict);
            }

            if (EnemyOptions.AIPropertySetting != EnemyOptions.AIProperrtyRandomization.Unchanged)
            {
                enemyRandomizer.RandomizeBehaviourProperties(content, random);
            }

            doorRandomizer.AddToContent(content);

            if (GeneralOptions.DarkTowers)
            {
                // world 7 (Zenis) default palette
                content[Section.GetOffset(15, ROM.WorldToPaletteTable + 7)] = PaletteRandomizer.DarkPalette;
                // palette-to-music table entry 1 (Trunk Towers palette)
                content[Section.GetOffset(15, ROM.Palette_To_Music_Table_Palettes + 1)] = PaletteRandomizer.DarkPalette;

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
                textRandomizer.RandomizeTitles(content, customTextFileContents);
            }

            textRandomizer.UpdateText(shopRandomizer, giftRandomizer, doorRandomizer, segmentRandomizer, content, customTextFileContents);

            var titleText = Text.GetAllTitleText(content, Section.GetOffset(12, ROM.StringTitleScreenCopyright),
                                                 Section.GetOffset(12, ROM.StringTitleLicensedTerminator));
            Text.AddTitleText(0, "RANDUMIZER V29B5", titleText);
            var hash = Util.Fnv1aHash(flags).ToString("X8");

            Text.AddTitleText(1, $"FLAG HASH {hash}", titleText);
            Text.AddTitleText(2, $"SEED {seed}", titleText);
            Text.SetAllTitleText(content, titleText, Section.GetOffset(12, ROM.StringTitleScreenCopyright));

            var paletteRandomizer = new PaletteRandomizer(random);
            RandomizeExtras(content, random, doorRandomizer, paletteRandomizer, out bool paletteToMusicLogicReplaced);

            if (GeneralOptions.ShuffleTowers)
            {
                AddTowerShuffleModifications(content, paletteToMusicLogicReplaced, paletteRandomizer.FinalPalette, paletteRandomizer.BranchPalette);
            }

            string suffix = "";

            if (ExtraOptions.AppendSuffix)
            {
                suffix = TextRandomizer.GetSuffix(random);
            }

            var spoilers = new List<string>();
            if (GeneralOptions.GenerateSpoilerLog)
            {
                spoilers.Add("Randumizer v0.29 beta 5");
                spoilers.Add($"Seed {seed}");
                spoilers.Add($"Flags {flags}");
#if DEBUG
                spoilers.Add($"Randomization attempts: {attempts}");
                spoilers.Add($"Screen randomization attempts: {screenAttempts}");
                foreach (var key in attemptDictionary.Keys)
                {
                    spoilers.Add($"{key}: {attemptDictionary[key]}");
                }
#endif
                var hints = textRandomizer.GetHints(shopRandomizer, giftRandomizer, doorRandomizer, segmentRandomizer, true);
                foreach (var hint in hints)
                {
                    string spoiler = hint.Replace(Text.spaceChar, ' ');
                    spoiler = spoiler.Replace(Text.lineBreakChar, ' ');
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

                foreach (var shop in shopRandomizer.Shops)
                {
                    spoilers.Add($"{shop.ShopId}");
                    foreach (var item in shop.Items)
                    {
                        spoilers.Add($"{item.Id} {item.Price}");
                    }
                }

                foreach (var staticPrice in shopRandomizer.StaticPrices)
                {
                    spoilers.Add($"{staticPrice.ShopId} {staticPrice.Price}");
                }

                var enemyData = new Dictionary<Sprite.SpriteId, SpriteType>();
                for (int i = 0; i < 100; i++)
                {
                    if (Sprite.enemies.Contains((Sprite.SpriteId)i) ||
                        Sprite.Projectiles.Contains((Sprite.SpriteId)i) ||
                        (Sprite.SpriteId)i == Sprite.SpriteId.Rock)
                    {
                        var id = (Sprite.SpriteId)i;
                        var hp = enemyHPTable.Entries[i][0];
                        var damage = enemyDamageTable.Entries[i][0];
                        Sprite.SpriteId ai;
                        if (enemyBehaviourDict.ContainsKey(id))
                        {
                            ai = enemyBehaviourDict[id];
                        }
                        else
                        {
                            ai = id;
                        }

                        enemyData[id] = new SpriteType(id, hp, damage, ai);
                    }
                }

                var spells = new Dictionary<Spell.Id, Spell>();
                spells[Spell.Id.Deluge] = new Spell(Spell.Id.Deluge, content[Section.GetOffset(14, ROM.MagicCostTable)],
                                                    content[Section.GetOffset(14, ROM.MagicDamageTable)]);
                spells[Spell.Id.Thunder] = new Spell(Spell.Id.Thunder, content[Section.GetOffset(14, ROM.MagicCostTable + 1)],
                                                     content[Section.GetOffset(14, ROM.MagicDamageTable + 1)]);
                spells[Spell.Id.Fire] = new Spell(Spell.Id.Fire, content[Section.GetOffset(14, ROM.MagicCostTable + 2)],
                                                  content[Section.GetOffset(14, ROM.MagicDamageTable + 2)]);
                spells[Spell.Id.Death] = new Spell(Spell.Id.Death, content[Section.GetOffset(14, ROM.MagicCostTable + 3)],
                                                   content[Section.GetOffset(14, ROM.MagicDamageTable + 3)]);
                spells[Spell.Id.Tilte] = new Spell(Spell.Id.Tilte, content[Section.GetOffset(14, ROM.MagicCostTable + 4)],
                                                   content[Section.GetOffset(14, ROM.MagicDamageTable + 4)]);

                spoilers.Add($"Id Hp Damage AI");
                foreach (var data in enemyData.Values)
                {
                    spoilers.Add($"{data.Id} {data.Hp} {data.Damage} {data.AI}");
                }

                spoilers.Add($"Id Manacost Damage");
                foreach (var spell in spells.Values)
                {
                    spoilers.Add($"{spell.SpellId} {spell.ManaCost} {spell.Damage}");
                }
#endif
            }

            return new RandomizationResult(content, spoilers, suffix);
        }

        // this applies the famous sameworld-door to otherstage-door hack
        private void AddTowerShuffleModifications(byte[] content, bool paletteToMusicLogicReplaced, byte finalPalette, byte branchPalette)
        {
            AsmHacks.DynamicHackFlexibleDoorsApplyPendingStage(content, 15, ROM.HackApplyPendingStage);
            AsmHacks.DynamicHackFlexibleDoorsHandlePalette(content, 15, ROM.HackClearPendingStageAndApplyPalette);
            AsmHacks.DynamicHackFlexibleDoorsExtractStageAndDoorRequirement(content, 15, ROM.HackExtractStageAndDoorRequirement);

            // if the palette-to-music logic has not already been replaced, extend it for
            // tower shuffle so palettes missing from the vanilla map (Branch and Zenis)
            // can select the correct music when same-world doors lead to other stages
            if (!paletteToMusicLogicReplaced)
            {
                AsmHacks.DynamicHackFlexibleDoorsCustomPaletteToMusic(content, 15, ROM.HackCustomPaletteToMusicHandler,
                    branchPalette, finalPalette);
            }
            AsmHacks.DynamicHackFlexibleDoorsClearFlagAndLoadWorld(content, 15, ROM.HackClearPendingStageAndLoadWorld);

            //Set starting screen to Eolis shop screen,
            //since the original starting screen is now a tower
            content[Section.GetOffset(15, ROM.Start_Screen_Index)] = Eolis.ShopScreen;
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

            AddSublevels(levels, content);
            SubLevel.SubLevelDict[SubLevel.Id.MiddleTrunk].Screens[4].Sprites[0].RequiresMattock = true;
            if (ItemOptions.GuaranteeMattock)
            {
                SubLevel.SubLevelDict[SubLevel.Id.MiddleTrunk].Screens[4].Sprites[0].ShouldBeShuffled = false;
                SubLevel.SubLevelDict[SubLevel.Id.MiddleTrunk].Screens[4].Sprites[0].Id = Sprite.SpriteId.MattockBossLocked;
            }

            if (Util.AllCoreWorldScreensRandomized())
            {
                var topItem = Level.LevelDict[WorldNumber.Branch].Screens[30].Sprites[2];
                topItem.ShouldBeShuffled = false;
                topItem.Id = ItemRandomizer.GetRandomItem(random);
            }

            SubLevel.SubLevelDict[SubLevel.Id.MiddleTrunk].RequiresMattock = true;
            SubLevel.SubLevelDict[SubLevel.Id.EastTrunk].Screens[5].Sprites[0].RequiresWingBoots = true;
            var branchHiddenItem = SubLevel.SubLevelDict[SubLevel.Id.EastBranch].Screens[8].Sprites[0];
            branchHiddenItem.RequiresWingBoots = true;
            SubLevel.SubLevelDict[SubLevel.Id.MiddleMist].RequiresWingboots = true;
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

        private void AddSublevels(List<Level> levels, byte[] content)
        {
            foreach (var level in levels)
            {
                if (level.Number == WorldNumber.Eolis)
                {
                    var sublevel = new SubLevel(SubLevel.Id.Eolis, level.Screens.GetRange(1, level.Screens.Count - 1));
                    SubLevel.SubLevelDict[SubLevel.Id.Eolis] = sublevel;
                    level.SubLevels.Add(sublevel);
                    if (GeneralOptions.ShuffleTowers)
                    {
                        level.AddSubLevel(SubLevel.Id.OutsideEolis, 0, 0, 0);
                    }
                }
                else if (level.Number == WorldNumber.Trunk)
                {
                    level.AddSubLevel(SubLevel.Id.EarlyTrunk, 0, 7, 6);
                    level.AddSubLevel(SubLevel.Id.MiddleTrunk, 8, 12, 6);
                    level.AddSubLevel(SubLevel.Id.LateTrunk, 22, 26, 6);
                    level.AddSubLevel(SubLevel.Id.TowerOfTrunk, 13, 21, 7);
                    level.AddSubLevel(SubLevel.Id.EastTrunk, 29, 40, 6);
                    level.AddSubLevel(SubLevel.Id.TowerOfFortress, 41, level.Screens.Count - 3, 7);
                    level.AddSubLevel(SubLevel.Id.JokerHouse, level.Screens.Count - 2, level.Screens.Count - 1, 7);

                    SubLevel.SkySpringSublevel = SubLevel.Id.EastTrunk;
                    SubLevel.FortressSpringSublevel = SubLevel.Id.TowerOfFortress;
                    SubLevel.JokerSpringSublevel = SubLevel.Id.JokerHouse;
                }
                else if (level.Number == WorldNumber.Mist)
                {
                    level.AddSubLevel(SubLevel.Id.EarlyMist, 16, 17, 10);
                    level.SubLevels[0].AddScreen(level.Screens[1]);
                    level.SubLevels[0].AddScreen(level.Screens[9]);
                    level.AddSubLevel(SubLevel.Id.MiddleMist, 12, 13, 10);
                    level.SubLevels[1].AddScreen(level.Screens[0]);
                    level.SubLevels[1].AddScreen(level.Screens[6]);
                    level.SubLevels[1].AddScreen(level.Screens[13]);
                    level.SubLevels[1].AddScreen(level.Screens[22]);
                    level.SubLevels[1].AddScreen(level.Screens[Mist.VictimLeftScreen]);
                    level.AddSubLevel(SubLevel.Id.LateMist, 23, 32, 10);
                    level.SubLevels[2].AddScreen(level.Screens[Mist.VictimRightScreen]);
                    level.SubLevels[2].AddScreen(level.Screens[Mist.FireMageScreen]);
                    level.AddSubLevel(SubLevel.Id.TowerOfSuffer, 47, 61, 11);
                    level.AddSubLevel(SubLevel.Id.TowerOfMist, 62, 76, 11);
                    level.AddSubLevel(SubLevel.Id.MasconTower, 77, 79, 11);
                    level.AddSubLevel(SubLevel.Id.VictimTower, 80, level.Screens.Count - 1, 11);
                }
                else if (level.Number == WorldNumber.Branch)
                {
                    level.AddSubLevel(SubLevel.Id.EarlyBranch, 3, 6, 8);
                    level.AddSubLevelScreens(level.SubLevels[0], 10, 14);
                    level.AddSubLevel(SubLevel.Id.BattleHelmetWing, 7, 9, 9);
                    level.AddSubLevel(SubLevel.Id.MiddleBranch, 15, 18, 9);
                    level.AddSubLevel(SubLevel.Id.DropDownWing, 22, 24, 9);
                    level.AddSubLevel(SubLevel.Id.EastBranch, 25, 35, 8);
                    level.AddSubLevel(SubLevel.Id.BackFromEastBranch, 19, 19, 9);
                    level.AddSubLevel(SubLevel.Id.LateBranch, Branch.DaybreakRightScreen, level.Screens.Count - 1, 8);
                }
                else if (level.Number == WorldNumber.Dartmoor)
                {
                    level.AddSubLevel(SubLevel.Id.Dartmoor, 0, 14, 12);
                    level.AddSubLevel(SubLevel.Id.CastleFraternal, 16, 20, 13);
                    level.AddSubLevelScreens(level.SubLevels[1], 22, level.Screens.Count - 1);
                    level.AddSubLevel(SubLevel.Id.KingGrieve, 21, 21, 13);
                }
                else if (level.Number == WorldNumber.EvilOnesLair)
                {
                    level.AddSubLevel(SubLevel.Id.EvilOnesLair, 0, level.Screens.Count - 1, 15);
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
                else if (level.Number == WorldNumber.Towns)
                {
                    level.AddSubLevel(SubLevel.Id.Apolune, 0, 1, 27);
                    level.AddSubLevel(SubLevel.Id.Forepaw, 2, 3, 27);
                    level.AddSubLevel(SubLevel.Id.Mascon, 4, 5, 27);
                    level.AddSubLevel(SubLevel.Id.Victim, 6, 7, 27);
                    level.AddSubLevel(SubLevel.Id.Conflate, 8, 9, 27);
                    level.AddSubLevel(SubLevel.Id.Daybreak, 10, 11, 27);
                    level.AddSubLevel(SubLevel.Id.DartmoorCity, 12, 13, 27);

                    level.Screens[0].Doors.Add(DoorId.ApoluneItemShop);
                    level.Screens[0].Doors.Add(DoorId.ApoluneKeyShop);
                    level.Screens[0].Doors.Add(DoorId.ApoluneBar);
                    level.Screens[1].Doors.Add(DoorId.ApoluneGuru);
                    level.Screens[1].Doors.Add(DoorId.ApoluneHospital);
                    level.Screens[1].Doors.Add(DoorId.ApoluneHouse);

                    level.Screens[2].Doors.Add(DoorId.ForepawMeatShop);
                    level.Screens[2].Doors.Add(DoorId.ForepawItemShop);
                    level.Screens[3].Doors.Add(DoorId.ForepawGuru);
                    level.Screens[3].Doors.Add(DoorId.ForepawHospital);
                    level.Screens[3].Doors.Add(DoorId.ForepawHouse);
                    level.Screens[3].Doors.Add(DoorId.ForepawKeyShop);

                    level.Screens[4].Doors.Add(DoorId.MasconBar);
                    level.Screens[4].Doors.Add(DoorId.MasconMeatShop);
                    level.Screens[4].Doors.Add(DoorId.MasconItemShop);
                    level.Screens[5].Doors.Add(DoorId.MasconKeyShop);
                    level.Screens[5].Doors.Add(DoorId.MasconHouse);
                    level.Screens[5].Doors.Add(DoorId.MasconHospital);

                    level.Screens[6].Doors.Add(DoorId.VictimGuru);
                    level.Screens[6].Doors.Add(DoorId.VictimHospital);
                    level.Screens[6].Doors.Add(DoorId.VictimHouse);
                    level.Screens[7].Doors.Add(DoorId.VictimMeatShop);
                    level.Screens[7].Doors.Add(DoorId.VictimKeyShop);
                    level.Screens[7].Doors.Add(DoorId.VictimItemShop);
                    level.Screens[7].Doors.Add(DoorId.VictimBar);

                    level.Screens[8].Doors.Add(DoorId.ConflateHouse);
                    level.Screens[8].Doors.Add(DoorId.ConflateMeatShop);
                    level.Screens[8].Doors.Add(DoorId.ConflateGuru);
                    level.Screens[8].Doors.Add(DoorId.ConflateHospital);
                    level.Screens[9].Doors.Add(DoorId.ConflateItemShop);
                    level.Screens[9].Doors.Add(DoorId.ConflateBar);

                    level.Screens[10].Doors.Add(DoorId.DaybreakKeyShop);
                    level.Screens[10].Doors.Add(DoorId.DaybreakItemShop);
                    level.Screens[10].Doors.Add(DoorId.DaybreakMeatShop);
                    level.Screens[10].Doors.Add(DoorId.DaybreakBar);
                    level.Screens[11].Doors.Add(DoorId.DaybreakGuru);
                    level.Screens[11].Doors.Add(DoorId.DaybreakHouse);

                    level.Screens[12].Doors.Add(DoorId.DartmoorGuru);
                    level.Screens[12].Doors.Add(DoorId.DartmoorBar);
                    level.Screens[12].Doors.Add(DoorId.DartmoorMeatShop);
                    level.Screens[13].Doors.Add(DoorId.DartmoorHospital);
                    level.Screens[13].Doors.Add(DoorId.DartmoorKeyShop);
                    level.Screens[13].Doors.Add(DoorId.DartmoorItemShop);
                }
            }
        }

        private void AddMiscHacks(byte[] content, Random random)
        {
            //Allow menu on first Eolis screen
            content[Section.GetOffset(15, ROM.CheckShowPlayerMenu_BEQ_Return, 0xC000)] = OpCode.NOP;
            content[Section.GetOffset(15, ROM.CheckShowPlayerMenu_BEQ_Return + 1, 0xC000)] = OpCode.NOP;

            if (ItemOptions.SmallKeyLimit == ItemOptions.KeyLimit.Zero)
            {
                //Use the fact that the small key messages are not used to add new ring messages
                content[Section.GetOffset(15, ROM.OpenDoorWithRingOfElf_ScriptID)] = 0x02;
                content[Section.GetOffset(15, ROM.OpenDoorWithRingOfDworf_ScriptID)] = 0x7B;
                content[Section.GetOffset(15, ROM.OpenDoorWithDemonsRing_ScriptID)] = 0x7C;
            }

            if (ItemOptions.FixPendantBug)
            {
                content[Section.GetOffset(14, ROM.PendantCheckOffset)] = OpCode.BEQ;
            }

            if (GeneralOptions.FlexibleItems)
            {
                AsmHacks.DynamicHackFlexibleItems(content, ROM.HackSellAnyItem);
            }

            if (GeneralOptions.AddKillSwitch)
            {
                AsmHacks.DynamicHackKillSwitch(content, 15, ROM.HackKillswitch);
            }

            if (GeneralOptions.AllowLoweringRespawn)
            {
                if (GeneralOptions.MiscDoorSetting == GeneralOptions.MiscDoors.Unchanged ||
                    GeneralOptions.MiscDoorSetting == GeneralOptions.MiscDoors.ShuffleExcludeTowns ||
                    GeneralOptions.MiscDoorSetting == GeneralOptions.MiscDoors.ShuffleIncludeTownsExceptGurusAndKeyshops ||
                    GeneralOptions.MiscDoorSetting == GeneralOptions.MiscDoors.ShuffleIncludeTownsExceptGurus)
                {
                    // set guru-given spawn point no matter what
                    var section = new Section();
                    section.NOP(5);
                    section.AddToContent(content, Section.GetOffset(12, ROM.ScriptActionSetSpawnPoint_CMP_Current));
                }
                else
                {
                    // allow the guru-given spawn point to move backward, but never set it to spawn point 0
                    var section = new Section();
                    section.CMP_imm(0x00);
                    section.BEQ(0x04);
                    section.NOP();
                    section.AddToContent(content, Section.GetOffset(12, ROM.ScriptActionSetSpawnPoint_CMP_Current));
                }
            }

            if (GeneralOptions.PreventKnockbackOnLadders)
            {
                AsmHacks.DynamicHackPreventKnockbackOnLadders(content, 15, ROM.HackPreventLadderKnockback);
            }

            if (GeneralOptions.MoveSpringQuestRequirement)
            {
                // remove the check on all 3 springs being open before using the push-blocks
                // ring of ruby check is still present
                var springSection = new Section();
                springSection.NOP(2);
                springSection.AddToContent(content, Section.GetOffset(15, ROM.Player_CheckPushingBlock_Check3Springs));
            }

            if (ItemOptions.WingbootDurationSetting != ItemOptions.WingBootDurations.Unchanged)
            {
                AsmHacks.StaticHackWingbootDurations(random, content, ItemOptions.WingbootDurationSetting);
            }


            if (ItemOptions.BuffGloves)
            {
                // set duration of glove to 40 (vanilla is 20)
                var section = new Section();
                section.Db(40);
                section.AddToContent(content, Section.GetOffset(15, ROM.Player_PickUpGlove_DurationValue));
            }

            if (ItemOptions.BuffHourglass)
            {
                // removes the code halving and re-drawing HP
                var section = new Section();
                section.NOP(9);
                section.AddToContent(content, Section.GetOffset(15, ROM.Player_UseHourGlass_ReduceHP));
            }

            if (ItemOptions.ShieldSetting == 0 || ItemOptions.ShieldSetting == 1)
            {
                AsmHacks.DynamicHackOintmentWorksWithShield(content, ROM.HackOintmentWorksWithShield);
            }

            if (ItemOptions.ShieldSetting == 1)
            {
                AsmHacks.DynamicHackStrengthenShieldsAgainstMagic(content, ROM.HackStrongerShields);
            }

            if (GeneralOptions.FastText)
            {
                AsmHacks.DynamicHackFastText(content, ROM.HackFastText, ROM.HackFastTextHelper);
                AsmHacks.DynamicHackFastTextHelper(content, ROM.HackFastTextHelper);
            }

            if (GeneralOptions.FastStart)
            {
                AsmHacks.DynamicHackFastStart(content, ROM.HackFastStart,
                    ItemOptions.RandomizeKeys == ItemOptions.KeyRandomization.Unchanged);
            }

            AsmHacks.DynamicHackDoorRequirementHandler(content, 15, ROM.HackNewDoorRequirementHandler,
                GeneralOptions.DragonSlayerRequired, GeneralOptions.PendantRodRubyRequired, GeneralOptions.MoveSpringQuestRequirement,
                GeneralOptions.UpdateMiscText && ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Unchanged);

            if (ItemOptions.MattockUsage == ItemOptions.MattockUsages.AnywhereExceptBannedScreensAllowMattockLockedItems ||
                ItemOptions.MattockUsage == ItemOptions.MattockUsages.AnywhereExceptBannedScreens)
            {
                AsmHacks.DynamicHackUseMattockAnywhereExceptBannedScreens(content, 15, ROM.HackMattockAnywhere);
            }
            else if (ItemOptions.MattockUsage != ItemOptions.MattockUsages.Unchanged)
            {
                var mattocksection = new Section();
                mattocksection.LDA_imm(0x00);
                mattocksection.NOP(4);
                mattocksection.AddToContent(content, Section.GetOffset(15, ROM.Player_UseMattock_LDA_MetatileID));
            }

            if (ItemOptions.ReplacePoison)
            {
                AsmHacks.DynamicHackPoisonAsManaPotion(content, ROM.HackPoisonAsManaPotion);
            }

            if (GeneralOptions.ShuffleSegments == GeneralOptions.SegmentShuffle.AllSegments)
            {
                AsmHacks.DynamicHackSegmentShuffleUpdateStageForOtherWorldTransition(content, 15,
                    ROM.HackOtherWorldTransitionSetShuffledStage,
                    ROM.HackWorldToShuffledStageTable,
                    Door.worldDict);
            }
        }

        private void RandomizeExtras(byte[] content, Random random, DoorRandomizer doorRandomizer, PaletteRandomizer paletteRandomizer, out bool paletteToMusicLogicReplaced)
        {
            // tell the caller whether this function replaced the vanilla palette-to-music logic,
            // so tower shuffle does not install another handler over the same code
            paletteToMusicLogicReplaced = false;

            if (ExtraOptions.RandomizePalettes)
            {
                paletteRandomizer.RandomizePalettes(content, random);
                doorRandomizer.RandomizeTowerPalettes(paletteRandomizer, content);
            }

            if (ExtraOptions.MusicSetting != Music.Unchanged)
            {
                paletteRandomizer.RandomizeMusic(content, random);
            }

            var section = new Section();
            if (ExtraOptions.MusicSetting == Music.None)
            {
                // bypass palette-to-music selection when music is disabled
                section.LDA_imm(0x00);
                section.NOP(2);
                paletteToMusicLogicReplaced = true;
            }
            else if (ExtraOptions.MusicSetting == Music.Random && ExtraOptions.RandomizePalettes)
            {
                // replace the vanilla palette lookup by deriving the music-table index
                // directly from the randomized destination palette
                section.LSR();
                section.TAX();
                section.INX();
                section.TXA();
                paletteToMusicLogicReplaced = true;
            }

            if (paletteToMusicLogicReplaced)
            {
                // remove the remainder of the vanilla palette-to-music lookup loop
                section.NOP(9);
                section.AddToContent(content, Section.GetOffset(15, ROM.Palette_Check_Loop_CMP_X));
            }

            if (ExtraOptions.RandomizeSounds)
            {
                var soundRandomizer = new SoundRandomizer();
                soundRandomizer.RandomizeSounds(content, random);
            }

            // only rebuild the soundtrack if music is enabled, and a non-original
            // soundtrack has been selected
            if (ExtraOptions.MusicSetting != Music.None &&
                ExtraOptions.SoundtrackSetting != Soundtrack.Original)
            {
                MusicTrackRandomizer.RandomizeMusicTracks(content, random,
                    ExtraOptions.SoundtrackSetting == Soundtrack.Mix);
            }
        }

        public static string GetOutputFilename(string inputFileName, int seed, string flags, string suffix)
        {
            int dotIndex = inputFileName.IndexOf(".nes");
#if DEBUG
            return inputFileName.Insert(dotIndex, "_" + seed.ToString() + suffix);
#else
            return inputFileName.Insert(dotIndex, "_" + seed.ToString() + "_" + flags + suffix);
#endif
        }
    }
}