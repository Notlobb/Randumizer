using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class Table
    {
        public List<byte[]> Entries { get; set; } = new List<byte[]>();

        private int offset;
        private int tableSize;
        private int entrySize;

        public Table(int offset, int tableSize, int entrySize, byte[] content)
        {
            this.offset = offset;
            this.tableSize = tableSize;
            this.entrySize = entrySize;

            for (int i = 0; i < tableSize; i++)
            {
                byte[] entry = new byte[entrySize];
                for (int j = 0; j < entrySize; j++)
                {
                    entry[j] = content[offset + i * entrySize + j];
                }

                Entries.Add(entry);
            }
        }

        public void AddToContent(byte[] content)
        {
            for (int i = 0; i < tableSize; i++)
            {
                for (int j = 0; j < entrySize; j++)
                {
                    content[offset + i * entrySize + j] = Entries[i][j];
                }
            }
        }
    }
}
