using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

/* ************************************************************************
 Centralized flag serialization schema.

 The annotated option classes define the schema, which is reflected on once
 at startup and validated. This class provides conversion between the live
 option classes, serialized flag strings, and a portable object[] settings
 representation used by the serializer and deserializer
************************************************************************* */

namespace FaxanaduRando.Randomizer
{
    // annotations for configuration class members

    // flag nos - must be unique for each setting
    // all bools must be before any non-bools
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class Flag(int index) : Attribute
    {
        public int Index { get; } = index;
    }

    // enum ranges will be [0, enum member count) with an implicit max from the type
    // but ints require a specified max value
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FlagMaxValue(int maxValue) : Attribute
    {
        public int MaxValue { get; } = maxValue;
    }

    // set on properties that are members of the Options-classes
    // but will not be part of serialization
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FlagNone() : Attribute
    {
    }

    internal sealed class FlagEntry
    {
        public required int FlagNo { get; init; }
        public required PropertyInfo Property { get; init; }
        public required Type Type { get; init; }
        public int? MaxValue { get; init; }
    }

    internal static class FlagsCodec
    {
        // add an offset to all non-bool flag numbers so it is easy to add new bool settings later
        public const int NonBoolFlagBase = 100;

        // how many settings that are actually boolean values
        public static int BoolCount { get; private set; }

        // entries sorted by flag no - used by encoder/decoder
        public static IReadOnlyList<FlagEntry> Entries { get; }

        static FlagsCodec()
        {
            Entries = BuildSchema();
        }

        // serializes all settings into a string
        public static string Serialize(object[] optionValues)
        {
            if (optionValues.Length != Entries.Count)
                throw new ArgumentException("Incorrect number of values.", nameof(optionValues));

            var boolBytes = new List<byte>();
            var values = new List<int>();

            // encode booleans first - 4 bools per byte
            int boolIndex = 0;
            while (boolIndex < Entries.Count && Entries[boolIndex].Type == typeof(bool))
            {
                int nibble = 0;

                for (int bit = 0;
                     bit < 4 && boolIndex < Entries.Count && Entries[boolIndex].Type == typeof(bool);
                     bit++, boolIndex++)
                {
                    bool value = (bool)optionValues[boolIndex];

                    if (value)
                        nibble |= 1 << bit;
                }

                boolBytes.Add((byte)nibble);
            }

            for (; boolIndex < Entries.Count; boolIndex++)
            {
                var entry = Entries[boolIndex];
                int value = entry.Type.IsEnum ? Convert.ToInt32(optionValues[boolIndex]) : (int)optionValues[boolIndex];
                int maxValue = entry.Type.IsEnum ? Enum.GetValues(entry.Type).Length - 1 : entry.MaxValue!.Value;

                if (value < 0 || value > maxValue)
                {
                    throw new InvalidOperationException(
                        $"{entry.Property.DeclaringType!.Name}.{entry.Property.Name}: value {value} outside allowed range.");
                }

                values.Add(value);
            }

            var sb = new StringBuilder();
            foreach (byte b in boolBytes)
            {
                sb.Append(b.ToString("X1"));
            }

            for (int i = 0; i < values.Count; i += 2)
            {
                string test = values[i].ToString();

                if (i + 1 < values.Count)
                    test += values[i + 1].ToString();
                else
                    test += "0";

                sb.Append(encodingDict[test]);
            }

            return sb.ToString();
        }

        // reads concrete option class members
        public static object[] ReadSettings()
        {
            var values = new object[Entries.Count];

            for (int i = 0; i < Entries.Count; i++)
            {
                values[i] = Entries[i].Property.GetValue(null)!;
            }

            return values;
        }

        // serializes current settings
        public static string Serialize()
        {
            return Serialize(ReadSettings());
        }

        // deserializes a string into an array of objects that can be stored in the
        // reflected-on options class members, but does not mutate any external state
        // will throw on strings not adhering to the schema
        public static object[] Deserialize(string flags)
        {
            var values = new object[Entries.Count];

            int charIndex = 0;
            int valueIndex = 0;

            // Decode bools (4 per hex digit)
            while (valueIndex < Entries.Count && Entries[valueIndex].Type == typeof(bool))
            {
                if (charIndex >= flags.Length)
                    throw new InvalidOperationException("Unexpected end of flag string.");

                int nibble;
                char c = flags[charIndex++];

                if (c >= '0' && c <= '9')
                    nibble = c - '0';
                else if (c >= 'A' && c <= 'F')
                    nibble = c - 'A' + 10;
                else
                    throw new InvalidOperationException($"Invalid hex character '{c}'.");

                for (int bit = 0; bit < 4 && valueIndex < Entries.Count && Entries[valueIndex].Type == typeof(bool); bit++, valueIndex++)
                {
                    values[valueIndex] = (nibble & (1 << bit)) != 0;
                }
            }

            // decode enums / ints (2 values per encoded value)
            while (valueIndex < Entries.Count)
            {
                if (charIndex >= flags.Length)
                    throw new InvalidOperationException("Unexpected end of flag string.");

                string code;

                if (flags[charIndex] == 'Z')
                {
                    if (charIndex + 1 >= flags.Length)
                        throw new InvalidOperationException("Unexpected end of flag string.");

                    code = flags.Substring(charIndex, 2);
                    charIndex += 2;
                }
                else
                {
                    code = flags[charIndex].ToString();
                    charIndex++;
                }

                if (!decodingDict.TryGetValue(code, out string pair))
                    throw new InvalidOperationException($"Invalid flag code '{code}'.");

                for (int j = 0; j < 2 && valueIndex < Entries.Count; j++, valueIndex++)
                {
                    var entry = Entries[valueIndex];
                    int value = pair[j] - '0';

                    int maxValue = entry.Type.IsEnum ? Enum.GetValues(entry.Type).Length - 1 : entry.MaxValue!.Value;

                    if (value > maxValue)
                    {
                        throw new InvalidOperationException(
                            $"{entry.Property.DeclaringType!.Name}.{entry.Property.Name}: decoded value {value} exceeds maximum {maxValue}.");
                    }

                    values[valueIndex] = entry.Type.IsEnum ? Enum.ToObject(entry.Type, value) : value;
                }
            }

            // Allow exactly one unused padding digit in the last encoded character.
            if (charIndex != flags.Length)
                throw new InvalidOperationException("Trailing characters in flag string.");

            return values;
        }

        // should be called with a result from Deserialize
        // to populate the correct options class members
        public static void ApplySettings(object[] values)
        {
            if (values.Length != Entries.Count)
            {
                throw new ArgumentException("Incorrect number of values.", nameof(values));
            }

            for (int i = 0; i < Entries.Count; i++)
            {
                Entries[i].Property.SetValue(null, values[i]);
            }
        }

        // helper that both deserializes and updates options
        public static void DeserializeAndApply(string flags)
        {
            ApplySettings(Deserialize(flags));
        }

        // helper that generates a random schema-validated settings object
        // to be used for regression testing (or randomizer-ception in the front-end?)
        // if useCustomText is true you need to provide a text-file to the randomizer logic
        public static object[] GenerateRandom(Random rng, bool useCustomText)
        {
            var values = new object[Entries.Count];

            for (int i = 0; i < Entries.Count; i++)
            {
                var entry = Entries[i];

                if (entry.Property.DeclaringType == typeof(TextOptions) &&
                    entry.Property.Name == nameof(TextOptions.UseCustomText))
                {
                    values[i] = useCustomText;
                }
                else if (entry.Type == typeof(bool))
                {
                    values[i] = rng.Next(2) != 0;
                }
                else if (entry.Type.IsEnum)
                {
                    values[i] = Enum.ToObject(
                        entry.Type,
                        rng.Next(Enum.GetValues(entry.Type).Length));
                }
                else // int
                {
                    values[i] = rng.Next(entry.MaxValue!.Value + 1);
                }
            }

            return values;
        }

        // builds the flags schema and validates it
        private static IReadOnlyList<FlagEntry> BuildSchema()
        {
            var entries = new List<FlagEntry>();

            // all option classes we reflect on
            var optionTypes = new[]
                {
                typeof(GeneralOptions),
                typeof(EnemyOptions),
                typeof(ItemOptions),
                typeof(TextOptions),
                typeof(ExtraOptions)
                };

            // loop over all classes
            foreach (var type in optionTypes)
            {
                // loop over all public and static properties of the class
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
                {
                    var flag = property.GetCustomAttribute<Flag>();
                    var noFlag = property.GetCustomAttribute<FlagNone>();
                    var maxValue = property.GetCustomAttribute<FlagMaxValue>()?.MaxValue;

                    // check annotation consistency
                    if ((flag == null) == (noFlag == null))
                    {
                        throw new InvalidOperationException(
                            $"{type.Name}.{property.Name} must have exactly one of [Flag] or [FlagNone].");
                    }

                    // [FlagNone] was set - skip
                    if (noFlag != null)
                    {
                        continue;
                    }

                    entries.Add(new FlagEntry
                    {
                        FlagNo = flag.Index,
                        Property = property,
                        Type = property.PropertyType,
                        MaxValue = maxValue
                    });
                }
            }

            // schema validation - any throws below are programmer errors
            entries.Sort((a, b) => a.FlagNo.CompareTo(b.FlagNo));

            // disallow duplicate flag numbers
            var duplicate = entries
                .GroupBy(e => e.FlagNo)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicate != null)
            {
                throw new InvalidOperationException($"Duplicate flag number {duplicate.Key}.");
            }

            // validate each schema entry
            // and that all bools come before any non-bools
            bool seenNonBool = false;

            foreach (var entry in entries)
            {
                if (entry.FlagNo < 0)
                {
                    throw new InvalidOperationException(
                        $"{entry.Property.DeclaringType!.Name}.{entry.Property.Name}: flag number must be non-negative.");
                }

                if (entry.Type == typeof(int))
                {
                    if (entry.MaxValue == null)
                    {
                        throw new InvalidOperationException(
                            $"{entry.Property.DeclaringType!.Name}.{entry.Property.Name}: ints require [FlagMaxValue].");
                    }
                }
                else if (entry.Type != typeof(bool) && !entry.Type.IsEnum)
                {
                    throw new InvalidOperationException(
                        $"{entry.Property.DeclaringType!.Name}.{entry.Property.Name}: unsupported property type '{entry.Type.Name}'.");
                }

                if (entry.Type == typeof(bool))
                {
                    BoolCount++;
                    if (seenNonBool)
                        throw new InvalidOperationException(
               $"{entry.Property.DeclaringType!.Name}.{entry.Property.Name}: bool flags must come before non-bool flags.");
                }
                else
                    seenNonBool = true;
            }

            return entries;
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
