using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class Trunk : Level
    {
        public const byte EarlyTrunkStartScreen = 0;
        public const byte ApoluneLeftScreen = 7;
        public const byte ApoluneRightScreen = 8;
        public const byte TowerOfTrunkScreen = 11;
        public const byte TowerOfTrunkEntranceScreen = 13;
        public const byte MattockScreen = 12;
        public const byte LateTrunkStartScreen = 22;
        public const byte ForePawLeftScreen = 26;
        public const byte ForePawRightScreen = 29;
        public const byte JokerHouseScreen = 30;
        public const byte SkySpringScreen = 37;
        public const byte TowerOfFortressScreen = 40;
        public const byte TowerOfFortressEntranceScreen = 41;
        public const byte FortressSpringScreen = 61;
        public const byte JokerHouseEntranceScreen = 62;
        public const byte JokerSpringScreen = 63;

        public static Screen End1 = null;
        public static Screen Middle1 = null;
        public static Screen Middle2 = null;
        public static Screen End2 = null;

        private List<byte> extraUpCandidates = new List<byte>()
        {
            50,
            53,
        };

        private List<byte> extraDownCandidates = new List<byte>()
        {
            51,
            54,
        };

        public Trunk(WorldNumber number, byte[] content) : base(number, content)
        {
            Screens[EarlyTrunkStartScreen].Doors.Add(DoorId.TrunkEntrance);
            Screens[6].Doors.Add(DoorId.TrunkSecretShop);
            Screens[TowerOfTrunkScreen].Doors.Add(DoorId.TowerOfTrunk);
            Screens[TowerOfTrunkEntranceScreen].Doors.Add(DoorId.TowerOfTrunkReturn);
            Screens[JokerHouseScreen].Doors.Add(DoorId.JokerHouse);
            Screens[JokerHouseEntranceScreen].Doors.Add(DoorId.JokerHouseReturn);
            Screens[TowerOfFortressScreen].Doors.Add(DoorId.TowerOfFortress);
            Screens[TowerOfFortressScreen].Doors.Add(DoorId.TrunkExit);
            Screens[TowerOfFortressEntranceScreen].Doors.Add(DoorId.TowerOfFortressReturn);
            Screens[45].Doors.Add(DoorId.FortressGuru);
            Screens[JokerSpringScreen].Gifts.Add(GiftItem.Id.JokerSpring);
            Screens[MattockScreen].ConnectedScreen = Screens[LateTrunkStartScreen];
        }

        public override int GetStartOffset()
        {
            return 0x2C2F0;
        }

        public override int GetEndOffset()
        {
            return 0x2C491;
        }

        public override void CorrectScreenSprites()
        {
            var temp = Screens[38].Sprites;
            Screens[38].Sprites = Screens[34].Sprites;

            for (int i = 34; i > 27; i--)
            {
                Screens[i].Sprites = Screens[i - 1].Sprites;
            }

            for (int i = 40; i < 61; i++)
            {
                Screens[i].Sprites = Screens[i + 1].Sprites;
            }

            Screens[FortressSpringScreen].Sprites = temp;
        }

        public override void GetEnds(ref List<Screen> startScreens, ref List<Screen> endScreens, Random random)
        {
            startScreens = new List<Screen>();
            startScreens.Add(Screens[0]);

            var middleEnds = new List<byte>();

            if (random.Next(0, 2) == 0)
            {
                middleEnds.Add(ApoluneLeftScreen);
                middleEnds.Add(ApoluneRightScreen);
            }
            else
            {
                middleEnds.Add(ApoluneRightScreen);
                middleEnds.Add(ApoluneLeftScreen);
            }

            var lateEnds = new List<byte> { MattockScreen, LateTrunkStartScreen };
            var eastEnds = new List<byte>();

            if (random.Next(0, 2) == 0)
            {
                eastEnds.Add(ForePawLeftScreen);
                eastEnds.Add(ForePawRightScreen);
            }
            else
            {
                eastEnds.Add(ForePawRightScreen);
                eastEnds.Add(ForePawLeftScreen);
            }

            if (random.Next(0, 2) == 0)
            {
                var tmp = middleEnds;
                middleEnds = eastEnds;
                eastEnds = tmp;
            }

            if (random.Next(0, 2) == 0)
            {
                var tmp = lateEnds;
                lateEnds = eastEnds;
                eastEnds = tmp;
            }

            startScreens.Add(Screens[middleEnds[1]]);
            startScreens.Add(Screens[lateEnds[1]]);
            startScreens.Add(Screens[eastEnds[1]]);
            startScreens.Add(Screens[TowerOfFortressEntranceScreen]);
            startScreens.Add(Screens[TowerOfTrunkEntranceScreen]);
            startScreens.Add(Screens[JokerHouseEntranceScreen]);

            endScreens = new List<Screen>();
            endScreens.Add(Screens[middleEnds[0]]);
            endScreens.Add(Screens[lateEnds[0]]);
            endScreens.Add(Screens[eastEnds[0]]);
            endScreens.Add(Screens[TowerOfFortressScreen]);

            endScreens.Add(Screens[33]);
            endScreens.Add(Screens[60]);
            endScreens.Add(Screens[JokerSpringScreen]);
            endScreens.Add(Screens[FortressSpringScreen]);
            endScreens.Add(Screens[21]);

            if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Unchanged)
            {
                Util.ShuffleList(endScreens, endScreens.Count - 5, endScreens.Count - 2, random);
                Util.ShuffleList(endScreens, endScreens.Count - 2, endScreens.Count - 1, random);
            }
            else
            {
                Util.ShuffleList(endScreens, endScreens.Count - 5, endScreens.Count - 1, random);
            }
        }

        public override List<Screen> GetCandidates(Random random)
        {
            var candidates = Screens.GetRange(1, 5);
            candidates.AddRange(Screens.GetRange(9, 2));
            candidates.Add(Screens[14]);
            candidates.Add(Screens[15]);
            candidates.Add(Screens[17]);
            candidates.Add(Screens[18]);
            candidates.Add(Screens[19]);
            candidates.Add(Screens[20]);
            candidates.Add(Screens[25]);
            candidates.Add(Screens[31]);
            candidates.AddRange(Screens.GetRange(38, 2));
            candidates.Add(Screens[42]);
            candidates.Add(Screens[46]);
            candidates.Add(Screens[48]);
            candidates.Add(Screens[49]);
            candidates.Add(Screens[59]);

            Util.ShuffleList(candidates, 0, candidates.Count - 1, random);
            return candidates;
        }

        public override List<Screen> GetSpecialScreens(Random random)
        {
            var specialScreens = new List<Screen>();
            specialScreens.Add(Screens[6]);
            specialScreens.Add(Screens[TowerOfTrunkScreen]);
            specialScreens.Add(Screens[24]);
            specialScreens.Add(Screens[JokerHouseScreen]);
            specialScreens.Add(Screens[32]);
            specialScreens.Add(Screens[43]);
            specialScreens.Add(Screens[45]);
            specialScreens.Add(Screens[58]);

            Util.ShuffleList(specialScreens, 0, specialScreens.Count - 1, random);
            return specialScreens;
        }

        public override bool RandomizeScreens(Random random, ref uint attempts)
        {
            bool result = base.RandomizeScreens(random, ref attempts);
            if (!result)
            {
                return result;
            }

            foreach (var sublevel in SubLevels)
            {
                sublevel.RequiresMattock = false;

                foreach (var screen in sublevel.Screens)
                {
                    if (screen.Number == SkySpringScreen)
                    {
                        SubLevel.SkySpringSublevel = sublevel.SubLevelId;
                    }
                    else if (screen.Number == FortressSpringScreen)
                    {
                        SubLevel.FortressSpringSublevel = sublevel.SubLevelId;
                    }
                    else if (screen.Number == JokerSpringScreen)
                    {
                        SubLevel.JokerSpringSublevel = sublevel.SubLevelId;
                    }
                    else if (screen.Number == MattockScreen)
                    {
                        sublevel.RequiresMattock = true;
                    }
                }
            }

            Screens[55].ScrollData.Down = 60;
            Screens[60].Sprites[1].SetX(11);
            Screens[60].Sprites[1].SetY(5);
            if (Screens[ForePawRightScreen].ScrollData.Down == 255)
            {
                Screens[ForePawRightScreen].ScrollData.Down = JokerSpringScreen;
            }

            return result;
        }

        public override bool CreateSublevels(List<Screen> startScreens, List<Screen> endScreens, List<Screen> candidates, List<Screen> specialScreens, Random random, uint attempts)
        {
            Util.ShuffleList(extraDownCandidates, 0, extraDownCandidates.Count - 1, random);
            Util.ShuffleList(extraUpCandidates, 0, extraUpCandidates.Count - 1, random);
            Screens[22].FriendConnections = new Dictionary<Direction, Screen>();
            Screens[22].FriendConnections[Direction.Up] = Screens[extraDownCandidates[0]];
            Screens[24].FriendConnections = new Dictionary<Direction, Screen>();
            Screens[24].FriendConnections[Direction.Down] = Screens[extraUpCandidates[0]];
            Screens[58].FriendConnections = new Dictionary<Direction, Screen>();
            Screens[58].FriendConnections[Direction.Down] = Screens[extraUpCandidates[1]];

            bool result = CreateSublevel(startScreens[0], endScreens[0], candidates, specialScreens, random, SubLevel.Id.EarlyTrunk, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[1], endScreens[1], candidates, specialScreens, random, SubLevel.Id.MiddleTrunk, attempts);
            if (!result)
            {
                return result;
            }

            if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Unchanged)
            {
                if (specialScreens.Count > 0)
                {
                    return false;
                }
            }

            result = CreateSublevel(startScreens[2], endScreens[2], candidates, specialScreens, random, SubLevel.Id.LateTrunk, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[3], endScreens[3], candidates, specialScreens, random, SubLevel.Id.EastTrunk, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[4], endScreens[4], candidates, specialScreens, random, SubLevel.Id.TowerOfFortress, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[5], endScreens[5], candidates, specialScreens, random, SubLevel.Id.TowerOfTrunk, attempts);
            if (!result)
            {
                return result;
            }

            if (!GeneralOptions.ShuffleTowers &&
                specialScreens.Count > 0)
            {
                return false;
            }

            result = CreateSublevel(startScreens[6], endScreens[6], candidates, specialScreens, random, SubLevel.Id.JokerHouse, attempts);
            if (!result)
            {
                return result;
            }

            if (specialScreens.Count > 0)
            {
                return false;
            }

            result = CreateSublevel(Screens[47], endScreens[7], candidates, specialScreens, random, Screens[47].ParentSublevel, attempts, false, true);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(Screens[58], endScreens[8], candidates, specialScreens, random, Screens[58].ParentSublevel, attempts, false, true);
            if (!result)
            {
                return result;
            }

            if (candidates.Count > 10)
            {
                return false;
            }

            var skyScreens = new List<Screen>()
            {
                Screens[34],
                Screens[35],
                Screens[36],
            };

            Util.ShuffleList(skyScreens, 0, skyScreens.Count - 1, random);
            Screens[32].FriendEnds[Direction.Up] = skyScreens[0];
            skyScreens[0].FriendEnds[Direction.Up] = skyScreens[1];
            skyScreens[1].FriendEnds[Direction.Up] = skyScreens[2];
            skyScreens[2].FriendEnds[Direction.Up] = Screens[SkySpringScreen];

            End1 = endScreens[0];
            Middle1 = startScreens[1];
            if (startScreens[3].Number == MattockScreen ||
                startScreens[3].Number == LateTrunkStartScreen)
            {
                Middle2 = endScreens[1];
                End2 = startScreens[2];
            }
            else
            {
                Middle2 = endScreens[2];
                End2 = startScreens[3];
            }

            byte right1 = 23;
            byte right2 = 44;
            if (random.Next(0, 2) == 0)
            {
                right1 = 44;
                right2 = 23;
            }
            Screens[LateTrunkStartScreen].FriendEnds[Direction.Right] = Screens[right1];
            Screens[43].FriendEnds[Direction.Right] = Screens[right2];

            return result;
        }

        public override void SetupScreens()
        {
            startToSpecial[TowerOfTrunkEntranceScreen] = TowerOfTrunkScreen;
            startToSpecial[JokerHouseEntranceScreen] = JokerHouseScreen;

            Screens[43].FriendConnections[Direction.Up] = Screens[47];
            Screens[60].FriendEnds[Direction.Down] = Screens[55];

            Screens[EarlyTrunkStartScreen].Directions.Add(Direction.Left);
            Screens[EarlyTrunkStartScreen].Directions.Add(Direction.Right);
            Screens[EarlyTrunkStartScreen].OpenTilesLeft.Add(8);
            Screens[EarlyTrunkStartScreen].OpenTilesLeft.Add(9);
            Screens[EarlyTrunkStartScreen].OpenTilesLeft.Add(10);
            Screens[EarlyTrunkStartScreen].OpenTilesLeft.Add(11);
            Screens[EarlyTrunkStartScreen].OpenTilesRight.Add(8);
            Screens[EarlyTrunkStartScreen].OpenTilesRight.Add(9);
            Screens[EarlyTrunkStartScreen].OpenTilesRight.Add(10);
            Screens[EarlyTrunkStartScreen].OpenTilesRight.Add(11);
            Screens[1].Directions.Add(Direction.Left);
            Screens[1].Directions.Add(Direction.Right);
            Screens[1].OpenTilesLeft.Add(8);
            Screens[1].OpenTilesLeft.Add(9);
            Screens[1].OpenTilesLeft.Add(10);
            Screens[1].OpenTilesLeft.Add(11);
            Screens[1].OpenTilesRight.Add(8);
            Screens[1].OpenTilesRight.Add(9);
            Screens[1].OpenTilesRight.Add(10);
            Screens[1].OpenTilesRight.Add(11);
            Screens[2].Directions.Add(Direction.Left);
            Screens[2].Directions.Add(Direction.Right);
            Screens[2].OpenTilesLeft.Add(8);
            Screens[2].OpenTilesLeft.Add(9);
            Screens[2].OpenTilesLeft.Add(10);
            Screens[2].OpenTilesLeft.Add(11);
            Screens[2].OpenTilesRight.Add(8);
            Screens[2].OpenTilesRight.Add(9);
            Screens[2].OpenTilesRight.Add(10);
            Screens[2].OpenTilesRight.Add(11);
            Screens[3].Directions.Add(Direction.Left);
            Screens[3].Directions.Add(Direction.Up);
            Screens[3].OpenTilesLeft.Add(1);
            Screens[3].OpenTilesLeft.Add(2);
            Screens[3].OpenTilesLeft.Add(3);
            Screens[3].OpenTilesLeft.Add(4);
            Screens[3].OpenTilesLeft.Add(5);
            Screens[3].OpenTilesLeft.Add(6);
            Screens[3].OpenTilesLeft.Add(7);
            Screens[3].OpenTilesLeft.Add(8);
            Screens[3].OpenTilesLeft.Add(9);
            Screens[3].OpenTilesLeft.Add(10);
            Screens[3].OpenTilesLeft.Add(11);
            Screens[3].OpenTilesUp.Add(9);
            Screens[4].Directions.Add(Direction.Right);
            Screens[4].Directions.Add(Direction.Down);
            Screens[4].OpenTilesRight.Add(8);
            Screens[4].OpenTilesRight.Add(9);
            Screens[4].OpenTilesRight.Add(10);
            Screens[4].OpenTilesRight.Add(11);
            Screens[4].OpenTilesDown.Add(9);
            Screens[5].Directions.Add(Direction.Left);
            Screens[5].Directions.Add(Direction.Right);
            Screens[5].OpenTilesLeft.Add(8);
            Screens[5].OpenTilesLeft.Add(9);
            Screens[5].OpenTilesLeft.Add(10);
            Screens[5].OpenTilesLeft.Add(11);
            Screens[5].OpenTilesRight.Add(8);
            Screens[5].OpenTilesRight.Add(9);
            Screens[5].OpenTilesRight.Add(10);
            Screens[5].OpenTilesRight.Add(11);
            Screens[6].Directions.Add(Direction.Left);
            Screens[6].Directions.Add(Direction.Right);
            Screens[6].OpenTilesLeft.Add(8);
            Screens[6].OpenTilesLeft.Add(9);
            Screens[6].OpenTilesLeft.Add(10);
            Screens[6].OpenTilesLeft.Add(11);
            Screens[6].OpenTilesRight.Add(5);
            Screens[6].OpenTilesRight.Add(6);
            Screens[6].OpenTilesRight.Add(8);
            Screens[6].OpenTilesRight.Add(9);
            Screens[6].OpenTilesRight.Add(10);
            Screens[6].OpenTilesRight.Add(11);
            Screens[ApoluneLeftScreen].Directions.Add(Direction.Left);
            Screens[ApoluneLeftScreen].OpenTilesLeft.Add(8);
            Screens[ApoluneLeftScreen].OpenTilesLeft.Add(9);
            Screens[ApoluneLeftScreen].OpenTilesLeft.Add(10);
            Screens[ApoluneLeftScreen].OpenTilesLeft.Add(11);
            Screens[ApoluneRightScreen].Directions.Add(Direction.Right);
            Screens[ApoluneRightScreen].OpenTilesRight.Add(8);
            Screens[ApoluneRightScreen].OpenTilesRight.Add(9);
            Screens[ApoluneRightScreen].OpenTilesRight.Add(10);
            Screens[ApoluneRightScreen].OpenTilesRight.Add(11);
            Screens[9].Directions.Add(Direction.Left);
            Screens[9].Directions.Add(Direction.Right);
            Screens[9].OpenTilesLeft.Add(8);
            Screens[9].OpenTilesLeft.Add(9);
            Screens[9].OpenTilesLeft.Add(10);
            Screens[9].OpenTilesLeft.Add(11);
            Screens[9].OpenTilesRight.Add(8);
            Screens[9].OpenTilesRight.Add(9);
            Screens[9].OpenTilesRight.Add(10);
            Screens[9].OpenTilesRight.Add(11);
            Screens[10].Directions.Add(Direction.Left);
            Screens[10].Directions.Add(Direction.Right);
            Screens[10].OpenTilesLeft.Add(8);
            Screens[10].OpenTilesLeft.Add(9);
            Screens[10].OpenTilesLeft.Add(10);
            Screens[10].OpenTilesLeft.Add(11);
            Screens[10].OpenTilesRight.Add(8);
            Screens[10].OpenTilesRight.Add(9);
            Screens[10].OpenTilesRight.Add(10);
            Screens[10].OpenTilesRight.Add(11);
            Screens[TowerOfTrunkScreen].Directions.Add(Direction.Left);
            Screens[TowerOfTrunkScreen].Directions.Add(Direction.Right);
            Screens[TowerOfTrunkScreen].OpenTilesLeft.Add(8);
            Screens[TowerOfTrunkScreen].OpenTilesLeft.Add(9);
            Screens[TowerOfTrunkScreen].OpenTilesLeft.Add(10);
            Screens[TowerOfTrunkScreen].OpenTilesLeft.Add(11);
            Screens[TowerOfTrunkScreen].OpenTilesRight.Add(8);
            Screens[TowerOfTrunkScreen].OpenTilesRight.Add(9);
            Screens[TowerOfTrunkScreen].OpenTilesRight.Add(10);
            Screens[TowerOfTrunkScreen].OpenTilesRight.Add(11);
            Screens[MattockScreen].Directions.Add(Direction.Left);
            Screens[MattockScreen].OpenTilesLeft.Add(8);
            Screens[MattockScreen].OpenTilesLeft.Add(9);
            Screens[MattockScreen].OpenTilesLeft.Add(10);
            Screens[MattockScreen].OpenTilesLeft.Add(11);
            Screens[13].Directions.Add(Direction.Left);
            Screens[13].Directions.Add(Direction.Right);
            Screens[13].OpenTilesLeft.Add(9);
            Screens[13].OpenTilesLeft.Add(10);
            Screens[13].OpenTilesRight.Add(7);
            Screens[13].OpenTilesRight.Add(8);
            Screens[13].OpenTilesRight.Add(9);
            Screens[13].OpenTilesRight.Add(10);
            Screens[14].Directions.Add(Direction.Left);
            Screens[14].Directions.Add(Direction.Right);
            Screens[14].OpenTilesLeft.Add(2);
            Screens[14].OpenTilesLeft.Add(3);
            Screens[14].OpenTilesLeft.Add(4);
            Screens[14].OpenTilesLeft.Add(5);
            Screens[14].OpenTilesLeft.Add(7);
            Screens[14].OpenTilesLeft.Add(8);
            Screens[14].OpenTilesLeft.Add(9);
            Screens[14].OpenTilesLeft.Add(10);
            Screens[14].OpenTilesRight.Add(7);
            Screens[14].OpenTilesRight.Add(8);
            Screens[14].OpenTilesRight.Add(9);
            Screens[14].OpenTilesRight.Add(10);
            Screens[15].Directions.Add(Direction.Left);
            Screens[15].Directions.Add(Direction.Up);
            Screens[15].OpenTilesLeft.Add(7);
            Screens[15].OpenTilesLeft.Add(8);
            Screens[15].OpenTilesLeft.Add(9);
            Screens[15].OpenTilesLeft.Add(10);
            Screens[15].OpenTilesUp.Add(2);
            Screens[16].Directions.Add(Direction.Right);
            Screens[16].Directions.Add(Direction.Down);
            Screens[16].OpenTilesRight.Add(7);
            Screens[16].OpenTilesRight.Add(8);
            Screens[16].OpenTilesRight.Add(9);
            Screens[16].OpenTilesRight.Add(10);
            Screens[16].OpenTilesDown.Add(2);
            Screens[17].Directions.Add(Direction.Left);
            Screens[17].Directions.Add(Direction.Right);
            Screens[17].OpenTilesLeft.Add(8);
            Screens[17].OpenTilesLeft.Add(9);
            Screens[17].OpenTilesLeft.Add(10);
            Screens[17].OpenTilesLeft.Add(11);
            Screens[17].OpenTilesRight.Add(8);
            Screens[17].OpenTilesRight.Add(9);
            Screens[17].OpenTilesRight.Add(10);
            Screens[17].OpenTilesRight.Add(11);
            Screens[18].Directions.Add(Direction.Left);
            Screens[18].Directions.Add(Direction.Up);
            Screens[18].OpenTilesLeft.Add(8);
            Screens[18].OpenTilesLeft.Add(9);
            Screens[18].OpenTilesLeft.Add(10);
            Screens[18].OpenTilesLeft.Add(11);
            Screens[18].OpenTilesUp.Add(4);
            Screens[19].Directions.Add(Direction.Right);
            Screens[19].Directions.Add(Direction.Down);
            Screens[19].OpenTilesRight.Add(1);
            Screens[19].OpenTilesRight.Add(2);
            Screens[19].OpenTilesRight.Add(3);
            Screens[19].OpenTilesRight.Add(4);
            Screens[19].OpenTilesRight.Add(6);
            Screens[19].OpenTilesRight.Add(7);
            Screens[19].OpenTilesRight.Add(8);
            Screens[19].OpenTilesDown.Add(4);
            Screens[20].Directions.Add(Direction.Left);
            Screens[20].Directions.Add(Direction.Right);
            Screens[20].OpenTilesLeft.Add(6);
            Screens[20].OpenTilesLeft.Add(7);
            Screens[20].OpenTilesLeft.Add(8);
            Screens[20].OpenTilesRight.Add(5);
            Screens[20].OpenTilesRight.Add(6);
            Screens[20].OpenTilesRight.Add(7);
            Screens[20].OpenTilesRight.Add(8);
            Screens[21].Directions.Add(Direction.Left);
            Screens[21].OpenTilesLeft.Add(6);
            Screens[21].OpenTilesLeft.Add(7);
            Screens[21].OpenTilesLeft.Add(8);
            //Screens[LateTrunkStartScreen].Directions.Add(Direction.Up);
            //Screens[LateTrunkStartScreen].OpenTilesUp.Add(1);
            Screens[24].Directions.Add(Direction.Right);
            //Screens[24].Directions.Add(Direction.Down);
            //Screens[24].OpenTilesDown.Add(1);
            Screens[24].OpenTilesRight.Add(7);
            Screens[24].OpenTilesRight.Add(8);
            Screens[24].OpenTilesRight.Add(9);
            Screens[24].OpenTilesRight.Add(10);
            Screens[25].Directions.Add(Direction.Left);
            Screens[25].Directions.Add(Direction.Right);
            Screens[25].OpenTilesLeft.Add(2);
            Screens[25].OpenTilesLeft.Add(3);
            Screens[25].OpenTilesLeft.Add(4);
            Screens[25].OpenTilesLeft.Add(5);
            Screens[25].OpenTilesLeft.Add(7);
            Screens[25].OpenTilesLeft.Add(8);
            Screens[25].OpenTilesLeft.Add(9);
            Screens[25].OpenTilesLeft.Add(10);
            Screens[25].OpenTilesRight.Add(2);
            Screens[25].OpenTilesRight.Add(3);
            Screens[25].OpenTilesRight.Add(4);
            Screens[25].OpenTilesRight.Add(5);
            Screens[25].OpenTilesRight.Add(7);
            Screens[25].OpenTilesRight.Add(8);
            Screens[25].OpenTilesRight.Add(9);
            Screens[25].OpenTilesRight.Add(10);
            Screens[ForePawLeftScreen].Directions.Add(Direction.Left);
            Screens[ForePawLeftScreen].OpenTilesLeft.Add(7);
            Screens[ForePawLeftScreen].OpenTilesLeft.Add(8);
            Screens[ForePawLeftScreen].OpenTilesLeft.Add(9);
            Screens[ForePawLeftScreen].OpenTilesLeft.Add(10);
            Screens[ForePawRightScreen].Directions.Add(Direction.Up);
            Screens[ForePawRightScreen].Directions.Add(Direction.Down);
            Screens[ForePawRightScreen].OpenTilesUp.Add(4);
            //Screens[ForePawRightScreen].OpenTilesDown.Add(2);
            Screens[JokerHouseScreen].Directions.Add(Direction.Left);
            Screens[JokerHouseScreen].Directions.Add(Direction.Right);
            Screens[JokerHouseScreen].OpenTilesLeft.Add(7);
            Screens[JokerHouseScreen].OpenTilesLeft.Add(8);
            Screens[JokerHouseScreen].OpenTilesLeft.Add(9);
            Screens[JokerHouseScreen].OpenTilesLeft.Add(10);
            Screens[JokerHouseScreen].OpenTilesRight.Add(7);
            Screens[JokerHouseScreen].OpenTilesRight.Add(8);
            Screens[JokerHouseScreen].OpenTilesRight.Add(9);
            Screens[JokerHouseScreen].OpenTilesRight.Add(10);
            Screens[31].Directions.Add(Direction.Left);
            Screens[31].Directions.Add(Direction.Right);
            Screens[31].OpenTilesLeft.Add(7);
            Screens[31].OpenTilesLeft.Add(8);
            Screens[31].OpenTilesLeft.Add(9);
            Screens[31].OpenTilesLeft.Add(10);
            Screens[31].OpenTilesRight.Add(7);
            Screens[31].OpenTilesRight.Add(8);
            Screens[31].OpenTilesRight.Add(9);
            Screens[31].OpenTilesRight.Add(10);
            Screens[32].Directions.Add(Direction.Left);
            Screens[32].Directions.Add(Direction.Right);
            Screens[32].OpenTilesLeft.Add(7);
            Screens[32].OpenTilesLeft.Add(8);
            Screens[32].OpenTilesLeft.Add(9);
            Screens[32].OpenTilesLeft.Add(10);
            Screens[32].OpenTilesRight.Add(6);
            Screens[32].OpenTilesRight.Add(7);
            Screens[32].OpenTilesRight.Add(8);
            Screens[32].OpenTilesRight.Add(9);
            Screens[33].Directions.Add(Direction.Left);
            Screens[33].OpenTilesLeft.Add(7);
            Screens[33].OpenTilesLeft.Add(8);
            Screens[33].OpenTilesLeft.Add(9);
            Screens[33].OpenTilesLeft.Add(10);
            Screens[38].Directions.Add(Direction.Left);
            Screens[38].Directions.Add(Direction.Down);
            Screens[38].OpenTilesLeft.Add(1);
            Screens[38].OpenTilesLeft.Add(2);
            Screens[38].OpenTilesLeft.Add(3);
            Screens[38].OpenTilesLeft.Add(4);
            Screens[38].OpenTilesLeft.Add(7);
            Screens[38].OpenTilesLeft.Add(8);
            Screens[38].OpenTilesLeft.Add(9);
            Screens[38].OpenTilesLeft.Add(10);
            Screens[38].OpenTilesDown.Add(4);
            Screens[39].Directions.Add(Direction.Left);
            Screens[39].Directions.Add(Direction.Right);
            Screens[39].OpenTilesLeft.Add(7);
            Screens[39].OpenTilesLeft.Add(8);
            Screens[39].OpenTilesLeft.Add(9);
            Screens[39].OpenTilesLeft.Add(10);
            Screens[39].OpenTilesRight.Add(7);
            Screens[39].OpenTilesRight.Add(8);
            Screens[39].OpenTilesRight.Add(9);
            Screens[39].OpenTilesRight.Add(10);
            Screens[TowerOfFortressScreen].Directions.Add(Direction.Right);
            Screens[TowerOfFortressScreen].OpenTilesRight.Add(1);
            Screens[TowerOfFortressScreen].OpenTilesRight.Add(2);
            Screens[TowerOfFortressScreen].OpenTilesRight.Add(3);
            Screens[TowerOfFortressScreen].OpenTilesRight.Add(4);
            Screens[TowerOfFortressScreen].SecondOpenTilesRight.Add(7);
            Screens[TowerOfFortressScreen].SecondOpenTilesRight.Add(8);
            Screens[TowerOfFortressScreen].SecondOpenTilesRight.Add(9);
            Screens[TowerOfFortressScreen].SecondOpenTilesRight.Add(10);
            Screens[TowerOfFortressEntranceScreen].Directions.Add(Direction.Right);
            Screens[TowerOfFortressEntranceScreen].OpenTilesRight.Add(7);
            Screens[TowerOfFortressEntranceScreen].OpenTilesRight.Add(8);
            Screens[TowerOfFortressEntranceScreen].OpenTilesRight.Add(9);
            Screens[TowerOfFortressEntranceScreen].OpenTilesRight.Add(10);
            Screens[42].Directions.Add(Direction.Left);
            Screens[42].Directions.Add(Direction.Right);
            Screens[42].OpenTilesLeft.Add(7);
            Screens[42].OpenTilesLeft.Add(8);
            Screens[42].OpenTilesLeft.Add(9);
            Screens[42].OpenTilesLeft.Add(10);
            Screens[42].OpenTilesRight.Add(7);
            Screens[42].OpenTilesRight.Add(8);
            Screens[42].OpenTilesRight.Add(9);
            Screens[42].OpenTilesRight.Add(10);
            Screens[43].Directions.Add(Direction.Left);
            //Screens[43].Directions.Add(Direction.Right);
            //Screens[43].Directions.Add(Direction.Up);
            Screens[43].OpenTilesLeft.Add(7);
            Screens[43].OpenTilesLeft.Add(8);
            Screens[43].OpenTilesLeft.Add(9);
            Screens[43].OpenTilesLeft.Add(10);
            Screens[43].OpenTilesRight.Add(7);
            Screens[43].OpenTilesRight.Add(8);
            Screens[43].OpenTilesRight.Add(9);
            Screens[43].OpenTilesRight.Add(10);
            Screens[43].OpenTilesUp.Add(6);
            Screens[45].Directions.Add(Direction.Right);
            Screens[45].Directions.Add(Direction.Down);
            Screens[45].OpenTilesRight.Add(7);
            Screens[45].OpenTilesRight.Add(8);
            Screens[45].OpenTilesRight.Add(9);
            Screens[45].OpenTilesRight.Add(10);
            Screens[45].OpenTilesDown.Add(2);
            Screens[46].Directions.Add(Direction.Left);
            Screens[46].Directions.Add(Direction.Right);
            Screens[46].OpenTilesLeft.Add(7);
            Screens[46].OpenTilesLeft.Add(8);
            Screens[46].OpenTilesLeft.Add(9);
            Screens[46].OpenTilesLeft.Add(10);
            Screens[46].OpenTilesRight.Add(2);
            Screens[46].OpenTilesRight.Add(3);
            Screens[46].OpenTilesRight.Add(4);
            Screens[47].Directions.Add(Direction.Left);
            Screens[47].Directions.Add(Direction.Right);
            //Screens[47].Directions.Add(Direction.Down);
            Screens[47].OpenTilesLeft.Add(2);
            Screens[47].OpenTilesLeft.Add(3);
            Screens[47].OpenTilesLeft.Add(4);
            Screens[47].OpenTilesRight.Add(7);
            Screens[47].OpenTilesRight.Add(8);
            Screens[47].OpenTilesRight.Add(9);
            Screens[47].OpenTilesRight.Add(10);
            Screens[47].OpenTilesDown.Add(6);
            Screens[48].Directions.Add(Direction.Left);
            Screens[48].Directions.Add(Direction.Right);
            Screens[48].OpenTilesLeft.Add(7);
            Screens[48].OpenTilesLeft.Add(8);
            Screens[48].OpenTilesLeft.Add(9);
            Screens[48].OpenTilesLeft.Add(10);
            Screens[48].OpenTilesRight.Add(7);
            Screens[48].OpenTilesRight.Add(8);
            Screens[48].OpenTilesRight.Add(9);
            Screens[48].OpenTilesRight.Add(10);
            Screens[49].Directions.Add(Direction.Left);
            Screens[49].Directions.Add(Direction.Right);
            Screens[49].OpenTilesLeft.Add(2);
            Screens[49].OpenTilesLeft.Add(3);
            Screens[49].OpenTilesLeft.Add(4);
            Screens[49].OpenTilesLeft.Add(5);
            Screens[49].OpenTilesLeft.Add(7);
            Screens[49].OpenTilesLeft.Add(8);
            Screens[49].OpenTilesLeft.Add(9);
            Screens[49].OpenTilesLeft.Add(10);
            Screens[49].OpenTilesRight.Add(7);
            Screens[49].OpenTilesRight.Add(8);
            Screens[49].OpenTilesRight.Add(9);
            Screens[49].OpenTilesRight.Add(10);
            Screens[50].Directions.Add(Direction.Left);
            Screens[50].OpenTilesLeft.Add(7);
            Screens[50].OpenTilesLeft.Add(8);
            Screens[50].OpenTilesLeft.Add(9);
            Screens[50].OpenTilesLeft.Add(10);
            Screens[50].OpenTilesUp.Add(1);
            Screens[51].Directions.Add(Direction.Right);
            Screens[51].OpenTilesRight.Add(8);
            Screens[51].OpenTilesRight.Add(9);
            Screens[51].OpenTilesRight.Add(10);
            Screens[51].OpenTilesDown.Add(1);
            Screens[53].Directions.Add(Direction.Right);
            Screens[53].OpenTilesRight.Add(2);
            Screens[53].OpenTilesRight.Add(3);
            Screens[53].OpenTilesRight.Add(4);
            Screens[53].OpenTilesRight.Add(5);
            Screens[53].OpenTilesUp.Add(1);
            Screens[54].Directions.Add(Direction.Left);
            Screens[54].OpenTilesLeft.Add(2);
            Screens[54].OpenTilesLeft.Add(3);
            Screens[54].OpenTilesLeft.Add(4);
            Screens[54].OpenTilesLeft.Add(5);
            Screens[54].OpenTilesDown.Add(1);
            Screens[56].Directions.Add(Direction.Up);
            Screens[56].Directions.Add(Direction.Down);
            Screens[56].OpenTilesUp.Add(14);
            Screens[56].OpenTilesDown.Add(1);
            Screens[58].Directions.Add(Direction.Left);
            Screens[58].Directions.Add(Direction.Right);
            Screens[58].OpenTilesLeft.Add(7);
            Screens[58].OpenTilesLeft.Add(8);
            Screens[58].OpenTilesLeft.Add(9);
            Screens[58].OpenTilesLeft.Add(10);
            Screens[58].OpenTilesRight.Add(3);
            Screens[58].OpenTilesRight.Add(4);
            Screens[58].OpenTilesRight.Add(5);
            Screens[58].OpenTilesRight.Add(6);
            Screens[59].Directions.Add(Direction.Left);
            Screens[59].Directions.Add(Direction.Right);
            Screens[59].OpenTilesLeft.Add(3);
            Screens[59].OpenTilesLeft.Add(4);
            Screens[59].OpenTilesLeft.Add(5);
            Screens[59].OpenTilesLeft.Add(6);
            Screens[59].OpenTilesRight.Add(6);
            Screens[59].OpenTilesRight.Add(7);
            Screens[59].OpenTilesRight.Add(8);
            Screens[60].Directions.Add(Direction.Right);
            Screens[60].OpenTilesRight.Add(1);
            Screens[60].OpenTilesRight.Add(2);
            Screens[60].OpenTilesRight.Add(3);
            Screens[60].OpenTilesRight.Add(7);
            Screens[60].OpenTilesRight.Add(8);
            Screens[60].OpenTilesRight.Add(9);
            Screens[60].OpenTilesRight.Add(10);
            Screens[FortressSpringScreen].Directions.Add(Direction.Left);
            Screens[FortressSpringScreen].OpenTilesLeft.Add(1);
            Screens[FortressSpringScreen].OpenTilesLeft.Add(2);
            Screens[FortressSpringScreen].OpenTilesLeft.Add(3);
            Screens[JokerHouseEntranceScreen].Directions.Add(Direction.Right);
            Screens[JokerHouseEntranceScreen].OpenTilesRight.Add(7);
            Screens[JokerHouseEntranceScreen].OpenTilesRight.Add(8);
            Screens[JokerHouseEntranceScreen].OpenTilesRight.Add(9);
            Screens[JokerHouseEntranceScreen].OpenTilesRight.Add(10);
            Screens[JokerSpringScreen].Directions.Add(Direction.Left);
            Screens[JokerSpringScreen].OpenTilesLeft.Add(7);
            Screens[JokerSpringScreen].OpenTilesLeft.Add(8);
            Screens[JokerSpringScreen].OpenTilesLeft.Add(9);
            Screens[JokerSpringScreen].OpenTilesLeft.Add(10);
        }
    }
}
