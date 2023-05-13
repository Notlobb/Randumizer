using FaxanaduRando.Randomizer;

if (args.Length < 2)
{
    Console.WriteLine("Not enough arguments. Expected path to input file and path to output file.");
    return;
}

byte[] content = File.ReadAllBytes(args[0]);

//General problem: We want to add new code to the ROM, but just inserting new instructions would change the size of the ROM and the address of subsequent instructions.
//Instead, we replace an existing instruction with a jump to an empty part of the ROM, where we add the old, replaced instruction plus the new code.
//In this case, it is bank 15, address 0xC8BF that is replaced. If we check the dissassembled ROM, we can see that this is part of the CountdownOintment subroutine, and the instruction to decrease the ointment time is replaced.
//Another useful resource about 6502 opcodes is http://6502.org/tutorials/6502opcodes.html
//The jump is to bank 15, address 0xFCD0 which is in the beginning of the empty space at the end of the bank.
var jumpSection = new Section();
jumpSection.Bytes.Add(OpCode.JSR);
//The address is 0xFCD0, but will be added as 0xD0, 0xFC due to 6502 being little endian
jumpSection.Bytes.Add(0xD0);
jumpSection.Bytes.Add(0xFC);
jumpSection.AddToContent(content, Section.GetOffset(15, 0xC8BF, 0xC000));

var ointmentSection = new Section();
//Re-add the replaced instruction at the bew location
ointmentSection.Bytes.Add(OpCode.DECAbsolute);
ointmentSection.Bytes.Add(0x27);
ointmentSection.Bytes.Add(0x04);
//Add new code
//Load the ointment timer to accumulator
ointmentSection.Bytes.Add(OpCode.LDAAbsolute);
ointmentSection.Bytes.Add(0x27);
ointmentSection.Bytes.Add(0x04);
//Jump to new subroutine
//This is also to an empty part of the bank, but we will also add code there
ointmentSection.Bytes.Add(OpCode.JSR);
ointmentSection.Bytes.Add(0x00);
ointmentSection.Bytes.Add(0xFD);
//After the jump, load the ointment timer again as it was loaded before the first jump
ointmentSection.Bytes.Add(OpCode.LDAAbsolute);
ointmentSection.Bytes.Add(0x27);
ointmentSection.Bytes.Add(0x04);
ointmentSection.Bytes.Add(OpCode.RTS);
ointmentSection.AddToContent(content, Section.GetOffset(15, 0xFCD0, 0xC000));

//TODO document
ointmentSection = new Section();
ointmentSection.Bytes.Add(OpCode.STAAbsolute);
ointmentSection.Bytes.Add(0xEC);
ointmentSection.Bytes.Add(0x00);
ointmentSection.Bytes.Add(OpCode.LDAImmediate);
ointmentSection.Bytes.Add(0x00);
ointmentSection.Bytes.Add(OpCode.STAAbsolute);
ointmentSection.Bytes.Add(0xED);
ointmentSection.Bytes.Add(0x00);
ointmentSection.Bytes.Add(OpCode.STAAbsolute);
ointmentSection.Bytes.Add(0xEE);
ointmentSection.Bytes.Add(0x00);
ointmentSection.Bytes.Add(OpCode.LDAImmediate);
ointmentSection.Bytes.Add(0x20);
ointmentSection.Bytes.Add(OpCode.STAAbsolute);
ointmentSection.Bytes.Add(0xE9);
ointmentSection.Bytes.Add(0x00);
ointmentSection.Bytes.Add(OpCode.LDAImmediate);
ointmentSection.Bytes.Add(0x37);
ointmentSection.Bytes.Add(OpCode.STAAbsolute);
ointmentSection.Bytes.Add(0xE8);
ointmentSection.Bytes.Add(0x00);
ointmentSection.Bytes.Add(OpCode.LDYImmediate);
ointmentSection.Bytes.Add(0x02);
ointmentSection.Bytes.Add(OpCode.JSR);
ointmentSection.Bytes.Add(0x06);
ointmentSection.Bytes.Add(0xFA);
ointmentSection.Bytes.Add(OpCode.RTS);
ointmentSection.AddToContent(content, Section.GetOffset(15, 0xFD00, 0xC000));

ointmentSection = new Section();
ointmentSection.Bytes.Add(OpCode.JSR);
ointmentSection.Bytes.Add(0x00);
ointmentSection.Bytes.Add(0xFE);
ointmentSection.AddToContent(content, Section.GetOffset(15, 0xC05A, 0xC000));

ointmentSection = new Section();
ointmentSection.Bytes.Add(OpCode.JSR);
ointmentSection.Bytes.Add(0x90);
ointmentSection.Bytes.Add(0xF9);
ointmentSection.Bytes.Add(OpCode.JSR);
ointmentSection.Bytes.Add(0x00);
ointmentSection.Bytes.Add(0xFD);
ointmentSection.Bytes.Add(OpCode.RTS);
ointmentSection.AddToContent(content, Section.GetOffset(15, 0xFE00, 0xC000));

File.WriteAllBytes(args[1], content);