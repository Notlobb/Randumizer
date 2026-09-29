using System;

namespace FaxanaduRando.Randomizer
{
    public static class SramSave
    {
        // Save file header, used for validation along with a 16-bit additive checksum.
        private static readonly byte[] Header = { 0x45, 0x6F, 0x45, 0x00 };

        // SRAM zero-page scratch variables.
        private const byte SramValid = 0xe5;

        private const byte ChecksumLo = 0x2c;
        private const byte ChecksumHi = 0x2d;

        private const byte RangesLeft = 0x30;
        private const byte SourceHiOffset = 0x31;
        private const byte TargetHiOffset = 0x32;

        private const byte SourcePtrLo = 0xe2;
        private const byte SourcePtrHi = 0xe3;
        private const byte TargetPtrLo = 0xe4;
        private const byte TargetPtrHi = 0xe5;
        private const byte TablePtrLo = 0xe6;
        private const byte TablePtrHi = 0xe7;

        private static int InstallRangesTable(byte[] content, int cpuAddr)
        {
            var ranges = new (ushort Address, byte Count)[]
            {
                (RAM.InventoryWeapons, 42), // all equips and inventories
                (RAM.SpecialItems, 2), // special items and normal quest flags
                (RAM.PlayerTitle, 1),
                (RAM.SpawnPoint, 1),
                // (0x04C2, 4), // name in the JP version if ever needed
                (RAM.ExtendedFlags, 31), // 31 bytes of free ram
                (RAM.XPLowByte, 5), // xp and gold which CAN be restored from save
            };

            Array.Sort(ranges, (a, b) => a.Address.CompareTo(b.Address));

            var section = new Section();

            // Data table: range count followed by <address, byte count> pairs.
            section.Db((byte)ranges.Length);

            foreach (var range in ranges)
            {
                section.Dw(range.Address);
                section.Db(range.Count);
            }

            return section.FlushToContent(content, 12, cpuAddr);
        }

        private static int InstallCopyRange(byte[] content, int cpuAddr)
        {
            var section = new Section();

            // Source address base in SourcePtrLo/SourcePtrHi.
            // Target address base in TargetPtrLo/TargetPtrHi.
            // Byte count is in X.
            section.LDY_imm(0x00);

            section.Label("copy_range_loop");
            section.LDA_ind_y(SourcePtrLo);
            section.STA_ind_y(TargetPtrLo);
            section.INY();
            section.DEX();
            section.BNE("copy_range_loop");
            section.RTS();

            return section.FlushToContent(content, 12, cpuAddr);
        }

        private static int InstallCopyRanges(byte[] content, int cpuAddr, ushort copyRangeAddr, ushort rangeTableAddr)
        {
            var section = new Section();

            // Table pointer.
            section.LDA_imm((byte)(rangeTableAddr & 0xFF));
            section.STA_zp(TablePtrLo);
            section.LDA_imm((byte)(rangeTableAddr >> 8));
            section.STA_zp(TablePtrHi);

            section.LDY_imm(0x00);
            section.LDA_ind_y(TablePtrLo);
            section.STA_zp(RangesLeft);

            section.Label("copy_ranges_loop");

            // Low byte is identical for RAM and SRAM.
            section.INY();
            section.LDA_ind_y(TablePtrLo);
            section.STA_zp(SourcePtrLo);
            section.STA_zp(TargetPtrLo);

            // Source/target high bytes.
            section.INY();
            section.LDA_ind_y(TablePtrLo);
            section.CLC();
            section.ADC_zp(SourceHiOffset);
            section.STA_zp(SourcePtrHi);

            section.LDA_ind_y(TablePtrLo);
            section.CLC();
            section.ADC_zp(TargetHiOffset);
            section.STA_zp(TargetPtrHi);

            // Length.
            section.INY();
            section.LDA_ind_y(TablePtrLo);
            section.TAX();

            // Preserve table Y.
            section.TYA();
            section.PHA();
            section.JSR(copyRangeAddr);
            section.PLA();
            section.TAY();

            section.DEC_zp(RangesLeft);
            section.BNE("copy_ranges_loop");
            section.RTS();

            return section.FlushToContent(content, 12, cpuAddr);
        }

        private static int InstallCalcChecksum(byte[] content, int cpuAddr, int rangeTableAddr)
        {
            var section = new Section();

            // Table pointer.
            section.LDA_imm((byte)(rangeTableAddr & 0xFF));
            section.STA_zp(TablePtrLo);
            section.LDA_imm((byte)(rangeTableAddr >> 8));
            section.STA_zp(TablePtrHi);

            // Clear checksum.
            section.LDA_imm(0x00);
            section.STA_zp(ChecksumLo);
            section.STA_zp(ChecksumHi);

            // Get range count.
            section.LDY_imm(0x00);
            section.LDA_ind_y(TablePtrLo);
            section.STA_zp(RangesLeft);

            section.Label("checksum_ranges_loop");

            // SRAM pointer = RAM address + $6000.
            section.INY();
            section.LDA_ind_y(TablePtrLo);
            section.STA_zp(SourcePtrLo);

            section.INY();
            section.LDA_ind_y(TablePtrLo);
            section.CLC();
            section.ADC_imm(0x60);
            section.STA_zp(SourcePtrHi);

            // Byte count.
            section.INY();
            section.LDA_ind_y(TablePtrLo);
            section.TAX();

            // Preserve table position.
            section.TYA();
            section.PHA();

            section.LDY_imm(0x00);

            section.Label("checksum_range_loop");

            section.LDA_zp(ChecksumLo);
            section.CLC();
            section.ADC_ind_y(SourcePtrLo);
            section.STA_zp(ChecksumLo);

            section.LDA_zp(ChecksumHi);
            section.ADC_imm(0x00);
            section.STA_zp(ChecksumHi);

            section.INY();
            section.DEX();
            section.BNE("checksum_range_loop");

            // Restore table position.
            section.PLA();
            section.TAY();

            section.DEC_zp(RangesLeft);
            section.BNE("checksum_ranges_loop");
            section.RTS();

            return section.FlushToContent(content, 12, cpuAddr);
        }

        private static int InstallValidate(byte[] content, int cpuAddr, ushort calcChecksumAddr)
        {
            var section = new Section();

            // Check header.
            section.LDA_abs(0x6800);
            section.CMP_imm(Header[0]);
            section.BNE("invalid");

            section.LDA_abs(0x6801);
            section.CMP_imm(Header[1]);
            section.BNE("invalid");

            section.LDA_abs(0x6802);
            section.CMP_imm(Header[2]);
            section.BNE("invalid");

            section.LDA_abs(0x6803);
            // CMP #Header[3] can be omitted while the version number is 0.
            section.BNE("invalid");

            // Calculate and compare checksum.
            section.JSR(calcChecksumAddr);

            section.LDA_zp(ChecksumLo);
            section.CMP_abs(0x6804);
            section.BNE("invalid");

            section.LDA_zp(ChecksumHi);
            section.CMP_abs(0x6805);
            section.RTS(); // Z reflects final comparison.

            section.Label("invalid");
            section.LDA_imm(0x01); // Z clear.
            section.RTS();

            return section.FlushToContent(content, 12, cpuAddr);
        }

        private static int InstallSave(byte[] content, int cpuAddr, ushort copyRangesAddr, ushort calcChecksumAddr)
        {
            var section = new Section();

            section.JSR("save_ranges");
            section.JSR(calcChecksumAddr);

            // Store checksum.
            section.LDA_zp(ChecksumLo);
            section.STA_abs(0x6804);
            section.LDA_zp(ChecksumHi);
            section.STA_abs(0x6805);

            // Write header last.
            section.LDA_imm(Header[0]);
            section.STA_abs(0x6800);
            section.LDA_imm(Header[1]);
            section.STA_abs(0x6801);
            section.LDA_imm(Header[2]);
            section.STA_abs(0x6802);
            section.LDA_imm(Header[3]);
            section.STA_abs(0x6803);
            section.RTS();

            // Internal helper: save RAM -> SRAM ranges.
            section.Label("save_ranges");
            section.LDA_imm(0x00);
            section.STA_zp(SourceHiOffset);
            section.LDA_imm(0x60);
            section.STA_zp(TargetHiOffset);
            section.JMP(copyRangesAddr);

            return section.FlushToContent(content, 12, cpuAddr);
        }

        private static int InstallLoad(byte[] content, int cpuAddr, ushort copyRangesAddr)
        {
            var section = new Section();

            // Load: SRAM -> RAM.
            section.LDA_imm(0x60);
            section.STA_zp(SourceHiOffset);
            section.LDA_imm(0x00);
            section.STA_zp(TargetHiOffset);
            section.JMP(copyRangesAddr);

            return section.FlushToContent(content, 12, cpuAddr);
        }

        private static void InstallShowMantra(byte[] content, ushort saveAddr, byte? saveStringId)
        {
            var section = new Section();

            section.JSR(saveAddr);

            if (saveStringId.HasValue)
            {
                section.LDA_imm(saveStringId.Value);
                section.JMP(ROM.ScriptActionShowMessage_JSR_MsgLoad);
            }
            else
                section.JMP(ROM.Scripts_InvokeNextAction);

            section.FlushToContent(content, 12, ROM.ScriptActionShowMantra);
        }

        private static void InstallChooseContinue(byte[] content, ushort loadAddr)
        {
            var section = new Section();
            // Update the jump target of the routine called when the player chooses CONTINUE.
            section.Dw((ushort)(loadAddr - 1));
            section.FlushToContent(content, 15, ROM.ChooseContinue_TargetAddr);
        }

        private static int InstallValidateOnStartup(byte[] content, ushort cpuAddr, ushort validateAddr)
        {
            var section = new Section();

            // Install hook.
            section.JSR(cpuAddr);
            section.FlushToContent(content, 12, ROM.StartScreen_Draw);

            // New routine: returns 1 in SramValid if a valid save file exists, otherwise 0.
            section = new Section();

            section.JSR(validateAddr);
            section.BEQ("valid");

            section.LDA_imm(0x00);
            section.BEQ("return");

            section.Label("valid");
            section.LDA_imm(0x01);

            section.Label("return");
            section.STA_zp(SramValid);
            section.JMP(ROM.PPU_WaitUntilFlushed);

            return section.FlushToContent(content, 12, cpuAddr);
        }

        private static int InstallDisableContinueOnInvalidSave(byte[] content, ushort cpuAddr)
        {
            var section = new Section();

            // Hook: verify SRAM before going through with selection.
            section.JSR(cpuAddr);
            section.NOP(10);
            section.FlushToContent(content, 12, ROM.StartScreen_InputSelect);

            // Set default selection to CONTINUE if a valid save exists.
            section = new Section();
            section.LDA_zp(SramValid);
            section.FlushToContent(content, 12, ROM.StartScreen_DefaultSelection);

            // New routine: allow selection switch if SRAM save is valid.
            section = new Section();

            section.LDA_zp(SramValid);
            section.BNE("valid");

            section.LDA_imm(0x0D);
            section.JSR(ROM.Sound_PlayEffect);
            section.RTS();

            section.Label("valid");
            section.LDA_abs(RAM.StartScreenSelection);
            section.EOR_imm(0x01);
            section.STA_abs(RAM.StartScreenSelection);

            section.LDA_imm(0x0B);
            section.JSR(ROM.Sound_PlayEffect);
            section.RTS();

            return section.FlushToContent(content, 12, cpuAddr);
        }

        private static int InstallWriteStartScreenAttributes(byte[] content, ushort cpuAddr)
        {
            const ushort ppuStart = 0x23EC;
            const byte attrValid = 0x50;
            const byte attrInvalid = 0x5F;

            var section = new Section();

            // Hook.
            section.JSR(cpuAddr);
            section.FlushToContent(content, 12, ROM.StartScreen_Draw_JSR_PPU_WriteTilesFromCHRRAM);

            // New routine.
            section = new Section();

            // Call overwritten at hook site.
            section.JSR(ROM.PPU_WriteTilesFromCHRRAM);

            // Set PPU address.
            section.LDA_imm((byte)(ppuStart >> 8));
            section.STA_abs(PPU.PPU_ADDR);
            section.LDA_imm((byte)(ppuStart & 0xFF));
            section.STA_abs(PPU.PPU_ADDR);

            // Write attributes according to save validity.
            section.LDA_zp(SramValid);
            section.BNE("valid");

            // gray CONTINUE
            section.LDA_imm(attrInvalid);
            section.STA_abs(PPU.PPU_DATA);
            section.STA_abs(PPU.PPU_DATA);
            section.RTS();

            // yellow-ish CONTINUE
            section.Label("valid");
            section.LDA_imm(attrValid);
            section.STA_abs(PPU.PPU_DATA);
            section.STA_abs(PPU.PPU_DATA);
            section.RTS();

            return section.FlushToContent(content, 12, cpuAddr);
        }

        private static void InstallNoGoldXpReload(byte[] content, ushort cpuAddr)
        {
            var section = new Section();

            // Replace the six-byte far call that initializes gold and XP from title
            // with the only invariant we need to preserve. Perfect byte fit.
            section.LDA_abs(RAM.PlayerTitle);
            section.STA_abs(RAM.PendingTitle);

            section.FlushToContent(content, 15, cpuAddr);
        }

        private static void EnableSram(byte[] content)
        {
            const byte batteryBackedPrgRam = 0x02;
            const byte nes2HeaderMask = 0x0C;
            const byte nes2HeaderValue = 0x08;
            const byte prgRamMask = 0x0F;
            const byte prgNvram8K = 0x70;

            bool isNes2 = (content[7] & nes2HeaderMask) == nes2HeaderValue;

            // Mark PRG-RAM as battery-backed.
            content[6] |= batteryBackedPrgRam;

            // NES 2.0 additionally specifies the PRG-NVRAM size.
            if (isNes2)
                content[10] = (byte)((content[10] & prgRamMask) | prgNvram8K);
        }

        public static void Install(byte[] content, bool keepGoldXpOnSramLoad,
            bool keepGoldXpOnDeath, bool colorText, byte? saveStringId)
        {
            if (keepGoldXpOnSramLoad)
                InstallNoGoldXpReload(content, ROM.Continue_InitXpGold);

            if (keepGoldXpOnDeath)
                InstallNoGoldXpReload(content, ROM.Player_HandleDeath_InitGoldXP);

            // dynamic block - overwrite mantra decode
            int cpuAddr = ROM.MantraLoad;

            int tableAddr = cpuAddr;
            cpuAddr = InstallRangesTable(content, cpuAddr);

            int copyRangeAddr = cpuAddr;
            cpuAddr = InstallCopyRange(content, cpuAddr);

            int copyRangesAddr = cpuAddr;
            cpuAddr = InstallCopyRanges(content, cpuAddr, (ushort)copyRangeAddr, (ushort)tableAddr);

            int calcChecksumAddr = cpuAddr;
            cpuAddr = InstallCalcChecksum(content, cpuAddr, tableAddr);

            int validateAddr = cpuAddr;
            cpuAddr = InstallValidate(content, cpuAddr, (ushort)calcChecksumAddr);

            int saveAddr = cpuAddr;
            cpuAddr = InstallSave(content, cpuAddr, (ushort)copyRangesAddr, (ushort)calcChecksumAddr);

            int loadAddr = cpuAddr;
            cpuAddr = InstallLoad(content, cpuAddr, (ushort)copyRangesAddr);

            int startupValidateAddr = cpuAddr;
            cpuAddr = InstallValidateOnStartup(content, (ushort)startupValidateAddr, (ushort)validateAddr);

            int startScreenSelectGuardAddr = cpuAddr;
            cpuAddr = InstallDisableContinueOnInvalidSave(content, (ushort)startScreenSelectGuardAddr);

            if (colorText)
            {
                int startScreenAttributeAddr = cpuAddr;
                cpuAddr = InstallWriteStartScreenAttributes(content, (ushort)startScreenAttributeAddr);
            }

            InstallShowMantra(content, (ushort)saveAddr, saveStringId);
            InstallChooseContinue(content, (ushort)loadAddr);

            // Enable battery-backed SRAM in the ROM header.
            EnableSram(content);
        }
    }
}
