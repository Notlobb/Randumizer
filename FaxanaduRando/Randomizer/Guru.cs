namespace FaxanaduRando.Randomizer
{
    public class Guru
    {
        public enum GuruId
        {
            Eolis,
            Apolune,
            Forepaw,
            Mascon,
            Victim,
            Conflate,
            Daybreak,
            Dartmoor
        }

        public GuruId Id { get; set; }

        public OtherWorldNumber World { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Screen { get; set; }
        public byte WorldSpawn { get; set; }

        public Guru(GuruId id, OtherWorldNumber world, byte x, byte y, byte worldSpawn, byte screen)
        {
            Id = id;
            World = world;
            X = x;
            Y = y;
            WorldSpawn = worldSpawn;
            Screen = screen;
        }
    }
}
