using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class Section
    {
        public List<byte> Bytes { get; set; } = new List<byte>();

        public void AddToContent(byte[] content, int offset)
        {
            for (int i = 0; i < Bytes.Count; i++)
            {
                content[offset + i] = Bytes[i];
            }
        }

        public static int GetOffset(int bank, int address, int start)
        {
            return (bank * 0x4000 + (address - start) + 16);
        }

        // PRG bank 15 maps at $C000; all other banks map at $8000
        public static int GetOffset(int bank, int address)
        {
            return GetOffset(bank, address, bank == 0x0f ? 0xC000 : 0x8000);
        }

        // 6502 assembly-aware API
        public int Size()
        {
            return Bytes.Count;
        }

        // resolves labels, applies the patch, clears internal state,
        // and returns the next available cpu address (and throws if it is > 1 past end of bank)
        public int FlushToContent(byte[] content, int bank, int cpuAddr)
        {
            ResolveLabels();

            int nextAddr = cpuAddr + Size();
            int bankEnd = (bank == 15 ? 0xFFFF : 0xBFFF);

            if (nextAddr > bankEnd + 1)
                throw new RandomizationException("Hack crosses bank {:bank} boundary");

            AddToContent(content, Section.GetOffset(bank, cpuAddr));

            Clear();
            return nextAddr;
        }

        // .db
        public void Db(byte value)
        {
            Bytes.Add(value);
        }

        // .dw
        public void Dw(ushort value)
        {
            Bytes.Add((byte)(value & 0xFF));
            Bytes.Add((byte)(value >> 8));
        }

        // 6502 instruction wrappers: mnemonic (uppercase), addressing mode (lowercase)
        // adding some aliases where it is natural to do so

        // jumps and calls
        public void JMP_abs(ushort addr)
        {
            Db(OpCode.JMPAbsolute);
            Dw(addr);
        }

        public void JMP(ushort addr)
        {
            JMP_abs(addr);
        }

        public void JMP_ind(ushort addr)
        {
            Db(OpCode.JMPIndirect);
            Dw(addr);
        }

        public void JSR(ushort addr)
        {
            Db(OpCode.JSR);
            Dw(addr);
        }

        public void RTS()
        {
            Db(OpCode.RTS);
        }

        // loads
        public void LDA_imm(byte value)
        {
            Db(OpCode.LDAImmediate);
            Db(value);
        }

        public void LDA_zp(byte address)
        {
            Db(OpCode.LDAZeroPage);
            Db(address);
        }

        public void LDA_abs(ushort address)
        {
            Db(OpCode.LDAAbsolute);
            Dw(address);
        }

        public void LDA_abs_x(ushort address)
        {
            Db(OpCode.LDAAbsoluteX);
            Dw(address);
        }

        public void LDA_ind_x(byte addr)
        {
            Db(OpCode.LDAIndirectX);
            Db(addr);
        }

        public void LDA_abs_y(ushort addr)
        {
            Db(OpCode.LDAAbsoluteY);
            Dw(addr);
        }

        public void LDA_ind_y(byte addr)
        {
            Db(OpCode.LDAIndirectY);
            Db(addr);
        }

        public void LDX_imm(byte value)
        {
            Db(OpCode.LDXImmediate);
            Db(value);
        }

        public void LDX_abs(ushort addr)
        {
            Db(OpCode.LDXAbsolute);
            Dw(addr);
        }

        public void LDY_imm(byte value)
        {
            Db(OpCode.LDYImmediate);
            Db(value);
        }

        public void LDY_abs(ushort addr)
        {
            Db(OpCode.LDYAbsolute);
            Dw(addr);
        }

        public void LDY_abs_x(ushort addr)
        {
            Db(OpCode.LDYAbsoluteX);
            Dw(addr);
        }

        // stores
        public void STA_zp(byte addr)
        {
            Db(OpCode.STAZeroPage);
            Db(addr);
        }

        public void STA_abs(ushort addr)
        {
            Db(OpCode.STAAbsolute);
            Dw(addr);
        }

        public void STA_abs_x(ushort addr)
        {
            Db(OpCode.STAAbsoluteX);
            Dw(addr);
        }

        public void STX_abs(ushort addr)
        {
            Db(OpCode.STXAbsolute);
            Dw(addr);
        }

        public void STY_abs(ushort addr)
        {
            Db(OpCode.STYAbsolute);
            Dw(addr);
        }

        // compares
        public void CMP_imm(byte value)
        {
            Db(OpCode.CMPImmediate);
            Db(value);
        }

        public void CMP_abs(ushort addr)
        {
            Db(OpCode.CMPAbsolute);
            Dw(addr);
        }

        public void CMP_abs_x(ushort addr)
        {
            Db(OpCode.CMPAbsoluteX);
            Dw(addr);
        }

        public void CPY_imm(byte value)
        {
            Db(OpCode.CPY);
            Db(value);
        }

        // branches
        public void BCC(sbyte offset)
        {
            Db(OpCode.BCC);
            Db((byte)offset);
        }

        public void BCS(sbyte offset)
        {
            Db(OpCode.BCS);
            Db((byte)offset);
        }

        public void BEQ(sbyte offset)
        {
            Db(OpCode.BEQ);
            Db((byte)offset);
        }

        public void BNE(sbyte offset)
        {
            Db(OpCode.BNE);
            Db((byte)offset);
        }

        public void BPL(sbyte offset)
        {
            Db(OpCode.BPL);
            Db((byte)offset);
        }

        public void BMI(sbyte offset)
        {
            Db(OpCode.BMI);
            Db((byte)offset);
        }

        // branches - label overloads
        public void BCC(string label)
        {
            Branch(OpCode.BCC, label);
        }

        public void BCS(string label)
        {
            Branch(OpCode.BCS, label);
        }

        public void BEQ(string label)
        {
            Branch(OpCode.BEQ, label);
        }

        public void BNE(string label)
        {
            Branch(OpCode.BNE, label);
        }

        public void BPL(string label)
        {
            Branch(OpCode.BPL, label);
        }

        public void BMI(string label)
        {
            Branch(OpCode.BMI, label);
        }

        // logic
        public void AND_imm(byte value)
        {
            Db(OpCode.ANDImmediate);
            Db(value);
        }

        public void ORA_imm(byte value)
        {
            Db(OpCode.ORAImmediate);
            Db(value);
        }

        // registers
        public void TAY()
        {
            Db(OpCode.TAY);
        }

        public void TYA()
        {
            Db(OpCode.TYA);
        }

        public void TAX()
        {
            Db(OpCode.TAX);
        }

        public void TXA()
        {
            Db(OpCode.TXA);
        }

        public void TSX()
        {
            Db(OpCode.TSX);
        }

        public void TXS()
        {
            Db(OpCode.TXS);
        }

        public void INX()
        {
            Db(OpCode.INX);
        }

        public void DEX()
        {
            Db(OpCode.DEX);
        }

        public void INY()
        {
            Db(OpCode.INY);
        }

        public void DEY()
        {
            Db(OpCode.DEY);
        }

        // shifts
        public void LSR_a()
        {
            Db(OpCode.LSRA);
        }

        public void ASL_a()
        {
            Db(OpCode.ASLA);
        }

        public void LSR(int count = 1)
        {
            for (int i = 0; i < count; ++i)
                LSR_a();
        }

        public void ASL(int count = 1)
        {
            for (int i = 0; i < count; ++i)
                ASL_a();
        }

        // math
        public void INC_abs(ushort addr)
        {
            Db(OpCode.INCAbsolute);
            Dw(addr);
        }

        public void DEC_abs(ushort addr)
        {
            Db(OpCode.DECAbsolute);
            Dw(addr);
        }

        // flags
        public void CLC()
        {
            Db(OpCode.CLC);
        }

        public void SEC()
        {
            Db(OpCode.SEC);
        }

        public void EOR_imm(byte value)
        {
            Db(OpCode.EORImmediate);
            Db(value);
        }

        // stack
        public void PHA()
        {
            Db(OpCode.PHA);
        }

        public void PLA()
        {
            Db(OpCode.PLA);
        }

        // misc
        public void NOP(int count = 1)
        {
            for (int i = 0; i < count; ++i)
                Db(OpCode.NOP);
        }

        // defines a label at the current relative location - no byte output
        public void Label(string label)
        {
            if (_labels.ContainsKey(label))
                throw new InvalidOperationException($"Duplicate label: {label}");

            _labels[label] = Bytes.Count;
        }

        // internal implementation of relative branch label resolution logic follows
        private void Clear()
        {
            Bytes.Clear();
            _labels.Clear();
            _branchRefs.Clear();
        }

        // maps label names to byte offsets within this section
        private readonly Dictionary<string, int> _labels = new Dictionary<string, int>();
        // records unresolved branches to be patched during label resolution
        private readonly List<BranchRef> _branchRefs = new List<BranchRef>();

        private struct BranchRef
        {
            public int Offset;
            public string Label;
        }

        private void Branch(byte opcode, string label)
        {
            Db(opcode);

            _branchRefs.Add(new BranchRef
            {
                Offset = Bytes.Count,
                Label = label
            });

            Db(0); // placeholder, patched later during label resolution
        }

        // resolves label-based branches by patching placeholder branch offsets
        private void ResolveLabels()
        {
            foreach (var branch in _branchRefs)
            {
                if (!_labels.TryGetValue(branch.Label, out int target))
                    throw new InvalidOperationException($"Undefined label: {branch.Label}");

                int next = branch.Offset + 1;
                int delta = target - next;

                if (delta < sbyte.MinValue || delta > sbyte.MaxValue)
                    throw new InvalidOperationException($"Branch out of range: {branch.Label}");

                Bytes[branch.Offset] = (byte)(sbyte)delta;
            }
        }

    }
}
