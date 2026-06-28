namespace FaxanaduRando.Randomizer
{
    internal static class RAM
    {
        // zero page RAM
        public const byte ZP_Joy1_ChangedButtonMask = 0x19;

        // main RAM
        public const ushort PlayerIsDead = 0x0438;
    }

    internal static class ROM
    {
        public const ushort GameLoop_CheckPauseGame_JSR_Sprites_FlipRanges = 0xE039;
        public const ushort Sprites_FlipRanges = 0xCBA8;
    }
}
