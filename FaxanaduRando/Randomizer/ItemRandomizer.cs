using System;
using System.Collections.Generic;
using System.Linq;

namespace FaxanaduRando.Randomizer
{
    using CheckFunction = Func<ShopRandomizer, GiftRandomizer, DoorRandomizer, List<Level>, HashSet<ShopRandomizer.Id>, HashSet<SubLevel.Id>, HashSet<Guru.GuruId>, HashSet<int>, HashSet<DoorId>, bool>;

    public class ItemRandomizer
    {
        private Dictionary<Sprite.SpriteId, ShopRandomizer.Id> shopIdDictionary = new Dictionary<Sprite.SpriteId, ShopRandomizer.Id>
        {
            { Sprite.SpriteId.Battlehelmet, ShopRandomizer.Id.Battlehelmet},
            { Sprite.SpriteId.Battlesuit, ShopRandomizer.Id.Battlesuit},
            { Sprite.SpriteId.Dragonslayer, ShopRandomizer.Id.Dragonslayer},
            { Sprite.SpriteId.RockSnakeOrJokerKey, ShopRandomizer.Id.JokerKey},
            { Sprite.SpriteId.KeyAce, ShopRandomizer.Id.AceKey},
            { Sprite.SpriteId.MattockOrRingRuby, ShopRandomizer.Id.RubyRing},
            { Sprite.SpriteId.RingDworf, ShopRandomizer.Id.DworfRing},
            { Sprite.SpriteId.RingDemon, ShopRandomizer.Id.DemonRing},
            { Sprite.SpriteId.Wingboots, ShopRandomizer.Id.Wingboots},
            { Sprite.SpriteId.WingbootsBossLocked, ShopRandomizer.Id.Wingboots},
            { Sprite.SpriteId.RedPotion, ShopRandomizer.Id.RedPotion},
            { Sprite.SpriteId.RedPotion2, ShopRandomizer.Id.RedPotion},
            { Sprite.SpriteId.Hourglass, ShopRandomizer.Id.Hourglass},
            { Sprite.SpriteId.MattockBossLocked, ShopRandomizer.Id.Mattock},
            { Sprite.SpriteId.Rod, ShopRandomizer.Id.Rod},
            { Sprite.SpriteId.Elixir, ShopRandomizer.Id.Elixir},
            { Sprite.SpriteId.BlackOnyx, ShopRandomizer.Id.BlackOnyx},
            { Sprite.SpriteId.Pendant, ShopRandomizer.Id.Pendant},
        };

        private static readonly HashSet<WorldNumber> offsetsToShuffle= new HashSet<WorldNumber>
        {
            WorldNumber.Trunk,
            WorldNumber.Mist,
            WorldNumber.Branch,
            WorldNumber.Dartmoor,
            WorldNumber.EvilOnesLair,
        };

        public Random Rand {get; set;}
        public ItemRandomizer(Random random)
        {
            Rand = random;
            if (ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Mixed)
            {
                shopIdDictionary.Remove(Sprite.SpriteId.RockSnakeOrJokerKey);
                shopIdDictionary.Remove(Sprite.SpriteId.MattockOrRingRuby);
                shopIdDictionary.Remove(Sprite.SpriteId.KeyAce);
                shopIdDictionary.Remove(Sprite.SpriteId.RingDemon);
                shopIdDictionary.Remove(Sprite.SpriteId.RingDworf);
            }
        }

        public static Sprite.SpriteId GetRandomItem(Random random)
        {
            var possibleItems = new List<Sprite.SpriteId>
            {
                Sprite.SpriteId.Hourglass,
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

            return possibleItems.ElementAt(random.Next(possibleItems.Count));
        }

        public bool ShuffleItems(List<Level> levels,
                                 ShopRandomizer shopRandomizer,
                                 GiftRandomizer giftRandomizer,
                                 DoorRandomizer doorRandomizer,
                                 SegmentRandomizer segmentRandomizer,
                                 byte[] content,
                                 out uint attempts)
        {
            var items = new List<Sprite>();
            var itemIds = new HashSet<Sprite.SpriteId>();
            var extraItems = new List<Sprite.SpriteId>();
            var ids = new List<Sprite.SpriteId>();
            var startingWeapon = ShopRandomizer.Id.Dagger;
            if (ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.Random)
            {
                startingWeapon = Rand.Next(3) == 0 ? ShopRandomizer.Id.Dagger : ShopRandomizer.Id.Longsword;
            }
            else if (ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.Dagger)
            {
                startingWeapon = ShopRandomizer.Id.Dagger;
            }
            else if (ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.LongSword)
            {
                startingWeapon = ShopRandomizer.Id.Longsword;
            }
            var startingSpell = ShopRandomizer.Id.Deluge;
            var spells = new List<ShopRandomizer.Id>();

            if (ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Unchanged)
            {
                RandomizeIntro();
                spells.AddRange(ShopRandomizer.spellIds);
                int spellIndex = Rand.Next(spells.Count);
                startingSpell = spells[spellIndex];
                spells.RemoveAt(spellIndex);

                itemIds = new HashSet<Sprite.SpriteId>();
                items = CollectItemsToShuffle(levels, itemIds);
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Id == Sprite.SpriteId.Glove2)
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
                    if (ItemOptions.BigKeyLimit != ItemOptions.KeyLimit.Zero)
                    {
                        extraItems.Add(Sprite.SpriteId.RockSnakeOrJokerKey);
                        extraItems.Add(Sprite.SpriteId.KeyAce);
                    }
                    else
                    {
                        extraItems.Add(GetRandomItem(Rand));
                        extraItems.Add(GetRandomItem(Rand));
                    }

                    extraItems.Add(Sprite.SpriteId.MattockOrRingRuby);
                    extraItems.Add(Sprite.SpriteId.RingDworf);
                    extraItems.Add(Sprite.SpriteId.RingDemon);
                    extraItems.Add(Sprite.SpriteId.RedPotion);
                    extraItems.Add(Sprite.SpriteId.Wingboots);

                    extraItems.Add(Sprite.SpriteId.Wingboots);
                    extraItems.Add(Sprite.SpriteId.Wingboots);
                    extraItems.Add(Sprite.SpriteId.Hourglass);
                    extraItems.Add(Sprite.SpriteId.Elixir);

                    if (IncludeEolisGuru(giftRandomizer))
                    {
                        extraItems.Add(Sprite.SpriteId.Elixir);
                    }
                }

                ids.AddRange(extraItems);
            }

            Util.ShuffleList(ids, 0, ids.Count - 1, Rand);
            var toShopIds = ids.GetRange(ids.Count - extraItems.Count, extraItems.Count);

            shopRandomizer.SetStaticItems(startingWeapon, startingSpell);

            var shopIds = shopRandomizer.GetBaseIds(Rand);
            if (ItemOptions.RandomizeKeys != ItemOptions.KeyRandomization.Unchanged)
            {
                if (ItemOptions.SmallKeyLimit != ItemOptions.KeyLimit.Zero)
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
                }
                else
                {
                    shopIds.Add(ShopRandomizer.GetMiscItem(Rand));
                    shopIds.Add(ShopRandomizer.GetMiscItem(Rand));
                    shopIds.Add(ShopRandomizer.GetMiscItem(Rand));
                    shopIds.Add(ShopRandomizer.GetMiscItem(Rand));
                    shopIds.Add(ShopRandomizer.GetMiscItem(Rand));
                    shopIds.Add(ShopRandomizer.GetMiscItem(Rand));
                    shopIds.Add(ShopRandomizer.GetMiscItem(Rand));
                    shopIds.Add(ShopRandomizer.GetMiscItem(Rand));
                    shopIds.Add(ShopRandomizer.GetMiscItem(Rand));
                }

                if (ItemOptions.RandomizeKeys == ItemOptions.KeyRandomization.Shuffled &&
                    !ItemOptions.IncludeSomeEolisDoors)
                {
                    shopIds.Add(ShopRandomizer.GetMiscItem(Rand));
                }
                else
                {
                    shopIds.Add(ShopRandomizer.Id.ElfRing);
                }

                shopIds.Add(ShopRandomizer.GetMiscItem(Rand));
            }

            if (ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.NoGuaranteed ||
                ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.GuaranteeOnlyWithNoSpells)
            {
                shopIds.Add(ShopRandomizer.Id.Dagger);
                shopIds.Add(ShopRandomizer.Id.Longsword);
            }
            else if (startingWeapon == ShopRandomizer.Id.Dagger)
            {
                shopIds.Add(ShopRandomizer.Id.Longsword);
            }
            else
            {
                var miscId = ShopRandomizer.GetMiscItem(Rand);
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
                    var miscId = ShopRandomizer.GetMiscItem(Rand);
                    shopIds.Add(miscId);
                }
            }

            int giftCount = giftRandomizer.GiftItems.Count;
            if ((!IncludeEolisGuru(giftRandomizer)))
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
                    shopIds.Add(ShopRandomizer.GetMiscItem(Rand));
                }

                shopIds.Add(ShopRandomizer.Id.Wingboots);
                shopIds.Add(ShopRandomizer.Id.Elixir);
                shopIds.Add(ShopRandomizer.Id.Hourglass);

                shopIds.Add(ShopRandomizer.GetMiscItem(Rand));
                shopIds.Add(ShopRandomizer.GetMiscItem(Rand));

                if (IncludeEolisGuru(giftRandomizer))
                {
                    shopIds.Add(ShopRandomizer.GetMiscItem(Rand));
                }

                var giftItems = new List<ShopRandomizer.Id>();
                if (ItemOptions.BigKeyLimit != ItemOptions.KeyLimit.Zero)
                {
                    giftItems.Add(ShopRandomizer.Id.JokerKey);
                    giftItems.Add(ShopRandomizer.Id.AceKey);
                }
                else
                {
                    giftItems.Add(ShopRandomizer.GetMiscItem(Rand));
                    giftItems.Add(ShopRandomizer.GetMiscItem(Rand));
                }

                giftItems.Add(ShopRandomizer.Id.RubyRing);
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
                    SetIds(ids, 0, items.Count, items);

                    if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.NoMixed ||
                        ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Mixed)
                    {
                        Util.ShuffleList(shopIds, 0, shopIds.Count - giftCount - 1, Rand);
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

                if (GeneralOptions.DoorTypeSetting != GeneralOptions.DoorTypeShuffle.Unchanged)
                {
                    doorRandomizer.ShuffleDoorTypes(Rand);
                }

                if (GeneralOptions.ShuffleTowers)
                {
                    doorRandomizer.ShuffleTowers(Rand);
                }

                if (GeneralOptions.ShuffleSegments != GeneralOptions.SegmentShuffle.Unchanged)
                {
                    segmentRandomizer.ShuffleSegments(Rand);
                }

                if (ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.GuaranteeOnlyWithNoSpells)
                {
                    if (!EolisHasWeaponOrSpell(shopRandomizer, giftRandomizer, doorRandomizer))
                    {
                        continue;
                    }
                }

                if (!CheckSingleGifts(giftRandomizer))
                {
                    continue;
                }

                valid = CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, new HashSet<ShopRandomizer.Id>(),
                                   new HashSet<SubLevel.Id>(), new HashSet<Guru.GuruId>(), new HashSet<DoorId>());

                if (valid)
                {
                    if (ItemOptions.GuaranteeElixirNearFortress)
                    {
                        valid = GuaranteedElixir(shopRandomizer, giftRandomizer, doorRandomizer, levels);
                    }
                }
            }

            if (!valid)
            {
                return false;
            }

            segmentRandomizer.UpdateDoors(doorRandomizer, shopRandomizer);
            if (ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Unchanged)
            {
                shopRandomizer.RandomizePrices(Rand, doorRandomizer);
            }

            shopRandomizer.AddToContent(content);
            giftRandomizer.AddToContent(content);
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
                                HashSet<Guru.GuruId> gurus,
                                HashSet<DoorId> doors)
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
            var transitionOffsets = new HashSet<int>();

            CheckDoor(DoorId.EolisItemShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors);
            CheckDoor(DoorId.EolisKeyShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors);
            CheckDoor(DoorId.EolisGuru, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors);

            CheckDoor(DoorId.EolisHouse, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors);
            CheckDoor(DoorId.EolisMeatShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors);
            CheckDoor(DoorId.MartialArtsShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors);
            CheckDoor(DoorId.EolisMagicShop, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors);

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus, doors);
            }

            if (!ids.Contains(doorRandomizer.GetExitKey(DoorRandomizer.ExitDoor.EolisExit)))
            {
                return false;
            }

            bool result = GetCheck(worlds[1].number)(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus, transitionOffsets, doors);
            if (!result)
            {
                return false;
            }

            result = GetCheck(worlds[2].number)(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus, transitionOffsets, doors);
            if (!result)
            {
                return false;
            }

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus, doors);
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
                result = GetCheck(worlds[3].number)(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus, transitionOffsets, doors);
                if (!result)
                {
                    return false;
                }

                result = GetCheck(worlds[4].number)(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus, transitionOffsets, doors);
                if (!result)
                {
                    return false;
                }
            }

            if (!(GeneralOptions.IncludeEvilOnesFortress && GeneralOptions.ShuffleTowers) &&
                GeneralOptions.MoveFinalRequirements)
            {
                TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EvilOnesLair], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors);
            }

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus, doors);
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
                                HashSet<Guru.GuruId> gurus,
                                HashSet<int> transitionOffsets,
                                HashSet<DoorId> doors)
        {
            int itemCount = ids.Count;
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyTrunk], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors);
            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus, doors);
            }

            if (CanWin(ids, traversedSublevels))
            {
                return true;
            }

            if (ItemOptions.MattockUsage != ItemOptions.MattockUsages.AnywhereUpdateLogic &&
                !GeneralOptions.MoveSpringQuestRequirement)
            {
                if (!(traversedSublevels.Contains(SubLevel.SkySpringSublevel) &&
                      traversedSublevels.Contains(SubLevel.FortressSpringSublevel) &&
                      traversedSublevels.Contains(SubLevel.JokerSpringSublevel)))
                {
                    return false;
                }

                if (!ids.Contains(ShopRandomizer.Id.RubyRing) ||
                    !ids.Contains(ShopRandomizer.Id.Elixir) ||
                    !ids.Contains(ShopRandomizer.Id.Wingboots))
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
                if (!ids.Contains(ShopRandomizer.Id.Wingboots))
                {
                    return false;
                }
            }

            if (!doors.Contains(DoorId.TrunkExit))
            {
                return false;
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
                               HashSet<Guru.GuruId> gurus,
                               HashSet<int> transitionOffsets,
                               HashSet<DoorId> doors)
        {
            int itemCount = ids.Count;
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyMist], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors);
            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus, doors);
            }

            if (CanWin(ids, traversedSublevels))
            {
                return true;
            }

            if (!ids.Contains(ShopRandomizer.Id.Wingboots))
            {
                return false;
            }

            if (!doors.Contains(DoorId.MistExit))
            {
                return false;
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
                                 HashSet<Guru.GuruId> gurus,
                                 HashSet<int> transitionOffsets,
                                 HashSet<DoorId> doors)
        {
            int itemCount = ids.Count;
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyBranch], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors);
            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus, doors);
            }

            if (CanWin(ids, traversedSublevels))
            {
                return true;
            }

            if (!Util.AllCoreWorldScreensRandomized())
            {
                if (!ids.Contains(doorRandomizer.GetLevelKey(doorRandomizer.Doors[DoorId.EastBranch])))
                {
                    return false;
                }
            }

            if (!doors.Contains(DoorId.BranchExit))
            {
                return false;
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
                                   HashSet<Guru.GuruId> gurus,
                                   HashSet<int> transitionOffsets,
                                   HashSet<DoorId> doors)
        {
            int itemCount = ids.Count;
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.Dartmoor], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors);
            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus, doors);
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

            if (!doors.Contains(DoorId.DartmoorExit))
            {
                return false;
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
            var tempTransitions = new HashSet<int>();
            var tempDoors = new HashSet<DoorId>();

            CheckDoor(DoorId.EolisItemShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, tempTransitions, tempDoors);
            CheckDoor(DoorId.EolisKeyShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, tempTransitions, tempDoors);
            CheckDoor(DoorId.EolisGuru, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, tempTransitions, tempDoors);
            CheckDoor(DoorId.EolisMeatShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, tempTransitions, tempDoors);
            CheckDoor(DoorId.EolisHouse, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, tempTransitions, tempDoors);
            CheckDoor(DoorId.MartialArtsShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, tempTransitions, tempDoors);
            CheckDoor(DoorId.EolisMagicShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, tempTransitions, tempDoors);
            CheckDoor(DoorId.MartialArtsShop, giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, tempTransitions, tempDoors);

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
            var conflateGift = giftRandomizer.ItemDict[GiftItem.Id.ConflateGuru].Item;
            if (ItemOptions.RandomizeKeys == ItemOptions.KeyRandomization.Randomized)
            {
                if (conflateGift == ShopRandomizer.Id.JokerKey ||
                    conflateGift == ShopRandomizer.Id.AceKey)
                {
                    return false;
                }
            }

            if (conflateGift == ShopRandomizer.Id.KingKey ||
                conflateGift == ShopRandomizer.Id.QueenKey ||
                conflateGift == ShopRandomizer.Id.JackKey)
            {
                return false;
            }

            return true;
        }

        private bool GuaranteedElixir(ShopRandomizer shopRandomizer, GiftRandomizer giftRandomizer, DoorRandomizer doorRandomizer, List<Level> levels)
        {
            if (Util.AllCoreWorldScreensRandomized())
            {
                if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Unchanged)
                    {
                    //Not supported for now
                    return true;
                }
            }

            var items = new List<Sprite>();
            var itemIds = new HashSet<Sprite.SpriteId>();

            bool SetElixir()
            {
                foreach (var item in items)
                {
                    if (!Sprite.KeyItems.Contains(item.Id))
                    {
                        item.Id = Sprite.SpriteId.Elixir;
                        return true;
                    }
                }

                return false;
            }

            var tempIds = new HashSet<ShopRandomizer.Id>();
            var tempSublevels = new HashSet<SubLevel.Id>();
            var tempGurus = new HashSet<Guru.GuruId>();
            var tempDoors = new HashSet<DoorId>();
            tempIds.Add(ShopRandomizer.Id.Book);

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, new HashSet<int>(), tempDoors, true);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, new HashSet<int>(), tempDoors, true);
            if (tempSublevels.Contains(SubLevel.FortressSpringSublevel))
            {
                if (tempIds.Contains(ShopRandomizer.Id.Elixir))
                {
                    return true;
                }

                CollectSublevelItems(SubLevel.SubLevelDict[SubLevel.Id.EarlyTrunk], items, itemIds, doorRandomizer);
                CollectSublevelItems(SubLevel.SubLevelDict[SubLevel.FortressSpringSublevel], items, itemIds, doorRandomizer);
                Util.ShuffleList(items, 0, items.Count - 1, Rand);
                return SetElixir();
            }

            tempIds = new HashSet<ShopRandomizer.Id>();
            tempSublevels = new HashSet<SubLevel.Id>();
            tempGurus = new HashSet<Guru.GuruId>();
            tempIds.Add(ShopRandomizer.Id.Book);

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EastTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, new HashSet<int>(), tempDoors, true);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EastTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, new HashSet<int>(), tempDoors, true);

            if (tempSublevels.Contains(SubLevel.FortressSpringSublevel))
            {
                if (tempIds.Contains(ShopRandomizer.Id.Elixir))
                {
                    return true;
                }

                CollectSublevelItems(SubLevel.SubLevelDict[SubLevel.Id.EastTrunk], items, itemIds, doorRandomizer);
                CollectSublevelItems(SubLevel.SubLevelDict[SubLevel.FortressSpringSublevel], items, itemIds, doorRandomizer);
                Util.ShuffleList(items, 0, items.Count - 1, Rand);
                return SetElixir();
            }

            tempIds = new HashSet<ShopRandomizer.Id>();
            tempSublevels = new HashSet<SubLevel.Id>();
            tempGurus = new HashSet<Guru.GuruId>();
            tempIds.Add(ShopRandomizer.Id.Book);

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyMist], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, new HashSet<int>(), tempDoors, true);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.LateMist], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, new HashSet<int>(), tempDoors, true);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyMist], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, new HashSet<int>(), tempDoors, true);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.LateMist], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, new HashSet<int>(), tempDoors, true);

            if (tempSublevels.Contains(SubLevel.FortressSpringSublevel))
            {
                if (tempIds.Contains(ShopRandomizer.Id.Elixir))
                {
                    return true;
                }

                CollectSublevelItems(SubLevel.SubLevelDict[SubLevel.Id.EarlyMist], items, itemIds, doorRandomizer);
                CollectSublevelItems(SubLevel.SubLevelDict[SubLevel.Id.LateMist], items, itemIds, doorRandomizer);
                CollectSublevelItems(SubLevel.SubLevelDict[SubLevel.FortressSpringSublevel], items, itemIds, doorRandomizer);
                Util.ShuffleList(items, 0, items.Count - 1, Rand);
                return SetElixir();
            }

            tempIds = new HashSet<ShopRandomizer.Id>();
            tempSublevels = new HashSet<SubLevel.Id>();
            tempGurus = new HashSet<Guru.GuruId>();
            tempIds.Add(ShopRandomizer.Id.Book);
            tempIds.Add(doorRandomizer.GetLevelKey(doorRandomizer.Doors[DoorId.EastBranch]));

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyBranch], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, new HashSet<int>(), tempDoors, true);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.MiddleBranch], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, new HashSet<int>(), tempDoors, true);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyBranch], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, new HashSet<int>(), tempDoors, true);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.MiddleBranch], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, new HashSet<int>(), tempDoors, true);

            if (tempSublevels.Contains(SubLevel.FortressSpringSublevel))
            {
                if (tempIds.Contains(ShopRandomizer.Id.Elixir))
                {
                    return true;
                }

                CollectSublevelItems(SubLevel.SubLevelDict[SubLevel.Id.EarlyBranch], items, itemIds, doorRandomizer);
                CollectSublevelItems(SubLevel.SubLevelDict[SubLevel.Id.MiddleBranch], items, itemIds, doorRandomizer);
                CollectSublevelItems(SubLevel.SubLevelDict[SubLevel.FortressSpringSublevel], items, itemIds, doorRandomizer);
                Util.ShuffleList(items, 0, items.Count - 1, Rand);
                return SetElixir();
            }

            tempIds = new HashSet<ShopRandomizer.Id>();
            tempSublevels = new HashSet<SubLevel.Id>();
            tempGurus = new HashSet<Guru.GuruId>();
            tempIds.Add(ShopRandomizer.Id.Book);

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.Dartmoor], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, new HashSet<int>(), tempDoors, true);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.Dartmoor], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus, new HashSet<int>(), tempDoors, true);

            if (tempSublevels.Contains(SubLevel.FortressSpringSublevel))
            {
                if (tempIds.Contains(ShopRandomizer.Id.Elixir))
                {
                    return true;
                }

                CollectSublevelItems(SubLevel.SubLevelDict[SubLevel.Id.Dartmoor], items, itemIds, doorRandomizer);
                CollectSublevelItems(SubLevel.SubLevelDict[SubLevel.FortressSpringSublevel], items, itemIds, doorRandomizer);
                Util.ShuffleList(items, 0, items.Count - 1, Rand);
                return SetElixir();
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
                if (!(ids.Contains(ShopRandomizer.Id.Dragonslayer) &&
                    ids.Contains(ShopRandomizer.Id.Battlesuit) &&
                    ids.Contains(ShopRandomizer.Id.Battlehelmet)))
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
                    !ids.Contains(ShopRandomizer.Id.Wingboots))
                {
                    return false;
                }

                if (!traversedSublevels.Contains(SubLevel.SkySpringSublevel) ||
                    !traversedSublevels.Contains(SubLevel.FortressSpringSublevel) ||
                    !traversedSublevels.Contains(SubLevel.JokerSpringSublevel))
                {
                    return false;
                }
            }

            return true;
        }

        private void CheckDoor(DoorId doorId, GiftRandomizer giftRandomizer, DoorRandomizer doorRandomizer,
                               HashSet<ShopRandomizer.Id> ids, HashSet<SubLevel.Id> traversedSublevels,
                               HashSet<Guru.GuruId> gurus, HashSet<int> transitionOffsets, HashSet<DoorId> doors)
        {
            doors.Add(doorId);
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
                TraverseSubLevel(building.Sublevel, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors);
            }
        }

        private void TraverseSubLevel(SubLevel subLevel, GiftRandomizer giftRandomizer, DoorRandomizer doorRandomizer,
                                      HashSet<ShopRandomizer.Id> ids, HashSet<SubLevel.Id> traversedSublevels,
                                      HashSet<Guru.GuruId> gurus, HashSet<int> transitionOffsets, HashSet<DoorId> doors,
                                      bool elixirCheck = false)
        {
            traversedSublevels.Add(subLevel.SubLevelId);
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
                            if (!ids.Contains(ShopRandomizer.Id.Wingboots))
                            {
                                continue;
                            }
                        }

                        ids.Add(shopIdDictionary[sprite.Id]);
                    }
                }

                foreach (var door in screen.Doors)
                {
                    CheckDoor(door, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors);
                }

                foreach (var gift in screen.Gifts)
                {
                    ids.Add(giftRandomizer.ItemDict[gift].Item);
                }
            }

            if (subLevel.RequiresMattock && !ids.Contains(ShopRandomizer.Id.Mattock) && !elixirCheck)
            {
                return;
            }

            if (subLevel.RequiresWingboots && !ids.Contains(ShopRandomizer.Id.Wingboots) && !elixirCheck)
            {
                return;
            }

            foreach (var screen in subLevel.Screens)
            {
                if (screen.Transition != null)
                {
                    if (!transitionOffsets.Contains(screen.Transition.Offset))
                    {
                        transitionOffsets.Add(screen.Transition.Offset);
                        TraverseSubLevel(SubLevel.SubLevelDict[screen.Transition.ToScreenReference.ParentSublevel], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors, elixirCheck);
                    }
                }

                if (screen.ConnectedScreen != null && !elixirCheck)
                {
                    TraverseSubLevel(SubLevel.SubLevelDict[screen.ConnectedScreen.ParentSublevel], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus, transitionOffsets, doors, elixirCheck);
                }
            }
        }

        private void CheckGiftLocation(GiftItem gift, HashSet<ShopRandomizer.Id> ids, HashSet<SubLevel.Id> traversedSublevels,
                                       HashSet<Guru.GuruId> gurus)
        {
            if (gift.LocationId == GiftItem.Id.FortressGuru)
            {
                if (!(ids.Contains(ShopRandomizer.Id.Wingboots) &&
                    traversedSublevels.Contains(SubLevel.SkySpringSublevel)))
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
                if (!ids.Contains(ShopRandomizer.Id.Battlesuit))
                {
                    return;
                }
            }
            else if (gift.LocationId == GiftItem.Id.FraternalGuru)
            {
                if (!ids.Contains(ShopRandomizer.Id.Dragonslayer))
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

        private void AddShopItems(Shop shop, HashSet<ShopRandomizer.Id> ids)
        {
            foreach (var item in shop.Items)
            {
                ids.Add(item.Id);
            }
        }

        private List<Sprite> CollectItemsToShuffle(List<Level> levels, HashSet<Sprite.SpriteId> itemIds)
        {
            var items = new List<Sprite>();
            foreach (var level in levels)
            {
                if (offsetsToShuffle.Contains(level.Number))
                {
                    foreach (var sublevel in level.SubLevels)
                    {
                        CollectItems(sublevel.Screens, items, itemIds);
                    }

                }
                else if (level.Number == WorldNumber.Buildings)
                {
                    CollectItems(level.Screens, items, itemIds);
                }
            }

            foreach (var level in levels)
            {
                if (offsetsToShuffle.Contains(level.Number))
                {
                    foreach (var screen in level.Screens)
                    {
                        foreach (var sprite in screen.Sprites)
                        {
                            if (sprite.Id == Sprite.SpriteId.Glove2 ||
                                sprite.Id == Sprite.SpriteId.MattockOrRingRuby)
                            {
                                continue;
                            }

                            if (Sprite.vanillaItemIds.Contains(sprite.Id) && !itemIds.Contains(sprite.Id) &&
                                sprite.ShouldBeShuffled)
                            {
                                items.Add(sprite);
                                itemIds.Add(sprite.Id);
                            }
                        }
                    }
                }
            }

            return items;
        }

        private void CollectItems(List<Screen> screens, List<Sprite> items, HashSet<Sprite.SpriteId> itemIds)
        {
            foreach (var screen in screens)
            {
                foreach (var sprite in screen.Sprites)
                {
                    if (sprite.ShouldBeShuffled && Sprite.vanillaItemIds.Contains(sprite.Id))
                    {
                        items.Add(sprite);
                        itemIds.Add(sprite.Id);
                    }
                }
            }
        }

        private void CollectSublevelItems(SubLevel subLevel, List<Sprite> items, HashSet<Sprite.SpriteId> itemIds, DoorRandomizer doorRandomizer)
        {
            if (subLevel == null)
            {
                return;
            }

            CollectItems(subLevel.Screens, items, itemIds);
            foreach (var screen in subLevel.Screens)
            {
                foreach (var door in screen.Doors)
                {
                    if (doorRandomizer.Doors.ContainsKey(door))
                    {
                        CollectSublevelItems(doorRandomizer.Doors[door].Sublevel, items, itemIds, doorRandomizer);
                    }
                }
            }
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

        private bool IncludeEolisGuru(GiftRandomizer giftRandomizer)
        {
            return giftRandomizer.ItemDict[GiftItem.Id.EolisGuru].ShouldBeRandomized;
        }

        private void RandomizeIntro()
        {
            foreach (var item in Level.LevelDict[WorldNumber.Eolis].Screens[0].Sprites)
            {
                //Not a shuffle but there's only one intro item in Eolis with a weird location
                var possibleItems = new List<Sprite.SpriteId>
                {
                    Sprite.SpriteId.RedPotion,
                    Sprite.SpriteId.Ointment,
                    Sprite.SpriteId.Elixir,
                    Sprite.SpriteId.WingbootsBossLocked,
                };

                if (GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.Unchanged ||
                    EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Easy ||
                    EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Normal ||
                    EnemyOptions.EnemySet == EnemyOptions.EnemySetType.NonMixed ||
                    EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Scaling ||
                    EnemyOptions.EnemySet == EnemyOptions.EnemySetType.Unchanged)
                {
                    possibleItems.Add(Sprite.SpriteId.Glove);
                    possibleItems.Add(Sprite.SpriteId.Poison);
                    possibleItems.Add(Sprite.SpriteId.MattockBossLocked);
                    if (ItemOptions.AlwaysSpawnSmallItems)
                    {
                        possibleItems.Add(Sprite.SpriteId.Hourglass);
                    }
                }

                item.Id = possibleItems.ElementAt(Rand.Next(possibleItems.Count));
                if (item.Id == Sprite.SpriteId.WingbootsBossLocked ||
                    item.Id == Sprite.SpriteId.Hourglass ||
                    item.Id == Sprite.SpriteId.MattockBossLocked)
                {
                    //These items don't fall down
                    item.SetY(10);
                }
            }
        }
    }
}
