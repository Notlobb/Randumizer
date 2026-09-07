using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public static class AsmHacks
    {

        public static int DynamicHackKillSwitch(byte[] content, ushort cpu_addr)
        {
            var switchSection = new Section();

            // install hook - reference the new routine
            switchSection.JSR(cpu_addr);
            switchSection.FlushToContent(content, 15, ROM.GameLoop_CheckPauseGame_JSR_Sprites_FlipRanges);

            // call the routine vanilla would have called if we didn't install the hook
            switchSection.JSR(ROM.Sprites_FlipRanges);
            switchSection.LDA_zp(RAM.ZP_Joy1_ChangedButtonMask);
            // test bit 5: select button
            switchSection.AND_imm(0b00100000);
            switchSection.BEQ("@select_not_pressed");
            switchSection.LDA_imm(0x01);
            switchSection.STA_abs(RAM.PlayerIsDead);
            switchSection.Label("@select_not_pressed");
            switchSection.RTS();
            // patch rom with this new routine
            return switchSection.FlushToContent(content, 15, cpu_addr);
        }

        public static int DynamicHackPreventKnockbackOnLadders(byte[] content, ushort cpu_addr)
        {
            var section = new Section();

            // add hook to new routine near the end of vanilla Player_UpdatePosFromKnockback
            section.JSR(cpu_addr);
            section.NOP();
            section.FlushToContent(content, 15, ROM.Player_UpdatePosFromKnockback_LDA_08);

            // call the routine that checks if player is climbing and sets a status flag in zeropage
            section.JSR(ROM.Player_CheckIfOnLadder);
            section.LDA_zp(RAM.ZP_Player_Flags);
            // test bit 3 - is player climbing
            section.AND_imm(0b00001000);
            section.BEQ("@not_climbing");
            // climbing - prevent knockback
            section.LDA_imm(0x00);
            section.STA_zp(RAM.ZP_Player_MoveAcceleration_U);
            section.RTS();
            // not climbing - perform the vanilla behavior overwritten by the hook
            section.Label("@not_climbing");
            section.LDA_imm(0x08);
            section.STA_zp(RAM.ZP_Player_MoveAcceleration_U);
            section.RTS();
            // patch rom with this new routine
            return section.FlushToContent(content, 15, cpu_addr);
        }

        // in vanilla door requirements range from 0-8, where 0 is no requirement
        // this hack adds a requirement value 9 which only allows a door to be opened
        // based on certain requirments which are themselves an aggrregate of options
        public static int DynamicHackDoorRequirementHandler(byte[] content, ushort cpu_addr,
            bool dragonSlayerRequired, bool pendantRodRubyRequired, bool moveSpringQuestRequirement,
            bool showDragonSlayerFailureText)
        {
            var reqSection = new Section();

            // install hook - reference the new routine
            reqSection.JMP_abs(cpu_addr);
            reqSection.FlushToContent(content, 15, ROM.Game_RunDoorRequirementHandler_BEQ_RTS);

            // new door requirement handler
            reqSection.BNE("@door_has_requirement");
            reqSection.RTS();

            reqSection.Label("@door_has_requirement");
            reqSection.CMP_imm(0x09);
            reqSection.BEQ("@req_09");
            reqSection.ASL();
            // jump back to the vanilla routine and continue vanilla logic for requirements 1-8
            // we do ASL first since we overwrote that with the hook
            reqSection.JMP(ROM.Game_RunDoorRequirementHandler_TAY);

            // we are using the new non-vanilla requirement, run our custom logic
            reqSection.Label("@req_09");

            if (dragonSlayerRequired)
            {
                reqSection.LDA_abs(RAM.EquippedWeapon);
                // test for equpped weapon 3 - dragon slayer
                reqSection.CMP_imm(0x03);
                reqSection.BEQ("@has_dragon_slayer");

                // dragon slayer required but not equipped
                if (showDragonSlayerFailureText)
                {
                    reqSection.LDA_imm(0x00); // run script 0 (show message "you're not dressed for work")
                    reqSection.JSR(ROM.MMC1_LoadBankAndJump);
                    reqSection.Db(0x0c);               // inline param: bank 12
                    reqSection.Dw(ROM.Scripts_Begin); // inline param: script routine address
                }
                reqSection.RTS();

                reqSection.Label("@has_dragon_slayer");
            }

            if (pendantRodRubyRequired)
            {
                reqSection.LDA_abs(RAM.SpecialItems);
                // test bit 1 - pendant
                reqSection.AND_imm(0b00000010);
                reqSection.BNE("@has_pendant");
                reqSection.RTS();

                reqSection.Label("@has_pendant");
                reqSection.LDA_abs(RAM.SpecialItems);
                // test bit 2 - magical rod
                reqSection.AND_imm(0b00000100);
                reqSection.BNE("@has_rod");
                reqSection.RTS();

                reqSection.Label("@has_rod");
                reqSection.LDA_abs(RAM.SpecialItems);
                // test bit 6 - ring of ruby
                reqSection.AND_imm(0b01000000);
                reqSection.BNE("@has_ruby_ring");
                reqSection.RTS();

                reqSection.Label("@has_ruby_ring");
            }

            if (moveSpringQuestRequirement)
            {
                reqSection.LDA_abs(RAM.QuestFlags);
                // test bits 0, 1 and 2 - have all 3 springs been opened?
                reqSection.AND_imm(0b00000111);
                reqSection.CMP_imm(0b00000111);
                reqSection.BEQ("@all_springs_open");
                reqSection.RTS();
                reqSection.Label("@all_springs_open");
            }

            // all requirements passed
            reqSection.JMP_abs(ROM.Game_UnlockDoor);
            return reqSection.FlushToContent(content, 15, cpu_addr);
        }

        // apply the pending stage, mark a stage change as pending, then load the destination outside area
        public static int DynamicHackFlexibleDoorsApplyPendingStage(byte[] content, ushort cpu_addr)
        {
            var section = new Section();

            // update the sameworld-door logic to jump into our new routine instead of vanilla
            section.JMP(cpu_addr);
            section.FlushToContent(content, 15, ROM.Player_CheckHandleEnterDoor_enterScreen);

            // new routine for applying pending stage, and clearing the stage-is-pending flag
            section.LDA_imm(0x01);
            section.STA_abs(RAM.StageChangePending);
            section.LDA_abs(RAM.PendingStage);
            section.STA_abs(RAM.CurrentStage);
            section.JSR(ROM.Game_SetupAndLoadOutsideArea);
            section.RTS();
            return section.FlushToContent(content, 15, cpu_addr);
        }

        // clear the pending stage flag and apply palette handling
        public static int DynamicHackFlexibleDoorsHandlePalette(byte[] content, ushort cpu_addr)
        {
            var section = new Section();

            // update the stage palette logic to jump into our palette handler
            section.JMP(cpu_addr);
            section.NOP(2);
            section.FlushToContent(content, 15, ROM.Game_LoadCurrentArea_LoadPalette);

            // new routine for handling hack door palette
            section.LDA_imm(0x00);
            section.JSR(ROM.Screen_LoadSpritePalette);
            section.LDA_abs(RAM.StageChangePending);
            section.CMP_imm(0x01);
            section.BEQ("@stage_pending");
            // hack stage change not pending, use vanilla palette logic
            section.JMP(ROM.Game_LoadCurrentArea_LDX_Stage);

            // hack stage change pending - clear the flag and load screen
            section.Label("@stage_pending");
            section.LDA_imm(0x00);
            section.STA_abs(RAM.StageChangePending);
            section.JMP(ROM.Screen_Load);

            return section.FlushToContent(content, 15, cpu_addr);
        }

        // extract stage and door requirement from hacked same-world doors
        public static int DynamicHackFlexibleDoorsExtractStageAndDoorRequirement(byte[] content, ushort cpu_addr)
        {
            var section = new Section();

            // instead of storing A in door requirement ram directly, jump to the new routine
            section.JSR(cpu_addr);
            section.FlushToContent(content, 15, ROM.Area_SetStateFromDoorDestination_STA_DoorReq);

            // extract stage and actual door requirement from hack-door data: Hack_ExtractStageAndRequirement
            section.TAY();
            // get stage from the requirement byte (hi nibble)
            section.LSR(4);
            section.STA_abs(RAM.PendingStage);
            section.TYA();
            // get actual requirement (lo nibble)
            section.AND_imm(0x0f);
            section.STA_abs(RAM.CurrentDoor_KeyRequirement);
            section.RTS();

            return section.FlushToContent(content, 15, cpu_addr);
        }

        // clear the pending stage-change flag before continuing with the vanilla outside-area setup and loading logic
        public static int DynamicHackFlexibleDoorsClearFlagAndLoadWorld(byte[] content, ushort cpu_addr)
        {
            var section = new Section();

            // hook vanilla code into our new routine
            section.JMP(cpu_addr);
            section.FlushToContent(content, 15, ROM.Player_EnterDoorToOutside_JMP_SetupArea);

            // clear pending hack stage change flag and load world
            section.LDA_imm(0x00);
            section.STA_abs(RAM.StageChangePending);
            section.JMP(ROM.Game_SetupAndLoadOutsideArea);
            return section.FlushToContent(content, 15, cpu_addr);
        }

        // implement a custom palette to music handler when the door hack is applied
        // TODO: Can be solved more efficiently by emitting extended key (palette) and value (music) tables instead
        // and updating the table references instead
        // this function takes 26 bytes or so, which is enough for a table with 13 entries (we need 9 in total)
        public static int DynamicHackFlexibleDoorsCustomPaletteToMusic(byte[] content, ushort cpu_addr,
            byte branchPalette, byte finalPalette)
        {
            var sec = new Section();

            // hook vanilla code into our new routine
            sec.JSR(cpu_addr);
            sec.FlushToContent(content, 15, ROM.Palette_Check_Loop_CMP_X);

            sec.CMP_imm(branchPalette);
            sec.BNE("@not_branches");
            // if branches - use music 4 (Branches)
            sec.LDA_imm(0x04);
            sec.BNE("@store_palette");

            sec.Label("@not_branches");
            sec.CMP_imm(finalPalette);
            sec.BEQ("@final_palette");

            // compare incoming palette with the lookup table and continue with vanilla mapping logic
            sec.CMP_abs_x(ROM.Palette_To_Music_Table_Palettes);
            sec.RTS();

            sec.Label("@final_palette");
            // if zenis - use music 16 (Zenis)
            sec.LDA_imm(0x10);

            sec.Label("@store_palette");
            sec.STA_abs((ushort)RAM.ZP_Music_Current); // TODO: can use STA_zp
            sec.STA_abs(RAM.World_DefaultMusic);
            sec.RTS();

            return sec.FlushToContent(content, 15, cpu_addr);
        }

        // Other-world transitions normally change the current world without changing
        // the current stage. With full segment shuffle, the destination world may have
        // moved to a different stage, so update CurrentStage during the transition
        //
        // TODO: Make the hack routine and lookup table contiguous automatically and
        // return their combined size
        public static int DynamicHackSegmentShuffleUpdateStageForOtherWorldTransition(byte[] content, ushort cpu_addr,
            ushort table_cpu_addr, Dictionary<OtherWorldNumber, WorldNumber> wDict)
        {
            var newSection = new Section();

            // replace the vanilla INY instruction with a call to the new routine
            // the routine reproduces the overwritten instruction before returning
            newSection.JSR(cpu_addr);
            newSection.FlushToContent(content, 15, ROM.HandleOtherWorldTransition_INY);

            // at the call site, A contains the destination world loaded from the
            // other-world transition data; use it as an index into the
            // world-to-shuffled-stage lookup table
            newSection.TAX();
            newSection.LDA_abs_x(table_cpu_addr);
            newSection.STA_abs(RAM.CurrentStage);

            // reproduce the overwritten vanilla INY instruction, then load the next
            // field in the transition entry: the destination screen index
            // vanilla code stores this value after the routine returns
            newSection.INY();
            newSection.LDA_ind_y(0x02);
            newSection.RTS();
            int bytecount = newSection.FlushToContent(content, 15, cpu_addr);

            // map each world ID to the stage containing that world after segment
            // shuffling. The destination world ID is used directly as the table index
            content[Section.GetOffset(15, table_cpu_addr)] = (byte)wDict[OtherWorldNumber.Eolis];
            content[Section.GetOffset(15, table_cpu_addr + 1)] = (byte)wDict[OtherWorldNumber.Trunk];
            content[Section.GetOffset(15, table_cpu_addr + 2)] = (byte)wDict[OtherWorldNumber.Mist];
            content[Section.GetOffset(15, table_cpu_addr + 3)] = (byte)wDict[OtherWorldNumber.Towns];
            content[Section.GetOffset(15, table_cpu_addr + 4)] = (byte)wDict[OtherWorldNumber.Buildings];
            content[Section.GetOffset(15, table_cpu_addr + 5)] = (byte)wDict[OtherWorldNumber.Branch];
            content[Section.GetOffset(15, table_cpu_addr + 6)] = (byte)wDict[OtherWorldNumber.Dartmoor];
            content[Section.GetOffset(15, table_cpu_addr + 7)] = (byte)wDict[OtherWorldNumber.EvilOnesLair];

            return bytecount;
        }

        public static int DynamicHackBossLockedItemsCheckNoBossesRemaining(byte[] content, ushort cpu_addr)
        {
            // create the hook
            var section = new Section();
            section.JSR(cpu_addr);

            // install the hook in several places
            section.AddToContent(content, Section.GetOffset(14, ROM.SpriteBehavior_BattleSuit_CheckForBosses));
            section.AddToContent(content, Section.GetOffset(14, ROM.SpriteBehavior_BattleHelmet_CheckForBosses));
            section.AddToContent(content, Section.GetOffset(14, ROM.SpriteBehavior_DragonSlayer_CheckForBosses));
            section.AddToContent(content, Section.GetOffset(14, ROM.SpriteBehavior_QMattock_CheckForBosses));
            section.AddToContent(content, Section.GetOffset(14, ROM.SpriteBehavior_QWingBoots_CheckForBosses));
            section.AddToContent(content, Section.GetOffset(14, ROM.SpriteBehavior_BlackOnyx_CheckForBosses));
            section.FlushToContent(content, 14, ROM.SpriteBehavior_Pendant_CheckForBosses);

            // new hack function
            section.TXA();
            section.PHA();
            section.LDY_imm(0x07); // loop over the 8 sprite slots backwards

            // TODO: branch directly here instead
            section.Label("@next_sprite_loop");
            section.LDA_abs_y(RAM.SpritesOnScreen);
            section.TAX();
            section.LDA_abs_x(ROM.SpriteCategoryTable);
            section.CMP_imm(0x07); // sprite category 7 is "boss"

            section.BNE("@sprite_not_boss");
            section.PLA();
            section.TAX();
            section.CLC();
            section.RTS(); // clear carry - signal the item sprite can not be shown

            section.Label("@sprite_not_boss");
            section.DEY();
            section.TYA();
            section.BPL("@next_sprite");
            section.PLA();
            section.TAX();
            section.SEC(); // set carry - signal no bosses - the item sprite can be shown
            section.RTS();

            // TODO: Remove this label and JMP. BPL can branch directly to @next_sprite_loop.
            section.Label("@next_sprite");
            section.JMP("@next_sprite_loop");

            return section.FlushToContent(content, 14, cpu_addr);
        }

        public static int DynamicHackUseMattockAnywhereExceptBannedScreens(byte[] content, ushort cpu_addr)
        {
            var mattocksection = new Section();

            // insert the hook, and allow mattock to be used if A=0 on return
            mattocksection.JSR(cpu_addr);
            mattocksection.NOP(3);
            mattocksection.FlushToContent(content, 15, ROM.Player_UseMattock_LDA_MetatileID);

            // new function in free space; returns 0 if mattock can be used
            mattocksection.LDA_abs(RAM.ZP_CurrentWorld); // TODO: LDA_zp
            mattocksection.CMP_imm(0x01);
            mattocksection.BNE("@not_trunk_screen_40");
            mattocksection.LDA_abs(RAM.ZP_CurrentScreen); // TODO: LDA_zp
            mattocksection.CMP_imm(0x28);
            mattocksection.BNE("@not_trunk_screen_40");
            // trunk screen 40 (final spring screen) - illegal
            mattocksection.LDA_imm(0x01);
            mattocksection.RTS();

            mattocksection.Label("@not_trunk_screen_40");
            mattocksection.LDA_abs(RAM.ZP_CurrentWorld); // TODO: LDA_zp
            mattocksection.CMP_imm(0x05);
            mattocksection.BNE("@not_branches_screen_30");
            mattocksection.LDA_abs(RAM.ZP_CurrentScreen); // TODO: LDA_zp
            mattocksection.CMP_imm(0x1E);
            mattocksection.BNE("@not_branches_screen_30");
            // branches screen 30 (magical rod screen) - illegal
            mattocksection.LDA_imm(0x01);
            mattocksection.RTS();

            mattocksection.Label("@not_branches_screen_30");
            // not an illegal screen - return 0 (mattock can be used)
            mattocksection.LDA_imm(0x00);
            mattocksection.RTS();

            return mattocksection.FlushToContent(content, 15, cpu_addr);
        }

        public static int DynamicHackFastText(byte[] content, ushort cpu_addr)
        {
            // Change AND mask from #$03 to #$00 (display char every frame)
            content[Section.GetOffset(15, ROM.TextBox_ShowNextChar_IfReady_TimerMask)] = 0x00;

            // Hook: replace "LDA #$01 / STA TextBox_PlayTextSound" with JSR + NOPs
            var newSection = new Section();
            newSection.JSR(cpu_addr);
            newSection.NOP(2);
            newSection.FlushToContent(content, 15, ROM.TextBox_ShowNextChar_LDA_01);

            // New subroutine: preserve original 4-frame sound cadence
            newSection.LDA_abs(RAM.TextBox_Timer);
            newSection.AND_imm(0x02);
            newSection.LSR_a();
            newSection.EOR_imm(0x01);
            newSection.STA_abs(RAM.TextBox_PlayTextSound);
            newSection.RTS();

            return newSection.FlushToContent(content, 15, cpu_addr);
        }

        // repurposes five vanilla sprite IDs as collectible gift items
        // installs their pickup logic and graphics, and configures the sprite behavior
        // category, animation layout, hitbox, tile count, and update handler needed for them to function as world pickups
        public static int DynamicHackConfigureGiftItemSprites(List<Level> levels, byte[] content, ushort cpu_addr,
            Table spriteBehaviourScriptTable, bool ignoreWingBootsQuestRequirement)
        {

            // gift items require the quest check removed; non-unchanged big-item spawning already removes it,
            // so this check only prevents writing the same patch twice
            if (ignoreWingBootsQuestRequirement)
            {
                StaticHackIgnoreWingBootsQuestRequirement(content);
            }

            /* ********************************************
             * Repurposed sprite IDs used for gift items:
             *
             * Sprite 18: Rock Snake    -> Joker Key
             * Sprite 53: Blue Lady     -> Demon's Ring
             * Sprite 54: Child         -> Ring of Dworf
             * Sprite 56: Martial Arts  -> Ace Key
             * Sprite 80: Mattock       -> Ruby Ring
             * ********************************************/

            // free up sprite IDs that will be repurposed for gift-item sprites
            // replace any existing uses with equivalent alternate sprite IDs
            foreach (var level in levels)
            {
                foreach (var screen in level.Screens)
                {
                    foreach (var sprite in screen.Sprites)
                    {
                        if (sprite.Id == Sprite.SpriteId.MattockOrRingRuby)
                        {
                            sprite.Id = Sprite.SpriteId.MattockBossLocked;
                        }
                        else if (sprite.Id == Sprite.SpriteId.Glove2)
                        {
                            sprite.Id = Sprite.SpriteId.Glove;
                        }
                        else if (sprite.Id == Sprite.SpriteId.KeyAce)
                        {
                            sprite.Id = (Sprite.SpriteId)59;
                        }
                    }
                }
            }

            // give all gift-item sprites the quest Wing Boots behavior script
            // in vanilla, this script delegates directly to an ASM procedure via a script opcode
            // allowing the gift items to reach the patched player pickup logic below
            spriteBehaviourScriptTable.Entries[(int)Sprite.SpriteId.KeyAce] =
                spriteBehaviourScriptTable.Entries[(int)Sprite.SpriteId.WingbootsBossLocked];
            spriteBehaviourScriptTable.Entries[(int)Sprite.SpriteId.RingDworf] =
                spriteBehaviourScriptTable.Entries[(int)Sprite.SpriteId.WingbootsBossLocked];
            spriteBehaviourScriptTable.Entries[(int)Sprite.SpriteId.RingDemon] =
                spriteBehaviourScriptTable.Entries[(int)Sprite.SpriteId.WingbootsBossLocked];
            spriteBehaviourScriptTable.Entries[(int)Sprite.SpriteId.RockSnakeOrJokerKey] =
                spriteBehaviourScriptTable.Entries[(int)Sprite.SpriteId.WingbootsBossLocked];
            spriteBehaviourScriptTable.Entries[(int)Sprite.SpriteId.MattockOrRingRuby] =
                spriteBehaviourScriptTable.Entries[(int)Sprite.SpriteId.WingbootsBossLocked];

            var newSection = new Section();

            // add the hook from vanilla item pickup logic into the hack version
            // add a NOP to be aligned with the next check (we are overwriting the mattock-pickup logic in vanilla)
            newSection.JMP(cpu_addr);
            newSection.NOP();
            newSection.FlushToContent(content, 15, ROM.Player_PickUp);

            // remove the check on glove pickup (static patch)
            newSection.NOP(4);
            newSection.FlushToContent(content, 15, ROM.Player_PickUp_checkGlove1);

            // the new pick-up hack routine follows
            newSection.TAX();
            newSection.CMP_imm((byte)Sprite.SpriteId.RingDemon);
            newSection.BNE("@not_demon_ring");
            newSection.LDA_abs(RAM.SpecialItems);
            // set bit 4 - demon's ring
            newSection.ORA_imm(0b00010000);
            newSection.STA_abs(RAM.SpecialItems);
            // play sound effect 8 - item pick up
            newSection.LDA_imm(0x08);
            newSection.JSR(ROM.Sound_PlayEffect);
            newSection.TXA();

            newSection.Label("@not_demon_ring");
            newSection.CMP_imm((byte)Sprite.SpriteId.RingDworf);
            newSection.BNE("@not_dworf_ring");
            newSection.LDA_abs(RAM.SpecialItems);
            // set bit 5 - ring of dworf
            newSection.ORA_imm(0b00100000);
            newSection.STA_abs(RAM.SpecialItems);
            // play sound effect 8 - item pick up
            newSection.LDA_imm(0x08);
            newSection.JSR(ROM.Sound_PlayEffect);
            newSection.TXA();

            newSection.Label("@not_dworf_ring");
            newSection.CMP_imm((byte)Sprite.SpriteId.KeyAce);
            newSection.BNE("@not_key_ace");
            // add item 4 (key ace) to the inventory if there is room
            newSection.LDA_imm(0x04);
            newSection.JSR(ROM.Player_PickUpItem);
            // play sound effect 8 - item pick up
            newSection.LDA_imm(0x08);
            newSection.JSR(ROM.Sound_PlayEffect);
            newSection.TXA();

            newSection.Label("@not_key_ace");
            newSection.CMP_imm((byte)Sprite.SpriteId.MattockOrRingRuby);
            newSection.BNE("@not_ring_ruby");
            newSection.LDA_abs(RAM.SpecialItems);
            // set bit 6 - ring of ruby
            newSection.ORA_imm(0b01000000);
            newSection.STA_abs(RAM.SpecialItems);
            // play sound effect 8 - item pick up
            newSection.LDA_imm(0x08);
            newSection.JSR(ROM.Sound_PlayEffect);
            newSection.TXA();

            newSection.Label("@not_ring_ruby");
            newSection.CMP_imm((byte)Sprite.SpriteId.RockSnakeOrJokerKey);
            newSection.BNE("@not_joker_key");
            // play sound effect 8 - item pick up
            newSection.LDA_imm(0x08);
            newSection.JSR(ROM.Sound_PlayEffect);
            // add item 8 (key jo) to the inventory if there is room
            newSection.LDA_imm(0x08);
            newSection.JMP(ROM.Player_PickUpItem);

            newSection.Label("@not_joker_key");
            newSection.TXA();
            // continue with vanilla logic from where we left off
            newSection.JMP(ROM.Player_PickUp_CheckMagicalRod);

            int next_cpu_addr = newSection.FlushToContent(content, 15, cpu_addr);

            var spriteTypeTable = new Table(Section.GetOffset(14, ROM.SpriteCategoryTable), 100, 1, content);
            // set the following sprites to have category 5 - item (Mattock already has this category)
            spriteTypeTable.Entries[(int)Sprite.SpriteId.RockSnakeOrJokerKey][0] = 5;
            spriteTypeTable.Entries[(int)Sprite.SpriteId.RingDemon][0] = 5;
            spriteTypeTable.Entries[(int)Sprite.SpriteId.RingDworf][0] = 5;
            spriteTypeTable.Entries[(int)Sprite.SpriteId.KeyAce][0] = 5;
            spriteTypeTable.AddToContent(content);

            var phaseIndexTable = new Table(Section.GetOffset(14, ROM.SpriteStartAnimationFrameTable, 0x8000), 100, 1, content);
            var index = phaseIndexTable.Entries[(int)Sprite.SpriteId.RingDemon][0];

            int bank7Offset = Section.GetOffset(7, 0x8000, 0x8000);
            int animationPointerOffset = bank7Offset + Util.GetPointer(content, bank7Offset + 6);
            int animationOffset = bank7Offset + Util.GetPointer(content, animationPointerOffset + index * 2);

            // first byte of any animation frame is (h-1)*16+(w-1), so 0x11 is a frame of 2x2 chr-tiles
            content[animationOffset] = 0x11;

            int bank6Offset = Section.GetOffset(6, 0x8000, 0x8000);
            int bank10Offset = Section.GetOffset(10, 0x8000, 0x8000);
            int spritePointer1 = Util.GetPointer(content, bank6Offset);
            int spritePointer2 = Util.GetPointer(content, bank7Offset);

            // locate the sprite CHR pointer tables in banks 6 and 7
            // sprite IDs below $37 use the bank 6 table; IDs $37 and above use bank 7
            int spriteDataPointerOffset1 = bank6Offset + spritePointer1 + ((int)Sprite.SpriteId.RingDemon) * 2;
            int spriteDataPointerOffset2 = bank6Offset + spritePointer1 + ((int)Sprite.SpriteId.RingDworf) * 2;
            int spriteDataPointerOffset3 = bank7Offset + spritePointer2 + ((int)Sprite.SpriteId.KeyAce - 0x37) * 2;
            int spriteDataPointerOffset4 = bank7Offset + spritePointer2 + ((int)Sprite.SpriteId.MattockOrRingRuby - 0x37) * 2;
            int spriteDataPointerOffset5 = bank6Offset + spritePointer1 + ((int)Sprite.SpriteId.RockSnakeOrJokerKey) * 2;
            var pointer = Util.GetPointer(content, spriteDataPointerOffset3);
            pointer += 64;
            var bytes = BitConverter.GetBytes(pointer);
            content[spriteDataPointerOffset4] = bytes[0];
            content[spriteDataPointerOffset4 + 1] = bytes[1];
            int spriteDataOffset1 = bank6Offset + Util.GetPointer(content, spriteDataPointerOffset1);
            int spriteDataOffset2 = bank6Offset + Util.GetPointer(content, spriteDataPointerOffset2);
            int spriteDataOffset3 = bank7Offset + Util.GetPointer(content, spriteDataPointerOffset3);
            int spriteDataOffset4 = bank7Offset + Util.GetPointer(content, spriteDataPointerOffset4);
            int spriteDataOffset5 = bank6Offset + Util.GetPointer(content, spriteDataPointerOffset5);

            // install 2x2 CHR graphics as sprite-gfx for Demon Ring, Ring of Dworf,
            // Key Ace, and Ring of Ruby from source tiles (background-gfx) in bank 10
            for (int i = 0; i < 32; i++)
            {
                content[spriteDataOffset1 + i] = content[bank10Offset + 84 * 16 + i];
                content[spriteDataOffset1 + 32 + i] = content[bank10Offset + 92 * 16 + i];
                content[spriteDataOffset2 + i] = content[bank10Offset + 86 * 16 + i];
                content[spriteDataOffset2 + 32 + i] = content[bank10Offset + 92 * 16 + i];
                content[spriteDataOffset3 + i] = content[bank10Offset + 118 * 16 + i];
                content[spriteDataOffset3 + 32 + i] = content[bank10Offset + 120 * 16 + i];
                content[spriteDataOffset4 + i] = content[bank10Offset + 88 * 16 + i];
                content[spriteDataOffset4 + 32 + i] = content[bank10Offset + 92 * 16 + i];
            }

            for (int i = 0; i < 16; i++)
            {
                content[spriteDataOffset5 + i] = content[bank10Offset + 197 * 16 + i];
                content[spriteDataOffset5 + 16 + i] = content[bank10Offset + 119 * 16 + i];
            }

            for (int i = 0; i < 16; i++)
            {
                content[spriteDataOffset5 + 32 + i] = content[bank10Offset + 198 * 16 + i];
                content[spriteDataOffset5 + 32 + 16 + i] = content[bank10Offset + 121 * 16 + i];
            }

            var tilesTable = new Table(Section.GetOffset(15, ROM.SpritesChrTileCountTable, 0xC000), 100, 1, content);
            var sizeTable = new Table(Section.GetOffset(14, ROM.SpriteHitBoxTable, 0x8000), 100, 1, content);
            var subBehaviourTable = new Table(Section.GetOffset(14, ROM.SpriteUpdateHandlerTable, 0x8000), 100, 2, content);

            // give all gift items the Demon Ring's start animation frame, which uses a common 2x2 tile layout
            // each sprite still renders its own CHR tiles; only the frame layout is shared
            // they will also remain on this single frame because they use the Glove's 1-frame update handler
            phaseIndexTable.Entries[(int)Sprite.SpriteId.RingDworf][0] = phaseIndexTable.Entries[(int)Sprite.SpriteId.RingDemon][0];
            phaseIndexTable.Entries[(int)Sprite.SpriteId.KeyAce][0] = phaseIndexTable.Entries[(int)Sprite.SpriteId.RingDemon][0];
            phaseIndexTable.Entries[(int)Sprite.SpriteId.MattockOrRingRuby][0] = phaseIndexTable.Entries[(int)Sprite.SpriteId.RingDemon][0];
            phaseIndexTable.Entries[(int)Sprite.SpriteId.RockSnakeOrJokerKey][0] = phaseIndexTable.Entries[(int)Sprite.SpriteId.RingDemon][0];

            // set all items' hitboxes to index 0 - which is 15x15 px
            sizeTable.Entries[(int)Sprite.SpriteId.RingDemon][0] = 0;
            sizeTable.Entries[(int)Sprite.SpriteId.RingDworf][0] = 0;
            sizeTable.Entries[(int)Sprite.SpriteId.KeyAce][0] = 0;
            sizeTable.Entries[(int)Sprite.SpriteId.MattockOrRingRuby][0] = 0;
            sizeTable.Entries[(int)Sprite.SpriteId.RockSnakeOrJokerKey][0] = 0;

            // all items - as sprites - will use 4 chr-tiles (2x2)
            tilesTable.Entries[(int)Sprite.SpriteId.RingDemon][0] = 4;
            tilesTable.Entries[(int)Sprite.SpriteId.RingDworf][0] = 4;
            tilesTable.Entries[(int)Sprite.SpriteId.KeyAce][0] = 4;
            tilesTable.Entries[(int)Sprite.SpriteId.MattockOrRingRuby][0] = 4;
            tilesTable.Entries[(int)Sprite.SpriteId.RockSnakeOrJokerKey][0] = 4;

            // make all the items use the same sprite update handler as the glove
            subBehaviourTable.Entries[(int)Sprite.SpriteId.RingDemon] = subBehaviourTable.Entries[(int)Sprite.SpriteId.Glove];
            subBehaviourTable.Entries[(int)Sprite.SpriteId.RingDworf] = subBehaviourTable.Entries[(int)Sprite.SpriteId.Glove];
            subBehaviourTable.Entries[(int)Sprite.SpriteId.KeyAce] = subBehaviourTable.Entries[(int)Sprite.SpriteId.Glove];
            subBehaviourTable.Entries[(int)Sprite.SpriteId.MattockOrRingRuby] = subBehaviourTable.Entries[(int)Sprite.SpriteId.Glove];
            subBehaviourTable.Entries[(int)Sprite.SpriteId.RockSnakeOrJokerKey] = subBehaviourTable.Entries[(int)Sprite.SpriteId.Glove];

            phaseIndexTable.AddToContent(content);
            tilesTable.AddToContent(content);
            sizeTable.AddToContent(content);
            subBehaviourTable.AddToContent(content);

            return next_cpu_addr;
        }

        public static int DynamicHackPoisonAsManaPotion(byte[] content, ushort cpu_addr)
        {
            var jumpSection = new Section();
            jumpSection.JSR(cpu_addr);
            jumpSection.AddToContent(content, Section.GetOffset(15, ROM.GameLoop_CheckUseCurrentItem_LDA_SelectedItem));

            var manaPotionSection = new Section();
            manaPotionSection.LDA_abs(RAM.SelectedItem);
            manaPotionSection.CMP_imm(0x11);
            manaPotionSection.BNE("@poison_not_selected");
            manaPotionSection.JSR(ROM.Player_FillHPAndMP_fillMPLoop);
            manaPotionSection.JSR(ROM.Player_ClearSelectedItem);

            manaPotionSection.Label("@poison_not_selected");
            manaPotionSection.LDA_abs(RAM.SelectedItem);
            manaPotionSection.RTS();

            int next_cpu_addr = manaPotionSection.FlushToContent(content, 15, cpu_addr);

            var convertPoisonSection = new Section();

            // play sound effect 8 - item picked up
            convertPoisonSection.Db(0x08);
            convertPoisonSection.JSR(ROM.Sound_PlayEffect);
            // place poison (black potion) in inventory
            convertPoisonSection.LDA_imm(0x11);
            convertPoisonSection.JSR(ROM.Player_PickUpItem);
            // TODO: this is a typo of 0x0378 which is RAM addr of current sprite index
            // X is overwritten by caller before use so this could be removed even in vanilla
            convertPoisonSection.LDX_abs(0x7803);
            convertPoisonSection.RTS();
            // TODO: These two NOPs can also be removed
            convertPoisonSection.NOP(2);

            convertPoisonSection.FlushToContent(content, 15, ROM.Player_PickUpPoison_DamageSoundIndex);

            return next_cpu_addr;
        }

        // start the game with full health, mana, some gold and optionally the ring of elf
        // this is a dynamic hack for bank 14 - the bank 15 portion is static
        public static int DynamicHackFastStart(byte[] content, ushort cpu_addr, bool startWithRingOfElf)
        {
            // start with 80 mp
            var healthSection = new Section();
            healthSection.Db(0x50);
            healthSection.AddToContent(content, Section.GetOffset(15, ROM.Start_Mana));

            // start with 80 hp
            healthSection = new Section();
            healthSection.Db(0x50);
            healthSection.AddToContent(content, Section.GetOffset(15, ROM.Start_Health));

            // relies on bank 14 being loaded in at the time this routine is run
            var newSection = new Section();
            newSection.JSR(cpu_addr);
            newSection.FlushToContent(content, 15, ROM.Game_Start_JSR_Game_LoadFirstLevel);

            // start with 1500 gold (256*5+220)
            newSection = new Section();
            newSection.LDA_imm(0xDC);
            newSection.STA_abs(RAM.GoldLowByte);
            newSection.LDA_imm(0x05);
            newSection.STA_abs(RAM.GoldMiddleByte);

            if (startWithRingOfElf)
            {
                // start with special items bit 7 set (ring of elf)
                newSection.LDA_imm(0b10000000);
                newSection.STA_abs(RAM.SpecialItems);
            }

            newSection.JSR(ROM.Game_LoadFirstLevel);
            newSection.RTS();

            return newSection.FlushToContent(content, 14, cpu_addr);
        }

        public static int DynamicHackFlexibleItems(byte[] content, ushort cpu_addr)
        {

            // allow items to be used in buildings
            // compare with nonexistent world $ff rather than $04 (buildings world)
            content[Section.GetOffset(12, ROM.PlayerMenu_HandleInventoryMenuInput_CMP_WorldNo)] = 0xFF;

            // allow items to be used regardless of player state
            // remove the check for player state flags when trying to use an item
            content[Section.GetOffset(15, ROM.GameLoop_CheckUseCurrentItem_BNE_Return)] = OpCode.NOP;
            content[Section.GetOffset(15, ROM.GameLoop_CheckUseCurrentItem_BNE_Return + 1)] = OpCode.NOP;

            // allow any item to be sold, including items not present in the shop's sell table
            var sellSection = new Section();
            sellSection.JMP(cpu_addr);
            sellSection.FlushToContent(content, 12, ROM.ShowSellMenu_JSR_FindSellMenuEntry);

            sellSection.JSR(ROM.FindSellMenuEntry);
            sellSection.CMP_imm(0xFF);
            sellSection.BEQ("@shop_entry_missing");
            // item has a normal sell-table entry; continue with vanilla logic
            sellSection.JMP(ROM.ShowSellMenu_LDX_StringCount);

            // add the otherwise-unsellable item to the menu and give it a default price of 100
            sellSection.Label("@shop_entry_missing");
            sellSection.TXA();
            sellSection.LDX_abs(RAM.UIStringCount);
            sellSection.STA_abs_x(RAM.UIDataArray);
            sellSection.LDA_imm(100);
            sellSection.STA_abs_x(RAM.ShopItemCostsLo);
            sellSection.LDA_imm(0x00);
            sellSection.JMP(ROM.ShowSellMenu_STA_CostHi);

            int next_cpu_addr = sellSection.FlushToContent(content, 12, cpu_addr);

            // preserve the original item ID in X when no sell-table entry is found
            content[Section.GetOffset(12, ROM.FindSellMenuEntry_TAX)] = OpCode.NOP;

            return next_cpu_addr;
        }

        // fix vanilla ointment invincibility when a shield is equipped
        // while ointment is active, bypass the shield-vs-magic collision check so
        // magic attacks continue through the normal player-hit logic where ointment
        // invincibility is handled
        public static int DynamicHackOintmentWorksWithShield(byte[] content, ushort cpu_addr)
        {
            var section = new Section();
            section.JSR(cpu_addr);
            section.FlushToContent(content, 14, ROM.Player_CheckShieldHitByMagic);

            // return shield value 3 while ointment is active, causing the vanilla
            // shield-vs-magic routine to return immediately; otherwise use the equipped shield
            section.LDA_abs(RAM.DurationOintment);
            section.BPL("@ointment_active");
            section.LDA_abs(RAM.EquippedShield);
            section.RTS();

            section.Label("@ointment_active");
            section.LDA_imm(0x03);
            section.RTS();

            return section.FlushToContent(content, 14, cpu_addr);
        }

        // strengthen shields against magic:
        // the Magic Shield negates damage completely; all other shields reduce it to 1/8
        // if the ointment hack is also applied, the damage check is negated completely for the Battle Helmet
        public static int DynamicHackStrengthenShieldsAgainstMagic(byte[] content, ushort cpu_addr)
        {
            var section = new Section();
            section.JSR(cpu_addr);
            section.NOP();
            section.FlushToContent(content, 14, ROM.Player_HandleHitByMagic_LSR_A);

            section = new Section();
            section.CPY_imm(0x02);
            section.BEQ("@return_0");
            section.LSR(3);
            section.RTS();

            section.Label("@return_0");
            section.LDA_imm(0x00);
            section.RTS();
            return section.FlushToContent(content, 14, cpu_addr);
        }

        // removes the check if quest flag 3 is set when loading Zorigeriru Wing Boots making them always available
        public static void StaticHackIgnoreWingBootsQuestRequirement(byte[] content)
        {
            var newSection = new Section();
            newSection.NOP(10);
            newSection.AddToContent(content, Section.GetOffset(14, ROM.SpriteBehavior_WingBootsDroppedByZorugeriru_LDA_QuestFlags));
        }

        // sets the title to wing boots duration table values based on the corresponding setting
        public static void StaticHackWingbootDurations(Random random, byte[] content, ItemOptions.WingBootDurations durationType)
        {
            var wbsection = new Section();

            switch (durationType)
            {
                case ItemOptions.WingBootDurations.Permanent:
                    AddDurations(1, 1, 1, 1);

                    // add another static hack to prevent decrement of the wingboots' duration
                    var bootSection = new Section();
                    bootSection.NOP(3);
                    bootSection.FlushToContent(content, 15, ROM.WingBootsDurationDecrement);
                    break;

                case ItemOptions.WingBootDurations.Random:
                    var duration = (byte)random.Next(10, 46);
                    wbsection.Db(duration);
                    duration += (byte)random.Next(1, 11);
                    wbsection.Db(duration);
                    duration += (byte)random.Next(1, 11);
                    wbsection.Db(duration);
                    duration += (byte)random.Next(1, 11);
                    wbsection.Db(duration);
                    break;

                case ItemOptions.WingBootDurations.Always40:
                    AddDurations(40, 40, 40, 40);
                    break;

                case ItemOptions.WingBootDurations.Always20:
                    AddDurations(20, 20, 20, 20);
                    break;

                case ItemOptions.WingBootDurations.ScalesUpFrom40:
                    AddDurations(40, 45, 50, 55);
                    break;

                case ItemOptions.WingBootDurations.ScalesUpFrom30:
                    AddDurations(30, 35, 40, 45);
                    break;

                case ItemOptions.WingBootDurations.ScalesUpFrom20:
                    AddDurations(20, 30, 40, 50);
                    break;

                case ItemOptions.WingBootDurations.ScalesUpFrom10:
                    AddDurations(10, 20, 30, 40);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(durationType), durationType,
                    "Invalid Wing Boots duration type");
            }

            wbsection.FlushToContent(content, 15, ROM.TitleToWingbootsDurationTable);

            // helper local to this function
            void AddDurations(params byte[] values)
            {
                foreach (var value in values)
                    wbsection.Db(value);
            }
        }

    }
}
