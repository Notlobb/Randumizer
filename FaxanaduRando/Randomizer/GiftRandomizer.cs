using System.Collections.Generic;

namespace FaxanaduRando
{
    public class GiftRandomizer
    {
        public List<GiftItem> GiftItems { get; set; } = new List<GiftItem>();
        public Dictionary<GiftItem.Id, GiftItem> ItemDict { get; set; } = new Dictionary<GiftItem.Id, GiftItem>();
        public int BarRank { get; set; } = 10;

        private static readonly HashSet<ShopRandomizer.Id> regiftableItems = new HashSet<ShopRandomizer.Id>
        {
            ShopRandomizer.Id.AceKey,
            ShopRandomizer.Id.KingKey,
            ShopRandomizer.Id.QueenKey,
            ShopRandomizer.Id.JackKey,
            ShopRandomizer.Id.JokerKey,
            ShopRandomizer.Id.Hourglass,
            ShopRandomizer.Id.RedPotion,
            ShopRandomizer.Id.WingBoots,
            ShopRandomizer.Id.Elixir,
            ShopRandomizer.Id.BlackPotion,
            ShopRandomizer.Id.Mattock,
        };

        private static readonly HashSet<GiftItem.Id> regiftableLocations = new HashSet<GiftItem.Id>
        {
            GiftItem.Id.VictimBar,
            GiftItem.Id.FireMage,
            GiftItem.Id.AceKeyHouse,
            GiftItem.Id.FraternalGuru,
        };

        private static readonly HashSet<GiftItem.Id> optionallyRegiftableLocations = new HashSet<GiftItem.Id>
        {
            GiftItem.Id.FortressGuru,
            GiftItem.Id.JokerHouse,
        };

        public GiftRandomizer(byte[] content)
        {
            var giftItem = new GiftItem(GiftItem.Id.FortressGuru,
                                    Section.GetOffset(12, 0xA193, 0x8000),
                                    Section.GetOffset(12, 0xA199, 0x8000),
                                    content);
            GiftItems.Add(giftItem);
            ItemDict.Add(giftItem.LocationId, giftItem);

            giftItem = new GiftItem(GiftItem.Id.JokerHouse,
                                    Section.GetOffset(12, 0xA1CE, 0x8000),
                                    Section.GetOffset(12, 0xA1D4, 0x8000),
                                    content);
            GiftItems.Add(giftItem);
            ItemDict.Add(giftItem.LocationId, giftItem);

            giftItem = new GiftItem(GiftItem.Id.VictimBar,
                                    Section.GetOffset(12, 0xA25B, 0x8000),
                                    Section.GetOffset(12, 0xA261, 0x8000),
                                    content);
            GiftItems.Add(giftItem);
            ItemDict.Add(giftItem.LocationId, giftItem);

            giftItem = new GiftItem(GiftItem.Id.AceKeyHouse,
                                    Section.GetOffset(12, 0xA285, 0x8000),
                                    Section.GetOffset(12, 0xA28B, 0x8000),
                                    content);
            GiftItems.Add(giftItem);
            ItemDict.Add(giftItem.LocationId, giftItem);

            giftItem = new GiftItem(GiftItem.Id.FraternalGuru,
                                    Section.GetOffset(12, 0xA345, 0x8000),
                                    Section.GetOffset(12, 0xA34B, 0x8000),
                                    content);
            GiftItems.Add(giftItem);
            ItemDict.Add(giftItem.LocationId, giftItem);

            giftItem = new GiftItem(GiftItem.Id.FireMage,
                                    Section.GetOffset(12, 0xA36A, 0x8000),
                                    Section.GetOffset(12, 0xA373, 0x8000),
                                    content);
            GiftItems.Add(giftItem);
            ItemDict.Add(giftItem.LocationId, giftItem);

            giftItem = new GiftItem(GiftItem.Id.ConflateGuru,
                                    Section.GetOffset(12, 0xA635, 0x8000),
                                    Section.GetOffset(12, 0xA63B, 0x8000),
                                    content);
            GiftItems.Add(giftItem);
            ItemDict.Add(giftItem.LocationId, giftItem);

            giftItem = new GiftItem(GiftItem.Id.EolisGuru,
                                    Section.GetOffset(12, 0xA0BE, 0x8000),
                                    Section.GetOffset(12, 0xA0C4, 0x8000),
                                    content);
            GiftItems.Add(giftItem);
            ItemDict.Add(giftItem.LocationId, giftItem);
        }

        public void AddGiftItems(List<ShopRandomizer.Id> ids)
        {
            int index = 0;
            foreach (var item in GiftItems)
            {
                if (index >= ids.Count)
                {
                    return;
                }

                if (ItemOptions.AllowMultipleGifts &&
                    regiftableItems.Contains(ids[index]) &&
                    optionallyRegiftableLocations.Contains(item.LocationId))
                {
                    item.ConditionItem = ShopRandomizer.Id.Book;
                }
                else if (regiftableItems.Contains(ids[index]) &&
                         regiftableLocations.Contains(item.LocationId))
                {
                    item.ConditionItem = ShopRandomizer.Id.Book;
                }
                else if (regiftableItems.Contains(ids[index]) && item.LocationId == GiftItem.Id.EolisGuru &&
                         Util.GurusShuffled())
                {
                    item.ConditionItem = ShopRandomizer.Id.Book;
                }
                else
                {
                    item.ConditionItem = ids[index];
                }

                item.Item = ids[index];
                index++;
            }
        }

        public void AddToContent(byte[] content)
        {
            foreach (var giftItem in GiftItems)
            {
                giftItem.AddToContent(content);
            }
        }
    }
}
