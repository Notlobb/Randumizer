namespace FaxanaduRando.Randomizer
{
    public class Transition
    {
        public byte FromScreen { get; set; }
        public OtherWorldNumber ToWorld { get; set; }
        public byte ToScreen { get; set; }
        public byte NewPosition { get; set; }
        public byte NewPalette { get; set; }
        public int Offset { get; set; }
        public byte OldPosition { get; set; }
        public Screen ToScreenReference { get; set; } = null;

        public Transition(int offset, byte[] content)
        {
            FromScreen = content[offset];
            ToWorld = (OtherWorldNumber)content[offset + 1];
            ToScreen = content[offset + 2];
            NewPosition = content[offset + 3];
            NewPalette = content[offset + 4];
            Offset = offset;
        }

        public Transition(Transition other)
        {
            FromScreen = other.FromScreen;
            ToWorld = other.ToWorld;
            ToScreen = other.ToScreen;
            NewPosition = other.NewPosition;
            NewPalette = other.NewPalette;
            OldPosition = other.OldPosition;
            Offset = other.Offset;
            ToScreenReference = other.ToScreenReference;
        }

        public void AddToContent(byte[] content)
        {
            content[Offset] = FromScreen;
            content[Offset + 1] = (byte)ToWorld;
            content[Offset + 2] = ToScreen;
            content[Offset + 3] = NewPosition;
            content[Offset + 4] = NewPalette;
        }
    }
}
