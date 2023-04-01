using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class SegmentRandomizer
    {
        private List<Screen> screens = new List<Screen>();
        private List<List<Screen>> endTransitionScreens = new List<List<Screen>>();
        private List<List<Screen>> middleTransitionScreens = new List<List<Screen>>();
        private List<List<Screen>> townTransitionScreens = new List<List<Screen>>();
        private List<List<Screen>> deadendTransitionScreens = new List<List<Screen>>();
        private List<Screen> trunkScreens = new List<Screen>();
        private List<Screen> mistScreens = new List<Screen>();
        private List<Screen> branchScreens1 = new List<Screen>();
        private List<Screen> branchScreens2 = new List<Screen>();
        private List<Screen> dartMoorScreens = new List<Screen>();
        private Screen conflateEntrance;
        private Screen dartmoorEntrance;

        public SegmentRandomizer(byte[] content)
        {
            var end1 = AddTransition(Level.LevelDict[WorldNumber.Trunk], 0xEAB1, content);
            var middle1 = AddTransition(Level.LevelDict[WorldNumber.Trunk], 0xEAB6, content);
            var middle2 = AddTransition(Level.LevelDict[WorldNumber.Trunk], 0xEABB, content);
            var end2 = AddTransition(Level.LevelDict[WorldNumber.Trunk], 0xEAC0, content);

            if (Util.AllCoreWorldScreensRandomized())
            {
                end1 = Trunk.End1;
                middle1 = Trunk.Middle1;
                middle2 = Trunk.Middle2;
                end2 = Trunk.End2;
            }

            endTransitionScreens.Add(new List<Screen> { end1, end2 });

            townTransitionScreens.Add(new List<Screen> { AddTransition(Level.LevelDict[WorldNumber.Towns], 0xEAC6, content),
                                                           AddTransition(Level.LevelDict[WorldNumber.Towns], 0xEACB, content) });

            middleTransitionScreens.Add(new List<Screen> { middle1, middle2 });

            townTransitionScreens.Add(new List<Screen> { AddTransition(Level.LevelDict[WorldNumber.Towns], 0xEAD0, content),
                                                           AddTransition(Level.LevelDict[WorldNumber.Towns], 0xEAD5, content) });

            screens.Add(end1);
            screens.Add(townTransitionScreens[0][0]);
            screens.Add(townTransitionScreens[0][1]);
            screens.Add(middle1);
            screens.Add(middle2);
            screens.Add(townTransitionScreens[1][0]);
            screens.Add(townTransitionScreens[1][1]);
            screens.Add(end2);

            end1 = AddTransition(Level.LevelDict[WorldNumber.Mist], 0xEB03, content);
            middle1 = AddTransition(Level.LevelDict[WorldNumber.Mist], 0xEB08, content);
            middle2 = AddTransition(Level.LevelDict[WorldNumber.Mist], 0xEB0D, content);
            end2 = AddTransition(Level.LevelDict[WorldNumber.Mist], 0xEB12, content);

            if (Util.AllCoreWorldScreensRandomized())
            {
                end1 = Mist.End1;
                middle1 = Mist.Middle1;
                middle2 = Mist.Middle2;
                end2 = Mist.End2;
            }

            endTransitionScreens.Add(new List<Screen> { end1, end2 });

            townTransitionScreens.Add(new List<Screen> { AddTransition(Level.LevelDict[WorldNumber.Towns], 0xEADA, content),
                                                           AddTransition(Level.LevelDict[WorldNumber.Towns], 0xEADF, content) });

            middleTransitionScreens.Add(new List<Screen> { middle1, middle2 });

            townTransitionScreens.Add(new List<Screen> { AddTransition(Level.LevelDict[WorldNumber.Towns], 0xEAE4, content),
                                                           AddTransition(Level.LevelDict[WorldNumber.Towns], 0xEAE9, content) });

            townTransitionScreens.Add(new List<Screen> { AddTransition(Level.LevelDict[WorldNumber.Towns], 0xEAF3, content),
                                                           AddTransition(Level.LevelDict[WorldNumber.Towns], 0xEAF8, content) });

            screens.Add(end1);
            screens.Add(townTransitionScreens[2][0]);
            screens.Add(townTransitionScreens[2][1]);
            screens.Add(middle1);
            screens.Add(middle2);
            screens.Add(townTransitionScreens[3][0]);
            screens.Add(townTransitionScreens[3][1]);
            screens.Add(end2);

            end1 = AddTransition(Level.LevelDict[WorldNumber.Branch], 0xEB1D, content);
            end2 = AddTransition(Level.LevelDict[WorldNumber.Branch], 0xEB22, content);
            endTransitionScreens.Add(new List<Screen> { end1, end2 });

            screens.Add(end1);
            screens.Add(townTransitionScreens[4][0]);
            screens.Add(townTransitionScreens[4][1]);
            screens.Add(end2);

            conflateEntrance = AddTransition(Level.LevelDict[WorldNumber.Branch], 0xEB18, content);
            middle1 = AddTransition(Level.LevelDict[WorldNumber.Towns], 0xEAEE, content);
            middle2 = Level.LevelDict[WorldNumber.Towns].Screens[middle1.Number + 1];
            deadendTransitionScreens.Add(new List<Screen> { middle1, middle2 });

            dartmoorEntrance = AddTransition(Level.LevelDict[WorldNumber.Dartmoor], 0xEB28, content);
            middle1 = AddTransition(Level.LevelDict[WorldNumber.Towns], 0xEAFD, content);
            middle2 = Level.LevelDict[WorldNumber.Towns].Screens[middle1.Number + 1];
            deadendTransitionScreens.Add(new List<Screen> { middle1, middle2 });

            foreach (var level in Level.LevelDict.Values)
            {
                foreach (var screen in level.Screens)
                {
                    if (screen.Transition != null)
                    {
                        var targetLevel = Level.LevelDict[Door.worldDict[screen.Transition.ToWorld]];
                        var targetScreen = targetLevel.Screens[screen.Transition.ToScreen];
                        targetScreen.Transition.OldPosition = screen.Transition.NewPosition;
                        screen.Transition.ToScreenReference = targetScreen;
                    }
                }
            }
        }

        public void ShuffleSegments(Random random)
        {
            Util.ShuffleList(townTransitionScreens, 0, townTransitionScreens.Count - 1, random);
            void SwapTransition(List<List<Screen>> source, List<List<Screen>> destination, int i)
            {
                var tempList = source[i];
                source[i] = destination[i];
                source[i][1].Transition = new Transition(tempList[1].Transition);
                source[i][1].Transition.FromScreen = source[i][1].Number;
                tempList[1].Transition = null;
                destination[i] = tempList;
            }

            SwapTransition(townTransitionScreens, deadendTransitionScreens, 0);
            SwapTransition(townTransitionScreens, deadendTransitionScreens, 1);

            int index = 0;
            if (GeneralOptions.ShuffleSegments == GeneralOptions.SegmentShuffle.TownsOnly)
            {
                var tempScreens = new List<Screen>()
                {
                    conflateEntrance,
                    deadendTransitionScreens[0][0],
                };

                var tempTransition = new Transition(tempScreens[0].Transition);
                SetTransition(tempScreens[1], tempTransition);
                SetTransition(tempScreens[0], tempScreens[1].Transition);
                tempScreens[0].Transition = tempTransition;

                tempScreens = new List<Screen>()
                {
                    dartmoorEntrance,
                    deadendTransitionScreens[1][0],
                };

                tempTransition = new Transition(tempScreens[0].Transition);
                SetTransition(tempScreens[1], tempTransition);
                SetTransition(tempScreens[0], tempScreens[1].Transition);
                tempScreens[0].Transition = tempTransition;

                Util.ShuffleList(townTransitionScreens, 0, townTransitionScreens.Count - 1, random);
                for (int i = 1; i < screens.Count - 1; i++)
                {
                    if (screens[i].ParentWorld == WorldNumber.Towns)
                    {
                        screens[i] = townTransitionScreens[index][0];
                        screens[i + 1] = townTransitionScreens[index][1];
                        index++;
                        i++;
                    }
                }
                ConnectScreens(screens);
            }
            else
            {
                trunkScreens = new List<Screen>()
                {
                    endTransitionScreens[0][0],
                };
                mistScreens = new List<Screen>()
                {
                    endTransitionScreens[1][0],
                };
                branchScreens1 = new List<Screen>()
                {
                    endTransitionScreens[2][0],
                };
                branchScreens2 = new List<Screen>()
                {
                    conflateEntrance
                };
                dartMoorScreens = new List<Screen>()
                {
                    dartmoorEntrance
                };

                var endpoints = new List<List<Screen>>()
                {
                    trunkScreens,
                    mistScreens,
                    branchScreens1,
                    branchScreens2,
                    dartMoorScreens,
                };

                var candidates = new List<List<Screen>>(townTransitionScreens);
                candidates.AddRange(middleTransitionScreens);
                Util.ShuffleList(candidates, 0, candidates.Count - 1, random);
                for (int i = 0; i < candidates.Count; i++)
                {
                    var next = endpoints[random.Next(0, endpoints.Count)];
                    next.Add(candidates[i][0]);
                    next.Add(candidates[i][1]);
                }

                trunkScreens.Add(endTransitionScreens[0][1]);
                mistScreens.Add(endTransitionScreens[1][1]);
                branchScreens1.Add(endTransitionScreens[2][1]);
                branchScreens2.Add(deadendTransitionScreens[0][0]);
                branchScreens2.Add(deadendTransitionScreens[0][1]);
                dartMoorScreens.Add(deadendTransitionScreens[1][0]);
                dartMoorScreens.Add(deadendTransitionScreens[1][1]);

                ConnectScreens(trunkScreens);
                ConnectScreens(mistScreens);
                ConnectScreens(branchScreens1);
                ConnectScreens(branchScreens2);
                ConnectScreens(dartMoorScreens);
            }
        }

        private Screen AddTransition(Level level, int offset, byte[] content)
        {
            var transition = new Transition(Section.GetOffset(15, offset, 0xC000), content);
            var screen = level.Screens[transition.FromScreen];
            screen.Transition = transition;
            return screen;
        }

        private void SetTransition(Screen source, Transition destination)
        {
            destination.ToWorld = Door.OtherWorldDict[source.ParentWorld];
            destination.ToScreen = source.Number;
            destination.ToScreenReference = source;
            destination.NewPosition = source.Transition.OldPosition;
            destination.NewPalette = SubLevel.SubLevelDict[source.ParentSublevel].Palette;
        }

        private void ConnectScreens(List<Screen> screens)
        {
            for (int i = 0; i < screens.Count - 1; i += 2)
            {
                var tempTransition = new Transition(screens[i].Transition);
                SetTransition(screens[i + 1], tempTransition);
                SetTransition(screens[i], screens[i + 1].Transition);
                screens[i].Transition = tempTransition;
            }
        }

        public void UpdateDoors(DoorRandomizer doorRandomizer, ShopRandomizer shopRandomizer)
        {
            var worlds = doorRandomizer.GetWorlds();
            var multipliers = new Dictionary<OtherWorldNumber, int>();
            for (int i = 0; i < worlds.Count; i++)
            {
                var world = Door.OtherWorldDict[worlds[i].number];
                multipliers[world] = i + 1;
            }

            if (GeneralOptions.ShuffleSegments != GeneralOptions.SegmentShuffle.AllSegments)
            {
                UpdateDoors(screens[1], doorRandomizer, shopRandomizer, OtherWorldNumber.Trunk, multipliers);
                UpdateDoors(screens[2], doorRandomizer, shopRandomizer, OtherWorldNumber.Trunk, multipliers);
                UpdateDoors(screens[5], doorRandomizer, shopRandomizer, OtherWorldNumber.Trunk, multipliers);
                UpdateDoors(screens[6], doorRandomizer, shopRandomizer, OtherWorldNumber.Trunk, multipliers);
                UpdateDoors(screens[9], doorRandomizer, shopRandomizer, OtherWorldNumber.Mist, multipliers);
                UpdateDoors(screens[10], doorRandomizer, shopRandomizer, OtherWorldNumber.Mist, multipliers);
                UpdateDoors(screens[13], doorRandomizer, shopRandomizer, OtherWorldNumber.Mist, multipliers);
                UpdateDoors(screens[14], doorRandomizer, shopRandomizer, OtherWorldNumber.Mist, multipliers);
                UpdateDoors(screens[17], doorRandomizer, shopRandomizer, OtherWorldNumber.Branch, multipliers);
                UpdateDoors(screens[18], doorRandomizer, shopRandomizer, OtherWorldNumber.Branch, multipliers);
                UpdateDoors(deadendTransitionScreens[0][0], doorRandomizer, shopRandomizer, OtherWorldNumber.Branch, multipliers);
                UpdateDoors(deadendTransitionScreens[0][1], doorRandomizer, shopRandomizer, OtherWorldNumber.Branch, multipliers);
                UpdateDoors(deadendTransitionScreens[1][0], doorRandomizer, shopRandomizer, OtherWorldNumber.Dartmoor, multipliers);
                UpdateDoors(deadendTransitionScreens[1][1], doorRandomizer, shopRandomizer, OtherWorldNumber.Dartmoor, multipliers);
            }
            else
            {
                foreach (var screen in trunkScreens)
                {
                    UpdateDoors(screen, doorRandomizer, shopRandomizer, OtherWorldNumber.Trunk, multipliers);
                }
                foreach (var screen in mistScreens)
                {
                    UpdateDoors(screen, doorRandomizer, shopRandomizer, OtherWorldNumber.Mist, multipliers);
                }
                foreach (var screen in branchScreens1)
                {
                    UpdateDoors(screen, doorRandomizer, shopRandomizer, OtherWorldNumber.Branch, multipliers);
                }
                foreach (var screen in branchScreens2)
                {
                    UpdateDoors(screen, doorRandomizer, shopRandomizer, OtherWorldNumber.Branch, multipliers);
                }
                foreach (var screen in dartMoorScreens)
                {
                    UpdateDoors(screen, doorRandomizer, shopRandomizer, OtherWorldNumber.Dartmoor, multipliers);
                }
            }
        }

        public List<string> GetHints()
        {
            List<string> hints = new List<string>();

            if (GeneralOptions.ShuffleSegments != GeneralOptions.SegmentShuffle.AllSegments)
            {
                hints.Add($"{screens[1].ParentSublevel} is at Apolune");
                hints.Add($"{screens[5].ParentSublevel} is at Forepaw");
                hints.Add($"{screens[9].ParentSublevel} is at Mascon");
                hints.Add($"{screens[13].ParentSublevel} is at Victim");
                hints.Add($"{screens[17].ParentSublevel} is at Daybreak");
                hints.Add($"{deadendTransitionScreens[0][0].ParentSublevel} is at Conflate");
                hints.Add($"{deadendTransitionScreens[1][0].ParentSublevel} is at Dartmoor City");
            }
            else
            {
                var sublevels = new HashSet<SubLevel.Id>();
                AddHints(hints, trunkScreens, sublevels, "in Trunk");
                AddHints(hints, mistScreens, sublevels, "in Mist");
                AddHints(hints, branchScreens1, sublevels, "in Branch");
                AddHints(hints, branchScreens2, sublevels, $"at Conflate in {branchScreens2[0].ParentSublevel}");
                AddHints(hints, dartMoorScreens, sublevels, $"at Dartmoor City in {dartMoorScreens[0].ParentSublevel}");
            }

            return hints;
        }

        private void AddHints(List<string> hints, List<Screen> screens, HashSet<SubLevel.Id> sublevels, string location)
        {
            for (int i = 1; i < screens.Count - 1; i++)
            {
                if (!sublevels.Contains(screens[i].ParentSublevel))
                {
                    sublevels.Add(screens[i].ParentSublevel);
                    hints.Add($"{screens[i].ParentSublevel} is {location}");
                }
            }
        }

        private void UpdateDoors(Screen target, DoorRandomizer doorRandomizer, ShopRandomizer shopRandomizer,
                                 OtherWorldNumber worldNumber, Dictionary<OtherWorldNumber, int> multipliers)
        {
            var sublevel = SubLevel.SubLevelDict[target.ParentSublevel];
            var sublevels = new HashSet<SubLevel.Id>();
            var worlds = doorRandomizer.GetWorlds();
            int multiplier = multipliers[worldNumber];

            void UpdateSublevel(SubLevel subLevel)
            {
                sublevels.Add(subLevel.SubLevelId);
                foreach (var screen in subLevel.Screens)
                {
                    foreach (var door in screen.Doors)
                    {
                        if (doorRandomizer.Doors.ContainsKey(door))
                        {
                            var shop = doorRandomizer.Doors[door].BuildingShop;
                            if (shop != null)
                            {
                                shop.Multiplier = multiplier;
                            }
                            if (shopRandomizer.StaticPriceDict.TryGetValue(door, out StaticPrice staticPrice))
                            {
                                staticPrice.Multiplier = multiplier;
                            }

                            if (GeneralOptions.ShuffleSegments != GeneralOptions.SegmentShuffle.Unchanged)
                            {
                                var guru = doorRandomizer.Doors[door].Guru;
                                if (guru != null)
                                {
                                    guru.OverrideWorldSpawn = (byte)Door.worldDict[worldNumber];
                                }
                            }

                            var child = doorRandomizer.Doors[door].Sublevel;
                            if (child != null && !sublevels.Contains(child.SubLevelId))
                            {
                                UpdateSublevel(child);
                            }
                        }
                    }
                }
            }

            UpdateSublevel(sublevel);
        }
    }
}
