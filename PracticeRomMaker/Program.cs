using FaxanaduRando.Randomizer;

if (args.Length < 2)
{
    Console.WriteLine("Not enough arguments. Expected path to input file and path to output file.");
    return;
}

byte[] content = File.ReadAllBytes(args[0]);

/*
How we insert new code into the ROM:
Because we cannot just insert new code in without breaking addressing throughout the ROM, we need to find an instruction
 that has the same length as a JSR (jump to subroutine), and put a new JSR in to our new subroutine, which itself will 
 need to do whatever the instuction that was replaced does.
Our new subroutine needs to go somewhere in the ROM that has unused addresses.
Resource for opcodes: http://6502.org/tutorials/6502opcodes.html

Tips to avoid issues:

Make sure new subroutines have enough space to do what you need to do. It can be easy to overflow into another inserted
 subroutine and see hard-to-explain crashes.
The hex code is little-endian, so the value 0x45AA would be registered in the hex as 0xAA 0x45. Subroutine.addStatement
 makes this less of an issue for coding, but this is still useful to know when debugging.
*/

/*
HUD legend
-------
L: Level (0 through 7)
SS: Screen (2 digits)
HH: "Full" health value (0-80)
hhh: "Fractional" health value (0-255)
MM: Mana value (0-80)
FFF: Frame count (0-255)
00: Ointment timer value

Currently:

L SS HH hhh MM          FFF
M:           E: 00000 T:00
P:           G: 0000000 OO

Ideally in the future it will look something like this:

L:0-00 H:00.000 M:00  F:000
M:           E: 00000 W:00
H:           G: 00000 O:00
*/

content = substituteJSR(0xFCD0, 0xC8BF, 15, content);
content = substituteJSR(0xFE20, 0xE036, 15, content);
content = substituteJSR(0xFE20, 0xDB4D, 15, content);
content = substituteJSR(0xFD30, 0xC1EF, 15, content);
content = substituteJSR(0xFD90, 0xFA7E, 15, content);

/* 
OINTMENT TIME VALUE
-------------------
Known issues:
- This will start as blank, and appear when you get an ointment.
- When expired it will show "55" as the value gets set to 0xFF, value for 255
*/
var ointmentCounter = new Subroutine();

// decrement the counter as that's the command we substituted to get here
ointmentCounter.addStatement(OpCode.DECAbsolute, MemoryAddress.DurationOintment);
ointmentCounter.JSR(0xFD00);
// return ointment to A register
ointmentCounter.addStatement(OpCode.LDAAbsolute, MemoryAddress.DurationOintment);
ointmentCounter.RTS();
content = ointmentCounter.addToContent(0xFCD0, 15, content);

// **** NOTE Ointment not showing "42" on init any more just is blank until active, will read 55 when done (255, concatenated)
ointmentCounter = new Subroutine();

ointmentCounter.SetAsciiValue(MemoryAddress.DurationOintment);
ointmentCounter.PrepareAscii();
ointmentCounter.SetAsciiPosition(0x78);
ointmentCounter.SetAsciiLength(0x02);
ointmentCounter.ShowAscii();
ointmentCounter.RTS();

content = ointmentCounter.addToContent(0xFD00, 15, content);

/* 
FRAME COUNT VALUE
-----------------
Known and possible issues:
- It's unknown so far if just drawing this to the screen slows the game down enough to ruin the purpose for this (timing for pause exploit)
- This doesn't give good context yet as the values that decrement the timers are not intuitive, that's a @todo item!
- Doesn't yet count down when messages are being drawn to the screen
*/
var frameCounter = new Subroutine();

frameCounter.SetAsciiValue(MemoryAddress.InterruptCounter);
frameCounter.JSR(0xCA25);
frameCounter.PrepareAscii();
frameCounter.SetAsciiPosition(0x38);
frameCounter.SetAsciiLength(0x03);
frameCounter.ShowAscii();
frameCounter.RTS();
content = frameCounter.addToContent(0xFE20, 15, content);

/* 
SCREEN IDENTIFIER
-----------------
This shows the "level" (world) and "screen", starting at 0-00
*/
var screenID = new Subroutine();

// call the original subroutine, InitSprite
screenID.JSR(0xC205);
// call level subroutine
screenID.JSR(0xFD60);
screenID.SetAsciiValue(MemoryAddress.CurrentScreen);
screenID.PrepareAscii();
screenID.SetAsciiPosition(0x23);
screenID.SetAsciiLength(0x02);
screenID.ShowAscii();
screenID.RTS();
content = screenID.addToContent(0xFD30, 15, content);

// level subroutine
screenID = new Subroutine();
screenID.SetAsciiValue(MemoryAddress.CurrentLevel);
screenID.PrepareAscii();
screenID.SetAsciiPosition(0x21);
screenID.SetAsciiLength(0x01);
screenID.ShowAscii();
screenID.RTS();
content = screenID.addToContent(0xFD60, 15, content);

/* 
NUMERIC PLAYER STATS
--------------------
This shows the health and mana of Faxanadude.
Full Health, Partial Health, Mana
Partial Health has values 0 through 255
Note that 0 is not "dead" but below 0 is.
*/
var playerStats = new Subroutine();

playerStats.addStatement(OpCode.JSR, 0xFE60);
content = playerStats.addToContent(0xFA8B, 15, content);

// health values
playerStats = new Subroutine();

// replace replaced function, which stores the new Health_Full value in the correct memory address
playerStats.addStatement(OpCode.STAAbsolute, MemoryAddress.Health_Full);
// since A already contains the value for Health_Full, let's just store it now in byte_ec for tile drawing
playerStats.addStatement(OpCode.STAAbsolute, MemoryAddress.byte_ec);
// also stashing it for later restoration
playerStats.addStatement(OpCode.STAAbsolute, MemoryAddress.byte_eb);
playerStats.PrepareAscii();
playerStats.SetAsciiPosition(0x26);
playerStats.SetAsciiLength(0x02);
playerStats.ShowAscii();
// subroutine to draw partial health value
playerStats.JSR(0xFEA0);
// restore A register from MemoryAddress.byte_eb
playerStats.addStatement(OpCode.LDAAbsolute, MemoryAddress.byte_eb);
playerStats.RTS();
content = playerStats.addToContent(0xFD90, 15, content);

playerStats = new Subroutine();

playerStats.SetAsciiValue(MemoryAddress.Health_Frac);
playerStats.PrepareAscii();
playerStats.SetAsciiPosition(0x29);
playerStats.SetAsciiLength(0x03);
playerStats.ShowAscii();
playerStats.RTS();
content = playerStats.addToContent(0xFEA0, 15, content);

// mana value
playerStats = new Subroutine();

// replace replaced function, which stores the new ManaPoints value in the correct memory address
playerStats.addStatement(OpCode.STAAbsolute, MemoryAddress.ManaPoints);
// since A already contains the value for ManaPoints, let's just store it now in byte_ec for tile drawing
playerStats.addStatement(OpCode.STAAbsolute, MemoryAddress.byte_ec);
// also stashing it for later restoration
playerStats.addStatement(OpCode.STAAbsolute, MemoryAddress.byte_eb);
playerStats.PrepareAscii();
playerStats.SetAsciiPosition(0x2D);
// Y register has something important in it, let's stash it in MemoryAddress.byte_ea for now
playerStats.addStatement(OpCode.STYAbsolute, MemoryAddress.byte_ea);
playerStats.SetAsciiLength(0x02);
playerStats.ShowAscii();
// restore Y register from MemoryAddress.byte_ea
playerStats.addStatement(OpCode.LDYAbsolute, MemoryAddress.byte_ea);
// restore A register from MemoryAddress.byte_eb
playerStats.addStatement(OpCode.LDAAbsolute, MemoryAddress.byte_eb);
playerStats.RTS();
content = playerStats.addToContent(0xFE60, 15, content);

File.WriteAllBytes(args[1], content);

// code I've been using to search for the M: P: G: T: HUD UI
// I've been searching for suspect addresses and replacing them to see what happens
// So far no luck. Have been checking the Faxanadu Facelift ROM for any hints how they did what they did with HP/MP
/*
var uiSearch = new Section();
uiSearch.Bytes.Add(0x4E);

uiSearch.AddToContent(content, Section.GetOffset(15, 0xF8CE, 0xC000));
*/


static byte[] substituteJSR(ushort target, ushort destination, byte bank, byte[] content) {
    var jsrSubSection = new Section();
    var littleEndianValue = convertToLittleEndian(target);
    jsrSubSection.Bytes.Add(OpCode.JSR);
    jsrSubSection.Bytes.Add(littleEndianValue.Item1);
    jsrSubSection.Bytes.Add(littleEndianValue.Item2);
    jsrSubSection.AddToContent(content, Section.GetOffset(bank, destination, 0xC000));
    return content;
}

static Tuple<byte, byte> convertToLittleEndian(ushort value) {
    byte lowByte = (byte)(value & 0xFF);
    byte highByte = (byte)((value >> 8) & 0xFF);
    return new Tuple<byte, byte>(lowByte, highByte);
}

/* KNOWN SUBROUTINES */
/* Labelled the same as they are in fax dump*/

public static class SubroutineAddress {
    public static readonly ushort ShowAscii = 0xFA06;
    public static readonly ushort sub_F990 = 0xF990;
}

/* KNOWN ADDRESSES */
/* Labelled the same as they are in fax dump*/
public static class MemoryAddress {
    public static readonly ushort byte_e8 = 0x00E8;
    public static readonly ushort byte_e9 = 0x00E9;
    public static readonly ushort byte_ea = 0x00EA;
    public static readonly ushort byte_eb = 0x00EB;
    public static readonly ushort byte_ec = 0x00EC;
    public static readonly ushort byte_ed = 0x00ED;
    public static readonly ushort byte_ee = 0x00EE;
    public static readonly ushort byte_ef = 0x00EF;
    public static readonly ushort DurationOintment = 0x0427;
    public static readonly ushort InterruptCounter = 0x001A; // essentially a frame counter
    public static readonly ushort CurrentScreen = 0x0063;
    public static readonly ushort CurrentLevel = 0x0024;
    public static readonly ushort Health_Full = 0x0431; // full health
    public static readonly ushort Health_Frac = 0x0432; // fractional health
    public static readonly ushort ManaPoints = 0x039A;
}

public class Subroutine {
    public Section section;
    public Subroutine() {
        section = new Section();
    }

    public void addStatement(byte code, byte value) {
        var litteEndian = convertToLittleEndian(value);
        section.Bytes.Add(code);
        section.Bytes.Add(value);
    }

    public void addStatement(byte code, ushort value) {
        var litteEndian = convertToLittleEndian(value);
        section.Bytes.Add(code);
        section.Bytes.Add(litteEndian.Item1);
        section.Bytes.Add(litteEndian.Item2);
    }

    public void addStatement(byte code) {
        section.Bytes.Add(code);
    }

    public byte[] addToContent(ushort destination, byte bank, byte[] content) {
        section.AddToContent(content, Section.GetOffset(bank, destination, 0xC000));
        return content;
    }

    public void SetAsciiValue(ushort pointer) {
        this.addStatement(OpCode.LDAAbsolute, pointer);
        this.addStatement(OpCode.STAAbsolute, MemoryAddress.byte_ec);
    }

    // number of tiles (characters) used for this chunk of ASCII
    public void SetAsciiLength(byte length) {
        this.addStatement(OpCode.LDYImmediate, length);
    }

    public void PrepareAscii() {
        // this fixes and issue where message boxes don't render right
        this.addStatement(OpCode.LDAImmediate, 0x00);
        this.addStatement(OpCode.STAAbsolute, MemoryAddress.byte_ed);
        this.addStatement(OpCode.STAAbsolute, MemoryAddress.byte_ee);

        // byte_e9 is used in $CFE7, base index of tile in PPU (row)
        this.addStatement(OpCode.LDAImmediate, (byte)0x20);
        this.addStatement(OpCode.STAAbsolute, MemoryAddress.byte_e9);
    }

    // position of tile (characters) after base index
    public void SetAsciiPosition(byte position) {
        this.addStatement(OpCode.LDAImmediate, position);
        this.addStatement(OpCode.STAAbsolute, MemoryAddress.byte_e8);
    }

    // sugar
    public void ShowAscii() {
        this.addStatement(OpCode.JSR, SubroutineAddress.ShowAscii);
    }

    public void RTS() {
        this.addStatement(OpCode.RTS);
    }

    public void JSR(ushort pointer) {
        this.addStatement(OpCode.JSR, pointer);
    }

    private static Tuple<byte, byte> convertToLittleEndian(ushort value) {
        byte lowByte = (byte)(value & 0xFF);
        byte highByte = (byte)((value >> 8) & 0xFF);
        return new Tuple<byte, byte>(lowByte, highByte);
    }
}