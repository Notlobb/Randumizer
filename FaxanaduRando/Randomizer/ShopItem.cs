using System;

namespace FaxanaduRando.Randomizer
{
    public class ShopItem : ShopEntry
    {
        public ShopRandomizer.Id Id { get; set; }

        public ShopItem(int offset, byte[] content, int priceOffset=1) : base(offset, content, priceOffset)
        {
            Id = (ShopRandomizer.Id)content[offset];
        }

        public override void AddToContent(byte[] content)
        {
            base.AddToContent(content);
            content[offset] = (byte)Id;
        }
    }
}
