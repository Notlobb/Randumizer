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
            var itemIds = new HashSet<Sprite.SpriteId>();
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
                RandomizeIntro();
                spells.AddRange(ShopRandomizer.spellIds);
                int spellIndex = Rand.Next(spells.Count);
                startingSpell = spells[spellIndex];
                spells.RemoveAt(spellIndex);

                itemIds = new HashSet<Sprite.SpriteId>();
                items = CollectItemsToShuffle(levels, itemIds);
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

                if (ItemOptions.RandomizeKeys == ItemOptions.KeyRandomization.Shuffled &&
                    !ItemOptions.IncludeSomeEolisDoors)
                {
                    shopIds.Add(GetMiscItem());
                }
                else
                {
                    shopIds.Add(ShopRandomizer.Id.ElfRing);
                }

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
            if ((!IncludeEolisGuru()) || Util.AllWorldScreensRandomized())
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
                TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EvilOnesLair], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
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

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyTrunk], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
            var middleTrunk = SubLevel.SubLevelDict[SubLevel.Id.MiddleTrunk];
            TraverseSubLevel(middleTrunk, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
            }

            if (CanWin(ids, traversedSublevels))
            {
                return true;
            }

            if (middleTrunk.RequiresMattock && !ids.Contains(ShopRandomizer.Id.Mattock))
            {
                return false;
            }

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.LateTrunk], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

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

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EastTrunk], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

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
                if (!(traversedSublevels.Contains(SubLevel.SkySpringSublevel) &&
                      traversedSublevels.Contains(SubLevel.FortressSpringSublevel) &&
                      traversedSublevels.Contains(SubLevel.JokerSpringSublevel)))
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
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.MiddleMist], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

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

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.LateMist], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

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

            if (itemCount < ids.Count)
            {
                CheckValid(shopRandomizer, giftRandomizer, doorRandomizer, levels, ids, traversedSublevels, gurus);
            }

            if (CanWin(ids, traversedSublevels))
            {
                return true;
            }

            if (!Util.AllWorldScreensRandomized())
            {
                if (!ids.Contains(doorRandomizer.GetLevelKey(doorRandomizer.Doors[DoorId.EastBranch])))
                {
                    return false;
                }
            }

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.MiddleBranch], giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);

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
                result &= CheckSingleGift(GiftItem.Id.JokerSpring, giftRandomizer);
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

        private bool GuaranteedElixir(ShopRandomizer shopRandomizer, GiftRandomizer giftRandomizer, DoorRandomizer doorRandomizer, List<Level> levels)
        {
            if (Util.AllWorldScreensRandomized())
            {
                if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Unchanged)
                    {
                    //Not supported for now
                    return true;
                }
            }

            var tempIds = new HashSet<ShopRandomizer.Id>();
            var tempSublevels = new HashSet<SubLevel.Id>();
            var tempGurus = new HashSet<Guru.GuruId>();
            tempIds.Add(ShopRandomizer.Id.Book);

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.MiddleTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.MiddleTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);

            if (tempSublevels.Contains(SubLevel.FortressSpringSublevel))
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

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.LateTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EastTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.LateTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EastTrunk], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);

            if (tempSublevels.Contains(SubLevel.FortressSpringSublevel))
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
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.MiddleMist], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.LateMist], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyMist], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.MiddleMist], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.LateMist], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);

            if (tempSublevels.Contains(SubLevel.FortressSpringSublevel))
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
            tempIds.Add(doorRandomizer.GetLevelKey(doorRandomizer.Doors[DoorId.EastBranch]));

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyBranch], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.MiddleBranch], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.EarlyBranch], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.MiddleBranch], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);

            if (tempSublevels.Contains(SubLevel.FortressSpringSublevel))
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

            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.Dartmoor], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);
            TraverseSubLevel(SubLevel.SubLevelDict[SubLevel.Id.Dartmoor], giftRandomizer, doorRandomizer, tempIds, tempSublevels, tempGurus);

            if (tempSublevels.Contains(SubLevel.FortressSpringSublevel))
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
            if (subLevel.SubLevelId == SubLevel.Id.EvilOnesLair)
            {
                if ((GeneralOptions.RandomizeScreens != GeneralOptions.ScreenRandomization.Unchanged) && (!ids.Contains(ShopRandomizer.Id.WingBoots)))
                {
                    return;
                }
            }

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

                foreach (var door in screen.Doors)
                {
                    CheckDoor(door, giftRandomizer, doorRandomizer, ids, traversedSublevels, gurus);
                }

                foreach (var gift in screen.Gifts)
                {
                    ids.Add(giftRandomizer.ItemDict[gift].Item);
                }
            }

            traversedSublevels.Add(subLevel.SubLevelId);
        }

        private void CheckGiftLocation(GiftItem gift, HashSet<ShopRandomizer.Id> ids, HashSet<SubLevel.Id> traversedSublevels,
                                       HashSet<Guru.GuruId> gurus)
        {
            if (gift.LocationId == GiftItem.Id.FortressGuru)
            {
                if (!(ids.Contains(ShopRandomizer.Id.WingBoots) &&
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
                            if (sprite.Id == Sprite.SpriteId.Glove2OrKeyJoker ||
                                sprite.Id == Sprite.SpriteId.MattockOrRingRuby)
                            {
                                continue;
                            }

                            if (Sprite.vanillaItemIds.Contains(sprite.Id) && !itemIds.Contains(sprite.Id))
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

        private void RandomizeIntro()
        {
            foreach (var item in Level.LevelDict[WorldNumber.Eolis].Screens[0].Sprites)
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

                item.Id = possibleItems.ElementAt(Rand.Next(possibleItems.Count));
                if (item.Id == Sprite.SpriteId.WingbootsBossLocked ||
                    item.Id == Sprite.SpriteId.HourGlass)
                {
                    //These items don't fall down
                    item.SetY(10);
                }
            }
        }
    }
}
