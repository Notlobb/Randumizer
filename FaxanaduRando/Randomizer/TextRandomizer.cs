using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    class TextRandomizer
    {
        private Random random;

        private static readonly List<string> ranks = new List<string>
        {
            "Novice",
            "Aspirant",
            "Battler",
            "Fighter",
            "Adept",
            "Chevalier",
            "Veteran",
            "Warrior",
            "Swordman",
            "Hero",
            "Soldier",
            "Myrmidon",
            "Champion",
            "Superhero",
            "Paladin",
            "Lord",
        };

        private static readonly Dictionary<Sprite.SpriteId, string> itemDict = new Dictionary<Sprite.SpriteId, string>
        {
            {Sprite.SpriteId.Elixir, "Elixir" },
            {Sprite.SpriteId.HourGlass, "Hourglass" },
            {Sprite.SpriteId.Wingboots, "Wingboots" },
            {Sprite.SpriteId.WingbootsBossLocked, "Wingboots" },
            {Sprite.SpriteId.BlackOnyx, "Black Onyx" },
            {Sprite.SpriteId.BattleHelmet, "BattleHelmet" },
            {Sprite.SpriteId.BattleSuit, "BattleSuit" },
            {Sprite.SpriteId.DragonSlayer, "DragonSlayer" },
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

        public TextRandomizer(Random random)
        {
            this.random = random;
        }

        public bool UpdateText(ShopRandomizer shopRandomizer, GiftRandomizer giftRandomizer, DoorRandomizer doorRandomizer, byte[] content)
        {
            var allText = Text.GetAllText(content);
            int oldLength = getLength(allText);

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
                    "Hi",
                    "Ni",
                    "Stand back. I just farted... Sorry",
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
                    "Shoutout to Tundra83",
                    "Shoutout to Cha0sFinale",
                    "Shoutout to ShinerCCC",
                    "Shoutout to LoZCardsfan23",
                    "Shoutout to OdinSpack",
                    "Shoutout to MeowthRocket",
                };

                Util.ShuffleList(communityHints, 0, communityHints.Count - 1, random);

                if (GeneralOptions.HintSetting == GeneralOptions.Hints.Community)
                {
                    hints.AddRange(communityHints);
                }

                int index = 0;
                int communityHintCount = GeneralOptions.HintSetting == GeneralOptions.Hints.Strong ? 2 : 10;
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
                    hints.Add("Hi");
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
                    string rank = ranks[giftRandomizer.BarRank];
                    var text = $"Are you a {rank}? Come back for {giftItem.Item}";
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
                    poisonText.Add("ETHEL'S BEANS");
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
                    AddText("Hi", allText, 37);
                }

                var kingTexts = new List<string>()
                {
                    "Shut up and take my money!",
                };

                AddText(kingTexts[random.Next(kingTexts.Count)], allText, 52);
                AddText("Hi", allText, 86); //Sky fountain
                AddText("Hi", allText, 125); //Ace key guy
                AddText("Hi", allText, 139); //Conflate guru
                AddText("Hi", allText, 161); //Fraternal guru

                var price = shopRandomizer.StaticPriceDict[DoorId.MartialArtsShop].Price;
                AddText($"Martial arts for {price}?", allText, 24);

                price = shopRandomizer.StaticPriceDict[DoorId.EolisMagicShop].Price;
                AddText($"Magic for {price}?", allText, 20);

                price = shopRandomizer.StaticPriceDict[DoorId.ApoluneHospital].Price;
                AddText(GetHospitalText(price), allText, 27);
                price = shopRandomizer.StaticPriceDict[DoorId.ForepawHospital].Price;
                AddText(GetHospitalText(price), allText, 28);
                price = shopRandomizer.StaticPriceDict[DoorId.MasconHospital].Price;
                AddText(GetHospitalText(price), allText, 29);
                price = shopRandomizer.StaticPriceDict[DoorId.VictimHospital].Price;
                AddText(GetHospitalText(price), allText, 30);
                price = shopRandomizer.StaticPriceDict[DoorId.ConflateHospital].Price;
                AddText(GetHospitalText(price), allText, 31);
                price = shopRandomizer.StaticPriceDict[DoorId.DartmoorHospital].Price;
                AddText(GetHospitalText(price), allText, 32);

                price = shopRandomizer.StaticPriceDict[DoorId.EolisMeatShop].Price;
                AddText(GetMeatShopText(price), allText, 6);
                price = shopRandomizer.StaticPriceDict[DoorId.ForepawMeatShop].Price;
                AddText(GetMeatShopText(price), allText, 7);
                price = shopRandomizer.StaticPriceDict[DoorId.MasconMeatShop].Price;
                AddText(GetMeatShopText(price), allText, 8);
                price = shopRandomizer.StaticPriceDict[DoorId.VictimMeatShop].Price;
                AddText(GetMeatShopText(price), allText, 9);
                price = shopRandomizer.StaticPriceDict[DoorId.ConflateMeatShop].Price;
                AddText(GetMeatShopText(price), allText, 10);
                price = shopRandomizer.StaticPriceDict[DoorId.DaybreakMeatShop].Price;
                AddText(GetMeatShopText(price), allText, 11);
                price = shopRandomizer.StaticPriceDict[DoorId.DartmoorMeatShop].Price;
                AddText(GetMeatShopText(price), allText, 12);
            }

            int newLength = getLength(allText);
            if (newLength > oldLength)
            {
                return false;
            }

            Text.SetAllText(content, allText);
            return true;
        }

        private string GetHospitalText(ushort price)
        {
            return $"{price}?";
        }

        private string GetMeatShopText(ushort price)
        {
            return $"Eat my meat for {price}?";
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

        private string InsertLineBreaks(string text)
        {
            var indices = new List<int>();

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

        public List<string> GetHints(ShopRandomizer shopRandomizer, GiftRandomizer giftRandomizer, DoorRandomizer doorRandomizer, bool spoilerLog=false)
        {
            var hints = new List<string>();

            hints.AddRange(GetTowerHints(doorRandomizer, spoilerLog));
            hints.AddRange(GetAllSublevelHints(spoilerLog));
            hints.AddRange(GetBuildingHints(doorRandomizer, spoilerLog));

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

        private List<string> GetAllSublevelHints(bool spoilerLog)
        {
            var hints = new List<string>();
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.EarlyTrunk], spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.EastTrunk], spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.EarlyMist], spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.LateMist], spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.EarlyBranch], spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.MiddleBranch], spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.DropDownWing], spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.EastBranch], spoilerLog));
            hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.Dartmoor], spoilerLog));
            if (!GeneralOptions.ShuffleTowers)
            {
                hints.AddRange(GetSublevelHints(SubLevel.SubLevelDict[SubLevel.Id.EvilOnesLair], spoilerLog));
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
            hints.Add(GetLevelKeyHint(doorRandomizer.LevelDoors[DoorId.EastBranch], doorRandomizer));
            hints.Add(GetLevelKeyHint(doorRandomizer.TowerDoors[DoorId.CastleFraternal], doorRandomizer));
            hints.Add(GetLevelKeyHint(doorRandomizer.LevelDoors[DoorId.DartmoorLateDoor], doorRandomizer));
            hints.Add(GetLevelKeyHint(doorRandomizer.TowerDoors[DoorId.KingGrieve], doorRandomizer));
            if (GeneralOptions.ShuffleTowers && GeneralOptions.IncludeEvilOnesFortress)
            {
                hints.Add(GetLevelKeyHint(doorRandomizer.TowerDoors[DoorId.EvilOnesLair], doorRandomizer));
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

        private List<string> GetTowerHints(DoorRandomizer doorRandomizer, bool spoilerLog)
        {
            var hints = new List<string>();
            foreach (var key in doorRandomizer.TowerDoors.Keys)
            {
                hints.AddRange(GetDoorHints(doorRandomizer.TowerDoors[key].OriginalId, doorRandomizer.TowerDoors[key], spoilerLog));
            }

            return hints;
        }

        private List<string> GetBuildingHints(DoorRandomizer doorRandomizer, bool spoilerLog)
        {
            var hints = new List<string>();
            foreach (var key in doorRandomizer.Buildings.Keys)
            {
                var building = doorRandomizer.Buildings[key];
                hints.AddRange(GetDoorHints(building.OriginalId, building, spoilerLog));
            }

            foreach (var key in doorRandomizer.TownDoors.Keys)
            {
                hints.AddRange(GetDoorHints(key, doorRandomizer.TownDoors[key], spoilerLog));
            }

            return hints;
        }

        private List<string> GetDoorHints(DoorId oldDoor, Door door, bool spoilerLog)
        {
            var hints = new List<string>();
            if (door.Gift != null)
            {
                string hint = $"{door.Id} has {door.Gift.Item}";
                hint = GetTail(hint, oldDoor, door.ShouldShuffle, true);
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
                        hint = GetTail(hint, oldDoor, door.ShouldShuffle, true);
                    }

                    hints.Add(hint);
                }

                if (weakHints)
                {
                    string prefix = $"{door.Id}";
                    string hint = GetTail(prefix, oldDoor, door.ShouldShuffle, false);
                    if (hint != prefix)
                    {
                        hints.Add(hint);
                    }
                }
            }
            if (door.BuildingShop != null)
            {
                var items = GetShopItems(door.BuildingShop, spoilerLog);
                foreach (var item in items)
                {
                    string hint = $"{door.Id} has {item}";
                    hint = GetTail(hint, door.OriginalId, door.ShouldShuffle, true);
                    hints.Add(hint);
                }
            }

            return hints;
        }

        private string GetTail(string hint, DoorId oldDoor, bool shouldShuffle, bool appendAnd)
        {
            if (shouldShuffle)
            {
                if (appendAnd)
                {
                    hint += " and is at ";
                }
                else
                {
                    hint += " is at ";
                }

                hint += $"{oldDoor}";
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

        private List<string> GetSublevelHints(SubLevel sublevel, bool spoilerLog)
        {
            var hints = new List<string>();
            var items = GetSublevelItems(sublevel, spoilerLog);
            foreach (var item in items)
            {
                hints.Add($"{sublevel.SubLevelId} has {item}");
            }

            return hints;
        }
    }
}
