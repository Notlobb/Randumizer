using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class PaletteRandomizer
    {
        public const byte DarkPalette = 4;
        public const byte BranchPalette = 8;
        public const byte FinalPalette = 15;

        public static readonly HashSet<byte> badPalettes = new HashSet<byte>
        {
            DarkPalette, 28,
        };

        public void RandomizePalettes(byte[] content, Random random)
        {
            var paletteSection = new Section();
            for (int i = 0; i < 16; i++)
            {
                paletteSection.Bytes.Add(GetRandomPalette(random));
            }
            paletteSection.AddToContent(content, Section.GetOffset(15, 0xDF4C, 0xC000));

            if (GeneralOptions.DarkTowers)
            {
                content[Section.GetOffset(15, 0xDF53, 0xC000)] = DarkPalette;
            }

            paletteSection = new Section();
            for (int i = 0; i < 10; i++)
            {
                paletteSection.Bytes.Add(GetRandomPalette(random));
            }
            paletteSection.AddToContent(content, Section.GetOffset(15, 0xE609, 0xC000));

            content[Section.GetOffset(15, 0xEA4A, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEA4E, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEAB0, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEAB5, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEABA, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEABF, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEAC4, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEACA, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEACF, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEAD4, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEAD9, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEADE, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEAE3, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEAE8, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEAED, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEAF2, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEAF7, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEAFC, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEB01, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEB07, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEB0C, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEB11, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEB16, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEB1C, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEB21, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEB26, 0xC000)] = GetRandomPalette(random);
            content[Section.GetOffset(15, 0xEB2C, 0xC000)] = GetRandomPalette(random);

            content[Section.GetOffset(15, 0xDD91, 0xC000)] = GetRandomPalette(random);

            content[Section.GetOffset(15, 0xDDEE, 0xC000)] = GetRandomPalette(random);
        }

        public void RandomizeMusic(byte[] content, Random random)
        {
            var paletteSection = new Section();
            for (int i = 0; i < 8; i++)
            {
                paletteSection.Bytes.Add(GetRandomMusic(random));
            }
            paletteSection.AddToContent(content, Section.GetOffset(15, 0xDF5C, 0xC000));

            paletteSection = new Section();
            for (int i = 0; i < 10; i++)
            {
                paletteSection.Bytes.Add(GetRandomMusic(random));
            }
            paletteSection.AddToContent(content, Section.GetOffset(15, 0xE5FF, 0xC000));

            paletteSection = new Section();
            for (int i = 0; i < 7; i++)
            {
                paletteSection.Bytes.Add(GetRandomMusic(random));
            }
            paletteSection.AddToContent(content, Section.GetOffset(15, 0xE570, 0xC000));

            content[Section.GetOffset(12, 0x9F3E, 0x8000)] = GetRandomMusic(random);
            content[Section.GetOffset(14, 0xA004, 0x8000)] = GetRandomMusic(random);
            content[Section.GetOffset(12, 0xA7A8, 0x8000)] = GetRandomMusic(random);
            content[Section.GetOffset(12, 0xA93E, 0x8000)] = GetRandomMusic(random);
            content[Section.GetOffset(15, 0xC5E7, 0xC000)] = GetRandomMusic(random);
            content[Section.GetOffset(15, 0xD989, 0xC000)] = GetRandomMusic(random);
            content[Section.GetOffset(15, 0xEFAC, 0xC000)] = GetRandomMusic(random);
            content[Section.GetOffset(15, 0xEFDA, 0xC000)] = GetRandomMusic(random);
            content[Section.GetOffset(15, 0xFC81, 0xC000)] = GetRandomMusic(random);

            content[Section.GetOffset(15, 0xDD9E, 0xC000)] = GetRandomMusic(random);

            content[Section.GetOffset(15, 0xDDE5, 0xC000)] = GetRandomMusic(random);
        }

        public static byte GetRandomPalette(Random random)
        {
            byte palette = (byte)random.Next(0, 32);
            if (badPalettes.Contains(palette))
            {
                palette = (byte)random.Next(0, 4);
            }

            return palette;
        }

        private byte GetRandomMusic(Random random)
        {
            if (ExtraOptions.MusicSetting == Music.None)
            {
                return 0;
            }

            return (byte)random.Next(1, 17);
        }
    }
}
