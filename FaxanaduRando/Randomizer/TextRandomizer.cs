using System;
using System.Collections.Generic;
using System.IO;

namespace FaxanaduRando.Randomizer
{
    class TextRandomizer
    {
        public const int numberOfTitles = 16;
        public const int titleLength = 16;

        private List<string> titles = new List<string>();
        private List<ushort> titleExperiences = new List<ushort>();
        private List<ushort> titleRewards = new List<ushort>();

        private Random random;

        private static readonly Dictionary<Sprite.SpriteId, string> itemDict = new Dictionary<Sprite.SpriteId, string>
        {
            {Sprite.SpriteId.Elixir, "Elixir" },
            {Sprite.SpriteId.Hourglass, "Hourglass" },
            {Sprite.SpriteId.Wingboots, "Wingboots" },
            {Sprite.SpriteId.WingbootsBossLocked, "Wingboots" },
            {Sprite.SpriteId.BlackOnyx, "Black Onyx" },
            {Sprite.SpriteId.Battlehelmet, "Battlehelmet" },
            {Sprite.SpriteId.Battlesuit, "Battlesuit" },
            {Sprite.SpriteId.Dragonslayer, "Dragonslayer" },
            {Sprite.SpriteId.KeyAce, "AceKey" },
            {Sprite.SpriteId.Pendant, "Pendant" },
            {Sprite.SpriteId.RingDemon, "DemonRing" },
            {Sprite.SpriteId.RingDworf, "DworfRing" },
            {Sprite.SpriteId.Rod, "Rod" },
            {Sprite.SpriteId.Ointment, "Ointment" },
            {Sprite.SpriteId.Ointment2, "Ointment" },
            {Sprite.SpriteId.RedPotion, "RedPotion" },
            {Sprite.SpriteId.RedPotion2, "RedPotion" },
            {Sprite.SpriteId.Glove, "Glove" },
            {Sprite.SpriteId.Poison, "Poison" },
            {Sprite.SpriteId.Poison2, "Poison" },
            {Sprite.SpriteId.MattockBossLocked, "Mattock" },
            {Sprite.SpriteId.MattockOrRingRuby, "Mattock2" },
            {Sprite.SpriteId.Glove2OrKeyJoker, "Glove2" },
        };

        public TextRandomizer(byte[] content, Random random)
        {
            this.random = random;
            titles = Text.GetTitles(content, Section.GetOffset(15, 0xF649, 0xC000));
            titleExperiences = GetTitleData(content, Section.GetOffset(15, 0xF749, 0xC000));
            titleRewards = GetTitleData(content, Section.GetOffset(15, 0xF767, 0xC000));
        }

        public Result UpdateText(ShopRandomizer shopRandomizer, GiftRandomizer giftRandomizer, DoorRandomizer doorRandomizer, byte[] content, string customTextFile)
        {
            var allText = Text.GetAllText(content);
            int oldLength = getLength(allText);
            var customTexts = new List<string>() {};
            string defaultHint = "Hi";

            if (GeneralOptions.FastText)
            {
                for(int i = 0; i < allText.Count; i++)
                {
                    allText[i] = allText[i].Replace(Text.lineBreakWithPauseChar,
                                                    Text.lineBreakChar);
                }
            }

            if (GeneralOptions.HintSetting != GeneralOptions.Hints.None)
            {
                var hints = new List<string>();
                if (GeneralOptions.HintSetting != GeneralOptions.Hints.Community)
                {
                    hints = GetHints(shopRandomizer, giftRandomizer, doorRandomizer);
                }

                // notes; 38 & 39 first NPC pre- and post- King talk
                /* known indices

                    38: first NPC, pre-King talk
                    39: first NPC, post-King talk
                */

                var indices = new List<int> { 38, 39, 41, 42, 45, 46, 47, 48, 49, 50,
                                              51, 54, 55,
                                              56, 57, 58, 59, 60,
                                              62, 63, 64, 65, 68, 69, 70,
                                              71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 87, 91,
                                              95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105,
                                              106, 107, 108, 109, 110, 111, 112, 113, 114,
                                              118, 119,
                                              122, 123, 127, 128, 129, 130,
                                              134, 135,
                                              136, 137, 140, 141, 142, 143, 144, 145,
                                              146, 147, 148, 149,
                                              150, 151,
                                              154, 155, 156, 157, 158, 159 };

            var communityHints = new List<string>()
            {
                    defaultHint,
                    "Ni",
                    "Who's talkin'?",
                    "I am Error",
                    "Sorry. I know nothing",
                    "Faxanadu or du not. There is no Faxanatry",
                    "All glory to the Evil One",
                    "I love the Evil One",
                    "Elves are the real evil ones",
                    "Have negative thoughts",
                    "Forget your mantra",
                    "The King? I didn't vote for him",
                    "This seed sucks",
                    "If only this game had chocobos",
                    "Silvers are in 9",
                    "Smoke!",
                    "i'm not a bad elf",
                    "I think the meat vendor might be a pervert",
                    "Let's live here together",
                    "I don't know how to say this... Faxanadu? Fax-anadu? Fazzanadu? Faxana-du? Faxanadu?",
                    "Why do they call it Faxanadu? It's not a fax and it's not du-ing anything!",
                    "What would Faxanadu?",
                    "I love the smell of the Fire spell in the morning. It smells like... victory",
                    "Dragon Slayer? I hardly knew her!",
                    "In theory, this seed can be beaten. But in theory, communism works. In theory...",
                    "Trying is the first step towards failure",
                    "The fortress of what? Oh, Zenis. I thought you said something else...",
                    "Are you in a race? If so, good luck!",
                    "Shoutout to Tundra83",
                    "Shoutout to Cha0sFinale",
                    "Shoutout to ShinerCCC",
                    "Shoutout to LoZCardsfan23",
                    "Shoutout to OdinSpack",
                    "Shoutout to MeowthRocket",
                    "Shoutout to Songbirder",
                    "Shoutout to Bogledowdee",
                };

                if (TextOptions.UseCustomText)
                {
                    string[] customTextLines = File.ReadAllLines(customTextFile); 

                    communityHints = new List<string>() {};
                    foreach (string customText in customTextLines)
                    {
                        if (customText.Trim().StartsWith(":"))
                        {
                            communityHints.Add(customText.Remove(0, 1));
                        }
                        else if (customText.Trim().StartsWith("default:"))
                        {
                            defaultHint = customText.Split(':')[1];
                        }
                        else if (customText.Trim().StartsWith("title:"))
                        {
                            // do nothing
                        }
                        else
                        {
                            customTexts.Add(customText);
                        }
                    }
                } 
            
                Util.ShuffleList(communityHints, 0, communityHints.Count - 1, random);

                if (GeneralOptions.HintSetting == GeneralOptions.Hints.Community)
                {
                    hints.AddRange(communityHints);
                }

                int index = 0;
                int communityHintCount = GeneralOptions.HintSetting == GeneralOptions.Hints.Strong ? 2 : 10;
                communityHintCount = Math.Min(communityHintCount, communityHints.Count);
                for (; index < communityHintCount; index++)
                {
                    hints.Add(communityHints[index]);
                }

                while (index < communityHints.Count && hints.Count < indices.Count)
                {
                    hints.Add(communityHints[index]);
                    index++;
                }

                while (hints.Count < indices.Count)
                {
                    hints.Add(defaultHint);
                }

                Util.ShuffleList(hints, 0, hints.Count - 1, random);
                Util.ShuffleList(indices, 0, indices.Count - 1, random);

                for (int i = 0; i < hints.Count; i++)
                {
                    if (i >= indices.Count)
                    {
                        break;
                    }

                    AddText(hints[i], allText, indices[i]);
                }
            }

            if (GeneralOptions.UpdateMiscText)
            {
                GiftItem giftItem;
                if (giftRandomizer.ItemDict.TryGetValue(GiftItem.Id.FireMage, out giftItem))
                {
                    var cost = shopRandomizer.StaticPriceDict[DoorId.FireMage].Price;
                    var text = $"{giftItem.Item} for {cost}?";
                    AddText(text, allText, 22);
                }

                if (giftRandomizer.ItemDict.TryGetValue(GiftItem.Id.FortressGuru, out giftItem))
                {
                    var text = $"Have you activated the sky spring? Come back for {giftItem.Item}";
                    AddText(text, allText, 83);
                }

                if (giftRandomizer.ItemDict.TryGetValue(GiftItem.Id.VictimBar, out giftItem))
                {
                    string rank = titles[giftRandomizer.BarRank];
                    ushort experience = titleExperiences[giftRandomizer.BarRank - 1];
                    var text = $"Have you achieved the title '{rank}'? Come back for {giftItem.Item}. You will need {experience} experience";
                    AddText(text, allText, 115);
                }

                if (giftRandomizer.ItemDict.TryGetValue(GiftItem.Id.AceKeyHouse, out giftItem))
                {
                    var text = $"Come back for {giftItem.Item}";
                    AddText(text, allText, 124);
                }

                if (giftRandomizer.ItemDict.TryGetValue(GiftItem.Id.ConflateGuru, out giftItem))
                {
                    var text = $"This guru has {giftItem.Item}";
                    if (Util.GurusShuffled())
                    {
                        string location = "Unknown";
                        foreach (var door in doorRandomizer.Doors.Values)
                        {
                            if (door.Id == DoorId.ConflateGuru)
                            {
                                location = door.OriginalId.ToString();
                                break;
                            }
                        }

                        text = $"This guru has {giftItem.Item} and is at {location}";
                    }

                    AddText(text, allText, 131);
                    AddText(text, allText, 132);
                    AddText(text, allText, 133);
                }

                if (giftRandomizer.ItemDict.TryGetValue(GiftItem.Id.FraternalGuru, out giftItem))
                {
                    var text = $"Do you have the Dragon Slayer? Come back for {giftItem.Item}";
                    AddText(text, allText, 160);
                }

                var mattockText = new List<string>()
                {
                    "MAAAAAAAATTTTTTTTTOOOOOOOOCK!",
                    "I now declare this the Mattock Expressway",
                };

                var hourglassText = new List<string>()
                {
                    "Za Warudo!",
                    "Stop the clock!",
                    "Stop! Hammer Time!",
                };

                var wingbootText = new List<string>()
                {
                    "Looks like Team Rocket is Blasting Off Again!",
                    "With boots I could walk on air.",
                    "This stream sponsored by Red Bull!",
                };

                var poisonText = new List<string>();
                if (ItemOptions.ReplacePoison)
                {
                    poisonText.Add("I'm holding Black Potion");
                    poisonText.Add("I now prossess Black Potion");
                }
                else
                {
                    poisonText.Add("SOAP POISONING");
                    poisonText.Add("I've touched posion");
                }

                var gloveText = new List<string>()
                {
                    "I love the power glove. It's so bad",
                    "If it doesn't fit, you must acquit.",
                    "Hey! You forgot the Power Glove!",
                    "No glove, no love",
                };

                var endingText = new List<string>()
                {
                    "You're winner!",
                    "Congraturation. This story is happy end. Thank you.",
                    "Conglaturation. You have completed a great game and prooved the justice of our culture.",
                    "You've won! You did it! You did it! I knew you would, I just knew you would!",
                    "Winners don't do potions",
                    "Winner winner chicken dinner!",
                    "You have finally managed to defeat the Evil One. But is he really dead? We're not telling! End.",
                    "Worry not my children, for the light that will radiate from me and my flower crown will touch the hearts of all of you as well. My beauty is a beauty to be shared, thank you!!!!!!!"
                };

                AddText(mattockText[random.Next(mattockText.Count)], allText, 170);
                AddText(hourglassText[random.Next(hourglassText.Count)], allText, 171);
                AddText(wingbootText[random.Next(wingbootText.Count)], allText, 172);
                AddText(poisonText[random.Next(poisonText.Count)], allText, 186);
                AddText(gloveText[random.Next(gloveText.Count)], allText, 187);
                AddText(endingText[random.Next(endingText.Count)], allText, 163);

                if (GeneralOptions.DragonSlayerRequired && ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Unchanged)
                {
                    var doorText = new List<string>()
                    {
                        "Do you need a sword to open a door?",
                        "You're not properly dressed for work!",
                    };

                    AddText(doorText[random.Next(doorText.Count)], allText, 37);
                }
                else
                {
                    //Intro text
                    AddText(defaultHint, allText, 37);
                }

                var kingTexts = new List<string>()
                {
                    "Shut up and take my money!",
                };

                AddText(kingTexts[random.Next(kingTexts.Count)], allText, 52);
                AddText(defaultHint, allText, 43); //Eolis guru
                AddText(defaultHint, allText, 86); //Sky fountain
                AddText(defaultHint, allText, 125); //Ace key guy
                AddText(defaultHint, allText, 139); //Conflate guru
                AddText(defaultHint, allText, 161); //Fraternal guru

                foreach (string customText in customTexts)
                {
                    string[] parts = customText.Split(':');
                    var textIndex = int.Parse(parts[0]);
                    AddText(parts[1], allText, textIndex);
                }

                var price = shopRandomizer.StaticPriceDict[DoorId.MartialArtsShop].Price;
                AddText(GetMartialArtsText(price, GetCustomText(customTexts, 24)), allText, 24);

                price = shopRandomizer.StaticPriceDict[DoorId.EolisMagicShop].Price;
                AddText(GetMagicShopText(price, GetCustomText(customTexts, 20)), allText, 20);

                price = shopRandomizer.StaticPriceDict[DoorId.ApoluneHospital].Price;
                AddText(GetHospitalText(price, GetCustomText(customTexts, 27)), allText, 27);
                price = shopRandomizer.StaticPriceDict[DoorId.ForepawHospital].Price;
                AddText(GetHospitalText(price, GetCustomText(customTexts, 28)), allText, 28);
                price = shopRandomizer.StaticPriceDict[DoorId.MasconHospital].Price;
                AddText(GetHospitalText(price, GetCustomText(customTexts, 29)), allText, 29);
                price = shopRandomizer.StaticPriceDict[DoorId.VictimHospital].Price;
                AddText(GetHospitalText(price, GetCustomText(customTexts, 30)), allText, 30);
                price = shopRandomizer.StaticPriceDict[DoorId.ConflateHospital].Price;
                AddText(GetHospitalText(price, GetCustomText(customTexts, 31)), allText, 31);
                price = shopRandomizer.StaticPriceDict[DoorId.DartmoorHospital].Price;
                AddText(GetHospitalText(price, GetCustomText(customTexts, 32)), allText, 32);

                price = shopRandomizer.StaticPriceDict[DoorId.EolisMeatShop].Price;
                AddText(GetMeatShopText(price, GetCustomText(customTexts, 6)), allText, 6);
                price = shopRandomizer.StaticPriceDict[DoorId.ForepawMeatShop].Price;
                AddText(GetMeatShopText(price, GetCustomText(customTexts, 7)), allText, 7);
                price = shopRandomizer.StaticPriceDict[DoorId.MasconMeatShop].Price;
                AddText(GetMeatShopText(price, GetCustomText(customTexts, 8)), allText, 8);
                price = shopRandomizer.StaticPriceDict[DoorId.VictimMeatShop].Price;
                AddText(GetMeatShopText(price, GetCustomText(customTexts, 9)), allText, 9);
                price = shopRandomizer.StaticPriceDict[DoorId.ConflateMeatShop].Price;
                AddText(GetMeatShopText(price, GetCustomText(customTexts, 10)), allText, 10);
                price = shopRandomizer.StaticPriceDict[DoorId.DaybreakMeatShop].Price;
                AddText(GetMeatShopText(price, GetCustomText(customTexts, 11)), allText, 11);
                price = shopRandomizer.StaticPriceDict[DoorId.DartmoorMeatShop].Price;
                AddText(GetMeatShopText(price, GetCustomText(customTexts, 12)), allText, 12);
            }

            int newLength = getLength(allText);
            if (newLength > oldLength)
            {
                return Result.TextTooLong;
            }

            Text.SetAllText(content, allText);
            return Result.Success;
        }

        public void RandomizeTitles(byte[] content, string customTextFile)
        {
            var newTitles = TextOptions.UseCustomText ? GetCustomTitles(customTextFile) : GetNewTitles();

            // if not enough titles are provided, then append existing titles.
            if (newTitles.Count < 16)
            {
                var standardTitles = GetNewTitles();
                Util.ShuffleList(standardTitles, 0, standardTitles.Count - 1, random);
                newTitles.AddRange(standardTitles.GetRange(0, 16 - newTitles.Count));
            }

            Util.ShuffleList(newTitles, 0, newTitles.Count - 1, random);
            titles = newTitles.GetRange(0, titles.Count);
            Text.SetTitles(titles, content);

            RandomizeData(titleExperiences, 2000);
            RandomizeData(titleRewards, 800);
            SetTitleData(titleExperiences, content, Section.GetOffset(15, 0xF749, 0xC000));
            SetTitleData(titleRewards, content, Section.GetOffset(15, 0xF767, 0xC000));
        }

        public List<string> GetHints(ShopRandomizer shopRandomizer, GiftRandomizer giftRandomizer, DoorRandomizer doorRandomizer, bool spoilerLog = false)
        {
            var hints = new List<string>();

            hints.AddRange(GetTowerHints(doorRandomizer, giftRandomizer, spoilerLog));
            hints.AddRange(GetAllSublevelHints(giftRandomizer, spoilerLog));
            hints.AddRange(GetBuildingHints(doorRandomizer, giftRandomizer, spoilerLog));

            if (ItemOptions.RandomizeKeys != ItemOptions.KeyRandomization.Unchanged)
            {
                var levelKeyHints = GetLevelKeyHints(doorRandomizer);
                var exitKeyHints = GetExitKeyHints(doorRandomizer);
                hints.AddRange(levelKeyHints);
                hints.AddRange(exitKeyHints);
            }

            if (GeneralOptions.ShuffleWorlds)
            {
                var worldHints = GetWorldHints(doorRandomizer);
                hints.AddRange(worldHints);
            }

            if (GeneralOptions.HintSetting == GeneralOptions.Hints.Strong &&
                !spoilerLog)
            {
                hints.AddRange(hints);
            }

            return hints;
        }

        public static string GetSuffix(Random random)
        {
            var beginningStrings = new List<string>()
            {
                "Red",
                "Blue",
                "Yellow",
                "Green",
                "White",
                "Black",
                "Gray",
                "Icy",
                "Orange",
                "Purple",
                "Silver",
                "Brown",
                "Cyan",
                "Tan",
                "Teal",
                "Amazing",
                "Awesome",
                "Attractive",
                "Bald",
                "Beautiful",
                "Chubby",
                "Clean",
                "Dazzling",
                "Drab",
                "Elegant",
                "Fancy",
                "Fit",
                "Flabby",
                "Glamorous",
                "Gorgeous",
                "Handsome",
                "Long",
                "Magnificent",
                "Muscular",
                "Plain",
                "Plump",
                "Quaint",
                "Scruffy",
                "Shapely",
                "Short",
                "Skinny",
                "Stocky",
                "Careful",
                "Clever",
                "Famous",
                "Gifted",
                "Hallowed",
                "Helpful",
                "Important",
                "Inexpensive",
                "Odd",
                "Poor",
                "Powerful",
                "Rich",
                "Shy",
                "Aggressive",
                "Ambitious",
                "Brave",
                "Calm",
                "Delightful",
                "Eager",
                "Faithful",
                "Gentle",
                "Happy",
                "Jolly",
                "Kind",
                "Nice",
                "Obedient",
                "Polite",
                "Proud",
                "Silly",
                "Victorious",
                "Witty",
                "Wonderful",
                "Angry",
                "Bewildered",
                "Clumsy",
                "Embarrassed",
                "Embarrassing",
                "Fierce",
                "Grumpy",
                "Itchy",
                "Jealous",
                "Lazy",
                "Mysterious",
                "Nervous",
                "Obnoxious",
                "Pitiful",
                "Repulsive",
                "Scary",
                "Thoughtless",
                "Worried",
                "Big",
                "Colossal",
                "Gigantic",
                "Great",
                "Huge",
                "Immense",
                "Large",
                "Massive",
                "Microscopic",
                "Short",
                "Small",
                "Tall",
                "Tiny",
                "Ancient",
                "Early",
                "Fast",
                "Long",
                "Modern",
                "Old",
                "Prehistoric",
                "Quick",
                "Slow",
                "Swift",
                "Young",
                "Acidic",
                "Bitter",
                "Delicious",
                "Fresh",
                "Greasy",
                "Juicy",
                "Hot",
                "Moldy",
                "Salty",
                "Sour",
                "Spicy",
                "Sweet",
                "Tasteless",
                "Tasty",
                "Yummy",
                "Chilly",
                "Cold",
                "Cool",
                "Damaged",
                "Damp",
                "Dirty",
                "Dry",
                "Fluffy",
                "Freezing",
                "Greasy",
                "Icy",
                "Sharp",
                "Slimy",
                "Sticky",
                "Strong",
                "Warm",
                "Weak",
                "Wet",
                "Wooden",
                "Metal",
                "Metallic",
                "Smart",
                "Wise",
                "Negative",
                "Positive",
                "Chaotic",
                "Shining",
                "Evil",
                "Good",
                "Neutral",
                "Dark",
                "Shadow",
                "Royal",
                "Poisonous",
                "Confused",
                "Surprised",
                "Bizarre",
                "Heavenly",
                "Holy",
                "Funny",
                "Hilarious",
                "Talented",
                "Skilled",
                "Weird",
                "Shady",
                "Tricky",
                "Adamant",
                "Bold",
                "Crazy",
                "Deadly",
                "Exotic",
                "Frantic",
                "Giant",
                "Heroic",
                "Imperial",
                "Solid",
                "Unexpected",
            };

            var middleStrings = new List<string>()
            {
                "Eolis",
                "Trunk",
                "Mist",
                "Branch",
                "Dartmoor",
                "Zenis",
                "Apolune",
                "Forepaw",
                "Mascon",
                "Victim",
                "Conflate",
                "Daybreak",
                "Elven",
                "Dwarven",
                "Guru",
                "Smoker",
                "Tundra",
                "Chaos",
                "Cardinals",
                "Zelda",
                "Link",
                "Mario",
                "Luigi",
                "Dragon",
                "Duck",
                "Metroid",
                "Monodron",
                "Wyvern",
                "Grieve",
                "King",
                "Queen",
                "One",
                "ShadowEura",
                "Clown",
                "Arctic",
                "Australian",
                "Asian",
                "African",
                "American",
                "European",
                "British",
                "Canadian",
                "Norwegian",
                "Swedish",
                "Danish",
                "Dutch",
                "French",
                "German",
                "Italian",
                "Spanish",
                "Stardust",
                "Time",
                "Mega",
                "Killer",
                "Skeleton",
                "Mantra",
                "Fencer",
                "Titan",
                "Alien",
                "Cheese",
                "Onion",
                "Moogle",
                "Chocobo",
                "Ghost",
                "Pirate",
                "Zombie",
                "Giant",
                "Snow",
                "Rock",
                "Death",
                "Deluge",
                "Fire",
                "Thunder",
                "Tilte",
                "Magic",
                "Monster",
                "Snake",
            };

            var endingStrings = new List<string>()
            {
                "Poison",
                "Posion",
                "Elixir",
                "Ointment",
                "Thoughts",
                "Smoke",
                "Tundra",
                "Bat",
                "Bee",
                "Beer",
                "Creature",
                "Cricket",
                "Dog",
                "Duck",
                "Dragon",
                "Giant",
                "Hornet",
                "Lobster",
                "Monodron",
                "Monster",
                "Naga",
                "Slug",
                "Strider",
                "Snowman",
                "Wolfman",
                "Wyvern",
                "Zombie",
                "Grieve",
                "King",
                "Queen",
                "Prince",
                "Capitalist",
                "Communist",
                "Dictator",
                "Emperor",
                "Clown",
                "Slime",
                "Adventure",
                "Diver",
                "Crusader",
                "Hero",
                "Stopper",
                "Man",
                "Knight",
                "Pokemon",
                "Charmander",
                "Squirtle",
                "Bulbasaur",
                "Mudkip",
                "Piplup",
                "Slowpoke",
                "Skeleton",
                "Mantra",
                "Key",
                "Fencer",
                "Titan",
                "Alien",
                "Cheese",
                "Onion",
                "Moogle",
                "Chocobo",
                "Warrior",
                "Quest",
                "Zelda",
                "Link",
                "Mario",
                "Luigi",
                "Ghost",
                "Pirate",
                "Cook",
                "Guy",
                "Inquisition",
                "Snake",
                "Swallow",
                "Magic",
                "Party",
            };

            foreach (var id in Enum.GetValues(typeof(ShopRandomizer.Id)))
            {
                endingStrings.Add(id.ToString());
            }

            string suffix = "_";
            suffix += beginningStrings[random.Next(beginningStrings.Count)];
            suffix += middleStrings[random.Next(middleStrings.Count)];
            suffix += endingStrings[random.Next(endingStrings.Count)];
            return suffix;
        }

        public List<string> GetTitleData()
        {
            var titleData = new List<string>();
            titleData.Add(titles[0] + " 0" + " 0");
            for (int i = 1; i < titles.Count; i++)
            {
                titleData.Add($"{titles[i]} {titleExperiences[i - 1]} {titleRewards[i - 1]}");
            }

            return titleData;
        }

        private void RandomizeData(List<ushort> dataList, int max)
        {
            int min = 1;
            int data = 0;
            int increment = max / 10;
            for (int i = 0; i < dataList.Count; i++)
            {
                data += random.Next(min, max + 1);
                data = Math.Min(data, ushort.MaxValue);
                dataList[i] = (ushort)data;
                max += increment;
                increment = (int)(increment * 1.1);
            }
        }

        private List<ushort> GetTitleData(byte[] content, int offset)
        {
            List<ushort> data = new List<ushort>();
            for (int i = 0; i < (numberOfTitles - 1) * 2; i+=2)
            {
                data.Add(BitConverter.ToUInt16(content, offset + i));
            }

            return data;
        }

        private void SetTitleData(List<ushort> data, byte[] content, int offset)
        {
            for (int i = 0; i < data.Count; i++)
            {
                var bytes = BitConverter.GetBytes(data[i]);
                content[offset + i * 2] = bytes[0];
                content[offset + i * 2 + 1] = bytes[1];
            }
        }

        private List<string> GetCustomTitles(string customTextFile)
        {
            string[] customTextFileLines = File.ReadAllLines(customTextFile); 

            var customTitles = new List<string>() {};
            foreach (string line in customTextFileLines)
            {
                if (line.Trim().StartsWith("title:")) {
                    customTitles.Add(line.Remove(0,6));
                }
            }

            return customTitles;
        }

        private List<string> GetNewTitles()
        {
            return new List<string>()
            {
                "Weiner",
                "Dufus",
                "Poindexter",
                "Peanut",
                "Dude",
                "Bro",
                "Homey",
                "RapMaster",
                "FunkLord",
                "Dawg",
                "Playa",
                "BlingMaster",
                "SpinMaster",
                "CoolCat",
                "FunkMeister",
                "Speedrunner",
                "Slowrunner",
                "Gamer",
                "RetroGamer",
                "OG",
                "EvilOne",
                "EvilTwo",
                "NeutralOne",
                "GoodOne",
                "Boomer",
                "Zoomer",
                "Pirate",
                "AssPirate",
                "Nerd",
                "SuperNerd",
                "Weeb",
                "CardinalsFan",
                "CardinalsHater",
                "AnimeFan",
                "AnimeHater",
                "Chad",
                "Incel",
                "PirateKing",
                "SpacePirate",
                "SpaceMarine",
                "FaxanaDude",
                "Dwarf",
                "DwarfSlayer",
                "DragonSlayer",
                "Guru",
                "Dweeb",
                "Stoner",
                "SDustCrusader",
                "StandUser",
                "OnionKnight",
                "Freelancer",
                "Thief",
                "Ninja",
                "Esper",
                "Chocobo",
                "Cultist",
                "KefkaCultist",
                "Returner",
                "DragonWarrior",
                "RandoRacer",
                "RandoDev",
                "Streamer",
                "Clown",
                "Donald",
                "Ronald",
                "OfficeWorker",
                "Plumber",
                "Janitor",
                "Baker",
                "Butcher",
                "Cook",
                "MeatEater",
                "King",
                "CrimsonKing",
                "KillerQueen",
                "PokemonTrainer",
                "PokemonChamp",
                "Pokemon",
                "Charmander",
                "Squirtle",
                "Bulbasaur",
                "Mudkip",
                "Piplup",
                "MissingNo",
                "MegaMan",
                "BatMan",
                "SpiderMan",
                "SuperMan",
                "Titan",
                "Ghost",
                "GhostBuster",
                "GhostPirate",
                "PirateGhost",
                "Angel",
                "ArchAngel",
                "HolyDiver",
                "FamilyMan",
                "Vampire",
                "Count",
                "VampireHunter",
                "VampireKiller",
                "HeroOfTime",
                "Link",
                "Zelda",
                "Gibdo",
                "Goriya",
                "Moblin",
                "Stalfos",
                "Darknut",
                "IronKnuckle",
                "Goblin",
                "Ogre",
                "Orc",
                "Troll",
                "GrandVizier",
                "PerfectMan",
                "PillarMan",
                "OneStabMan",
                "Archer",
                "Lancer",
                "Planeswalker",
                "Leekspinner",
                "AVGN",
                "Jedi",
                "Sith",
                "MarioBrother",
                "Mario",
                "Luigi",
                "Bowser",
                "Goomba",
                "KoopaTroopa",
                "KoopaKing",
                "KingKoopa",
                "Koopaling",
                "Yoshi",
                "Cannibal",
                "Peasant",
                "Peon",
                "Plebeian",
                "Pleb",
                "StreetFighter",
                "Dictator",
                "Lannister",
                "Capitalist",
                "Communist",
                "Saiyan",
                "SaiyanPrince",
                "SuperSaiyan",
                "MajorGeneral",
                "EdgeLord",
                "CringeLord",
                "ShadowEura",
                "Nothing",
                "NoTitle",
                "Error",
                "President",
                "ElPresidente",
                "NinjaTurtle",
                "Shredder",
                "Hippie",
                "Wyvern",
                "Sentinel",
                "MangoSentinel",
                "Priest",
                "Inquisitor",
                "Emperor",
                "GodEmperor",
                "Monster",
                "Santa",
                "SantaClaus",
                "RobotSanta",
                "Elf",
                "Grinch",
            };
        }

        private string GetCustomText(List<string> customTexts, int idNumber)
        {
            var customText = "";
            foreach (string text in customTexts)
            {
                string[] parts = text.Split(':');

                if (int.Parse(parts[0]) == idNumber)
                {
                    customText = parts[1];
                    break;
                }
            }

            return customText;
        }

        private string GetHospitalText(ushort price, string customText)
        {
            return customText.Length > 0 ? string.Format(customText, price) : $"{price}?";;
        }

        private string GetMeatShopText(ushort price, string customText)
        {
            return customText.Length > 0 ? string.Format(customText, price) : $"Eat my meat for {price}?";
        }

        private string GetMartialArtsText(ushort price, string customText)
        {
            return customText.Length > 0 ? string.Format(customText, price) : $"Martial arts for {price}?";
        }

        private string GetMagicShopText(ushort price, string customText)
        {
            return customText.Length > 0 ? string.Format(customText, price) : $"Magic for {price}?";
        }

        private int getLength(List<string> texts)
        {
            int length = 0;
            foreach (var text in texts)
            {
                length += text.Length;
            }
            return length;
        }

        private void AddText(string text, List<string> texts, int index)
        {
            text = InsertLineBreaks(text);
            texts[index] = text + Text.endOfTextChar;
        }

        private string AddLine(string text, List<string> lines, int index, int previousIndex)
        {
            var length = index - previousIndex;
            
            var line = text.Substring(previousIndex, length);
            var paddingMax = length > 14 ? 16 - length : 2;
            var padding = "".PadRight(paddingMax < 0 ? 0 : paddingMax, ' ');

            // bug: sometimes an extra line appears...
            // probably not that big of a deal though.

            if ((index >= 0) && (index < text.Length) && text[index] == '|')
                line = line + Text.lineBreakWithPauseChar + padding;

            return line;
        }

        private string InsertLineBreaks(string text)
        {
            var indices = new List<int>();
            if (TextOptions.UseCustomText)
            {
                for (int i = 16; i < text.Length; i += 16)
                {
                    bool indexed = false;
                    int j = i - 15;
                    int k = i;
                    int highest_j = 0;

                    while (j < k)
                    {
                        if (text[j] == '|')
                        {
                            indices.Add(j);
                            indexed = true;
                            highest_j = j;
                            // don't break as there could be more than one.
                        }

                        j++;
                    }

                    if (indexed == true)
                    {
                        i = highest_j;
                        continue;
                    }

                    j = i;
                    k = j - 16;

                    while (j > k)
                    {
                        if (text[j] == ' ')
                        {
                            indices.Add(j);
                            i = j;
                            break;
                        }

                        j--;
                    }
                }

                var lines = new List<string>();
                int previousIndex = 0;

                foreach (var index in indices)
                {
                    var line = AddLine(text, lines, index, previousIndex);
                    lines.Add(line + Text.lineBreakChar.ToString());
                    previousIndex = index + 1; // add one to skip the space or pausebreak
                }

                var lastLine = AddLine(text, lines, text.Length, previousIndex);
                lines.Add(lastLine);

                var newText = string.Join("", lines);

                return newText;
            }
            else
            {

                for (int i = 16; i < text.Length; i += 16)
                {
                    int j = i;
                    int k = j - 16;
                    while (j > k)
                    {
                        if (text[j] == ' ')
                        {
                            indices.Add(j);
                            i = j;
                            break;
                        }

                        j--;
                    }
                }

                var newText = text;
                foreach (var index in indices)
                {
                    newText = newText.Substring(0, index) + Text.lineBreakChar + text.Substring(index + 1);
                }

                return newText;
            }
        }

        private List<string> GetAllSublevelHints(GiftRandomizer giftRandomizer, bool spoilerLog)
        {
            var hints = new List<string>();
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.EarlyTrunk], giftRandomizer, spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.MiddleTrunk], giftRandomizer, spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.LateTrunk], giftRandomizer, spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.EastTrunk], giftRandomizer, spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.EarlyMist], giftRandomizer, spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.MiddleMist], giftRandomizer, spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.LateMist], giftRandomizer, spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.EarlyBranch], giftRandomizer, spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.MiddleBranch], giftRandomizer, spoilerLog));
            if (!Util.AllWorldScreensRandomized())
            {
                hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.DropDownWing], giftRandomizer, spoilerLog));
                hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.BackFromEastBranch], giftRandomizer, spoilerLog));
                hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.EastBranch], giftRandomizer, spoilerLog));
            }

            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.Dartmoor], giftRandomizer, spoilerLog));
            if (!(GeneralOptions.ShuffleTowers && GeneralOptions.IncludeEvilOnesFortress))
            {
                hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.EvilOnesLair], giftRandomizer, spoilerLog));
            }
            return hints;
        }

        private List<string> GetLevelKeyHints(DoorRandomizer doorRandomizer)
        {
            var hints = new List<string>();
            hints.Add(GetLevelKeyHint(doorRandomizer.TowerDoors[DoorId.TowerOfTrunk], doorRandomizer));
            hints.Add(GetLevelKeyHint(doorRandomizer.TowerDoors[DoorId.TowerOfFortress], doorRandomizer));
            hints.Add(GetLevelKeyHint(doorRandomizer.TowerDoors[DoorId.JokerHouse], doorRandomizer));
            hints.Add(GetLevelKeyHint(doorRandomizer.TowerDoors[DoorId.MasconTower], doorRandomizer));
            hints.Add(GetLevelKeyHint(doorRandomizer.TowerDoors[DoorId.TowerOfSuffer], doorRandomizer));
            hints.Add(GetLevelKeyHint(doorRandomizer.TowerDoors[DoorId.VictimTower], doorRandomizer));
            hints.Add(GetLevelKeyHint(doorRandomizer.TowerDoors[DoorId.TowerOfMist], doorRandomizer));
            hints.Add(GetLevelKeyHint(doorRandomizer.TowerDoors[DoorId.BattleHelmetWing], doorRandomizer));
            hints.Add(GetLevelKeyHint(doorRandomizer.Doors[DoorId.EastBranch], doorRandomizer));
            hints.Add(GetLevelKeyHint(doorRandomizer.TowerDoors[DoorId.CastleFraternal], doorRandomizer));
            hints.Add(GetLevelKeyHint(doorRandomizer.LevelDoors[DoorId.DartmoorLateDoor], doorRandomizer));
            hints.Add(GetLevelKeyHint(doorRandomizer.TowerDoors[DoorId.KingGrieve], doorRandomizer));
            if (GeneralOptions.ShuffleTowers && GeneralOptions.IncludeEvilOnesFortress)
            {
                hints.Add(GetLevelKeyHint(doorRandomizer.TowerDoors[DoorId.EvilOnesLair], doorRandomizer));
            }

            if (Util.AllWorldScreensRandomized())
            {
                hints.Add(GetLevelKeyHint(doorRandomizer.Doors[DoorId.DropdownWing], doorRandomizer));
                hints.Add(GetLevelKeyHint(doorRandomizer.Doors[DoorId.EastBranchLeft], doorRandomizer));
            }

            if (GeneralOptions.WorldDoorSetting == GeneralOptions.WorldDoors.ShuffleMoveKeys)
            {
                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.TrunkSecretShop], doorRandomizer));
                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.FortressGuru], doorRandomizer));

                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.FireMage], doorRandomizer));
                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.MistSmallHouse], doorRandomizer));
                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.MistGuru], doorRandomizer));
                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.BirdHospital], doorRandomizer));
                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.MistSecretShop], doorRandomizer));
                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.MistLargeHouse], doorRandomizer));
                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.AceKeyHouse], doorRandomizer));

                if (!(GeneralOptions.ShuffleTowers && GeneralOptions.IncludeEvilOnesFortress))
                {
                    hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.DartmoorHouse1], doorRandomizer));
                }

                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.DartmoorHouse2], doorRandomizer));
                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.DartmoorHouse3], doorRandomizer));
                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.DartmoorHouse4], doorRandomizer));
                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.LeftOfDartmoor], doorRandomizer));
                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.FarLeftDartmoor], doorRandomizer));
                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.FraternalGuru], doorRandomizer));
                hints.Add(GetLevelKeyHint(doorRandomizer.Buildings[DoorId.FraternalHouse], doorRandomizer));
            }

            return hints;
        }

        private string GetLevelKeyHint(Door door, DoorRandomizer doorRandomizer)
        {
            var item = doorRandomizer.GetLevelKey(door);
            string hint = $"{door.OriginalId} requires {item}";
            if (item == ShopRandomizer.Id.Book)
            {
                hint = $"{door.OriginalId} requires nothing";
            }
            return hint;
        }

        private List<string> GetExitKeyHints(DoorRandomizer doorRandomizer)
        {
            var hints = new List<string>();
            hints.Add(GetExitKeyHint(DoorRandomizer.ExitDoor.TrunkExit, doorRandomizer));
            hints.Add(GetExitKeyHint(DoorRandomizer.ExitDoor.MistExit, doorRandomizer));
            hints.Add(GetExitKeyHint(DoorRandomizer.ExitDoor.BranchExit, doorRandomizer));
            if (GeneralOptions.MoveFinalRequirements)
            {
                hints.Add(GetExitKeyHint(DoorRandomizer.ExitDoor.DartmoorExit, doorRandomizer));
            }
            return hints;
        }

        private string GetExitKeyHint(DoorRandomizer.ExitDoor door, DoorRandomizer doorRandomizer)
        {
            var item = doorRandomizer.GetExitKey(door);
            if (item == ShopRandomizer.Id.Book)
            {
                return $"{door} requires nothing";
            }

            string hint = $"{door} requires {item}";
            return hint;
        }

        private List<string> GetWorldHints(DoorRandomizer doorRandomizer)
        {
            var hints = new List<string>();
            hints.Add(GetWorldHint(1, doorRandomizer));
            hints.Add(GetWorldHint(2, doorRandomizer));
            if (!GeneralOptions.QuickSeed || (GeneralOptions.ShuffleTowers && GeneralOptions.IncludeEvilOnesFortress))
            {
                hints.Add(GetWorldHint(3, doorRandomizer));
                hints.Add(GetWorldHint(4, doorRandomizer));
            }

            return hints;
        }

        private string GetWorldHint(int index, DoorRandomizer doorRandomizer)
        {
            var worlds = doorRandomizer.GetWorlds();
            string source = worlds[index].number.ToString();
            string destination = worlds[index].forward.number.ToString();

            string hint = $"{source} leads to {destination}";
            return hint;
        }

        private List<string> GetTowerHints(DoorRandomizer doorRandomizer, GiftRandomizer giftRandomizer, bool spoilerLog)
        {
            var hints = new List<string>();
            foreach (var key in doorRandomizer.TowerDoors.Keys)
            {
                hints.AddRange(GetDoorHints(doorRandomizer.TowerDoors[key].OriginalId, doorRandomizer.TowerDoors[key], giftRandomizer, spoilerLog));
            }

            return hints;
        }

        private List<string> GetBuildingHints(DoorRandomizer doorRandomizer, GiftRandomizer giftRandomizer, bool spoilerLog)
        {
            var hints = new List<string>();
            foreach (var key in doorRandomizer.Buildings.Keys)
            {
                var building = doorRandomizer.Buildings[key];
                hints.AddRange(GetDoorHints(building.OriginalId, building, giftRandomizer, spoilerLog));
            }

            foreach (var key in doorRandomizer.TownDoors.Keys)
            {
                hints.AddRange(GetDoorHints(key, doorRandomizer.TownDoors[key], giftRandomizer, spoilerLog));
            }

            return hints;
        }

        private List<string> GetDoorHints(DoorId oldDoor, Door door, GiftRandomizer giftRandomizer, bool spoilerLog)
        {
            var hints = new List<string>();
            string locationHint;
            if (Util.AllWorldScreensRandomized() &&
                door.ParentSublevel != null)
            {
                locationHint = $" in {door.ParentSublevel.SubLevelId}";
            }
            else
            {
                locationHint = "";
            }

            if (door.Gift != null)
            {
                string hint = $"{door.Id} has {door.Gift.Item}";
                hint = GetTail(hint, oldDoor, door.ShouldShuffle, locationHint);
                hints.Add(hint);
            }
            if (door.Sublevel != null)
            {
                var items = GetSublevelItems(door.Sublevel, spoilerLog);
                bool weakHints = GeneralOptions.HintSetting == GeneralOptions.Hints.Weak && !spoilerLog;
                foreach (var item in items)
                {
                    string hint = $"{door.Id} has {item}";
                    if (!weakHints)
                    {
                        hint = GetTail(hint, oldDoor, door.ShouldShuffle, locationHint);
                    }

                    hints.Add(hint);
                }

                if (weakHints)
                {
                    string prefix = $"{door.Id} is";
                    string hint = GetTail(prefix, oldDoor, door.ShouldShuffle, "");
                    if (hint != prefix)
                    {
                        hints.Add(hint);
                    }
                }

                foreach (var screen in door.Sublevel.Screens)
                {
                    foreach (var gift in screen.Gifts)
                    {
                        var hint = $"{gift} has {giftRandomizer.ItemDict[gift].Item} in {door.Sublevel.SubLevelId}";
                        if(GeneralOptions.WorldDoorSetting != GeneralOptions.WorldDoors.Unchanged ||
                           GeneralOptions.ShuffleTowers)
                        {
                            hint += $" at {oldDoor}";
                        }

                        hints.Add(hint);
                    }
                }

                void GetSpringHint(string spring, List< string> springHints)
                {
                    var hint = $"{spring} is in {door.Sublevel.SubLevelId} at {oldDoor}";
                    springHints.Add(hint);
                }

                if (Util.AllWorldScreensRandomized())
                {
                    if (SubLevel.FortressSpringSublevel == door.Sublevel.SubLevelId)
                    {
                        GetSpringHint("FortressSpring", hints);
                    }

                    if (SubLevel.SkySpringSublevel == door.Sublevel.SubLevelId)
                    {
                        GetSpringHint("SkySpring", hints);
                    }
                }
            }
            if (door.BuildingShop != null)
            {
                var items = GetShopItems(door.BuildingShop, spoilerLog);
                foreach (var item in items)
                {
                    string hint = $"{door.Id} has {item}";
                    hint = GetTail(hint, door.OriginalId, door.ShouldShuffle, locationHint);
                    hints.Add(hint);
                }
            }

            return hints;
        }

        private string GetTail(string hint, DoorId oldDoor, bool shouldShuffle, string locationHint)
        {
            hint += locationHint;
            if (shouldShuffle)
            {
                hint += $" at {oldDoor}";
            }

            return hint;
        }

        private List<string> GetSublevelItems(SubLevel sublevel, bool spoilerLog)
        {
            var itemTexts = new List<string>();
            var items = sublevel.CollectItems();
            foreach (var item in items)
            {
                if (GeneralOptions.HintSetting == GeneralOptions.Hints.Strong &&
                    !spoilerLog && !(items.Count == 1 ||
                                     Sprite.KeyItems.Contains(item.Id)))
                {
                    continue;
                }

                if (itemDict.ContainsKey(item.Id))
                {
                    string itemText = itemDict[item.Id];
                    if (ItemOptions.ReplacePoison &&
                        itemText == "Poison")
                    {
                        itemText = "BlackPotion";
                    }

                    if (itemText == "Glove2")
                    {
                        if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Mixed)
                        {
                            itemText = "JokerKey";
                        }
                        else
                        {
                            itemText = "Glove";
                        }
                    }

                    if (itemText == "Mattock2")
                    {
                        if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Mixed)
                        {
                            itemText = "RubyRing";
                        }
                        else
                        {
                            itemText = "Mattock";
                        }
                    }

                    itemTexts.Add(itemText);
                }
            }

            return itemTexts;
        }

        private List<string> GetShopItems(Shop shop, bool spoilerLog)
        {
            var items = new List<string>();
            if (!spoilerLog && (shop.ShopId == Shop.Id.EolisItemShop ||
                                shop.ShopId == Shop.Id.EolisKeyShop))
            {
                return items;
            }

            if (!spoilerLog && GeneralOptions.MiscDoorSetting != GeneralOptions.MiscDoors.ShuffleIncludeTowns &&
               (shop.ShopId == Shop.Id.ApoluneKeyShop ||
                shop.ShopId == Shop.Id.ApoluneItemShop))
            {
                return items;
            }

            if (!spoilerLog && GeneralOptions.MiscDoorSetting == GeneralOptions.MiscDoors.Unchanged &&
                shop.ShopId == Shop.Id.ApoluneSecretShop)
            {
                return items;
            }

            foreach (var item in shop.Items)
            {
                if (ItemOptions.RandomizeKeys == ItemOptions.KeyRandomization.Unchanged)
                {
                    if (shop.IsKeyShop)
                    {
                        continue;
                    }
                }

                if (!(item.Id == ShopRandomizer.Id.RedPotion))
                {
                    if (GeneralOptions.HintSetting == GeneralOptions.Hints.Strong &&
                        !spoilerLog)
                    {
                        if (!ShopRandomizer.KeyItems.Contains(item.Id))
                        {
                            continue;
                        }
                    }

                    items.Add(item.Id.ToString());
                }
            }

            return items;
        }

        private List<string> GetSublevelHints(SubLevel sublevel, GiftRandomizer giftRandomizer, bool spoilerLog)
        {
            var hints = new List<string>();
            var items = GetSublevelItems(sublevel, spoilerLog);
            foreach (var item in items)
            {
                hints.Add($"{sublevel.SubLevelId} has {item}");
            }

            foreach (var screen in sublevel.Screens)
            {
                foreach (var gift in screen.Gifts)
                {
                    hints.Add($"{gift} has {giftRandomizer.ItemDict[gift].Item} and is in {sublevel.SubLevelId}");
                }
            }

            if (Util.AllWorldScreensRandomized())
            {
                if (SubLevel.FortressSpringSublevel == sublevel.SubLevelId)
                {
                    hints.Add($"FortressSpring is in {sublevel.SubLevelId}");
                }

                if (SubLevel.SkySpringSublevel == sublevel.SubLevelId)
                {
                    hints.Add($"SkySpring is in {sublevel.SubLevelId}");
                }
            }

            return hints;
        }
    }
}
