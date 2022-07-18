using System;

namespace FaxanaduRando
{
    public abstract class ShopEntry
    {
        public ushort Price { get; set; }
        public bool ShouldBeRandomized { get; set; } = true;

        protected int offset;
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
