using System.Collections.Generic;
using System.Text;

namespace FaxanaduRando.Randomizer
{
    class Text
    {
        //These are just unused characters used to represent special characters
        public const char lineBreakChar = 'ä';
        public const char lineBreakWithPauseChar = 'Ä';
        public const char endOfTextChar = 'ö';
        public const char spaceChar = 'å';
        public const char secondSpaceChar = 'Å';

        private const int textOffset = 0x34310;
        private const int textOffsetEnd = 0x373C9;

        private static readonly Dictionary<byte, char> charDict = new Dictionary<byte, char>
        {
                {0x20, spaceChar},
                {0x21, '!'},
                {0x22, '\"'},
                {0x23, '#'},
                {0x24, '$'},
                {0x25, '%'},
                {0x26, '&'},
                {0x27, '\''},
                {0x28, '('},
                {0x29, ')'},
                {0x2A, '*'},
                {0x2B, '+'},
                {0x2C, ','},
                {0x2D, '-'},
                {0x2E, '.'},
                {0x2F, '/'},
                {0x30, '0'},
                {0x31, '1'},
                {0x32, '2'},
                {0x33, '3'},
                {0x34, '4'},
                {0x35, '5'},
                {0x36, '6'},
                {0x37, '7'},
                {0x38, '8'},
                {0x39, '9'},
                {0x3A, ':'},
                {0x3B, ';'},
                {0x3C, '<'},
                {0x3D, '='},
                {0x3E, '>'},
                {0x3F, '?'},
                {0x40, '@'},
                {0x41, 'A'},
                {0x42, 'B'},
                {0x43, 'C'},
                {0x44, 'D'},
                {0x45, 'E'},
                {0x46, 'F'},
                {0x47, 'G'},
                {0x48, 'H'},
                {0x49, 'I'},
                {0x4A, 'J'},
                {0x4B, 'K'},
                {0x4C, 'L'},
                {0x4D, 'M'},
                {0x4E, 'N'},
                {0x4F, 'O'},
                {0x50, 'P'},
                {0x51, 'Q'},
                {0x52, 'R'},
                {0x53, 'S'},
                {0x54, 'T'},
                {0x55, 'U'},
                {0x56, 'V'},
                {0x57, 'W'},
                {0x58, 'X'},
                {0x59, 'Y'},
                {0x5A, 'Z'},
                {0x5B, '['},
                {0x5C, '\\'},
                {0x5D, ']'},
                {0x5E, '^'},
                {0x5F, '_'},
                {0x60, '`'},
                {0x61, 'a'},
                {0x62, 'b'},
                {0x63, 'c'},
                {0x64, 'd'},
                {0x65, 'e'},
                {0x66, 'f'},
                {0x67, 'g'},
                {0x68, 'h'},
                {0x69, 'i'},
                {0x6A, 'j'},
                {0x6B, 'k'},
                {0x6C, 'l'},
                {0x6D, 'm'},
                {0x6E, 'n'},
                {0x6F, 'o'},
                {0x70, 'p'},
                {0x71, 'q'},
                {0x72, 'r'},
                {0x73, 's'},
                {0x74, 't'},
                {0x75, 'u'},
                {0x76, 'v'},
                {0x77, 'w'},
                {0x78, 'x'},
                {0x79, 'y'},
                {0x7A, 'z'},
                {0x7B, '{'},
                {0x7C, '|'},
                {0x7D, '}'},
                {0x7E, '~'},
                {0xFB, secondSpaceChar},
                {0xFC, lineBreakWithPauseChar},
                {0xFD, ' '},
                {0xFE, lineBreakChar},
                {0xFF, endOfTextChar},
        };

        private static Dictionary<char, byte> reverseCharDict = Util.Reverse(charDict);

        public static List<string> GetAllText(byte[] content)
        {
            var allText = new List<string>();
            int offset = textOffset;
            string text = "";

            while (offset <= textOffsetEnd)
            {
                char c;
                if (!charDict.TryGetValue(content[offset], out c))
                {
                    c = ' ';
                }
                text += c;
                if (content[offset] == 0xFF)
                {
                    allText.Add(text);
                    text = "";
                }
                offset++;
            }

            return allText;
        }

        public static void SetAllText(byte[] content, List<string> allText)
        {
            int offset = textOffset;
            foreach (string text in allText)
            {
                foreach (var c in text)
                {
                    content[offset] = reverseCharDict[c];
                    offset++;
                }
            }
        }

        public static List<string> GetAllTitleText(byte[] content, int offset, int end)
        {
            var allText = new List<string>();
            string text = "";

            while (offset <= end)
            {
                char c;
                if (!titleCharDict.TryGetValue(content[offset], out c))
                {
                    c = ' ';
                }

                text += c;
                if (content[offset] == 0x0)
                {
                    allText.Add(text);
                    text = "";
                }
                
                offset++;
            }

            return allText;
        }

        public static void SetAllTitleText(byte[] content, List<string> allText, int offset)
        {
            foreach (string text in allText)
            {
                foreach (var c in text)
                {
                    content[offset] = reverseTitleCharDict[c];
                    offset++;
                }
            }
        }

        public static void AddTitleText(int index, string text, List<string> allText)
        {
            int length = allText[index].Length;
            if (text.Length < length && index < allText.Count)
            {
                text = text.PadRight(length - 1, ' ');
                text = text.Insert(text.Length, "ö");
                allText[index] = text;
            }
        }

        public static List<string> GetTitles(byte[] content, int offset)
        {
            var titles = new List<string>();
            for (int i = 0; i < TextRandomizer.numberOfTitles; i++)
            {
                StringBuilder sb = new StringBuilder(TextRandomizer.titleLength);
                for (int j = 0; j < TextRandomizer.titleLength; j++)
                {
                    var c = content[offset + i * TextRandomizer.titleLength + j];
                    sb.Append(charDict[c]);
                }

                var title = sb.ToString();
                title = title.Replace(spaceChar, ' ');
                title = title.TrimEnd(' ');
                titles.Add(title);
            }

            return titles;
        }

        public static void SetTitles(List<string> titles, byte[] content)
        {
            var offset = Section.GetOffset(15, 0xF649, 0xC000);
            var upperCaseOffset = Section.GetOffset(12, 0x8943, 0x8000);
            int counter = 0;
            foreach (var title in titles)
            {
                var newTitle = title;
                if (title.Length > TextRandomizer.titleLength - 2)
                {
                    newTitle = title.Substring(0, TextRandomizer.titleLength - 2);
                }

                var upperCaseTitle = newTitle.ToUpper();
                newTitle = newTitle.PadRight(TextRandomizer.titleLength, spaceChar);
                upperCaseTitle = upperCaseTitle.PadRight(TextRandomizer.titleLength - 1, spaceChar);
                content[upperCaseOffset + counter] = 14; //Length

                for (int i = 0; i < newTitle.Length; i++)
                {
                    content[offset + counter] = reverseCharDict[newTitle[i]];
                    if (i < upperCaseTitle.Length)
                    {
                        content[upperCaseOffset + counter + 1] = reverseCharDict[upperCaseTitle[i]];
                    }

                    counter++;
                }
            }
        }

        private static readonly Dictionary<byte, char> titleCharDict = new Dictionary<byte, char>
        {
            { 0x00, 'ö' },
            { 0x20, ' ' },
            { 0xD6, '0' },
            { 0xD7, '1' },
            { 0xD8, '2' },
            { 0xD9, '3' },
            { 0xDA, '4' },
            { 0xDB, '5' },
            { 0xDC, '6' },
            { 0xDD, '7' },
            { 0xDE, '8' },
            { 0xDF, '9' },
            { 0xE0, 'A' },
            { 0xE1, 'B' },
            { 0xE2, 'C' },
            { 0xE3, 'D' },
            { 0xE4, 'E' },
            { 0xE5, 'F' },
            { 0xE6, 'G' },
            { 0xE7, 'H' },
            { 0xE8, 'I' },
            { 0xE9, 'J' },
            { 0xEA, 'K' },
            { 0xEB, 'L' },
            { 0xEC, 'M' },
            { 0xED, 'N' },
            { 0xEE, 'O' },
            { 0xEF, 'P' },
            { 0xF0, 'Q' },
            { 0xF1, 'R' },
            { 0xF2, 'S' },
            { 0xF3, 'T' },
            { 0xF4, 'U' },
            { 0xF5, 'V' },
            { 0xF6, 'W' },
            { 0xF7, 'X' },
            { 0xF8, 'Y' },
            { 0xF9, 'Z' },
            { 0xFA, 'c' },
        };

        private static Dictionary<char, byte> reverseTitleCharDict = Util.Reverse(titleCharDict);
    }
}