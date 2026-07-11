namespace FaxanaduRando.Randomizer
{
    internal static class RAM
    {
        // zero page RAM
        public const byte ZP_Temp_01 = 0x01;
        public const byte ZP_Joy1_ChangedButtonMask = 0x19;
        public const byte ZP_CurrentWorld = 0x24;
        public const byte ZP_CurrentScreen = 0x63;
        public const byte ZP_Player_Flags = 0xa4;
        public const byte ZP_Player_MoveAcceleration_U = 0xaa;
        public const byte ZP_Music_Current = 0xfa;

        // main RAM
        public const ushort MessageID = 0x0213;
        public const ushort UIStringCount = 0x021F;
        public const ushort UIDataArray = 0x0220;
        public const ushort ShopItemCostsLo = 0x0228;
        public const ushort Textbox_TitleCharOffset = 0x021C;
        public const ushort SpritesOnScreen = 0x02CC;
        public const ushort GoldLowByte = 0x0392;
        public const ushort GoldMiddleByte = 0x0393;
        public const ushort EquippedWeapon = 0x03BD;
        public const ushort EquippedShield = 0x03BF;
        public const ushort SelectedItem = 0x03C1;
        public const ushort World_DefaultMusic = 0x03D1;
        public const ushort DurationOintment = 0x0427;
        public const ushort CurrentDoor_KeyRequirement = 0x042b;
        public const ushort SpecialItems = 0x042c;
        public const ushort QuestFlags = 0x042d;
        public const ushort CurrentStage = 0x0435;
        public const ushort PlayerIsDead = 0x0438;

        // unused vanilla RAM, accessed through the $1800-$1FFF mirror
        public const ushort PendingStage = 0x1ffe;          // mirrors 0x07fe
        public const ushort StageChangePending = 0x1fff;    // mirrors 0x07ff
    }

    internal static class ROM
    {
        // bank 12 vanilla code addresses (hook sites and callable routines)
        public const ushort Scripts_Begin = 0x8241;
        public const ushort ScriptActionSetSpawnPoint_CMP_Current = 0x8394;
        public const ushort ShowSellMenu_LDX_StringCount = 0x8696;
        public const ushort ShowSellMenu_STA_CostHi = 0x86A9;
        public const ushort ShowSellMenu_JSR_FindSellMenuEntry = 0x8691;
        public const ushort FindSellMenuEntry = 0x8704;
        public const ushort FindSellMenuEntry_TAX = 0x8716;
        public const ushort PlayerMenu_HandleInventoryMenuInput_CMP_WorldNo = 0x8B87;
        public const ushort StringTitleScreenCopyright = 0x9DCC;
        public const ushort StringTitleLicensedTerminator = 0x9E0D;
        public const ushort TitleScreenMusicIndex = 0x9F3E;
        public const ushort ScriptVictimBarCheckRankOperand = 0xA254;
        public const ushort SceneIntroChrHi = 0xA72A;
        public const ushort SceneOutroChrHi = 0xA72B;
        public const ushort SceneIntroPaletteLo = 0xA72C;
        public const ushort SceneOutroPaletteLo = 0xA72D;
        public const ushort IntroScreenMusicIndex = 0xA7A8;
        public const ushort OutroScreenMusicIndex = 0xA93E;
        // bank 12 free space addresses used for hack injection
        public const ushort HackSellAnyItem = 0xADA0;

        // bank 14 vanilla code addresses (hook sites and callable routines)
        public const ushort SpriteUpdateHandlerTable = 0x8087;
        public const ushort HitEnemyWithMagic_CalcDefense = 0x81CF;
        public const ushort HitEnemyWithMagic_CalcDefense_TempLo = 0x81D2;
        public const ushort HitEnemyWithMagic_ReduceEnemyHP = 0x81FD;
        public const ushort Player_CheckShieldHitByMagic = 0x877C;
        public const ushort Player_HandleHitByMagic_LSR_A = 0x87E8;
        public const ushort PendantCheckOffset = 0x8879;
        public const ushort Sprite_CheckHitByCastMagic_LDA_HitWidth = 0x8AFF;
        public const ushort Sprite_CheckHitByCastMagic_STA_Temp01 = 0x8B02;
        public const ushort MagicHitWidthTable = 0x8B73;
        public const ushort SpriteStartAnimationFrameTable = 0x8C9F;
        public const ushort SpriteBehaviorShadowEura_MusicIndex = 0xA004;
        public const ushort SpriteBehavior_BattleSuit_CheckForBosses = 0xA379;
        public const ushort SpriteBehavior_BattleHelmet_CheckForBosses = 0xA3A9;
        public const ushort Sprites_RemoveAll = 0xA3B4;
        public const ushort SpriteBehavior_DragonSlayer_CheckForBosses = 0xA3E4;
        public const ushort SpriteBehavior_QMattock_CheckForBosses = 0xA408;
        public const ushort SpriteBehavior_QWingBoots_CheckForBosses = 0xA42C;
        public const ushort SpriteBehavior_BlackOnyx_CheckForBosses = 0xA450;
        public const ushort SpriteBehavior_Pendant_CheckForBosses = 0xA474;
        public const ushort SpriteBehavior_WingBootsDroppedByZorugeriru_LDA_QuestFlags = 0xA418;
        public const ushort ItemWingBootsRandom_JMP_Sprites_Remove = 0xA506;
        public const ushort Sprite_Remove = 0xA523;
        public const ushort SpriteDropValueTable = 0xACED;
        public const ushort SpriteBehaviorScriptTable = 0xAD2D;
        public const ushort SpriteHitBoxTable = 0xB4DF;
        public const ushort SpriteCategoryTable = 0xB544;
        public const ushort SpriteHPTable = 0xB5A9;
        public const ushort SpriteXPTable = 0xB60E;
        public const ushort SpriteDropIndexTable = 0xB672;
        public const ushort SpriteDamageTable = 0xB6D7;
        public const ushort SpriteMagicDefenseTable = 0xB73B;
        public const ushort MagicDamageTable = 0xB7A0;
        public const ushort MagicCostTable = 0xB7A9;
        // bank 14 free space addresses used for hack injection
        public const ushort HackFastStart = 0xBDC0;
        public const ushort HackBossLockedItemsCheckNoBossesRemaining = 0xBF00;
        public const ushort HackStrongerShields = 0xBFC0;
        public const ushort HackOintmentWorksWithShield = 0xBFE0;

        // bank 15 vanilla code addresses (hook sites and callable routines)
        public const ushort GameLoop_CheckUseCurrentItem_BNE_Return = 0xC47C;
        public const ushort GameLoop_CheckUseCurrentItem_LDA_SelectedItem = 0xC48B;
        public const ushort Player_ClearSelectedItem = 0xC4BF;
        public const ushort Player_FillHPAndMP_fillMPLoop = 0xC506;
        public const ushort TitleToWingbootsDurationTable = 0xC599;
        public const ushort Player_UseHourGlass_ReduceHP = 0xC5D8;
        public const ushort WingBootsDurationDecrement = 0xC5AE;
        public const ushort Player_UseMattock_LDA_MetatileID = 0xC637;
        public const ushort Player_PickUp = 0xC764;
        public const ushort Player_PickUp_CheckMagicalRod = 0xC768;
        public const ushort Player_PickUp_checkGlove1 = 0xC7A5;
        public const ushort Player_PickUpGlove_DurationValue = 0xC7DD;
        public const ushort Player_PickUpPoison_DamageSoundIndex = 0xC845;
        public const ushort Player_PickUpItem = 0xC8CD;
        public const ushort Sprites_FlipRanges = 0xCBA8;
        public const ushort SpritesChrTileCountTable = 0xCE1B;
        public const ushort HourGlassMusicIndex = 0xC5E7;
        public const ushort Screen_LoadSpritePalette = 0xD062;
        public const ushort Sound_PlayEffect = 0xD0E4;
        public const ushort DeathMusicIndex = 0xD989;
        public const ushort Game_SetupAndLoadOutsideArea = 0xDADC;
        public const ushort Game_Start_JSR_Game_LoadFirstLevel = 0xDB2C;
        public const ushort Start_Mana = 0xDB30;
        public const ushort Screen_Load = 0xDD46;
        public const ushort SpawnInTemplePaletteIndex = 0xDD91;
        public const ushort SpawnInTempleMusicIndex = 0xDD9E;
        public const ushort GameEndKingsRoomMusicIndex = 0xDDE5;
        public const ushort GameEndKingsRoomPaletteIndex = 0xDDEE;
        public const ushort Game_LoadFirstLevel = 0xDEA7;
        public const ushort Start_Health = 0xDEAF;
        public const ushort Start_Screen_Index = 0xDECB;
        public const ushort Game_LoadCurrentArea_LDX_Stage = 0xDF22;
        public const ushort Game_LoadCurrentArea_LoadPalette = 0xDF1D;
        public const ushort WorldToPaletteTable = 0xDF4C;
        public const ushort WorldToMusicTable = 0xDF5C;
        public const ushort CheckShowPlayerMenu_BEQ_Return = 0xE01C;
        public const ushort GameLoop_CheckPauseGame_JSR_Sprites_FlipRanges = 0xE039;
        public const ushort Player_UpdatePosFromKnockback_LDA_08 = 0xE28C;
        public const ushort Palette_Check_Loop_CMP_X = 0xE54E;
        public const ushort Player_CheckHandleEnterDoor_enterScreen = 0xE565;
        public const ushort Palette_To_Music_Table_Palettes = 0xE569;
        public const ushort Palette_To_Music_Table_Music = 0xE570;
        public const ushort Player_EnterDoorToOutside_JMP_SetupArea = 0xE5d7;
        public const ushort BuildingToMusicTable = 0xE5FF;
        public const ushort BuildingToPaletteTable = 0xE609;
        public const ushort Player_CheckIfOnLadder = 0xE752;
        public const ushort Area_SetStateFromDoorDestination_STA_DoorReq = 0xE84c;
        public const ushort Player_CheckPushingBlock_Check3Springs = 0xE971;
        public const ushort SwTransTrunkScreen12To22PaletteIndex = 0xEA4A;
        public const ushort SwTransTrunkScreen22To12PaletteIndex = 0xEA4E;
        public const ushort HandleOtherWorldTransition_INY = 0xEA82;
        public const ushort OwTransTrunkScreen0Invalid = 0xEAB0;
        public const ushort OwTransTrunkRightApolune = 0xEAB5;
        public const ushort OwTransTrunkLeftApolune = 0xEABA;
        public const ushort OwTransTrunkRightForepaw = 0xEABF;
        public const ushort OwTransTrunkLeftForepaw = 0xEAC4;
        public const ushort OwTransApoluneLeft = 0xEACA;
        public const ushort OwTransApoluneRight = 0xEACF;
        public const ushort OwTransForepawLeft = 0xEAD4;
        public const ushort OwTransForepawRight = 0xEAD9;
        public const ushort OwTransMasconLeft = 0xEADE;
        public const ushort OwTransMasconRight = 0xEAE3;
        public const ushort OwTransVictimLeft = 0xEAE8;
        public const ushort OwTransVictimRight = 0xEAED;
        public const ushort OwTransConflateLeft = 0xEAF2;
        public const ushort OwTransDaybreakLeft = 0xEAF7;
        public const ushort OwTransDaybreakRight = 0xEAFC;
        public const ushort OwTransDartmoorTownRight = 0xEB01;
        public const ushort OwTransMistRightMascon = 0xEB07;
        public const ushort OwTransMistLeftMascon = 0xEB0C;
        public const ushort OwTransMistRightVictim = 0xEB11;
        public const ushort OwTransMistLeftVictim = 0xEB16;
        public const ushort OwTransBranchesRightConflate = 0xEB1C;
        public const ushort OwTransBranchesRightDaybreak = 0xEB21;
        public const ushort OwTransBranchesLeftDaybreak = 0xEB26;
        public const ushort OwTransDartmoorRightDartmoorTown = 0xEB2C;
        public const ushort Game_UnlockDoor = 0xEBE1;
        public const ushort Game_RunDoorRequirementHandler_BEQ_RTS = 0xEB32;
        public const ushort Game_RunDoorRequirementHandler_TAY = 0xEB35;
        public const ushort OpenDoorWithRingOfElf_ScriptID = 0xEBA9;
        public const ushort OpenDoorWithRingOfDworf_ScriptID = 0xEBB9;
        public const ushort OpenDoorWithDemonsRing_ScriptID = 0xEBC9;
        public const ushort BossMusicIndex = 0xEFAC;
        public const ushort EndGameTransitionMusicIndex = 0xEFDA;
        public const ushort TextBox_DisplayMessage = 0xF4A2;
        public const ushort TextBox_DisplayMessage_BEQ_TextBox_ShowMessage = 0xF4A5;
        public const ushort TextBox_ShowMessage_RTS = 0xF557;
        public const ushort MMC1_LoadBankAndJump = 0xF859;
        public const ushort MantraMusicIndex = 0xFC81;
        // bank 15 free space addresses used for hack injection
        public const ushort HackItemPickup = 0xFCD0;
        public const ushort HackOtherWorldTransitionSetShuffledStage = 0xfd50;
        public const ushort HackWorldToShuffledStageTable = 0xFD68;
        public const ushort HackClearPendingStageAndLoadWorld = 0xfda0;
        public const ushort HackPoisonAsManaPotion = 0xFD70;
        public const ushort HackMattockAnywhere = 0xFDD0;
        public const ushort HackApplyPendingStage = 0xFE00;
        public const ushort HackClearPendingStageAndApplyPalette = 0xFE20;
        public const ushort HackExtractStageAndDoorRequirement = 0xFE40;
        public const ushort HackCustomPaletteToMusicHandler = 0xFE60;
        public const ushort HackNewDoorRequirementHandler = 0xFE80;
        public const ushort HackPreventLadderKnockback = 0xFED0;
        public const ushort HackKillswitch = 0xFEE4;
        public const ushort HackFastText = 0xFF00;
        public const ushort HackFastTextHelper = 0xFF90;
    }

}
