using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FaxanaduRando.Randomizer
{
    public enum Item
    {
        HandDagger = 0,
        LongSword = 1,
        GiantBlade = 2,
        DragonSlayer = 3,
        LeatherArmor = 4,
        StuddedMail = 5,
        FullPlate = 6,
        BattleSuit = 7,
        SmallShield = 8,
        LargeShield = 9,
        MagicShield = 10,
        BattleHelmet = 11,
        Deluge = 12,
        Thunder = 13,
        Fire = 14,
        Death = 15,
        Tilte = 16,
        RingOfElf = 17,
        RubyRing = 18,
        RingOfDworf = 19,
        DemonsRing = 20,
        KeyA = 21,
        KeyK = 22,
        KeyQ = 23,
        KeyJ = 24,
        KeyJo = 25,
        Mattock = 26,
        Rod = 27,
        Crystal = 28,
        Lamp = 29,
        HourGlass = 30,
        Book = 31,
        WingBoots = 32,
        RedPotion = 33,
        BlackPotion = 34,
        Elixir = 35,
        Pendant = 36,
        BlackOnix = 37,
        FireCrystal = 38,
        // these three are never in inventories or shops so have no "name" or Item ID in the ROM
        // but we still want an enum so we can identify them for the purposes of custom dialog
        Ointment,
        Glove,
        Poison
    }

    public static class ItemNameRandomizer
    {
        public static readonly Dictionary<ShopRandomizer.Id, Item> itemRelation = new Dictionary<ShopRandomizer.Id, Item>
        {
            { ShopRandomizer.Id.Dagger, Item.HandDagger },
            { ShopRandomizer.Id.Longsword, Item.LongSword },
            { ShopRandomizer.Id.Giantblade, Item.GiantBlade },
            { ShopRandomizer.Id.Dragonslayer, Item.DragonSlayer },
            { ShopRandomizer.Id.Leatherarmour, Item.LeatherArmor },
            { ShopRandomizer.Id.Studdedmail, Item.StuddedMail },
            { ShopRandomizer.Id.Fullplate, Item.FullPlate },
            { ShopRandomizer.Id.Battlesuit, Item.BattleSuit },
            { ShopRandomizer.Id.Smallshield, Item.SmallShield },
            { ShopRandomizer.Id.Largeshield, Item.LargeShield },
            { ShopRandomizer.Id.Magicshield, Item.MagicShield },
            { ShopRandomizer.Id.Battlehelmet, Item.BattleHelmet },
            { ShopRandomizer.Id.Deluge, Item.Deluge },
            { ShopRandomizer.Id.Thunder, Item.Thunder },
            { ShopRandomizer.Id.Fire, Item.Fire },
            { ShopRandomizer.Id.Death, Item.Death },
            { ShopRandomizer.Id.Tilte, Item.Tilte },
            { ShopRandomizer.Id.ElfRing, Item.RingOfElf },
            { ShopRandomizer.Id.RubyRing, Item.RubyRing },
            { ShopRandomizer.Id.DworfRing, Item.RingOfDworf },
            { ShopRandomizer.Id.DemonRing, Item.DemonsRing },
            { ShopRandomizer.Id.AceKey, Item.KeyA },
            { ShopRandomizer.Id.KingKey, Item.KeyK },
            { ShopRandomizer.Id.QueenKey, Item.KeyQ },
            { ShopRandomizer.Id.JackKey, Item.KeyJ },
            { ShopRandomizer.Id.JokerKey, Item.KeyJo },
            { ShopRandomizer.Id.Mattock, Item.Mattock },
            { ShopRandomizer.Id.Rod, Item.Rod },
            { ShopRandomizer.Id.Crystal, Item.Crystal },
            { ShopRandomizer.Id.Lamp, Item.Lamp },
            { ShopRandomizer.Id.Hourglass, Item.HourGlass },
            { ShopRandomizer.Id.Book, Item.Book },
            { ShopRandomizer.Id.Wingboots, Item.WingBoots },
            { ShopRandomizer.Id.RedPotion, Item.RedPotion },
            { ShopRandomizer.Id.BlackPotion, Item.BlackPotion },
            { ShopRandomizer.Id.Elixir, Item.Elixir },
            { ShopRandomizer.Id.Pendant, Item.Pendant },
            { ShopRandomizer.Id.BlackOnyx, Item.BlackOnix },
            { ShopRandomizer.Id.FireCrystal, Item.FireCrystal }
        };

        private static readonly Dictionary<Item, List<string>> itemOptions = new Dictionary<Item, List<string>>
        {
            { Item.HandDagger, new List<string> { "Hand Dagger", "Iron Dagger", "Quick Blade", "Small Stab", "Nand Dagger", "Knife" } },
            { Item.LongSword, new List<string> { "Long Sword", "Steel Blade", "Le Sword Long", "XL Tooth Pick", "Short Sword", "Falchion", "Scimitar", "Sabre", "Rapier" } },
            { Item.GiantBlade, new List<string> { "Giant Blade", "Trident", "Very Big Fork", "Pitch Fork" } },
            { Item.DragonSlayer, new List<string> { "Dragon Slayer", "Big Stabby Boi", "Little Friend", "Boom Stick" } },
            { Item.LeatherArmor, new List<string> { "Leather Armor", "Hide Suit", "Tanned Vest", "Xanadude Garb" } },
            { Item.StuddedMail, new List<string> { "Studded Mail", "Half Plate" } },
            { Item.FullPlate, new List<string> { "Full Plate", "Dinner Plate" } },
            { Item.BattleSuit, new List<string> { "Battle Suit", "Law Suit", "Battle Onesie", "Spacesuit", "Snowsuit" } },
            { Item.SmallShield, new List<string> { "Small Shield", "Disk 2", "Round Buckler", "Light Defender" } },
            { Item.LargeShield, new List<string> { "Large Shield", "Tower Guard", "Heavy Plate", "Broad Shield" } },
            { Item.MagicShield, new List<string> { "Magic Shield", "XL Shield", "Surfboard" } },
            { Item.BattleHelmet, new List<string> { "Battle Helmet", "Brain Bucket", "Knight Cap", "Buckethead", "Battle Nelmet" } },
            { Item.Deluge, new List<string> { "Deluge", "Flamey Boi", "Fireball", "Blastin", "Fireball", "The Luge", "Fire Flood", "Spark", "Inferno", "Lighter", "Fir2", "Nuke", "Firaga", "Doodahleedoo", "op spell", "op magic" } },
            { Item.Thunder, new List<string> { "Thunder", "Lightning", "Storm", "Comet", "Nova", "Smoke", "Lit2", "Thundaga" } },
            { Item.Fire, new List<string> { "Fire", "Flame", "Force Push", "Fyre", "Fire Wave", "Air Wave" } },
            { Item.Death, new List<string> { "Death", "Doom Hex", "Doom Doom Doom", "Happy Funtime", "Spike" } },
            { Item.Tilte, new List<string> { "Tilte", "Tilto", "Tilta", "Tilti", "Title", "Tickle", "Tilted", "OnTilte" } },
            { Item.RingOfElf, new List<string> { "Ring Of Elf", "Elven Band", "Mood Ring", "Tree Ring" } },
            { Item.RubyRing, new List<string> { "Ruby Ring", "Ring of Fire", "Fountain Pass", "Ruby Band" } },
            { Item.RingOfDworf, new List<string> { "Ring Of Dworf", "Ring Of Dwurf", "Ring Of Worf", "Ring Of Dwouf", "Ring of Drarf", "Wring of Dorf", "Wring of Dwaf", "Ring of Twarf" } },
            { Item.DemonsRing, new List<string> { "Demons Ring", "Demon Sring", "Ring of Skull" } },
            { Item.KeyA, new List<string> { "Key A", "Ace Key", "Key of A", "Any Key", "Key of Ace", "Acetone Key", "Key of A Flat" } },
            { Item.KeyK, new List<string> { "Key K", "King Key", "2King2Furious", "King Ski", "Key of Key", "Key Key", "Key of K", "Key of Kings", "Key of King", "Kefka Key", "Kinky" } },
            { Item.KeyQ, new List<string> { "Key Q", "Quest Key", "Quick Key", "Cuke Key", "Queen Key", "Quality Key", "Quake Key", "Quail Key", "Quirk Key", "Quiet Key", "Killer Queen" } },
            { Item.KeyJ, new List<string> { "Key J", "Jail Key", "Jackie", "Jack Key", "Jill Key", "Key of Jack", "Key of Jill", "Jackalope Key", "Jacket Key", "Jay Key", "Key of Jay" } },
            { Item.KeyJo, new List<string> { "Key Jo", "Joker Key", "Key of Jokes", "Serious Key", "Key Josephine", "Jojo Key", "Key of Joker", "Jokey", "Joker", "Key of Joe", "Joe Key" } },
            { Item.Mattock, new List<string> { "Mattock", "Mining Pick", "Matlock", "Tockmat", "Maaaatlooock", "Lawyer Key", "Tooth Pick", "Lock Pick", "Pickaxe", "Maaaattooock" } },
            { Item.Rod, new List<string> { "Magical Rod", "Rod", "Oak Steak", "Regular Rod", "Normal Rod", "Inanimate Rod", "Carbon Rod", "Cattle Prod", "Oak Stake" } },
            { Item.Crystal, new List<string> { "Crystal" } },
            { Item.Lamp, new List<string> { "Lamp" } },
            { Item.HourGlass, new List<string> { "Hour Glass", "Timex", "Vial of Sand", "ChronoTrigger", "Timestopper", "Time Stop", "Stop" } },
            { Item.Book, new List<string> { "Book" } },
            { Item.WingBoots, new List<string> { "Wing Boots", "Skywalkers", "Air Jordans", "Fly Boots", "Wingshoes", "Wingboots", "Bootwings" } },
            { Item.RedPotion, new List<string> { "Red Potion", "Red Poison", "Red Potion TM", "Rad Potion", "Red Bull", "Mid Potion", "Fruit Punch" } },
            { Item.BlackPotion, new List<string> { "Black Potion", "Mana Potion", "Guinness", "Coffee", "Carob Juice", "Green potion", "Brown potion" } },
            { Item.Elixir, new List<string> { "Elixir", "Megalixir", "Fenix Down" } },
            { Item.Pendant, new List<string> { "Pendant", "Charm", "Magic Amulet", "Magic Pendant" } },
            { Item.BlackOnix, new List<string> { "Black Onix", "Dark Gem", "Shadow Stone", "Night Crystal", "Black Onyx", "Shiny Onix" } },
            { Item.FireCrystal, new List<string> { "Fire Crystal" } },
            { Item.Ointment, new List<string> { "Ointment", "Goo", "Vapo Rub", "Maple Syrup", "Star Salve", "Exfoliant", "Myrrh", "Lotion" } },
            { Item.Glove, new List<string> { "Glove", "Power Glove", "Gauntlet", "Hand Puppet", "Mitts" } },
            { Item.Poison, new List<string> { "Poison", "Venom", "Toxin", "Bane Juice" } }
        };

        public static Dictionary<Item, string> GenerateItemNameDictionary(Random random, bool randomize, string[] customTextFileContents)
        {
            var itemDictionary = new Dictionary<Item, string>();

            foreach (var item in itemOptions)
            {
                if (randomize)
                {
                    var randomValue = item.Value[random.Next(item.Value.Count)];
                    itemDictionary[item.Key] = randomValue;
                }
                else
                {
                    itemDictionary[item.Key] = item.Value.First(); // Default to the original ROM item name
                }
            }

            foreach (var line in customTextFileContents)
            {
                var parts = line.Split(':');
                // Item Id does not correspond to Dialog IDs so we exclude numerical custom text lines
                if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]) && !char.IsDigit(parts[0].Trim()[0])
                    && Enum.TryParse(parts[0].Trim(), out Item itemKey)
                    && itemDictionary.ContainsKey(itemKey))
                {
                    itemDictionary[itemKey] = parts[1].Trim();
                }
            }

            return itemDictionary;
        }


        public static string GetItemName(ShopRandomizer.Id shopItemId, Dictionary<Item, string> itemDictionary)
        {
            // Check if the ShopRandomizer.Id is mapped to an Item
            if (itemRelation.TryGetValue(shopItemId, out var relatedItem))
            {
                // Check if the Item exists in the itemDictionary
                if (itemDictionary.TryGetValue(relatedItem, out var itemName))
                {
                    return itemName;
                }
                else
                {
                    throw new KeyNotFoundException($"The item {relatedItem} was not found in the itemDictionary.");
                }
            }
            else
            {
                throw new KeyNotFoundException($"The shop item {shopItemId} does not have a related Item in itemRelation.");
            }
        }

    }
}
