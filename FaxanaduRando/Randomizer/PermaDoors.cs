using System;
using System.Collections.Generic;
using static FaxanaduRando.Randomizer.DoorRandomizer;

namespace FaxanaduRando.Randomizer
{
    public static class PermaDoors
    {

        private const byte DoorKeyReqMin = 0x01;
        private const byte DoorKeyReqMax = 0x05;
        private const byte FLAG_COUNT = 31;
        private const byte DoorFlagMax = FLAG_COUNT * 8 - 1;

        // zeropage scratch bytes and internal in/out value
        private const byte PtrLo = 0xe2;
        private const byte PtrHi = 0xe3;
        private const byte ValueIO = 0xe5;
        private const byte DATA_BANK = 9;

        private readonly struct DoorFlag
        {
            public readonly byte Screen;
            public readonly byte YX;
            public readonly byte Flag;

            public DoorFlag(byte screen, byte yx, byte flag)
            {
                Screen = screen;
                YX = yx;
                Flag = flag;
            }
        }

        private static int GetWorldIndex(WorldNumber world)
        {
            return world switch
            {
                WorldNumber.Eolis => (int)OtherWorldNumber.Eolis,
                WorldNumber.Trunk => (int)OtherWorldNumber.Trunk,
                WorldNumber.Mist => (int)OtherWorldNumber.Mist,
                WorldNumber.Branch => (int)OtherWorldNumber.Branch,
                WorldNumber.Dartmoor => (int)OtherWorldNumber.Dartmoor,
                WorldNumber.EvilOnesLair => (int)OtherWorldNumber.EvilOnesLair,
                _ => throw new InvalidOperationException("Invalid stage world")
            };
        }

        private static void AddStageDoor(List<List<DoorFlag>> table,
    DoorRandomizer doors, World world, PositionData position,
    ExitDoor exit, ref int flagCount)
        {
            byte requirement = (byte)doors.GetExitRequirement(exit);

            if (requirement < DoorKeyReqMin || requirement > DoorKeyReqMax)
                return;

            if (flagCount > DoorFlagMax)
                throw new InvalidOperationException("Too many key-locked doors");

            table[GetWorldIndex(world.index)].Add(new DoorFlag(
                position.screen,
                position.oldPos,
                (byte)(DoorFlagMax - flagCount++)
            ));
        }

        // creates a list of door-flags based on game data
        // outer dimension is world number
        private static List<List<DoorFlag>> MakeDoorFlagTable(DoorRandomizer doors)
        {
            List<List<DoorFlag>> result = new List<List<DoorFlag>>();

            // 8 actual worlds: Eolis, Trunk, Mist, Towns, Buildings,
            // Branch, Dartmoor, Evil Ones' Lair
            for (int i = 0; i < 8; ++i)
                result.Add(new List<DoorFlag>());

            int flagCount = 0;

            foreach (Door door in doors.Doors.Values)
            {
                byte requirement = (byte)door.key;

                if (requirement < DoorKeyReqMin || requirement > DoorKeyReqMax)
                    continue;

                if (flagCount > DoorFlagMax)
                    throw new InvalidOperationException("Too many key-locked doors");

                result[(int)door.World].Add(new DoorFlag(
                door.Position.screen, door.Position.oldPos, (byte)(DoorFlagMax - flagCount++)
                ));
            }

            var worlds = doors.Worlds;

            AddStageDoor(result, doors, worlds[WorldNumber.Eolis],
                worlds[WorldNumber.Eolis].forwardPosition,
                ExitDoor.EolisExit, ref flagCount);

            AddStageDoor(result, doors, worlds[WorldNumber.Trunk],
                worlds[WorldNumber.Trunk].backwardPosition,
                ExitDoor.TrunkExitBackwards, ref flagCount);
            AddStageDoor(result, doors, worlds[WorldNumber.Trunk],
                worlds[WorldNumber.Trunk].forwardPosition,
                ExitDoor.TrunkExit, ref flagCount);

            AddStageDoor(result, doors, worlds[WorldNumber.Mist],
                worlds[WorldNumber.Mist].backwardPosition,
                ExitDoor.MistExitBackwards, ref flagCount);
            AddStageDoor(result, doors, worlds[WorldNumber.Mist],
                worlds[WorldNumber.Mist].forwardPosition,
                ExitDoor.MistExit, ref flagCount);

            AddStageDoor(result, doors, worlds[WorldNumber.Branch],
                worlds[WorldNumber.Branch].backwardPosition,
                ExitDoor.BranchExitBackwards, ref flagCount);
            AddStageDoor(result, doors, worlds[WorldNumber.Branch],
                worlds[WorldNumber.Branch].forwardPosition,
                ExitDoor.BranchExit, ref flagCount);

            AddStageDoor(result, doors, worlds[WorldNumber.Dartmoor],
                worlds[WorldNumber.Dartmoor].backwardPosition,
                ExitDoor.DartmoorExitBackwards, ref flagCount);
            AddStageDoor(result, doors, worlds[WorldNumber.Dartmoor],
                worlds[WorldNumber.Dartmoor].forwardPosition,
                ExitDoor.DartmoorExit, ref flagCount);

            AddStageDoor(result, doors, worlds[WorldNumber.EvilOnesLair],
                worlds[WorldNumber.EvilOnesLair].backwardPosition,
                ExitDoor.EvilLairExit2, ref flagCount);

            return result;
        }

        // creates a pointer table from world no to byte array
        // array entries are: (screen, yx, flag no)
        // $ff ends the array
        private static int InstallLookupTable(byte[] content, byte bank,
            ushort cpu_addr, List<List<DoorFlag>> table)
        {
            Section code = new Section();

            // emit world ptrs
            for (int world = 0; world < table.Count; world++)
                code.Dw($"door_flags_world_{world}");

            // emit lookup array per world
            for (int world = 0; world < table.Count; world++)
            {
                code.Label($"door_flags_world_{world}");

                foreach (var door in table[world])
                {
                    code.Db(door.Screen);
                    code.Db(door.YX);
                    code.Db(door.Flag);
                }

                code.Db(0xff);
            }

            return code.FlushToContent(content, bank, cpu_addr);
        }

        private static int InstallTableWalker(byte[] content, byte bank, ushort cpu_addr, ushort table_addr)
        {
            Section code = new Section();

            code.LDA_zp(RAM.ZP_CurrentWorld);
            code.ASL();
            code.TAX();

            code.LDA_abs_x(table_addr);
            code.STA_zp(PtrLo);
            code.LDA_abs_x((ushort)(table_addr + 1));
            code.STA_zp(PtrHi);

            code.LDY_imm(0x00);

            code.Label("@next");
            code.LDA_ind_y(PtrLo);
            code.CMP_imm(0xff);
            code.BEQ("@not_found");

            code.CMP_zp(RAM.ZP_CurrentScreen);
            code.BNE("@skip");

            code.INY();
            code.LDA_ind_y(PtrLo);
            code.CMP_zp(RAM.ZP_DoorBlockPos);
            code.BEQ("@found");

            // screen matched, but Y is now +1
            code.INY();
            code.INY();
            code.JMP("@next");

            code.Label("@skip");
            code.INY();
            code.INY();
            code.INY();
            code.JMP("@next");

            code.Label("@found");
            code.INY();
            code.LDA_ind_y(PtrLo);
            // flag no stored in A
            code.RTS();

            code.Label("@not_found");
            code.LDA_imm(0xff);
            // $ff denotes no flag selection
            code.RTS();

            return code.FlushToContent(content, bank, cpu_addr);
        }

        // bitmask table for encoding and decoding extended flags
        private static int InstallBitmaskTable(byte[] content, byte bank, ushort cpu_addr)
        {
            Section code = new Section();
            code.Db(0x01);
            code.Db(0x02);
            code.Db(0x04);
            code.Db(0x08);
            code.Db(0x10);
            code.Db(0x20);
            code.Db(0x40);
            code.Db(0x80);
            return code.FlushToContent(content, bank, cpu_addr);
        }

        // A = flag number
        // Returns A = 0 if not set / invalid
        // Returns A != 0 if set
        private static int InstallCheckFlag(byte[] content, byte bank, ushort cpu_addr,
            ushort bitmask_table_addr)
        {
            Section code = new Section();

            code.CMP_imm(0xff);
            code.BEQ("@clear");

            code.TAX();
            code.LSR(3);
            code.TAY();

            code.TXA();
            code.AND_imm(0x07);
            code.TAX();

            code.LDA_abs_y(RAM.ExtendedFlags);
            code.AND_abs_x(bitmask_table_addr);
            code.RTS();

            code.Label("@clear");
            code.LDA_imm(0x00);
            code.RTS();

            return code.FlushToContent(content, bank, cpu_addr);
        }

        // A = flag number
        private static int InstallSetFlag(byte[] content, byte bank, ushort cpu_addr,
            ushort bitmask_table_addr)
        {
            Section code = new Section();

            code.CMP_imm(0xff);
            code.BEQ("@done");

            code.TAX();
            code.LSR(3);
            code.TAY();

            code.TXA();
            code.AND_imm(0x07);
            code.TAX();

            code.LDA_abs_y(RAM.ExtendedFlags);
            code.ORA_abs_x(bitmask_table_addr);
            code.STA_abs_y(RAM.ExtendedFlags);

            code.Label("@done");
            code.RTS();

            return code.FlushToContent(content, bank, cpu_addr);
        }

        // ZP addr VALUE_IO:
        // 0 = check
        // 1 = set
        //
        // returns:
        // check -> VALUE_IO=0 or VALUE_IO!=0
        // set -> ignored
        private static int InstallMain(byte[] content, byte bank, ushort cpu_addr,
            ushort table_walker_addr, ushort set_flag_addr, ushort check_flag_addr)
        {
            Section code = new Section();

            code.JSR(table_walker_addr);
            code.LDX_zp(ValueIO);
            code.BEQ("@check");

            code.JSR(set_flag_addr);
            code.RTS();

            code.Label("@check");
            code.JSR(check_flag_addr);
            code.STA_zp(ValueIO);
            code.RTS();

            return code.FlushToContent(content, bank, cpu_addr);
        }

        // main bank 9 installer of the perma-doors machinery
        // advanced the allocator, but returns the only cpu address the outside needs
        private static int InstallFreeBank(byte[] content, byte bank, DoorRandomizer doors,
            BankAddressAllocator alloc)
        {
            var door_flag_table = MakeDoorFlagTable(doors);

            ushort lookup_table_addr = alloc.GetAddress(bank);
            alloc.SetAddress(bank,
                InstallLookupTable(content, bank, lookup_table_addr, door_flag_table));

            ushort table_walker_addr = alloc.GetAddress(bank);
            alloc.SetAddress(bank,
                InstallTableWalker(content, bank, table_walker_addr, lookup_table_addr));

            ushort bitmask_table_addr = alloc.GetAddress(bank);
            alloc.SetAddress(bank,
                InstallBitmaskTable(content, bank, bitmask_table_addr));

            ushort check_flag_addr = alloc.GetAddress(bank);
            alloc.SetAddress(bank,
                InstallCheckFlag(content, bank, check_flag_addr, bitmask_table_addr));

            ushort set_flag_addr = alloc.GetAddress(bank);
            alloc.SetAddress(bank,
                InstallSetFlag(content, bank, set_flag_addr, bitmask_table_addr));

            ushort main_entry_addr = alloc.GetAddress(bank);
            alloc.SetAddress(bank,
                InstallMain(content, bank, main_entry_addr, table_walker_addr, set_flag_addr, check_flag_addr));

            return main_entry_addr;
        }

        // small bank 15 hack which clears the extended flag range which is not cleared on game init
        private static void InstallClearFlagMemory(byte[] content, BankAddressAllocator alloc)
        {
            ushort cpu_addr = alloc.GetAddress(15);

            Section code = new Section();

            code.LDX_imm(FLAG_COUNT);

            code.Label("@loop");
            code.STA_abs_x(RAM.ExtendedFlags - 1);
            code.DEX();
            code.BNE("@loop");

            code.JSR(ROM.Game_InitMMCAndBank);
            code.JMP(ROM.Game_Init_JSR_Game_InitScreenAndMusic);

            alloc.SetAddress(15, code.FlushToContent(content, 15, cpu_addr));

            // Replace JSR Game_InitMMCAndBank
            Section hook = new Section();
            hook.JMP(cpu_addr);
            hook.FlushToContent(content, 15, ROM.Game_Init_JSR_Game_InitMMCAndBank);
        }

        public static void Install(byte[] content, DoorRandomizer doors, BankAddressAllocator alloc)
        {
            // install the door -> RAM bit lookup machinery
            ushort other_bank_main_addr = (ushort)InstallFreeBank(content, DATA_BANK, doors, alloc);
            // clear the extended flag byte array on game init
            InstallClearFlagMemory(content, alloc);

            // install trampolines in bank 15
            ushort cpu_addr = alloc.GetAddress(15);

            Section code = new Section();

            code.Label("@set_flag_trampoline");
            code.LDA_imm(0x01);
            code.STA_zp(ValueIO);
            code.JSR("@shared_logic");
            code.LDA_imm(0xff);
            code.STA_abs(RAM.SelectedItem);
            code.RTS();

            code.Label("@check_flag_trampoline");
            code.LDA_imm(0x00);
            code.STA_zp(ValueIO);
            code.JSR("@shared_logic");
            code.LDA_zp(ValueIO);
            code.BEQ("@locked");

            // door was unlocked in the past, come on through
            code.LDA_imm(0x00);
            code.STA_abs(RAM.CurrentDoor_KeyRequirement);
            code.RTS();

            code.Label("@locked");
            // door still locked, let vanilla handle it
            code.LDA_abs(RAM.CurrentDoor_KeyRequirement);
            code.RTS();

            code.Label("@shared_logic");
            code.LDA_abs(RAM.CurrentROMBank);
            code.PHA();

            code.LDX_imm(DATA_BANK);
            code.JSR(ROM.MMC1_UpdateROMBank);
            code.JSR(other_bank_main_addr);

            code.PLA();
            code.TAX();
            code.JSR(ROM.MMC1_UpdateROMBank);
            code.RTS();

            ushort set_flag_trampoline = code.GetLabelAddress("@set_flag_trampoline", cpu_addr);
            ushort check_flag_trampoline = code.GetLabelAddress("@check_flag_trampoline", cpu_addr);

            alloc.SetAddress(15, code.FlushToContent(content, 15, cpu_addr));

            // install hooks
            code = new Section();
            code.JSR(check_flag_trampoline);
            code.FlushToContent(content, 15, ROM.Game_RunDoorRequirementHandler);

            code = new Section();
            code.JSR(set_flag_trampoline);
            code.NOP(2);
            code.FlushToContent(content, 15, ROM.Game_UnlockDoorWithKey_afterUse);
        }

    }
}
