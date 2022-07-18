using System;

namespace FaxanaduRando
{
    public class SoundRandomizer
    {
        public void RandomizeSounds(byte[] content, Random random)
        {
            content[Section.GetOffset(12, 0x857B, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(12, 0x85AA, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(12, 0x8AB2, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(12, 0x8BB8, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(12, 0x8BCF, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(12, 0x8BE6, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(12, 0x9071, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(12, 0x9343, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(12, 0x934B, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(12, 0x9418, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(12, 0x9F53, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(12, 0xA8D4, 0x8000)] = GetRandomSound(random);

            content[Section.GetOffset(14, 0x81B7, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0x8219, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0x87D7, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0x884A, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0x88AA, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0x89C8, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0x8B9B, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0x8BC1, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0x9D26, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0x9EB7, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0xAB45, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0xABA1, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0xAC76, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0xBA86, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0xBA8E, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0xBB5C, 0x8000)] = GetRandomSound(random);
            content[Section.GetOffset(14, 0xBBB3, 0x8000)] = GetRandomSound(random);

            content[Section.GetOffset(15, 0xC4DB, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC507, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC53F, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC548, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC585, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC5D4, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC650, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC6C7, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC6E1, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC6F3, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC713, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC733, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC75B, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC7D8, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC7ED, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC803, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC819, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC82F, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC845, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC86D, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xC883, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xD790, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xD93D, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xE99F, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xEBE9, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xEFDF, 0xC000)] = GetRandomSound(random);
            content[Section.GetOffset(15, 0xF4FA, 0xC000)] = GetRandomSound(random);
        }

        private byte GetRandomSound(Random random)
        {
            return (byte)random.Next(1, 29);
        }
    }
}
