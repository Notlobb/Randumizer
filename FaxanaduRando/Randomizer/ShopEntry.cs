using System;

namespace FaxanaduRando.Randomizer
{
    public abstract class ShopEntry
    {
        public ushort Price { get; set; }
        public ushort MaxPriceOverride { get; set; } = 0;

        public bool ShouldBeRandomized { get; set; } = true;
        public int Multiplier { get; set; } = 1;

        public int offset;
        private int priceOffset;

        public ShopEntry(int offset, byte[] content, int priceOffset)
        {
            this.offset = offset;
            this.priceOffset = priceOffset;
            Price = BitConverter.ToUInt16(content, offset + priceOffset);
        }

        public virtual void AddToContent(byte[] content)
        {
            var bytes = BitConverter.GetBytes(Price);
            for (int i = 0; i < bytes.Length; i++)
            {
                content[offset + priceOffset + i] = bytes[i];
            }
        }
    }
}
