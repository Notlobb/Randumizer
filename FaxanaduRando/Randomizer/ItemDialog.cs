using System;
using System.Collections.Generic;
using System.IO;

namespace FaxanaduRando.Randomizer
{
    public static class ItemDialog
    {

        public enum Id
        {
            QueenKeyHole = 164,
            KingKeyHole = 165,
            AceKeyHole = 166,
            JokerKeyHole = 167,
            RedPotionUsed = 169,
            MattockUsed = 170,
            HourGlassUsed = 171,
            WingBootsUsed = 172,
            ElixirUsed = 174,
            ElixirAcquired = 175,
            RedPotionAcquired = 176,
            MattockAcquired = 177,
            WingBootsAcquired = 178,
            HourGlassAcquired = 179,
            BattleSuitAcquired = 180,
            BattleHelmentAcquired = 181,
            DragonSlayerAcquired = 182,
            BlackOnyxAcquire = 183,
            PendantAcquired = 184,
            RodAcquired = 185,
            PoisonTouched = 186,
            GloveAcquired = 187,
            GloveGone = 188,
            OintmentAcquired = 189,
            OintmentGone = 190,
            WingBootsGone = 191,
            HourGlassGone = 192
        }

        private static readonly Dictionary<Id, List<string>> AlternativeDialogTemplates = new Dictionary<Id, List<string>>
        {
            // Black Potion
            { Id.PoisonTouched, new List<string> {
                "I'm holding {0}.",
                "I now prossess {0}.",
            }},
        };

        private static readonly Dictionary<Id, List<string>> DialogTemplates = new Dictionary<Id, List<string>>
        {
            { Id.QueenKeyHole, new List<string> {
                "There is the mark of \"{0}\" by the key hole.",
                "There is the silhouetto of \"{0}\" by the key hole.",
                "There is the mark of key by the \"{0}\" hole.",
            }},
            { Id.KingKeyHole, new List<string> {
                "There is the mark of \"{0}\" by the key hole.",
                "There is the mark of key by the \"{0}\" hole.",
            }},
            { Id.AceKeyHole, new List<string> {
                "There is the mark of \"{0}\" by the key hole.",
                "There is the mark of key by the \"{0}\" hole.",
            }},
            { Id.JokerKeyHole, new List<string> {
                "There is the mark of \"{0}\" by the key hole.",
                "There is the mark of key by the \"{0}\" hole.",
            }},
            { Id.RedPotionUsed, new List<string> {
                "I've used {0}.",
                "Down the hole!",
                "I chugged the {0}.",
            }},
            { Id.MattockUsed, new List<string> {
                "I've used {0}.",
                "MAAAAAAAATTTTTTTTTOOOOOOOOCK!",
                "I now declare this the Mattock Expressway",
            }},
            { Id.HourGlassUsed, new List<string> {
                "I've used {0}.",
                "Za Warudo!",
                "Stop the clock!",
                "Stop! Hammer Time!",
                "Time keeps slipping away",
            }},
            { Id.WingBootsUsed, new List<string> {
                "I've used {0}.",
                "Looks like Team Rocket is Blasting Off Again!",
                "With boots I could walk on air.",
                "This stream sponsored by Red Bull!",
                "I have to go now. My planet needs me.",
            }},
            { Id.ElixirUsed, new List<string> { "I've used {0}." } },
            { Id.ElixirAcquired, new List<string> { "I'm holding {0}." } },
            { Id.RedPotionAcquired, new List<string> { "I'm holding {0}." } },
            { Id.MattockAcquired, new List<string> { "I'm holding {0}." } },
            { Id.WingBootsAcquired, new List<string> { "I'm holding {0}." } },
            { Id.HourGlassAcquired, new List<string> { "I'm holding {0}." } },
            { Id.BattleSuitAcquired, new List<string> { "I've got the {0}." } },
            { Id.BattleHelmentAcquired, new List<string> { "I've got the {0}." } },
            { Id.DragonSlayerAcquired, new List<string> {
                "I've got the {0}.",
                "Say nello to my {0}!",
                "This..|is my {0}!",
            }},
            { Id.BlackOnyxAcquire, new List<string> { "I've got the {0}." } },
            { Id.PendantAcquired, new List<string> {
                "I've got the {0}.",
                "I've got the {0}.|It may or may not be cursed.",
            }},
            { Id.RodAcquired, new List<string> {
                "I've got the {0}.",
                "In {0} we trust.",
            } },
            { Id.PoisonTouched, new List<string> {
                "I've touched poison",
                "SOAP POISONING",
            }},
            { Id.GloveAcquired, new List<string> {
                "The {0} increases offensive power.",
                "I love the power glove. It's so bad",
                "If it doesn't fit, you must acquit.",
                "Hey! You forgot the Power Glove!",
                "No glove, no love",
            }},
            { Id.GloveGone, new List<string> { "The power of the {0} is gone." } },
            { Id.OintmentAcquired, new List<string> { "I am free from injury because of the {0}." } },
            { Id.OintmentGone, new List<string> { "The power of the {0} is gone." } },
            { Id.WingBootsGone, new List<string> { "The power of the {0} is gone." } },
            { Id.HourGlassGone, new List<string> { "The power of the {0} is gone." } }
        };

        private static readonly Dictionary<Id, Item> IdToItemMap = new Dictionary<Id, Item>
        {
            { Id.QueenKeyHole, Item.KeyQ },
            { Id.KingKeyHole, Item.KeyK },
            { Id.AceKeyHole, Item.KeyA },
            { Id.JokerKeyHole, Item.KeyJo },
            { Id.RedPotionUsed, Item.RedPotion },
            { Id.MattockUsed, Item.Mattock },
            { Id.HourGlassUsed, Item.HourGlass },
            { Id.WingBootsUsed, Item.WingBoots },
            { Id.ElixirUsed, Item.Elixir },
            { Id.ElixirAcquired, Item.Elixir },
            { Id.RedPotionAcquired, Item.RedPotion },
            { Id.MattockAcquired, Item.Mattock },
            { Id.WingBootsAcquired, Item.WingBoots },
            { Id.HourGlassAcquired, Item.HourGlass },
            { Id.PoisonTouched, Item.Poison },
            { Id.GloveAcquired, Item.Glove },
            { Id.GloveGone, Item.Glove },
            { Id.OintmentAcquired, Item.Ointment },
            { Id.OintmentGone, Item.Ointment },
            { Id.WingBootsGone, Item.WingBoots },
            { Id.HourGlassGone, Item.HourGlass }
        };

        public static Dictionary<int, string> GetDialog(
            Random RandomGenerator,
            Dictionary<Item, string> itemOptions,
            string[] customTextFileContents)
        {
            var result = new Dictionary<int, string>();
            var customTemplates = new Dictionary<Id, string>();

            // Load custom text if provided
            foreach (var line in customTextFileContents)
            {
                // compare text before colon to Id enum
                var parts = line.Split(':');
                if (parts.Length == 2 && Enum.TryParse(parts[0], out Id parsedId))
                {
                    customTemplates[parsedId] = parts[1].Trim();
                }
            }

            foreach (var idToItem in IdToItemMap)
            {
                var id = idToItem.Key;
                var item = idToItem.Value;

                string template;
                /*
                    We need to swap in Black Potion dialog to replace poison dialog when that option is used.
                    But, this can't be done with enums as they'd share the same Dialog ID and that causes ambiguity
                     in the execution. So we have an AlternativeDialogTemplate which just has that one case for now
                     but could be used in the future for other cases where we change the behaviour of an item.
                */
                if (ItemOptions.ReplacePoison && id == Id.PoisonTouched)
                {
                    template = customTemplates.ContainsKey(id)
                        ? customTemplates[id]
                        : GeneralOptions.UpdateMiscText && AlternativeDialogTemplates.ContainsKey(id)
                            ? AlternativeDialogTemplates[id][RandomGenerator.Next(AlternativeDialogTemplates[id].Count)]
                            : AlternativeDialogTemplates.ContainsKey(id) ? AlternativeDialogTemplates[id][0] : null;
                }
                else
                {
                    template = customTemplates.ContainsKey(id)
                        ? customTemplates[id]
                        : GeneralOptions.UpdateMiscText && DialogTemplates.ContainsKey(id)
                            ? DialogTemplates[id][RandomGenerator.Next(DialogTemplates[id].Count)]
                            : DialogTemplates.ContainsKey(id) ? DialogTemplates[id][0] : null;
                }

                // Keys are different and are referenced as hints rather than exact names
                string value;
                if (item == Item.KeyQ) value = "Queen";
                else if (item == Item.KeyA) value = "Ace";
                else if (item == Item.KeyK) value = "King";
                else if (item == Item.KeyJ) value = "Jack";
                else if (item == Item.KeyJo) value = "Joker";
                else if (itemOptions.TryGetValue(item, out var itemName))
                    value = itemName;
                else
                    value = null;

                if (template == null || value == null)
                {
                    continue;
                }

                result[(int)id] = string.Format(template, value);
            }

            return result;
        }
    }
}
