namespace FaxanaduRando
{
    public class Spell
    {
        public enum Id
        {
            Deluge,
            Thunder,
            Fire,
            Death,
            Tilte
        }

        public Id SpellId { get; set; }
        public byte ManaCost { get; set; }
        public byte Damage { get; set; }

        public Spell(Id id, byte manaCost, byte damage)
        {
            SpellId = id;
            ManaCost = manaCost;
            Damage = damage;
        }
    }
}
