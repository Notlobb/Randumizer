using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class ShopRandomizer
    {
        public enum Id
        {
            Dagger = 0,
            Longsword = 1,
            Giantblade = 2,
            Dragonslayer = 3,
            //RingElf2 = 17,
            //RingRuby2 = 18,
            //RingDworf2 = 19,
            //RingDemon2 = 20,
            Leatherarmour= 32,
            Studdedmail = 33,
            Fullplate = 34,
            Battlesuit = 35,
            Smallshield = 64,
            Largeshield = 65,
            Magicshield = 66,
            Battlehelmet = 67,
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
            Wingboots = 143,
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
            Id.Wingboots,
            Id.Elixir,
        };

        public static HashSet<Id> weaponIds = new HashSet<Id>
        {
            Id.Dagger,
            Id.Longsword,
            Id.Giantblade,
        };

        public static HashSet<Id> armorIds = new HashSet<Id>
        {
            Id.Leatherarmour,
            Id.Studdedmail,
            Id.Fullplate,
        };

        public static HashSet<Id> shieldIds = new HashSet<Id>
        {
            Id.Smallshield,
            Id.Largeshield,
            Id.Magicshield,
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
            Id.Wingboots,
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
            Id.Dragonslayer,
            Id.Battlesuit,
            Id.Battlehelmet,
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

            if (GeneralOptions.RandomizeScreens != GeneralOptions.ScreenRandomization.Unchanged)
            {
                var shopItem = new ShopItem(0x32441, content);
                shop.Items.Add(shopItem);
                shopItem.ShouldBeRandomized = false;
                shopItem.Id = Id.Wingboots;
                shopItem.MaxPriceOverride = 500;

                int offset = Section.GetOffset(12, 0xAD90, 0x8000);
                foreach (var item in shop.Items)
                {
                    item.offset = offset;
                    offset += 3;
                }

                offset = Section.GetOffset(12, 0xA381, 0x8000);
                content[offset] = 0x90;
                content[offset + 1] = 0xAD;
                offset = Section.GetOffset(12, 0xA387, 0x8000);
                content[offset] = 0x90;
                content[offset + 1] = 0xAD;
            }

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

        public List<Id> GetBaseIds(Random random)
        {
            var ids = new List<Id>();
            ids.Add(Id.Giantblade);
            ids.Add(Id.Giantblade);
            ids.Add(Id.Studdedmail);
            ids.Add(Id.Fullplate);
            ids.Add(Id.Largeshield);
            ids.Add(Id.Magicshield);
            ids.Add(Id.Hourglass);
            ids.Add(Id.Elixir);
            ids.Add(Id.Elixir);
            ids.Add(Id.Wingboots);
            ids.Add(Id.Wingboots);
            ids.Add(Id.Wingboots);
            ids.Add(Id.Mattock);
            ids.Add(GetMiscItem(random));

            if (GeneralOptions.ShuffleWorlds &&
                ItemOptions.RandomizeKeys == ItemOptions.KeyRandomization.Unchanged)
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
                    ids.Add(GetMiscItem(random));
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

        public static Id GetMiscItem(Random random)
        {
            if (ItemOptions.ReplacePoison && random.Next(0, 8) == 0)
            {
                return Id.BlackPotion;
            }

            return miscList[random.Next(miscList.Count)];
        }

        public void RandomizePrices(Random random, DoorRandomizer doorRandomizer)
        {
            SetShopPrice(doorRandomizer.TownDoors[DoorId.EolisItemShop], 800);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.EolisKeyShop], 400);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.EolisGuru], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.EolisHouse], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.EolisMeatShop], 400);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.EolisMagicShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.MartialArtsShop], 1000);

            SetShopPrice(doorRandomizer.Buildings[DoorId.TrunkSecretShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ApoluneItemShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ApoluneKeyShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ApoluneBar], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ApoluneGuru], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ApoluneHospital], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ApoluneHouse], 1000);

            SetShopPrice(doorRandomizer.TownDoors[DoorId.ForepawItemShop], 1500);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ForepawKeyShop], 1500);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ForepawGuru], 1500);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ForepawHospital], 1500);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ForepawHouse], 1500);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ForepawMeatShop], 1500);

            SetShopPrice(doorRandomizer.TownDoors[DoorId.MasconItemShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.MasconKeyShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.MasconBar], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.MasconHospital], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.MasconHouse], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.MasconMeatShop], 1000);
            SetShopPrice(doorRandomizer.Buildings[DoorId.MistSecretShop], 500);

            SetShopPrice(doorRandomizer.TownDoors[DoorId.VictimItemShop], 1500);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.VictimKeyShop], 1500);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.VictimBar], 1500);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.VictimGuru], 1500);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.VictimHospital], 1500);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.VictimHouse], 1500);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.VictimMeatShop], 1500);

            SetShopPrice(doorRandomizer.TownDoors[DoorId.ConflateItemShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ConflateGuru], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ConflateHospital], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ConflateHouse], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ConflateItemShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.ConflateMeatShop], 1000);

            SetShopPrice(doorRandomizer.TownDoors[DoorId.DaybreakItemShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DaybreakKeyShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DaybreakBar], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DaybreakGuru], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DaybreakHouse], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DaybreakMeatShop], 1000);

            SetShopPrice(doorRandomizer.TownDoors[DoorId.DartmoorItemShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DartmoorKeyShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DartmoorMeatShop], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DartmoorBar], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DartmoorGuru], 1000);
            SetShopPrice(doorRandomizer.TownDoors[DoorId.DartmoorHospital], 1000);

            foreach (var shop in Shops)
            {
                foreach (var item in shop.Items)
                {
                    int maxPrice = item.MaxPriceOverride > 0 ? item.MaxPriceOverride : (shop.MaxPrice * shop.Multiplier);
                    bool cheap = CheapIds.Contains(item.Id);
                    int price = RandomizePrice(maxPrice, cheap, random);
                    item.Price = (ushort)price;
                }
            }

            foreach (var staticPrice in StaticPrices)
            {
                int price = staticPrice.Price * staticPrice.Multiplier;
                int min = (int)(price * 0.2);
                int max = price;
                price = (random.Next(min, max + 1) / 10) * 10;
                staticPrice.Price = (ushort)price;
            }
        }

        private void SetShopPrice(Door building, int price)
        {
            if (StaticPriceDict.TryGetValue(building.Id, out StaticPrice staticPrice))
            {
                int divisor = staticPrice.Expensive ? 4 : 8;
                staticPrice.Price = AdjustForMultiplier((ushort)price, divisor);
            }

            if (building.BuildingShop != null)
            {
                building.BuildingShop.MaxPrice = price;
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
