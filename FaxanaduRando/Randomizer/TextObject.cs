namespace FaxanaduRando.Randomizer
{
    public class TextObject
    {
        public byte Text { get; set;}
        public int Offset { get; set;}

        public TextObject(byte[] content, int offset)
        {
            Text = content[offset];
            Offset = offset;
        }

        public void AddToContent(byte[] content)
        {
            content[Offset] = Text;
        }
    }
}
