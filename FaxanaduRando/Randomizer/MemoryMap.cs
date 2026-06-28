namespace FaxanaduRando.Randomizer
{
    internal static class RAM
    {
        // zero page RAM
        public const byte ZP_Joy1_ChangedButtonMask = 0x19;
        public const byte ZP_Player_Flags = 0xa4;
        public const byte ZP_Player_MoveAcceleration_U = 0xaa;

        // main RAM
        public const ushort PlayerIsDead = 0x0438;
    }

    internal static class ROM
    {
        // bank 15 vanilla code addresses (hook sites and callable routines)
        public const ushort Sprites_FlipRanges = 0xCBA8;
        public const ushort GameLoop_CheckPauseGame_JSR_Sprites_FlipRanges = 0xE039;
        public const ushort Player_UpdatePosFromKnockback_LDA_08 = 0xE28C;
        public const ushort Player_CheckIfOnLadder = 0xE752;
    }

}
