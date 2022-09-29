namespace FaxanaduRando.Randomizer
{
    public class GiftItem
    {
        public enum Id
        {
            EolisGuru,
            FortressGuru,
            JokerSpring,
            VictimBar,
            FireMage,
            AceKeyHouse,
            ConflateGuru,
            FraternalGuru,
        }

        public Id LocationId { get; set; }
        public ShopRandomizer.Id ConditionItem { get; set; }
        public ShopRandomizer.Id Item { get; set; }

        private int conditionOffset;
        private int itemOffset;

        public GiftItem(Id id, int conditionOffset, int itemOffset, byte[] content)
        {
            LocationId = id;
            ConditionItem = (ShopRandomizer.Id)content[conditionOffset];
            Item = (ShopRandomizer.Id)content[itemOffset];
            this.conditionOffset = conditionOffset;
            this.itemOffset = itemOffset;
        }

        public void AddToContent(byte[] content)
        {
            content[conditionOffset] = (byte)ConditionItem;
            content[itemOffset] = (byte)Item;
        }
    }
}
