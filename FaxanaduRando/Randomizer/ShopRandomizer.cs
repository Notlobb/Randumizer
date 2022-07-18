using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class ShopRandomizer
    {
        public enum Id
        {
            Dagger = 0,
            LongSword = 1,
            GiantBlade = 2,
            DragonSlayer = 3,
            RingElf2 = 17,
            RingRuby2 = 18,
            RingDworf2 = 19,
            RingDemon2 = 20,
            LeatherArmour= 32,
            StuddedMail = 33,
            FullPlate = 34,
            BattleSuit = 35,
            SmallShield = 64,
            LargeShield = 65,
            MagicShield = 66,
            BattleHelmet = 67,
            Deluge = 96,
            Thunder = 97,
            Fire = 98,
            Death = 99,
            Tilte = 100,
            ElfRing = 128,
            RubyRing = 129,
            DworfRing = 130,
            DemonRing = 131,
            AceKey = 132,
            KingKey = 133,
            QueenKey = 134,
            JackKey = 135,
            JokerKey = 136,
            Mattock = 137,
            Rod = 138,
            Crystal = 139,
            Lamp = 140,
            Hourglass = 141,
            Book = 142,
            WingBoots = 143,
            RedPotion = 144,
            BlackPotion = 145,
            Elixir = 146,
            Pendant = 147,
            BlackOnyx = 148,
            FireCrystal = 149,
        }

        public static readonly List<Id> miscList = new List<Id>
        {
            Id.RedPotion,
            Id.Hourglass,
            Id.WingBoots,
            Id.Elixir,
        };

        public static HashSet<Id> weaponIds = new HashSet<Id>
        {
            Id.Dagger,
            Id.LongSword,
            Id.GiantBlade,
        };

        public static HashSet<Id> armorIds = new HashSet<Id>
        {
            Id.LeatherArmour,
            Id.StuddedMail,
            Id.FullPlate,
        };

        public static HashSet<Id> shieldIds = new HashSet<Id>
        {
            Id.SmallShield,
            Id.LargeShield,
            Id.MagicShield,
        };

        public static HashSet<Id> spellIds = new HashSet<Id>
        {
            Id.Deluge,
            Id.Thunder,
            Id.Fire,
            Id.Death,
            Id.Tilte,
        };

        public static HashSet<Id> keyIds = new HashSet<Id>
        {
            Id.AceKey,
            Id.KingKey,
            Id.QueenKey,
            Id.JackKey,
            Id.JokerKey,
        };

        public static HashSet<Id> miscIds = new HashSet<Id>
        {
            Id.Hourglass,
            Id.Elixir,
            Id.WingBoots,
            Id.Mattock,
        };

        public static HashSet<Id> potionIds = new HashSet<Id>
        {
            Id.RedPotion,
        };

        public static HashSet<Id> CheapIds = new HashSet<Id>
        {
            Id.RedPotion,
            Id.BlackPotion,
            Id.JackKey,
            Id.QueenKey,
            Id.KingKey,
        };

        public static HashSet<Id> KeyItems = new HashSet<Id>
        {
            Id.DragonSlayer,
            Id.BattleSuit,
            Id.BattleHelmet,
            Id.ElfRing,
            Id.RubyRing,
            Id.DworfRing,
            Id.DemonRing,
            Id.JokerKey,
            Id.AceKey,
            Id.Pendant,
            Id.Rod,
        };

        public List<Shop> Shops { get; set; } = new List<Shop>();
        public Dictionary<Shop.Id, Shop> ShopDict { get; set; } = new Dictionary<Shop.Id, Shop>();
        public List<StaticPrice> StaticPrices { get; set; } = new List<StaticPrice>();
        public Dictionary<DoorId, StaticPrice> StaticPriceDict { get; set; } = new Dictionary<DoorId, StaticPrice>();

        public ShopRandomizer(byte[] content, DoorRandomizer doorRandomizer)
        {
            var shop = new Shop(Shop.Id.EolisKeyShop, true);
            shop.Items.Add(new ShopItem(0x3258A, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            shop = new Shop(Shop.Id.EolisItemShop);
            shop.Items.Add(new ShopItem(0x3243E, content));
            shop.Items.Add(new ShopItem(0x32441, content));
            shop.Items.Add(new ShopItem(0x32444, content));
            shop.Items.Add(new ShopItem(0x32447, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            var staticPrice = new StaticPrice(DoorId.MartialArtsShop, 0x32116, content);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);
            staticPrice = new StaticPrice(DoorId.EolisMagicShop, 0x32371, content);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);
            staticPrice = new StaticPrice(DoorId.EolisMeatShop, 0x324B5, content);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);

            shop = new Shop(Shop.Id.ApoluneKeyShop, true);
            shop.Items.Add(new ShopItem(0x3258E, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            shop = new Shop(Shop.Id.ApoluneSecretShop);
            shop.Items.Add(new ShopItem(0x32458, content));
            shop.Items.Add(new ShopItem(0x3245B, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            shop = new Shop(Shop.Id.ApoluneItemShop);
            shop.Items.Add(new ShopItem(0x3244B, content));
            shop.Items.Add(new ShopItem(0x3244E, content));
            shop.Items.Add(new ShopItem(0x32451, content));
            shop.Items.Add(new ShopItem(0x32454, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            shop = new Shop(Shop.Id.ForepawKeyShop, true);
            shop.Items.Add(new ShopItem(0x32592, content));
            shop.Items.Add(new ShopItem(0x32595, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            shop = new Shop(Shop.Id.ForepawItemShop);
            shop.Items.Add(new ShopItem(0x3245F, content));
            shop.Items.Add(new ShopItem(0x32462, content));
            shop.Items.Add(new ShopItem(0x32465, content));
            shop.Items.Add(new ShopItem(0x32468, content));
            shop.Items.Add(new ShopItem(0x3246B, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            staticPrice = new StaticPrice(DoorId.ApoluneHospital, 0x325B8, content, true);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);
            staticPrice = new StaticPrice(DoorId.ForepawMeatShop, 0x324C2, content);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);
            staticPrice = new StaticPrice(DoorId.ForepawHospital, 0x325C7, content, true);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);

            shop = new Shop(Shop.Id.MasconKeyShop, true);
            shop.Items.Add(new ShopItem(0x32599, content));
            shop.Items.Add(new ShopItem(0x3259C, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            shop = new Shop(Shop.Id.MasconItemShop);
            shop.Items.Add(new ShopItem(0x3246F, content));
            shop.Items.Add(new ShopItem(0x32472, content));
            shop.Items.Add(new ShopItem(0x32475, content));
            shop.Items.Add(new ShopItem(0x32478, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            shop = new Shop(Shop.Id.MasconSecretShop);
            shop.Items.Add(new ShopItem(0x3247C, content));
            shop.Items.Add(new ShopItem(0x3247F, content));
            shop.Items.Add(new ShopItem(0x32482, content));
            shop.Items.Add(new ShopItem(0x32485, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            shop = new Shop(Shop.Id.VictimKeyShop, true);
            shop.Items.Add(new ShopItem(0x325A0, content));
            shop.Items.Add(new ShopItem(0x325A3, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            shop = new Shop(Shop.Id.VictimItemShop);
            shop.Items.Add(new ShopItem(0x32489, content));
            shop.Items.Add(new ShopItem(0x3248C, content));
            shop.Items.Add(new ShopItem(0x3248F, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            staticPrice = new StaticPrice(DoorId.MasconMeatShop, 0x324CF, content);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);
            staticPrice = new StaticPrice(DoorId.MasconHospital, 0x325D6, content, true);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);
            staticPrice = new StaticPrice(DoorId.VictimMeatShop, 0x324DC, content);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);
            staticPrice = new StaticPrice(DoorId.VictimHospital, 0x325E5, content, true);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);
            staticPrice = new StaticPrice(DoorId.FireMage, 0x32380, content, true);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);

            shop = new Shop(Shop.Id.ConflateItemShop);
            shop.Items.Add(new ShopItem(0x32493, content));
            shop.Items.Add(new ShopItem(0x32496, content));
            shop.Items.Add(new ShopItem(0x32499, content));
            shop.Items.Add(new ShopItem(0x3249C, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            shop = new Shop(Shop.Id.DaybreakKeyShop, true);
            shop.Items.Add(new ShopItem(0x325A7, content));
            shop.Items.Add(new ShopItem(0x325AA, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            shop = new Shop(Shop.Id.DaybreakItemShop);
            shop.Items.Add(new ShopItem(0x324A0, content));
            shop.Items.Add(new ShopItem(0x324A6, content));
            shop.Items.Add(new ShopItem(0x324A3, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            staticPrice = new StaticPrice(DoorId.ConflateMeatShop, 0x324E9, content);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);
            staticPrice = new StaticPrice(DoorId.ConflateHospital, 0x325F4, content, true);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);
            staticPrice = new StaticPrice(DoorId.DaybreakMeatShop, 0x324F6, content);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);

            shop = new Shop(Shop.Id.DartmoorKeyShop, true);
            shop.Items.Add(new ShopItem(0x325AE, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            shop = new Shop(Shop.Id.DartmoorItemShop);
            shop.Items.Add(new ShopItem(0x324AA, content));
            shop.Items.Add(new ShopItem(0x324AD, content));
            Shops.Add(shop);
            ShopDict.Add(shop.ShopId, shop);

            staticPrice = new StaticPrice(DoorId.DartmoorMeatShop, 0x32503, content);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);
            staticPrice = new StaticPrice(DoorId.DartmoorHospital, 0x32603, content, true);
            StaticPrices.Add(staticPrice);
            StaticPriceDict.Add(staticPrice.ShopId, staticPrice);

            foreach (var tempShop in Shops)
            {
                foreach (var item in tempShop.Items)
                {
                    if (item.Id == Id.RedPotion)
                    {
                        item.ShouldBeRandomized = false;
                    }
                    if (ItemOptions.RandomizeKeys == ItemOptions.KeyRandomization.Unchanged && keyIds.Contains(item.Id))
                    {
                        item.ShouldBeRandomized = false;
                    }

                    if (ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.Random ||
                        ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.Dagger ||
                        ItemOptions.StartingWeapon == ItemOptions.StartingWeaponOptions.LongSword)
                    {
                        if (tempShop.ShopId == Shop.Id.EolisItemShop && item.Id == Id.Dagger)
                        {
                            item.ShouldBeRandomized = false;
                        }
                    }
                    if (ItemOptions.GuaranteeStartingSpell)
                    {
                        if (tempShop.ShopId == Shop.Id.EolisItemShop && item.Id == Id.Deluge)
                        {
                            item.ShouldBeRandomized = false;
                        }
                    }
                }
            }
        }

        public void AddShopItems(List<Id> ids)
        {
            int index = 0;
            foreach (var shop in Shops)
            {
                foreach (var item in shop.Items)
                {
                    if (index >= ids.Count)
                    {
                        return;
                    }

                    if (item.ShouldBeRandomized)
                    {
                        item.Id = ids[index];
                        index++;
                    }
                }
            }
        }

        public void SetStaticItems(Id startingWeapon,
                                   Id startingSpell)
        {
            if (ItemOptions.StartingWeapon != ItemOptions.StartingWeaponOptions.NoGuaranteed &&
                ItemOptions.StartingWeapon != ItemOptions.StartingWeaponOptions.GuaranteeOnlyWithNoSpells)
            {
                Shops[1].Items[0].Id = startingWeapon;
            }
            if (ItemOptions.GuaranteeStartingSpell)
            {
                Shops[1].Items[3].Id = startingSpell;
            }
        }

        public List<Id> GetBaseIds()
        {
            var ids = new List<Id>();
            ids.Add(Id.GiantBlade);
            ids.Add(Id.GiantBlade);
            ids.Add(Id.StuddedMail);
            ids.Add(Id.FullPlate);
            ids.Add(Id.LargeShield);
            ids.Add(Id.MagicShield);
            ids.Add(Id.Hourglass);
            ids.Add(Id.Elixir);
            ids.Add(Id.Elixir);
            ids.Add(Id.WingBoots);
            ids.Add(Id.WingBoots);
            ids.Add(Id.WingBoots);
            ids.Add(Id.WingBoots);
            ids.Add(Id.Mattock);

            if (GeneralOptions.ShuffleWorlds && ItemOptions.RandomizeKeys == ItemOptions.KeyRandomization.Unchanged)
            {
                ids.Add(Id.KingKey);
            }
            else
            {
                if (ItemOptions.ReplacePoison)
                {
                    ids.Add(Id.BlackPotion);
                }
                else
                {
                    ids.Add(Id.Hourglass);
                }
            }

            return ids;
        }

        public void AddToContent(byte[] content)
        {
            foreach (var shop in Shops)
            {
                foreach (var item in shop.Items)
                {
                    item.AddToContent(content);
                }
            }

            foreach (var staticPrice in StaticPrices)
            {
                staticPrice.AddToContent(content);
            }
        }

        private void SetShopPrice(Door building, int price)
        {
            if (StaticPriceDict.TryGetValue(building.Id, out StaticPrice staticPrice))
            {
                int divisor = staticPrice.Expensive ? 10 : 20;
                staticPrice.Price = AdjustForMultiplier((ushort)price, divisor);
            }

            if (building.BuildingShop != null)
            {
                building.BuildingShop.MaxPrice = price;
            }
        }

        public void RandomizePrices(Random random, DoorRandomizer doorRandomizer)
        {
            var worlds = doorRandomizer.GetWorlds();
            var multipliers = new Dictionary<WorldNumber, int>();
            for (int i = 0; i < worlds.Count; i++)
            {
                multipliers[worlds[i].number] = 5 + 5 * i;
            }

            SetShopPrice(doorRandomizer.TownDoors[DoorId.EolisItemShop], 800);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.EolisKeyShop], 400);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.EolisGuru], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.EolisHouse], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.EolisMeatShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.EolisMagicShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.MartialArtsShop], 1000);

            int multiplier = multipliers[WorldNumber.Trunk];
            SetShopPrice(doorRandomizer.Buildings[DoorId.TrunkSecretShop], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ApoluneItemShop], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ApoluneKeyShop], 100 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ApoluneBar], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ApoluneGuru], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ApoluneHospital], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ApoluneHouse], 200 * multiplier);

            SetShopPrice(doorRandomizer.TownDoors[DoorId.ForepawItemShop], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ForepawKeyShop], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ForepawGuru], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ForepawHospital], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ForepawHouse], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ForepawMeatShop], 300 * multiplier);

            multiplier = multipliers[WorldNumber.Mist];
            SetShopPrice(doorRandomizer.TownDoors[DoorId.MasconItemShop], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.MasconKeyShop], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.MasconBar], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.MasconHospital], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.MasconHouse], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.MasconMeatShop], 200 * multiplier);
            SetShopPrice(doorRandomizer.Buildings[DoorId.MistSecretShop], 1000);

            SetShopPrice(doorRandomizer.TownDoors[DoorId.VictimItemShop], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.VictimKeyShop], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.VictimBar], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.VictimGuru], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.VictimHospital], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.VictimHouse], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.VictimMeatShop], 300 * multiplier);

            multiplier = multipliers[WorldNumber.Branch];
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ConflateItemShop], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ConflateGuru], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ConflateHospital], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ConflateHouse], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ConflateItemShop], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ConflateMeatShop], 200 * multiplier);

            SetShopPrice(doorRandomizer.TownDoors[DoorId.DaybreakItemShop], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DaybreakKeyShop], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DaybreakBar], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DaybreakGuru], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DaybreakHouse], 300 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DaybreakMeatShop], 300 * multiplier);

            multiplier = multipliers[WorldNumber.Dartmoor];
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DartmoorItemShop], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DartmoorKeyShop], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DartmoorMeatShop], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DartmoorBar], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DartmoorGuru], 200 * multiplier);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DartmoorHospital], 200 * multiplier);

            foreach (var shop in Shops)
            {
                foreach (var item in shop.Items)
                {
                    int maxPrice = shop.MaxPrice;
                    bool cheap = CheapIds.Contains(item.Id);
                    int price = RandomizePrice(maxPrice, cheap, random);
                    item.Price = (ushort)price;
                }
            }

            foreach (var staticPrice in StaticPrices)
            {
                int min = (int)(staticPrice.Price * 0.2);
                int max = staticPrice.Price * 2;
                int price = (random.Next(min, max + 1) / 10) * 10;
                staticPrice.Price = (ushort)price;
            }
        }

        private int RandomizePrice(int maxPrice, bool cheap, Random random)
        {
            int minValue = maxPrice / 100;
            if (cheap)
            {
                minValue /= 2;
            }
            int maxValue = minValue * 5;
            int price = random.Next(minValue, maxValue + 1) * 20;
            return price;
        }

        private ushort AdjustForMultiplier(ushort price, int divisor)
        {
           return (ushort)(((price / divisor) / 10) * 10);
        }
    }
}
