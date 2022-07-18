namespace FaxanaduRando
{
    class Priest
    {
        public enum Id
        {
            Apolune,
            Forepaw,
            Mascon,
            Victim,
            Conflate,
            Daybreak,
            Dartmoor
        }

        public Id ID { get; set; }
        public byte RespawnValue { get; set; }
        private int offset;

        public Priest (Id id, int offset, byte[] content)
        {
            ID = id;
            RespawnValue = content[offset];
            this.offset = offset;
        }

        public void AddToContent(byte[] content)
        {
            content[offset] = RespawnValue;
        }
    }
}
