namespace FaxanaduRando.Randomizer
{
    public class OpCode
    {
        public const int ORAImmediate = 0x09;
        public const int ASLA = 0x0A;
        public const int BPL = 0x10;
        public const int CLC = 0x18;
        public const int JSR = 0x20;
        public const int ANDImmediate = 0x29;
        public const int SEC = 0x38;
        public const int PHA = 0x48;
        public const int LSRA = 0x4A;
        public const int RTS = 0x60;
        public const int PLA = 0x68;
        public const int STAZeroPage = 0x85;
        public const int DEY = 0x88;
        public const int TXA = 0x8A;
        public const int STYAbsolute = 0x8C;
        public const int STAAbsolute = 0x8D;
        public const int STXAbsolute = 0x8E;
        public const int LSR = 0x4A;
        public const int JMPAbsolute = 0x4C;
        public const int BCC = 0x90;
        public const int TYA = 0x98;
        public const int TXS = 0x9A;
        public const int STAAbsoluteX = 0x9D;
        public const int LDAIndirectX = 0xA1;
        public const int LDXImmediate = 0xA2;
        public const int TAY = 0xA8;
        public const int LDAZeroPage = 0xA5;
        public const int LDAImmediate = 0xA9;
        public const int TAX = 0xAA;
        public const int LDYImmediate = 0xA0;
        public const int LDYAbsolute = 0xAC;
        public const int LDAAbsolute = 0xAD;
        public const int LDXAbsolute = 0xAE;
        public const int TSX = 0xBA;
        public const int LDYAbsoluteX = 0xBC;
        public const int BCS = 0xB0;
        public const int LDAIndirectY = 0xB1;
        public const int LDAAbsoluteY = 0xB9;
        public const int LDAAbsoluteX = 0xBD;
        public const int CPY = 0xC0;
        public const int INY = 0xC8;
        public const int CMPImmediate = 0xC9;
        public const int CMPAbsolute = 0xCD;
        public const int DECAbsolute = 0xCE;
        public const int CMPAbsoluteX = 0xDD;
        public const int INX = 0xE8;
        public const int NOP = 0xEA;
        public const int INCAbsolute = 0xEE;
        public const int BNE = 0xD0;
        public const int BEQ = 0xF0;
    }
}
