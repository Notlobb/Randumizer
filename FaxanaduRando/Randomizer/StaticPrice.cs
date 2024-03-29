namespace FaxanaduRando.Randomizer
{
    public class StaticPrice : ShopEntry
    {
        public DoorId ShopId { get; set; }
        public bool Expensive { get; set; } = false;

        public StaticPrice(DoorId id, int offset, byte[] content, bool expensive=false, int priceOffset=0) : base(offset, content, priceOffset)
        {
            ShopId = id;
            Expensive = expensive;
            if (ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Unchanged)
            {
                if (ShopId == DoorId.FireMage)
                {
                    Price = 1000;
                }
                else if (Expensive)
                {
                    Price = 500;
                }
                else
                {
                    Price = 200;
                }
            }
        }
    }
}
