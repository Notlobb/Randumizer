using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace FaxanaduRando.Randomizer
{
    class FlagConverter : IMultiValueConverter
    {
        private const int boolCount = 33;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var bytes = new List<byte>();
            for (int i = 0; i < boolCount; i += 4)
            {
                int encodedValue = 0;
                for (int j = 0; j < 4; j++)
                {
                    if (i + j >= boolCount)
                    {
                        break;
                    }

                    int value;
                    if ((bool)values[i + j])
                    {
                        value = 1;
                    }
                    else
                    {
                        value = 0;
                    }

                    encodedValue |= (value << j);
                }

                bytes.Add((byte)encodedValue);
            }

            StringBuilder sb = new StringBuilder(values.Length);
            foreach (var value in bytes)
            {
                sb.Append(value.ToString("X1"));
            }

            for (int i = boolCount; i < values.Length - 1; i += 2)
            {
                var test = values[i].ToString();
                test += values[i + 1].ToString();
                sb.Append(encodingDict[test.ToString()]);

                if (i >= values.Length - 3 && (values.Length - i > 2))
                {
                    test = values[i + 2].ToString();
                    test += "0";
                    sb.Append(encodingDict[test.ToString()]);
                }
            }

            return sb.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            object[] values = new object[0];
            try
            {
                string text = (string)value;
                if (text.Length < boolCount / 4)
                {
                    return values;
                }

                List<byte> bytes = new List<byte>();
                int index = 0;
                for (; index < text.Length - 1; index++)
                {
                    string sub = text.Substring(index, 1);
                    int decodedValue = System.Convert.ToInt32(sub, 16);
                    for (int j = 0; j < 4; j++)
                    {
                        bytes.Add((byte)(decodedValue & 1));
                        decodedValue = decodedValue >> 1;
                        if (bytes.Count >= boolCount)
                        {
                            break;
                        }
                    }

                    if (bytes.Count >= boolCount)
                    {
                        break;
                    }
                }

                index++;
                var subText = text.Substring(index);
                for (int i = 0; i < subText.Length; i++)
                {
                    var substring = subText.Substring(i, 1);
                    if (substring == "Z")
                    {
                        substring = subText.Substring(i, 2);
                        i++;
                    }

                    substring = decodingDict[substring];
                    foreach (var ch in substring)
                    {
                        bytes.Add(System.Convert.ToByte(ch.ToString(), 10));
                    }
                }

                values = new object[bytes.Count];
                for (int i = 0; i < boolCount; i++)
                {
                    if (bytes[i] == 0)
                    {
                        values[i] = false;
                    }
                    else
                    {
                        values[i] = true;
                    }
                }

                for (int i = boolCount; i < bytes.Count; i++)
                {
                    values[i] = (int)bytes[i];
                }
            }
            catch (Exception)
            {
                //TODO
            }

            return values;
        }

        private static readonly Dictionary<string, string> encodingDict = new Dictionary<string, string>
        {
                {"00", "0"},
                {"01", "1"},
                {"02", "2"},
                {"03", "3"},
                {"04", "4"},
                {"05", "5"},
                {"06", "6"},
                {"07", "7"},
                {"08", "8"},
                {"09", "9"},
                {"10", "a"},
                {"11", "b"},
                {"12", "c"},
                {"13", "d"},
                {"14", "e"},
                {"15", "f"},
                {"16", "g"},
                {"17", "h"},
                {"18", "i"},
                {"19", "j"},
                {"20", "k"},
                {"21", "l"},
                {"22", "m"},
                {"23", "n"},
                {"24", "o"},
                {"25", "p"},
                {"26", "q"},
                {"27", "r"},
                {"28", "s"},
                {"29", "t"},
                {"30", "u"},
                {"31", "v"},
                {"32", "x"},
                {"33", "y"},
                {"34", "z"},
                {"35", "A"},
                {"36", "B"},
                {"37", "C"},
                {"38", "D"},
                {"39", "E"},
                {"40", "F"},
                {"41", "G"},
                {"42", "H"},
                {"43", "I"},
                {"44", "J"},
                {"45", "K"},
                {"46", "L"},
                {"47", "M"},
                {"48", "N"},
                {"49", "O"},
                {"50", "P"},
                {"51", "Q"},
                {"52", "R"},
                {"53", "S"},
                {"54", "T"},
                {"55", "U"},
                {"56", "V"},
                {"57", "X"},
                {"58", "Y"},
                {"59", "w"},
                {"60", "W"},
                {"61", "ZI"},
                {"62", "Z1"},
                {"63", "Z2"},
                {"64", "Z3"},
                {"65", "Z4"},
                {"66", "Z5"},
                {"67", "Z6"},
                {"68", "Z7"},
                {"69", "Z8"},
                {"70", "Z9"},
                {"71", "Zf"},
                {"72", "Zg"},
                {"73", "Zh"},
                {"74", "Zi"},
                {"75", "Zj"},
                {"76", "Zk"},
                {"77", "Zl"},
                {"78", "Zm"},
                {"79", "Zn"},
                {"80", "Zo"},
                {"81", "Zp"},
                {"82", "Zq"},
                {"83", "Zr"},
                {"84", "Zs"},
                {"85", "Zt"},
                {"86", "Zu"},
                {"87", "Zv"},
                {"88", "Zw"},
                {"89", "Zx"},
                {"90", "Zy"},
                {"91", "Zz"},
                {"92", "ZA"},
                {"93", "ZB"},
                {"94", "ZC"},
                {"95", "ZD"},
                {"96", "ZE"},
                {"97", "ZF"},
                {"98", "ZG"},
                {"99", "ZH"},
        };

        private static readonly Dictionary<string, string> decodingDict = Util.Reverse(encodingDict);
    }
}
