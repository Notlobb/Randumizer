namespace FaxanaduRando.Randomizer
{
    class SpriteType
    {
        public Sprite.SpriteId Id { get; set; } = Sprite.SpriteId.Undefined;
        public byte Hp { get; set; } = 0;
        public byte Damage { get; set; } = 0;

        public SpriteType(Sprite.SpriteId id, byte hp, byte damage)
        {
            Id = id;
            Hp = hp;
            Damage = damage;
        }
    }
}
