namespace FaxanaduRando.Randomizer
{
    public class ScrollingData
    {
        public byte Left;
        public byte Right;
        public byte Up;
        public byte Down;
        private int offset;

        public ScrollingData(byte[] content, int offset)
        {
            this.offset = offset;
            Left = content[offset];
            Right = content[offset + 1];
            Up = content[offset + 2];
            Down = content[offset + 3];
        }

        public void AddToContent(byte[] content)
        {
            content[offset] = Left;
            content[offset + 1] = Right;
            content[offset + 2] = Up;
            content[offset + 3] = Down;
        }
    }
}
