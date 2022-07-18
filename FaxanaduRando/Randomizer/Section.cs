using System.Collections.Generic;

namespace FaxanaduRando
{
    public class Section
    {
        public List<byte> Bytes { get; set; } = new List<byte>();

        public void AddToContent(byte[] content, int offset)
        {
            for (int i = 0; i < Bytes.Count; i++)
            {
                content[offset + i] = Bytes[i];
            }
        }

        public static int GetOffset(int bank, int address, int start)
        {
            return (bank * 0x4000 + (address - start) + 16);
        }
    }
}
