using System;
using System.Collections.Generic;
using System.Linq;

namespace FaxanaduRando.Randomizer
{
    using CheckFunction = Func<ShopRandomizer, GiftRandomizer, DoorRandomizer, List<Level>, HashSet<ShopRandomizer.Id>, HashSet<SubLevel.Id>, HashSet<Guru.GuruId>, bool>;

    public class ItemRandomizer
    {
        private Dictionary<Sprite.SpriteId, ShopRandomizer.Id> shopIdDictionary = new Dictionary<Sprite.SpriteId, ShopRandomizer.Id>
        {
            { Sprite.SpriteId.BattleHelmet, ShopRandomizer.Id.BattleHelmet},
            { Sprite.SpriteId.BattleSuit, ShopRandomizer.Id.BattleSuit},
            { Sprite.SpriteId.DragonSlayer, ShopRandomizer.Id.DragonSlayer},
            { Sprite.SpriteId.Glove2OrKeyJoker, ShopRandomizer.Id.JokerKey},
            { Sprite.SpriteId.KeyAce, ShopRandomizer.Id.AceKey},
            { Sprite.SpriteId.MattockOrRingRuby, ShopRandomizer.Id.RubyRing},
            { Sprite.SpriteId.RingDworf, ShopRandomizer.Id.DworfRing},
            { Sprite.SpriteId.RingDemon, ShopRandomizer.Id.DemonRing},
            { Sprite.SpriteId.Wingboots, ShopRandomizer.Id.WingBoots},
            { Sprite.SpriteId.WingbootsBossLocked, ShopRandomizer.Id.WingBoots},
            { Sprite.SpriteId.RedPotion, ShopRandomizer.Id.RedPotion},
            { Sprite.SpriteId.RedPotion2, ShopRandomizer.Id.RedPotion},
            { Sprite.SpriteId.HourGlass, ShopRandomizer.Id.Hourglass},
            { Sprite.SpriteId.MattockBossLocked, ShopRandomizer.Id.Mattock},
            { Sprite.SpriteId.Rod, ShopRandomizer.Id.Rod},
            { Sprite.SpriteId.Elixir, ShopRandomizer.Id.Elixir},
            { Sprite.SpriteId.BlackOnyx, ShopRandomizer.Id.BlackOnyx},
            { Sprite.SpriteId.Pendant, ShopRandomizer.Id.Pendant},
        };

        private static readonly HashSet<Level.StartOffset> offsetsToShuffle= new HashSet<Level.StartOffset>
        {
            Level.StartOffset.Trunk,
            Level.StartOffset.Mist,
            Level.StartOffset.Branch,
            Level.StartOffset.DartmoorArea,
            Level.StartOffset.EvilOnesLair,
            Level.StartOffset.Buildings,
        };

        public Random Rand {get; set;}

        public ItemRandomizer(Random random)
        {
            Rand = random;
            if (ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Mixed)
            {
                shopIdDictionary.Remove(Sprite.SpriteId.Glove2OrKeyJoker);
                shopIdDictionary.Remove(Sprite.SpriteId.MattockOrRingRuby);
                shopIdDictionary.Remove(Sprite.SpriteId.KeyAce);
                shopIdDictionary.Remove(Sprite.SpriteId.RingDemon);
                shopIdDictionary.Remove(Sprite.SpriteId.RingDworf);
            }
        }

        public bool ShuffleItems(List<Level> levels,
                                 ShopRandomizer shopRandomizer,
                                 GiftRandomizer giftRandomizer,
                                 DoorRandomizer doorRandomizer,
                                 byte[] content,
                                 out uint attempts)
        {
            var items = new List<Sprite>();
            var extraItems = new List<Sprite.SpriteId>();
            var ids = new List<Sprite.SpriteId>();
            var startingWeapon = ShopRandomizer.Id.Dagger;
            if (ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.Random)
            {
                startingWeapon = Rand.Next(3) == 0 ? ShopRandomizer.Id.Dagger : ShopRandomizer.Id.LongSword;
            }
            else if (ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.Dagger)
            {
                startingWeapon = ShopRandomizer.Id.Dagger;
            }
            else if (ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.LongSword)
            {
                startingWeapon = ShopRandomizer.Id.LongSword;
            }
            var startingSpell = ShopRandomizer.Id.Deluge;
            var spells = new List<ShopRandomizer.Id>();

            if (ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Unchanged)
            {
                spells.AddRange(ShopRandomizer.spellIds);
                int spellIndex = Rand.Next(spells.Count);
                startingSpell = spells[spellIndex];
                spells.RemoveAt(spellIndex);

                foreach (var level in levels)
                {
                    if (level.IsEolis())
                    {
                        //Not a shuffle but there's only one intro item in Eolis with a weird location
                        var possibleItems = new List<Sprite.SpriteId>
                        {
                            Sprite.SpriteId.Glove,
                            Sprite.SpriteId.Poison,
                            Sprite.SpriteId.RedPotion,
                            Sprite.SpriteId.Ointment,
                            Sprite.SpriteId.Elixir,
                            Sprite.SpriteId.WingbootsBossLocked,
                        };

                        if (ItemOptions.AlwaysSpawnSmallItems)
                        {
                            possibleItems.Add(Sprite.SpriteId.HourGlass);
                        }

                        foreach (var item in CollectItems(level))
                        {
                            item.Id = possibleItems.ElementAt(Rand.Next(possibleItems.Count));
                            if (item.Id == Sprite.SpriteId.WingbootsBossLocked ||
                                item.Id == Sprite.SpriteId.HourGlass)
                            {
                                //These items don't fall down
                                item.SetY(10);
                            }
                        }

                        break;
                    }
                }

                items = CollectItemsToShuffle(levels);
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Id == Sprite.SpriteId.Glove2OrKeyJoker)
                    {
                        ids.Add(Sprite.SpriteId.Glove);
                    }
                    else if (items[i].Id == Sprite.SpriteId.MattockOrRingRuby)
                    {
                        ids.Add(Sprite.SpriteId.MattockBossLocked);
                    }
                    else
                    {
                        ids.Add(items[i].Id);
                    }
                }

                if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Mixed)
                {
                    extraItems.Add(Sprite.SpriteId.Glove2OrKeyJoker);
                    extraItems.Add(Sprite.SpriteId.MattockOrRingRuby);
                    extraItems.Add(Sprite.SpriteId.KeyAce);
                    extraItems.Add(Sprite.SpriteId.RingDworf);
                    extraItems.Add(Sprite.SpriteId.RingDemon);
                    extraItems.Add(Sprite.SpriteId.RedPotion);
                    extraItems.Add(Sprite.SpriteId.Wingboots);

                    extraItems.Add(Sprite.SpriteId.Wingboots);
                    extraItems.Add(Sprite.SpriteId.Wingboots);
                    extraItems.Add(Sprite.SpriteId.HourGlass);
                    extraItems.Add(Sprite.SpriteId.Elixir);

                    if (IncludeEolisGuru())
                    {
                        extraItems.Add(Sprite.SpriteId.Elixir);
                    }
                }

                ids.AddRange(extraItems);
            }

            Util.ShuffleList(ids, 0, ids.Count - 1, Rand);
            var toShopIds = ids.GetRange(ids.Count - extraItems.Count, extraItems.Count);

            shopRandomizer.SetStaticItems(startingWeapon, startingSpell);

            var shopIds = shopRandomizer.GetBaseIds();
            if (ItemOptions.RandomizeKeys != ItemOptions.KeyRandomization.Unchanged)
            {
                shopIds.Add(ShopRandomizer.Id.JackKey);
                shopIds.Add(ShopRandomizer.Id.JackKey);
                shopIds.Add(ShopRandomizer.Id.JackKey);
                shopIds.Add(ShopRandomizer.Id.QueenKey);
                shopIds.Add(ShopRandomizer.Id.QueenKey);
                shopIds.Add(ShopRandomizer.Id.QueenKey);
                shopIds.Add(ShopRandomizer.Id.KingKey);
                shopIds.Add(ShopRandomizer.Id.KingKey);
                shopIds.Add(ShopRandomizer.Id.KingKey);
                shopIds.Add(ShopRandomizer.Id.ElfRing);
                shopIds.Add(GetMiscItem());
            }

            if (ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.NoGuaranteed ||
                ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.GuaranteeOnlyWithNoSpells)
            {
                shopIds.Add(ShopRandomizer.Id.Dagger);
                shopIds.Add(ShopRandomizer.Id.LongSword);
            }
            else if (startingWeapon == ShopRandomizer.Id.Dagger)
            {
                shopIds.Add(ShopRandomizer.Id.LongSword);
            }
            else
            {
                var miscId = GetMiscItem();
                shopIds.Add(miscId);
            }

            if (!ItemOptions.GuaranteeStartingSpell)
            {
                shopIds.Add(startingSpell);
            }

            shopIds.AddRange(spells);

            foreach (var id in toShopIds)
            {
                ShopRandomizer.Id shopId;
                if (shopIdDictionary.TryGetValue(id, out shopId))
                {
                    shopIds.Add(shopId);
                }
                else
                {
                    var miscId = GetMiscItem();
                    shopIds.Add(miscId);
                }
            }

            int giftCount = giftRandomizer.GiftItems.Count;
            if (!IncludeEolisGuru())
            {
                giftCount--;
            }

            if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Mixed)
            {
                Util.ShuffleList(shopIds, 0, shopIds.Count - 5, Rand);
                Util.ShuffleList(shopIds, shopIds.Count - giftCount, shopIds.Count - 1, Rand);
            }
            else
            {
                if (ItemOptions.ReplacePoison)
                {
                    shopIds.Add(ShopRandomizer.Id.BlackPotion);
                }
                else
                {
                    shopIds.Add(GetMiscItem());
                }

                shopIds.Add(ShopRandomizer.Id.WingBoots);
                shopIds.Add(ShopRandomizer.Id.Elixir);
                shopIds.Add(ShopRandomizer.Id.Hourglass);

                shopIds.Add(GetMiscItem());
                shopIds.Add(GetMiscItem());

                if (IncludeEolisGuru())
                {
                    shopIds.Add(GetMiscItem());
                }

                var giftItems = new List<ShopRandomizer.Id>();
                giftItems.Add(ShopRandomizer.Id.JokerKey);
                giftItems.Add(ShopRandomizer.Id.RubyRing);
                giftItems.Add(ShopRandomizer.Id.AceKey);
                giftItems.Add(ShopRandomizer.Id.DworfRing);
                giftItems.Add(ShopRandomizer.Id.DemonRing);
                shopIds.AddRange(giftItems);

                if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.NoMixed)
                {
                    Util.ShuffleList(shopIds, 0, shopIds.Count - giftItems.Count - 1, Rand);
                }
            }

            bool valid = false;
            attempts = 0;
            bool shuffleWorlds = GeneralOptions.ShuffleWorlds;

            while (!valid && attempts < 1000000)
            {
                attempts++;
                #if !DEBUG
                if (GeneralOptions.GenerateSpoilerLog)
                {
                    if (Rand.Next(0, 2) > 2)
                    {
                        break;
                    }
                }
                #endif

                if (GeneralOptions.ShuffleWorlds && attempts > 100000)
                {
                    shuffleWorlds = true;
                }

                if (ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Unchanged)
                {
                    Util.ShuffleList(ids, 0, ids.Count - extraItems.Count - 1, Rand);
                    SetIds(ids, 0, ids.Count - extraItems.Count, items);

                    if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.NoMixed ||
                        ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Mixed)
                    {
                        Util.ShuffleList(shopIds, 0, shopIds.Count - giftCount, Rand);
                        Util.ShuffleList(shopIds, shopIds.Count - giftCount, shopIds.Count - 1, Rand);
                    }
                    else
                    {
                        Util.ShuffleList(shopIds, 0, shopIds.Count - 1, Rand);
                    }

                    var shopList = shopIds.GetRange(0, shopIds.Count - giftCount);
                    shopRandomizer.AddShopItems(shopList);

                    var giftList = shopIds.GetRange(shopIds.Count - giftCount, giftCount);
                    giftRandomizer.AddGiftItems(giftList);
                }

                if (shuffleWorlds)
                {
                    doorRandomizer.ShuffleWorlds(Rand);
                    var worlds = doorRandomizer.GetWorlds();
                    shuffleWorlds = false;
                }

                if (GeneralOptions.MiscDoorSetting != GeneralOptions.MiscDoors.Unchanged)
                {
                    doorRandomizer.ShuffleMiscDoors(Rand);
                }

                if (ItemOptions.RandomizeKeys != ItemOptions.KeyRandomization.Unchanged)
                {
                    doorRandomizer.RandomizeKeys(content, Rand);
                }

                if (GeneralOptions.WorldDoorSetting != GeneralOptions.WorldDoors.Unchanged)
                {
                    doorRandomizer.ShuffleWorldDoors(Rand);
                }

                if (GeneralOptions.ShuffleTowers)
                {
                    doorRandomizer.ShuffleTowers(Rand);
                }

                if (ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.GuaranteeOnlyWithNoSpells)
                {
                    if (!EolisHasWeaponOrSpell(shopRandomizer, giftRandomizer, doorRandomizer))
                    {
                        continue;
                    }
                }

                if (ItemOptions.GuaranteeElixirNearFortress && !GuaranteedElixir(shopRandomizer, giftRandomizer, doorRandomizer, levels))
                {
                    continue;
                }

                if (ItemOptions.GuaranteeMattock && !GuaranteedMattock(shopRandomizer, giftRandomizer, doorRandomizer))
                {
                    continue;
                }

                if (!CheckSingleGifts(giftRandomizer))
                {
                    continue;
                }

                valid = CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, new HashSet<ShopRandomizer.Id>(),
                                   new HashSet<SubLevel.Id>(), new HashSet<Guru.GuruId>());
            }

            if (!valid)
            {
                return false;
            }

            if (ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Unchanged)
            {
                shopRandomizer.RandomizePrices(Rand, doorRandomizer);
                shopRandomizer.AddToContent(content);
                giftRandomizer.AddToContent(content);
            }

            return true;
        }

        private void SetIds(List<Sprite.SpriteId> ids, int startIndex, int endIndex,
                            List<Sprite> items)
        {
            for (int index = startIndex; index < endIndex; index++)
            {
                items[index].Id = ids[index];
            }
        }

        private bool CheckValid(ShopRandomizer shopRandomizer,
                                GiftRandomizer giftRandomizer,
                                DoorRandomizer doorRandomizer,
                                List<Level> levels,
                                HashSet<ShopRandomizer.Id> ids,
                                HashSet<SubLevel.Id> traversedSublevels,
                                HashSet<Guru.GuruId> gurus)
        {
            if (ItemOptions.BigItemSpawns == ItemOptions.BigItemSpawning.Unchanged)
            {
                foreach (var level in levels)
                {
                    foreach (var screen in level.Screens)
                    {
                        if (ContainsDoubleKeyItems(screen))
                        {
                            return false;
                        }
                    }
                }
            }

            ids.Add(ShopRandomizer.Id.Book);
            int itemCount = ids.Count;
            var worlds = doorRandomizer.GetWorlds();

            CheckDoor(DoorId.EolisItemShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.EolisKeyShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.EolisGuru, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.EolisHouse, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.EolisMeatShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.MartialArtsShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.EolisMagicShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
            }

            if (!ids.Contains(doorRandomizer.GetExitKey(DoorRandomizer.ExitDoor.EolisExit)))
            {
                return false;
            }

            bool result = GetCheck(worlds[1].number)(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
            if (!result)
            {
                return false;
            }

            result = GetCheck(worlds[2].number)(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
            if (!result)
            {
                return false;
            }

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
            }

            if (GeneralOptions.QuickSeed)
            {
                if (!FinalRequirementsMet(ids, traversedSublevels))
                {
                    return false;
                }
            }

            if (!Util.EarlyFinishPossible())
            {
                result = GetCheck(worlds[3].number)(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
                if (!result)
                {
                    return false;
                }

                result = GetCheck(worlds[4].number)(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
                if (!result)
                {
                    return false;
                }
            }

            if (!(GeneralOptions.IncludeEvilOnesFortress && GeneralOptions.ShuffleTowers) &&
                GeneralOptions.MoveFinalRequirements)
            {
                TraverseLevel(Level.LevelDict[Level.StartOffset.EvilOnesLair], ids);
            }

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
            }

            if (CanWin(ids, traversedSublevels))
            {
                return true;
            }

            if (!(GeneralOptions.IncludeEvilOnesFortress && GeneralOptions.ShuffleTowers))
            {
                if (FinalRequirementsMet(ids, traversedSublevels))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CanWin(HashSet<ShopRandomizer.Id> ids, HashSet<SubLevel.Id> traversedSublevels)
        {
            if (GeneralOptions.ShuffleTowers && GeneralOptions.IncludeEvilOnesFortress)
            {
                if (!traversedSublevels.Contains(SubLevel.Id.EvilOnesLair))
                {
                    return false;
                }

                if (GeneralOptions.MoveFinalRequirements && !FinalRequirementsMet(ids, traversedSublevels))
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        private CheckFunction GetCheck(WorldNumber number)
        {
            if (number == WorldNumber.Trunk)
            {
                return CheckTrunk;
            }
            else if (number == WorldNumber.Mist)
            {
                return CheckMist;
            }
            else if (number == WorldNumber.Branch)
            {
                return CheckBranch;
            }
            else if (number == WorldNumber.Dartmoor)
            {
                return CheckDartmoor;
            }

            return null;
        }

        private bool CheckTrunk(ShopRandomizer shopRandomizer,
                                GiftRandomizer giftRandomizer,
                                DoorRandomizer doorRandomizer,
                                List<Level> levels,
                                HashSet<ShopRandomizer.Id> ids,
                                HashSet<SubLevel.Id> traversedSublevels,
                                HashSet<Guru.GuruId> gurus)
        {
            int itemCount = ids.Count;
            CheckDoor(DoorId.TrunkSecretShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.ApoluneItemShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ApoluneKeyShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ApoluneBar, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ApoluneGuru, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ApoluneHospital, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ApoluneHouse, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.TowerOfTrunk, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
            }

            if (CanWin(ids, traversedSublevels))
            {
                return true;
            }

            if (!ids.Contains(ShopRandomizer.Id.Mattock))
            {
                return false;
            }

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyTrunk], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EastTrunk], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.ForepawItemShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ForepawKeyShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ForepawGuru, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ForepawHospital, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ForepawHouse, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ForepawMeatShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.TowerOfFortress, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.JokerHouse, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
            }

            if (CanWin(ids, traversedSublevels))
            {
                return true;
            }

            if (ItemOptions.MattockUsage != ItemOptions.MattockUsages.AnywhereUpdateLogic &&
                !GeneralOptions.MoveSpringQuestRequirement)
            {
                if (!(traversedSublevels.Contains(SubLevel.Id.TowerOfFortress) &&
                      traversedSublevels.Contains(SubLevel.Id.JokerHouse)))
                {
                    return false;
                }

                if (!ids.Contains(ShopRandomizer.Id.RubyRing) ||
                    !ids.Contains(ShopRandomizer.Id.Elixir) ||
                    !ids.Contains(ShopRandomizer.Id.WingBoots))
                {
                    return false;
                }
            }

            if (GeneralOptions.MoveSpringQuestRequirement &&
                ItemOptions.MattockUsage != ItemOptions.MattockUsages.AnywhereUpdateLogic)
            {
                if (!ids.Contains(ShopRandomizer.Id.RubyRing))
                {
                    return false;
                }
            }

            if (ItemOptions.MattockUsage == ItemOptions.MattockUsages.AnywhereUpdateLogic)
            {
                if (!ids.Contains(ShopRandomizer.Id.WingBoots))
                {
                    return false;
                }
            }

            if (!ids.Contains(doorRandomizer.GetExitKey(DoorRandomizer.ExitDoor.TrunkExit)))
            {
                return false;
            }

            return true;
        }

        private bool CheckMist(ShopRandomizer shopRandomizer,
                               GiftRandomizer giftRandomizer,
                               DoorRandomizer doorRandomizer,
                               List<Level> levels,
                               HashSet<ShopRandomizer.Id> ids,
                               HashSet<SubLevel.Id> traversedSublevels,
                               HashSet<Guru.GuruId> gurus)
        {
            int itemCount = ids.Count;

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyMist], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.MasconItemShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.MasconKeyShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.MasconBar, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.MasconHospital, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.MasconHouse, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.MasconMeatShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.MistLargeHouse, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.MistSmallHouse, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.BirdHospital, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.MasconTower, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.TowerOfSuffer, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
            }

            if (CanWin(ids, traversedSublevels))
            {
                return true;
            }

            if (!ids.Contains(ShopRandomizer.Id.WingBoots))
            {
                return false;
            }

            CheckDoor(DoorId.MistSecretShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.VictimItemShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.VictimKeyShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.VictimBar, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.VictimGuru, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.VictimHospital, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.VictimHouse, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.VictimMeatShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.FireMage, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.AceKeyHouse, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.LateMist], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.TowerOfMist, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.VictimTower, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
            }

            if (CanWin(ids, traversedSublevels))
            {
                return true;
            }

            if (!ids.Contains(doorRandomizer.GetExitKey(DoorRandomizer.ExitDoor.MistExit)))
            {
                return false;
            }

            return true;
        }

        private bool CheckBranch(ShopRandomizer shopRandomizer,
                                 GiftRandomizer giftRandomizer,
                                 DoorRandomizer doorRandomizer,
                                 List<Level> levels,
                                 HashSet<ShopRandomizer.Id> ids,
                                 HashSet<SubLevel.Id> traversedSublevels,
                                 HashSet<Guru.GuruId> gurus)
        {
            int itemCount = ids.Count;

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyBranch], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.ConflateGuru, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ConflateItemShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ConflateBar, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ConflateHospital, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ConflateHouse, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.ConflateMeatShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.BattleHelmetWing, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
            }

            if (CanWin(ids, traversedSublevels))
            {
                return true;
            }

            if (!ids.Contains(doorRandomizer.GetLevelKey(doorRandomizer.LevelDoors[DoorId.EastBranch])))
            {
                return false;
            }

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.MiddleBranch], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EastBranch], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.DropDownWing], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DaybreakItemShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DaybreakKeyShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DaybreakBar, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DaybreakGuru, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DaybreakHouse, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DaybreakMeatShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
            }

            if (CanWin(ids, traversedSublevels))
            {
                return true;
            }

            if (!ids.Contains(doorRandomizer.GetExitKey(DoorRandomizer.ExitDoor.BranchExit)))
            {
                return false;
            }

            return true;
        }

        private bool CheckDartmoor(ShopRandomizer shopRandomizer,
                                   GiftRandomizer giftRandomizer,
                                   DoorRandomizer doorRandomizer,
                                   List<Level> levels,
                                   HashSet<ShopRandomizer.Id> ids,
                                   HashSet<SubLevel.Id> traversedSublevels,
                                   HashSet<Guru.GuruId> gurus)
        {
            int itemCount = ids.Count;
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.Dartmoor], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DartmoorItemShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DartmoorKeyShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DartmoorMeatShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DartmoorBar, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DartmoorGuru, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DartmoorHospital, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.DartmoorHouse1, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DartmoorHouse2, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DartmoorHouse3, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.DartmoorHouse4, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.LeftOfDartmoor, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            CheckDoor(DoorId.FarLeftDartmoor, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.CastleFraternal, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            CheckDoor(DoorId.EvilOnesLair, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
            }

            if (CanWin(ids, traversedSublevels))
            {
                return true;
            }

            if (!HasLairEntranceAccess(doorRandomizer, ids))
            {
                return false;
            }

            if (!GeneralOptions.MoveFinalRequirements)
            {
                if (!FinalRequirementsMet(ids, traversedSublevels))
                {
                    return false;
                }
            }

            if (!ids.Contains(doorRandomizer.GetExitKey(DoorRandomizer.ExitDoor.DartmoorExit)))
            {
                return false;
            }

            return true;
        }

        private bool EolisHasWeaponOrSpell(ShopRandomizer shopRandomizer, GiftRandomizer giftRandomizer, DoorRandomizer doorRandomizer)
        {
            var tempIds = new HashSet<ShopRandomizer.Id>();
            tempIds.Add(ShopRandomizer.Id.Book);
            var tempSublevels = new HashSet<SubLevel.Id>();
            var tempGurus = new HashSet<Guru.GuruId>();

            CheckDoor(DoorId.EolisItemShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.EolisKeyShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.EolisGuru, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.EolisMagicShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.EolisMeatShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.EolisHouse, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.MartialArtsShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);

            foreach (var item in ShopRandomizer.spellIds)
            {
                if (tempIds.Contains(item))
                {
                    return true;
                }
            }

            foreach (var item in ShopRandomizer.weaponIds)
            {
                if (tempIds.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckSingleGifts(GiftRandomizer giftRandomizer)
        {
            if (ItemOptions.RandomizeKeys == ItemOptions.KeyRandomization.Unchanged ||
                ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Unchanged)
            {
                return true;
            }

            var result = CheckSingleGift(GiftItem.Id.ConflateGuru, giftRandomizer);
            if (!Util.GurusShuffled())
            {
                result &= CheckSingleGift(GiftItem.Id.EolisGuru, giftRandomizer);
            }

            if (!ItemOptions.AllowMultipleGifts)
            {
                result &= CheckSingleGift(GiftItem.Id.FortressGuru, giftRandomizer);
                result &= CheckSingleGift(GiftItem.Id.JokerHouse, giftRandomizer);
            }

            return result;
        }

        private bool CheckSingleGift(GiftItem.Id id, GiftRandomizer giftRandomizer)
        {
            if (giftRandomizer.ItemDict[id].Item == ShopRandomizer.Id.JokerKey ||
                giftRandomizer.ItemDict[id].Item == ShopRandomizer.Id.AceKey)
            {
                return false;
            }

            return true;
        }

        private bool GuaranteedMattock(ShopRandomizer shopRandomizer, GiftRandomizer giftRandomizer, DoorRandomizer doorRandomizer)
        {
            var tempIds = new HashSet<ShopRandomizer.Id>();
            tempIds.Add(ShopRandomizer.Id.Book);
            var tempSublevels = new HashSet<SubLevel.Id>();
            var tempGurus = new HashSet<Guru.GuruId>();
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EastTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);

            CheckDoor(DoorId.ForepawItemShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ForepawKeyShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ForepawGuru, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ForepawHospital, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ForepawHouse, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ForepawMeatShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);

            CheckDoor(DoorId.TowerOfFortress, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.JokerHouse, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);

            if (tempIds.Contains(ShopRandomizer.Id.Mattock))
            {
                return true;
            }

            return false;
        }

        private bool GuaranteedElixir(ShopRandomizer shopRandomizer, GiftRandomizer giftRandomizer, DoorRandomizer doorRandomizer, List<Level> levels)
        {
            var tempIds = new HashSet<ShopRandomizer.Id>();
            var tempSublevels = new HashSet<SubLevel.Id>();
            var tempGurus = new HashSet<Guru.GuruId>();
            tempIds.Add(ShopRandomizer.Id.Book);

            CheckDoor(DoorId.TrunkSecretShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ApoluneItemShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ApoluneKeyShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ApoluneBar, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ApoluneGuru, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ApoluneHospital, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ApoluneHouse, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.TowerOfTrunk, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);

            if (tempSublevels.Contains(SubLevel.Id.TowerOfFortress))
            {
                if (tempIds.Contains(ShopRandomizer.Id.Elixir))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            tempIds = new HashSet<ShopRandomizer.Id>();
            tempSublevels = new HashSet<SubLevel.Id>();
            tempGurus = new HashSet<Guru.GuruId>();
            tempIds.Add(ShopRandomizer.Id.Book);

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EastTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ForepawItemShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ForepawKeyShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ForepawGuru, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ForepawHospital, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ForepawHouse, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ForepawMeatShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.TowerOfFortress, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.JokerHouse, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);

            if (tempSublevels.Contains(SubLevel.Id.TowerOfFortress))
            {
                if (tempIds.Contains(ShopRandomizer.Id.Elixir))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            tempIds = new HashSet<ShopRandomizer.Id>();
            tempSublevels = new HashSet<SubLevel.Id>();
            tempGurus = new HashSet<Guru.GuruId>();
            tempIds.Add(ShopRandomizer.Id.Book);

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyMist], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.LateMist], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.MasconItemShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.MasconKeyShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.MasconBar, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.MasconHospital, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.MasconMeatShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.MistSecretShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.MistLargeHouse, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.MistSmallHouse, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.MistGuru, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.BirdHospital, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.VictimBar, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.VictimGuru, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.VictimHospital, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.VictimHouse, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.VictimItemShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.VictimKeyShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.VictimMeatShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.FireMage, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.AceKeyHouse, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.MasconTower, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.TowerOfSuffer, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.VictimTower, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.TowerOfMist, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);

            if (tempSublevels.Contains(SubLevel.Id.TowerOfFortress))
            {
                if (tempIds.Contains(ShopRandomizer.Id.Elixir))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            tempIds = new HashSet<ShopRandomizer.Id>();
            tempSublevels = new HashSet<SubLevel.Id>();
            tempGurus = new HashSet<Guru.GuruId>();
            tempIds.Add(ShopRandomizer.Id.Book);

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyBranch], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.MiddleBranch], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.DropDownWing], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EastBranch], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ConflateBar, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ConflateHospital, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ConflateHouse, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ConflateItemShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ConflateMeatShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.ConflateGuru, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DaybreakBar, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DaybreakGuru, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DaybreakHouse, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DaybreakItemShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DaybreakKeyShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DaybreakMeatShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.BattleHelmetWing, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);

            if (tempSublevels.Contains(SubLevel.Id.TowerOfFortress))
            {
                if (tempIds.Contains(ShopRandomizer.Id.Elixir))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            tempIds = new HashSet<ShopRandomizer.Id>();
            tempSublevels = new HashSet<SubLevel.Id>();
            tempGurus = new HashSet<Guru.GuruId>();
            tempIds.Add(ShopRandomizer.Id.Book);

            CheckDoor(DoorId.DartmoorBar, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DartmoorGuru, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DartmoorHospital, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DartmoorItemShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DartmoorKeyShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DartmoorMeatShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DartmoorHouse1, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DartmoorHouse2, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DartmoorHouse3, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.DartmoorHouse4, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.LeftOfDartmoor, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.FarLeftDartmoor, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.CastleFraternal, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            CheckDoor(DoorId.EvilOnesLair, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);

            if (tempSublevels.Contains(SubLevel.Id.TowerOfFortress))
            {
                if (tempIds.Contains(ShopRandomizer.Id.Elixir))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        private bool HasLairEntranceAccess(DoorRandomizer doorRandomizer,
                                           HashSet<ShopRandomizer.Id> ids)
        {
            bool hasMattock = ids.Contains(ShopRandomizer.Id.Mattock);
            bool hasKey = ids.Contains(doorRandomizer.GetLevelKey(doorRandomizer.Doors[DoorId.DartmoorLateDoor]));
            if (ItemOptions.MattockUsage == ItemOptions.MattockUsages.Unchanged)
            {
                if (!hasKey)
                {
                    return false;
                }
            }
            else
            {
                if ((!hasKey) && (!hasMattock))
                {
                    return false;
                }
            }

            return true;
        }

        private bool FinalRequirementsMet(HashSet<ShopRandomizer.Id> ids, HashSet<SubLevel.Id> traversedSublevels)
        {
            if (!ids.Contains(ShopRandomizer.Id.DemonRing))
            {
                return false;
            }

            if (GeneralOptions.DragonSlayerRequired)
            {
                if (!(ids.Contains(ShopRandomizer.Id.DragonSlayer) &&
                    ids.Contains(ShopRandomizer.Id.BattleSuit) &&
                    ids.Contains(ShopRandomizer.Id.BattleHelmet)))
                {
                    return false;
                }
            }

            if (GeneralOptions.PendantRodRubyRequired)
            {
                if (!(ids.Contains(ShopRandomizer.Id.Pendant) &&
                    ids.Contains(ShopRandomizer.Id.Rod) &&
                    ids.Contains(ShopRandomizer.Id.RubyRing)))
                {
                    return false;
                }
            }

            if (!GeneralOptions.MoveFinalRequirements && GeneralOptions.ShuffleWorlds)
            {
                if (!traversedSublevels.Contains(SubLevel.Id.Dartmoor))
                {
                    return false;
                }
            }

            if (GeneralOptions.MoveSpringQuestRequirement)
            {
                if (!ids.Contains(ShopRandomizer.Id.Elixir) ||
                    !ids.Contains(ShopRandomizer.Id.WingBoots))
                {
                    return false;
                }

                if (!traversedSublevels.Contains(SubLevel.Id.TowerOfFortress) ||
                    !traversedSublevels.Contains(SubLevel.Id.JokerHouse) ||
                    !traversedSublevels.Contains(SubLevel.Id.EastTrunk))
                {
                    return false;
                }
            }

            return true;
        }

        private void CheckDoor(DoorId doorId, GiftRandomizer giftRandomizer, DoorRandomizer doorRandomizer,
                               HashSet<ShopRandomizer.Id> ids, HashSet<SubLevel.Id> traversedSublevels,
                               HashSet<Guru.GuruId> gurus)
        {
            if (!doorRandomizer.Doors.ContainsKey(doorId))
            {
                return;
            }

            var building = doorRandomizer.Doors[doorId];
            if (!ids.Contains(doorRandomizer.GetLevelKey(building)))
            {
                return;
            }

            if (building.BuildingShop != null)
            {
                AddShopItems(building.BuildingShop, ids);
            }

            if (building.Guru != null)
            {
                if (building.Guru.Id != Guru.GuruId.Eolis)
                {
                    gurus.Add(building.Guru.Id);
                }
            }

            if (building.Gift != null)
            {
                CheckGiftLocation(building.Gift, ids, traversedSublevels, gurus);
            }

            if (building.Sublevel != null)
            {
                TraverseSubLevel(building.Sublevel, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            }
        }

        private void TraverseSubLevel(SubLevel subLevel, GiftRandomizer giftRandomizer, DoorRandomizer doorRandomizer,
                                      HashSet<ShopRandomizer.Id> ids, HashSet<SubLevel.Id> traversedSublevels,
                                      HashSet<Guru.GuruId> gurus)
        {
            foreach (var screen in subLevel.Screens)
            {
                foreach (var sprite in screen.Sprites)
                {
                    if (shopIdDictionary.Keys.Contains(sprite.Id))
                    {
                        if (sprite.Id == Sprite.SpriteId.Wingboots)
                        {
                            continue;
                        }

                        if (sprite.RequiresMattock)
                        {
                            if (!ids.Contains(ShopRandomizer.Id.Mattock))
                            {
                                continue;
                            }
                        }

                        if (sprite.RequiresWingBoots)
                        {
                            if (!ids.Contains(ShopRandomizer.Id.WingBoots))
                            {
                                continue;
                            }
                        }

                        ids.Add(shopIdDictionary[sprite.Id]);
                    }
                }
            }

            if (subLevel.SubLevelId == SubLevel.Id.TowerOfFortress)
            {
                CheckDoor(DoorId.FortressGuru, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            }
            else if (subLevel.SubLevelId == SubLevel.Id.JokerHouse)
            {
                ids.Add(giftRandomizer.ItemDict[GiftItem.Id.JokerHouse].Item);
            }
            else if (subLevel.SubLevelId == SubLevel.Id.CastleFraternal)
            {
                CheckDoor(DoorId.FraternalHouse, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
                CheckDoor(DoorId.FraternalGuru, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
                CheckDoor(DoorId.KingGrieve, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            }

            traversedSublevels.Add(subLevel.SubLevelId);
        }

        private void CheckGiftLocation(GiftItem gift, HashSet<ShopRandomizer.Id> ids, HashSet<SubLevel.Id> traversedSublevels,
                                       HashSet<Guru.GuruId> gurus)
        {
            if (gift.LocationId == GiftItem.Id.FortressGuru)
            {
                if (!(ids.Contains(ShopRandomizer.Id.Mattock) &&
                    ids.Contains(ShopRandomizer.Id.WingBoots) &&
                    traversedSublevels.Contains(SubLevel.Id.EastTrunk)))
                {
                    return;
                }
            }
            else if (gift.LocationId == GiftItem.Id.AceKeyHouse)
            {
                if (!ids.Contains(ShopRandomizer.Id.BlackOnyx))
                {
                    return;
                }
            }
            else if (gift.LocationId == GiftItem.Id.ConflateGuru)
            {
                if (!ids.Contains(ShopRandomizer.Id.BattleSuit))
                {
                    return;
                }
            }
            else if (gift.LocationId == GiftItem.Id.FraternalGuru)
            {
                if (!ids.Contains(ShopRandomizer.Id.DragonSlayer))
                {
                    return;
                }
            }
            else if (gift.LocationId == GiftItem.Id.VictimBar)
            {
                if (gurus.Count == 0)
                {
                    return;
                }
            }

            ids.Add(gift.Item);
        }

        private void TraverseLevel(Level level, HashSet<ShopRandomizer.Id> ids)
        {
            foreach (var screen in level.Screens)
            {
                foreach (var sprite in screen.Sprites)
                {
                    if (shopIdDictionary.Keys.Contains(sprite.Id))
                    {
                        ids.Add(shopIdDictionary[sprite.Id]);
                    }
                }
            }
        }

        private void AddShopItems(Shop shop, HashSet<ShopRandomizer.Id> ids)
        {
            foreach (var item in shop.Items)
            {
                ids.Add(item.Id);
            }
        }

        private List<Sprite> CollectItemsToShuffle(List<Level> levels)
        {
            var items = new List<Sprite>();
            foreach (var level in levels)
            {
                if (offsetsToShuffle.Contains(level.Start))
                {
                    var subItems = CollectItems(level);
                    items.AddRange(subItems);
                }
            }

            return items;
        }

        private List<Sprite> CollectItems(Level level)
        {
            var items = new List<Sprite>();
            foreach (var screen in level.Screens)
            {
                foreach (var sprite in screen.Sprites)
                {
                    if (Sprite.vanillaItemIds.Contains(sprite.Id))
                    {
                        if (level.Start == Level.StartOffset.DartmoorArea &&
                            sprite.Id == Sprite.SpriteId.RedPotion)
                        {
                            if (ItemOptions.MattockUsage == ItemOptions.MattockUsages.Unchanged ||
                                ItemOptions.MattockUsage == ItemOptions.MattockUsages.AnywhereExceptMistEntranceNoFraternalItemShuffle)
                            {
                                //Skip unreachable item
                                continue;
                            }
                            else
                            {
                                sprite.RequiresMattock = true;
                            }
                        }

                        items.Add(sprite);
                    }
                }
            }
            return items;
        }

        private bool ContainsDoubleKeyItems(Screen screen)
        {
            bool encounteredKeyitem = false;
            foreach (var sprite in screen.Sprites)
            {
                if (encounteredKeyitem && Sprite.KeyItems.Contains(sprite.Id))
                {
                    return true;
                }
                if (Sprite.KeyItems.Contains(sprite.Id))
                {
                    encounteredKeyitem = true;
                }
            }
            return false;
        }

        private ShopRandomizer.Id GetMiscItem()
        {
            if (ItemOptions.ReplacePoison && Rand.Next(0, 8) == 0)
            {
                return ShopRandomizer.Id.BlackPotion;
            }

            return ShopRandomizer.miscList[Rand.Next(ShopRandomizer.miscList.Count)];
        }

        private bool IncludeEolisGuru()
        {
            return GeneralOptions.FastStart || ItemOptions.RandomizeKeys != ItemOptions.KeyRandomization.Unchanged;
        }
    }
}
