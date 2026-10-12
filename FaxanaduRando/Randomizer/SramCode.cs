using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    // code that runs from SRAM: stored in a backing ROM bank, copied to SRAM during game
    // initialization, and called by its hooks without bank switching. the same layout as
    // FaxEdit's SRAM-backed general hacks: the code area follows the savefile at $6806,
    // the copy routine lives in the backing bank, and bank 15 only holds a 9-byte trampoline
    public class SramCode
    {
        // the savefile header and checksum end at $6805
        public const ushort SramStart = 0x6806;
        public const int SramEnd = 0x8000;
        private const int BackingBank = 9;

        private readonly List<byte> code = new List<byte>();

        public int Cursor => SramStart + code.Count;

        // assembles the section to run at sramAddr, which may skip ahead of the cursor
        // for alignment, and returns sramAddr
        public ushort Add(Section section, int sramAddr)
        {
            if (sramAddr < Cursor)
                throw new RandomizationException($"SRAM code at ${sramAddr:X4} overlaps earlier code");

            var bytes = section.Assemble(sramAddr);

            if (sramAddr + bytes.Length > SramEnd)
                throw new RandomizationException("SRAM code area is full");

            while (Cursor < sramAddr)
                code.Add(0xff);

            code.AddRange(bytes);
            return (ushort)sramAddr;
        }

        // writes the backing code and its copy routine, and hooks the copy into game initialization.
        // installs nothing if no code was added
        public void Install(byte[] content, BankAddressAllocator alloc)
        {
            if (code.Count == 0)
                return;

            int romAddr = alloc.GetAddress(BackingBank);
            var section = new Section();

            foreach (var value in code)
                section.Db(value);

            int copyAddr = section.FlushToContent(content, BackingBank, romAddr);

            // copy whole 256-byte pages through zero-page pointers, then the remainder
            int blocks = code.Count / 0x100;
            int remainder = code.Count % 0x100;

            section.TYA();
            section.PHA();
            section.LDA_imm((byte)romAddr);
            section.STA_zp(RAM.ZP_Temp_00);
            section.LDA_imm((byte)(romAddr >> 8));
            section.STA_zp(RAM.ZP_Temp_01);
            section.LDA_imm((byte)(SramStart & 0xff));
            section.STA_zp(RAM.ZP_Temp_02);
            section.LDA_imm((byte)(SramStart >> 8));
            section.STA_zp(RAM.ZP_Temp_03);

            if (blocks != 0)
            {
                section.LDX_imm((byte)blocks);
                section.LDY_imm(0x00);
                section.Label("@copy_page");
                section.LDA_ind_y(RAM.ZP_Temp_00);
                section.STA_ind_y(RAM.ZP_Temp_02);
                section.INY();
                section.BNE("@copy_page");
                section.INC_abs(RAM.ZP_Temp_01);
                section.INC_abs(RAM.ZP_Temp_03);
                section.DEX();
                section.BNE("@copy_page");
            }

            if (remainder != 0)
            {
                section.LDY_imm(0x00);
                section.Label("@copy_remainder");
                section.LDA_ind_y(RAM.ZP_Temp_00);
                section.STA_ind_y(RAM.ZP_Temp_02);
                section.INY();
                section.CPY_imm((byte)remainder);
                section.BNE("@copy_remainder");
            }

            section.PLA();
            section.TAY();
            section.RTS();
            alloc.SetAddress(BackingBank, section.FlushToContent(content, BackingBank, copyAddr));

            // trampoline: far call the copy routine, then run the call it replaces
            ushort trampolineAddr = alloc.GetAddress(15);
            section.JSR(ROM.MMC1_LoadBankAndJump);
            section.Db(BackingBank);
            section.Dw((ushort)(copyAddr - 1));
            section.JMP(ROM.Game_InitScreenAndMusic);
            alloc.SetAddress(15, section.FlushToContent(content, 15, trampolineAddr));

            // runs once at reset, right after MMC1 and bank initialization
            section.JSR(trampolineAddr);
            section.FlushToContent(content, 15, ROM.Game_Init_JSR_Game_InitScreenAndMusic);
        }
    }
}
