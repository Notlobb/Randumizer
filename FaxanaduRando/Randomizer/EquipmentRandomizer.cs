using System;
using System.Linq;

namespace FaxanaduRando.Randomizer
{
    // Per-item stat deltas applied by EquipmentRandomizer, indexed the same way as
    // the corresponding table entries. Empty means "this item type was left unchanged".
    public class EquipmentModifiers
    {
        public int[] WeaponModifiers { get; set; } = Array.Empty<int>();
        public int[] MagicModifiers { get; set; } = Array.Empty<int>();
        public int[] ArmorModifiers { get; set; } = Array.Empty<int>();

        // Post-randomization stat values, same indexing as the modifier arrays.
        public int[] WeaponValues { get; set; } = Array.Empty<int>();
        public int[] MagicValues { get; set; } = Array.Empty<int>();
        public int[] ArmorValues { get; set; } = Array.Empty<int>();

        // When set, item names show the resulting stat value ("HAND DAGGER 5")
        // instead of the delta from the vanilla value ("HAND DAGGER+1").
        public bool ShowExactValues { get; set; }
    }

    public static class EquipmentRandomizer
    {
        // Current single-byte value of every entry in a stat table.
        public static int[] ReadValues(Table table)
        {
            return Enumerable.Range(0, table.Entries.Count).Select(i => (int)table.Entries[i][0]).ToArray();
        }

        public static int[] RandomizeWeapons(Table weaponStrengths, Table weaponGloveStrengths, double scaleFactor, Random random)
        {
            var modifiers = new int[weaponStrengths.Entries.Count];
            for (int i = 0; i < weaponStrengths.Entries.Count; i++)
            {
                int baseStrength = weaponStrengths.Entries[i][0];
                int itemRange = Math.Max(1, (int)Math.Round(baseStrength * scaleFactor));
                int modifier = random.Next(-itemRange, itemRange + 1);

                // Dagger (index 0): modifier always >= 0
                if (i == 0 && modifier < 0)
                {
                    modifier = -modifier;
                }

                modifiers[i] = modifier;

                int newStrength = Math.Max(2, baseStrength + modifier);
                weaponStrengths.Entries[i][0] = (byte)Math.Min(255, newStrength);

                int baseGlove = weaponGloveStrengths.Entries[i][0];
                int newGlove = Math.Max(1, baseGlove + modifier);
                weaponGloveStrengths.Entries[i][0] = (byte)Math.Min(255, newGlove);
            }

            return modifiers;
        }

        public static int[] RandomizeMagic(Table magicDamage, double scaleFactor, Random random)
        {
            var modifiers = new int[magicDamage.Entries.Count];
            for (int i = 0; i < magicDamage.Entries.Count; i++)
            {
                int baseDamage = magicDamage.Entries[i][0];
                int itemRange = Math.Max(1, (int)Math.Round(baseDamage * scaleFactor));
                int modifier = random.Next(-itemRange, itemRange + 1);
                modifiers[i] = modifier;

                int newDamage = Math.Max(2, baseDamage + modifier);
                magicDamage.Entries[i][0] = (byte)Math.Min(255, newDamage);
            }

            return modifiers;
        }

        public static int[] SwapWeapons(Table weaponStrengths, Table weaponGloveStrengths, Random random)
        {
            int count = weaponStrengths.Entries.Count;
            int[] origStrengths = Enumerable.Range(0, count).Select(i => (int)weaponStrengths.Entries[i][0]).ToArray();
            int[] origGloves = Enumerable.Range(0, count).Select(i => (int)weaponGloveStrengths.Entries[i][0]).ToArray();
            int[] perm = GeneratePermutation(count, random);

            var modifiers = new int[count];
            for (int i = 0; i < count; i++)
            {
                weaponStrengths.Entries[i][0] = (byte)origStrengths[perm[i]];
                weaponGloveStrengths.Entries[i][0] = (byte)origGloves[perm[i]];
                modifiers[i] = origStrengths[perm[i]] - origStrengths[i];
            }
            return modifiers;
        }

        public static int[] SwapMagic(Table magicDamage, Random random)
        {
            int count = magicDamage.Entries.Count;
            int[] origDamage = Enumerable.Range(0, count).Select(i => (int)magicDamage.Entries[i][0]).ToArray();
            int[] perm = GeneratePermutation(count, random);

            var modifiers = new int[count];
            for (int i = 0; i < count; i++)
            {
                magicDamage.Entries[i][0] = (byte)origDamage[perm[i]];
                modifiers[i] = origDamage[perm[i]] - origDamage[i];
            }
            return modifiers;
        }

        public static int[] SwapArmor(Table armorDefense, Random random)
        {
            int count = armorDefense.Entries.Count;
            int[] origDefense = Enumerable.Range(0, count).Select(i => (int)armorDefense.Entries[i][0]).ToArray();
            int[] perm = GeneratePermutation(count, random);

            var modifiers = new int[count];
            for (int i = 0; i < count; i++)
            {
                armorDefense.Entries[i][0] = (byte)origDefense[perm[i]];
                modifiers[i] = origDefense[perm[i]] - origDefense[i];
            }
            return modifiers;
        }

        private static int[] GeneratePermutation(int count, Random random)
        {
            int[] perm = Enumerable.Range(0, count).ToArray();
            for (int i = count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (perm[i], perm[j]) = (perm[j], perm[i]);
            }
            return perm;
        }

        public static int[] RandomizeArmor(Table armorDefense, int range, Random random)
        {
            var modifiers = new int[armorDefense.Entries.Count];
            for (int i = 0; i < armorDefense.Entries.Count; i++)
            {
                int modifier = random.Next(-range, range + 1);

                // Studded Mail (index 1): modifier always >= 0
                if (i == 1 && modifier < 0)
                {
                    modifier = -modifier;
                }

                modifiers[i] = modifier;

                int baseDefense = armorDefense.Entries[i][0];
                int newDefense = Math.Max(0, baseDefense + modifier);
                armorDefense.Entries[i][0] = (byte)Math.Min(255, newDefense);
            }

            return modifiers;
        }
    }
}
