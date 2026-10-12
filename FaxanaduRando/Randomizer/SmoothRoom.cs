using System;

namespace FaxanaduRando.Randomizer
{
    // less slowdown on crowded screens. a port of FaxEdit's
    // AtlasDevSpriteSpeed, AtlasDevPpuDrainUnroll and AtlasDevQueueLess.
    // monster behavior, collisions, clipping and sprite order are left alone.
    // the helpers run from SRAM and bank 15 keeps only the hooks. the QoL Edition's text
    // canvas clear is also included: a message box only clears the canvas rows
    // the last message inked, instead of all 64 tiles twice per pickup.
    public static class SmoothRoom
    {
        // the sprite draw bodies are rewritten in place, and the drain loop becomes a jump
        private const int SpriteDraw_BodySize = 65;
        private const int PPUDrain_CopyLoopSize = 10;

        // payload threshold per drain, 1..64. stock is 48; 64 lets a fourth tile through
        private const byte DrainBudget = 64;

        // entity $30 (nest) also loads a companion set, so it keeps its normal upload
        private const byte NestEntity = 0x30;

        // the message box text canvas: CHR $1400-$17FF, 64 tiles. the canvas state is only
        // trusted while the magic byte holds this value
        private const byte CanvasPage = 0x14;
        private const byte CanvasCells = 64;
        private const byte CanvasMagic = 0x5a;
        private const byte ZP_PPUAddrLo = 0xe8;
        private const byte ZP_PPUAddrHi = 0xe9;

        // the code each patch replaces or depends on, checked before anything is written.
        // ?? marks bytes other options may change: the sprite bank split, and the nest
        // companion id that the enemy randomizer sets
        private static readonly (ushort Address, string Hex)[] RequiredCode =
        {
            (0xcb47, "a900855a855bf004a9ff855aa0008433843484358438843784398425a55a301be625a8" +
                     "b996cb8d0007a97f8d0107a9238d0207b998cb8d0307a004a9f0990007c8c8c8c8d0f7" +
                     "a51c29804980851c60"),
            (0xf0f8, "b13ac9fff064a539d05fa642a53c7d3df28500a53e6900d050a643a53d7d3df28501a53f6900d041"),
            (0xf1ac, "b13ac9fff064a539d05fa642a53c7d3df28500a53e6900d050a643a53d7d3df28501a53f6900d041"),
            (0xf120, "9848a5250a0a451caaa5019d0007e8b13a1865339d0007e8c8b13a45298501a5422901" +
                     "a8a5263924f2f006a50109208501a5019d0007e8a5009d00072028f268a8"),
            (0xf1d4, "9848a5250a0a451caaa5019d0007e8b13a1865339d0007e8c8b13a45298501a5422901" +
                     "a8a5263926f2f006a50109208501a5019d0007e8a5009d00072028f268a8"),
            (0xf161, "c8c8e642c640108f688540e643c64130034cf1f068aa201acca9008533852660"),
            (0xf215, "c8c8c6421091e643c64110874c75f101020201a525492085252920d00ae625a525c9209002e63960"),
            (0xcc1a, "8e0001a90185128a8dffff4a8dffff4a8dffff4a8dffff4a8dffffa512c901f047"),
            (0xcc7f, "4c1dccc61260"),
            (0xcf5b, "a9d08522a9068521"),
            (0xcf63, "a61fe420f04aa50a29fba8bd00051006297fc8c8c8c88c0020bd0005297f" +
                     "a8e8bd00058d0620e8bd00058d0620e8981865228522"),
            (0xcf97, "bd0005e88d072088d0f6"),
            (0xcfa1, "861fc621f00cbc000588c0f9b004a52230b0a9008d06208d062060"),
            (0xc27c, "a000ad8b03c9??9001c8b959c28d860360"),
            (0xc28d, "ad780348206fcda2078e7803bdcc02c9fff03c8d8b03207cc220b5cdad8b03c930" +
                     "d019ad8b0348a59a48a9??8d8b03207cc220b5cd68859a688d8b03ae7803a59a9d2c" +
                     "03b5c2c9f09005a9ff9dcc02ae7803ca10b4688d78034cf4cf"),
            (0xcd6f, "a9098599a900859860"),
            (0xcd78, "ad000148ae8603201accad00808502ad01801869808503ad8b03c9??9002e9??0aa8" +
                     "b1028596c8b1021869808597ad8b03a8b91bce859b68aa201acc60"),
            (0xcfca, "20d0cf90fb60a51f38e520f002c9246009804820cacf68a6209d0005e8a5e99d0005e8a5e89d0005e860"),
            (0xf44a, "a91485e9a90085e8a0209848a900a020204af82026f868a888d0ef60"),
            (0xf826, "a9201865e89002e6e985e860"),
            (0xf845, "9d0005e860489820dccf682045f888d0fa862060"),
            (0xcdb5, "a5988500a59906002a06002a06002a06002a859a2078cda59bd00160ad000148ae8603" +
                     "201acca59985e9a59885e8a91020dccfa000b1969d0005e8c8c01090f5862068aa201acc" +
                     "a5961869108596a59769008597a5981869108598a59969008599c69bd0b760"),
        };

        public static void Install(byte[] content, SramCode sram)
        {
            foreach (var (address, hex) in RequiredCode)
                RequireCode(content, address, hex);

            // the sprite bank split's selector, comparison and subtraction must agree
            byte split = content[Section.GetOffset(15, ROM.RoomLoad_SpriteBankSplit)];
            if (content[Section.GetOffset(15, ROM.SpriteUpload_SpriteBankSplit)] != split ||
                content[Section.GetOffset(15, ROM.SpriteUpload_SpriteBankSubtract)] != split)
                throw new RandomizationException("Less slowdown: inconsistent sprite bank split");

            var section = new Section();

            // sprite draw bodies have no allocation
            AddDrawBody(section, false);
            section.FlushToContent(content, 15, ROM.SpriteDraw_Body);
            AddDrawBody(section, true);
            section.FlushToContent(content, 15, ROM.SpriteDrawFlipped_Body);

            var clear = new Section();
            AddClearHelper(clear);
            var upload = new Section();
            AddUploadHelper(upload);
            var drain = new Section();
            AddDrainHelper(drain);

            int[] addr;

            int start = sram.Cursor;
            addr = PlaceHelpers(start, clear.Size(), upload.Size(), drain.Size(), out int end);

            if (end > SramCode.SramEnd)
                throw new RandomizationException($"Less slowdown needs {end - start} bytes of SRAM, " +
                    $"but only {SramCode.SramEnd - start} are free");

            // add in address order, since the SRAM code area only grows
            var helpers = new[] { (addr[0], clear), (addr[1], upload), (addr[2], drain) };
            Array.Sort(helpers, (a, b) => a.Item1.CompareTo(b.Item1));

            foreach (var (address, helper) in helpers)
                sram.Add(helper, address);

            InstallTextCanvas(content, sram);

            section.JMP((ushort)addr[0]);
            section.FlushToContent(content, 15, ROM.SpriteClear_STA_0700_Y);

            section.JSR((ushort)addr[1]);
            section.FlushToContent(content, 15, ROM.RoomLoad_JSR_SpriteUpload);

            section.JMP((ushort)addr[2]);
            section.NOP(PPUDrain_CopyLoopSize - 3);
            section.FlushToContent(content, 15, ROM.PPUDrain_CopyLoop);
            content[Section.GetOffset(15, ROM.PPUDrain_Threshold)] = (byte)(0x100 - DrainBudget);
        }

        // the QoL Edition's high-water canvas clear (H20 canvas). a tracker on the ppu queue
        // header records the highest canvas cell written, the clear zeroes only those rows
        // and resets it. its three state bytes live in SRAM, where nothing else writes; the
        // copy at reset leaves the magic byte clear, so the first box clears all 64 cells
        private static void InstallTextCanvas(byte[] content, SramCode sram)
        {
            var state = new Section();
            state.Db(0x00); // highest inked cell
            state.Db(0x00); // magic
            state.Db(0x00); // scratch
            ushort high = sram.Add(state, sram.Cursor);
            ushort magic = (ushort)(high + 1);
            ushort scratch = (ushort)(high + 2);

            var section = new Section();

            // tracker: replaces PHA / JSR PPUBuffer_WaitForCapacity. a = record length,
            // $e8/$e9 = ppu address. y is preserved; x is scratch, vanilla reloads it at once
            section.Label("@tracker");
            section.PHA();
            section.JSR(ROM.PPUBuffer_WaitForCapacity);
            section.LDA_zp(ZP_PPUAddrHi);
            section.SEC();
            section.SBC_imm(CanvasPage);
            section.CMP_imm(0x04);
            section.BCS("@tracked");
            section.ASL(4);
            section.STA_abs(scratch);
            section.LDA_zp(ZP_PPUAddrLo);
            section.LSR(4);
            section.ORA_abs(scratch);
            section.STA_abs(scratch); // first cell written
            section.TSX();
            section.LDA_abs_x(0x0101); // the length pushed above
            section.CLC();
            section.ADC_imm(0x0f);
            section.LSR(4); // cells touched
            section.CLC();
            section.ADC_abs(scratch);
            section.CMP_abs(high);
            section.BCC("@tracked");
            section.BEQ("@tracked");
            section.STA_abs(high);
            section.Label("@tracked");
            section.SEC();
            section.JMP(ROM.PPUBuffer_QueueCommandOrLength_PLA);

            // rows: replaces LDY #$20 / TYA and enters the clear loop with y = rows to clear
            section.Label("@rows");
            section.LDA_abs(magic);
            section.CMP_imm(CanvasMagic);
            section.BEQ("@rows_known");
            section.LDA_imm(CanvasCells);
            section.STA_abs(high);
            section.Label("@rows_known");
            section.LDA_abs(high);
            section.CLC();
            section.ADC_imm(0x01);
            section.LSR_a(); // two cells per row
            section.CMP_imm(0x21);
            section.BCC("@rows_fit");
            section.LDA_imm(0x20); // never past $17ff
            section.Label("@rows_fit");
            section.TAY();
            section.BEQ("@rows_skip");
            section.TYA();
            section.JMP(ROM.TextBox_ClearTextTiles_Loop);
            section.Label("@rows_skip");
            section.JMP("@done");

            // reset: replaces the loop's DEY / BNE. its RTS stays, three branches share it
            section.Label("@reset");
            section.DEY();
            section.BEQ("@done");
            section.TYA();
            section.JMP(ROM.TextBox_ClearTextTiles_Loop);
            section.Label("@done");
            section.LDA_imm(0x00);
            section.STA_abs(high);
            section.LDA_imm(CanvasMagic);
            section.STA_abs(magic);
            section.RTS();

            int addr = sram.Cursor;
            ushort tracker = section.GetLabelAddress("@tracker", (ushort)addr);
            ushort rows = section.GetLabelAddress("@rows", (ushort)addr);
            ushort reset = section.GetLabelAddress("@reset", (ushort)addr);
            sram.Add(section, addr);

            section.JMP(tracker);
            section.NOP();
            section.FlushToContent(content, 15, ROM.PPUBuffer_QueueCommandOrLength);
            section.JMP(rows);
            section.FlushToContent(content, 15, ROM.TextBox_ClearTextTiles_LDY_20);
            section.JMP(reset);
            section.FlushToContent(content, 15, ROM.TextBox_ClearTextTiles_DEY);
        }

        // returns the clear, upload and drain helper addresses. the drain helper must sit on
        // one page so no taken branch pays a page-cross cycle; every order is tried and the
        // one that ends lowest wins, so padding is only used when no order avoids it
        private static int[] PlaceHelpers(int start, int clearSize, int uploadSize, int drainSize, out int end)
        {
            int[] sizes = { clearSize, uploadSize, drainSize };
            int[][] orders = { new[] { 2, 0, 1 }, new[] { 2, 1, 0 }, new[] { 0, 2, 1 },
                               new[] { 1, 2, 0 }, new[] { 0, 1, 2 }, new[] { 1, 0, 2 } };
            int[] best = null;
            end = int.MaxValue;

            foreach (var order in orders)
            {
                var addr = new int[3];
                int cur = start;

                foreach (int i in order)
                {
                    if (i == 2 && (cur & 0xff) + sizes[i] > 0x100)
                        cur = (cur + 0xff) & ~0xff;

                    addr[i] = cur;
                    cur += sizes[i];
                }

                if (cur < end)
                {
                    end = cur;
                    best = addr;
                }
            }

            return best;
        }

        private static void RequireCode(byte[] content, ushort address, string hex)
        {
            int offset = Section.GetOffset(15, address);

            for (int i = 0; i < hex.Length / 2; i++)
            {
                string value = hex.Substring(2 * i, 2);

                if (value != "??" && content[offset + i] != Convert.ToByte(value, 16))
                    throw new RandomizationException($"Less slowdown: incompatible code at ${address + i:X4}");
            }
        }

        // clears every sprite y with straight stores. the loop is entered with y=0, or y=4 to
        // keep sprite zero for the hud split; the indexed store then clears sprite zero or
        // repeats the sprite one store, which saves FaxEdit's five-byte branch
        private static void AddClearHelper(Section section)
        {
            section.LDA_imm(0xf0);
            section.STA_abs_y(RAM.SpriteOAM);

            for (ushort addr = 0x0704; addr < 0x0800; addr += 4)
                section.STA_abs(addr);

            section.LDY_imm(0x00);
            section.JMP(ROM.SpriteClear_Done);
        }

        // writes one oam entry without saving the tile reader index on the stack
        private static void AddDrawBody(Section section, bool flipped)
        {
            section.LDA_zp(0x25);
            section.ASL(2);
            section.EOR_zp(0x1c);
            section.TAX();
            section.LDA_zp(RAM.ZP_Temp_01);
            section.STA_abs_x(RAM.SpriteOAM);
            section.LDA_ind_y(0x3a);
            section.CLC();
            section.ADC_zp(0x33);
            section.STA_abs_x(RAM.SpriteOAM + 1);
            section.INY();
            section.LDA_ind_y(0x3a);
            section.EOR_zp(0x29);
            section.STA_zp(RAM.ZP_Temp_01);

            // select the same priority bit as the normal/flipped mask tables,
            // leaving y on the attribute byte throughout
            section.LDA_zp(0x42);
            section.LSR_a();
            section.LDA_zp(0x26);

            if (flipped)
                section.BCS("@selected");
            else
                section.BCC("@selected");

            section.LSR_a();
            section.Label("@selected");
            section.AND_imm(0x01);
            section.BEQ("@attribute");
            section.LDA_zp(RAM.ZP_Temp_01);
            section.ORA_imm(0x20);
            section.STA_zp(RAM.ZP_Temp_01);
            section.Label("@attribute");
            section.LDA_zp(RAM.ZP_Temp_01);
            section.STA_abs_x(RAM.SpriteOAM + 2);
            section.LDA_zp(RAM.ZP_Temp_00);
            section.STA_abs_x(RAM.SpriteOAM + 3);
            section.JSR(ROM.SpriteDraw_AdvanceOAM);
            section.INY();
            section.JMP(flipped ? ROM.SpriteDrawFlipped_NextTile : ROM.SpriteDraw_NextTile);

            if (section.Size() > SpriteDraw_BodySize)
                throw new RandomizationException("Less slowdown: sprite draw body overflow");

            section.NOP(SpriteDraw_BodySize - section.Size());
        }

        // reuses the tile base of a higher room slot that already loaded the same entity
        private static void AddUploadHelper(Section section)
        {
            section.LDA_abs(RAM.RoomSpriteEntity);
            section.CMP_imm(NestEntity);
            section.BEQ("@upload");
            section.LDX_abs(RAM.RoomSpriteIndex);

            // the room loader visits slots seven through zero. only higher slots
            // have valid tile bases, and empty slots have already been skipped
            section.Label("@scan");
            section.INX();
            section.CPX_imm(0x08);
            section.BCS("@upload");
            section.LDA_abs_x(RAM.SpritesOnScreen);
            section.CMP_abs(RAM.RoomSpriteEntity);
            section.BNE("@scan");
            section.LDA_abs_x(RAM.SpriteChrTileBase);
            section.STA_zp(0x9a);
            section.RTS();
            section.Label("@upload");
            section.JMP(ROM.SpriteUpload);
        }

        // copies full 16-byte tiles to the ppu with straight loads and stores.
        // short records and records crossing the queue boundary keep the byte loop
        private static void AddDrainHelper(Section section)
        {
            section.Label("@block");
            section.CPY_imm(0x10);
            section.BCC("@remainder");
            section.CPX_imm(0xf1);
            section.BCS("@remainder");

            for (ushort k = 0; k < 0x10; k++)
            {
                section.LDA_abs_x((ushort)(0x0500 + k));
                section.STA_abs(PPU.PPU_DATA);
            }

            section.TXA();
            section.CLC();
            section.ADC_imm(0x10);
            section.TAX();
            section.TYA();
            section.SEC();
            section.SBC_imm(0x10);
            section.TAY();
            section.BNE("@block");
            section.BEQ("@done");
            section.Label("@remainder");
            section.LDA_abs_x(0x0500);
            section.INX();
            section.STA_abs(PPU.PPU_DATA);
            section.DEY();
            section.BNE("@remainder");
            section.Label("@done");
            section.JMP(ROM.PPUDrain_Continue);
        }
    }
}
