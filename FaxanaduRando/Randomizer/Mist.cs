using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class Mist : Level
    {
        public const byte TowerOfSufferScreen = 0;
        public const byte MasconTowerScreen = 1;
        public const byte MistGuruScreen = 6;
        public const byte MasconLeftScreen = 9;
        public const byte MasconRightScreen = 12;
        public const byte MistSecretShopScreen = 13;
        public const byte MistEntranceScreen = 17;
        public const byte TowerOfMistScreen = 23;
        public const byte MistExitScreen = 31; 
        public const byte VictimTowerScreen = 32;
        public const byte VictimLeftScreen = 34;
        public const byte VictimRightScreen = 37;
        public const byte FireMageScreen = 41;
        public const byte PendantScreen = 52;
        public const byte TowerOfSufferEntranceScreen = 62;
        public const byte TowerOfMistElixirScreen = 65;
        public const byte TowerOfMistEntranceScreen = 69;
        public const byte BlackOnyxScreen = 76;
        public const byte MasconTowerEndScreen = 77;
        public const byte MasconTowerEntranceScreen = 79;
        public const byte VictimTowerEntranceScreen = 80;

        private SubLevel.Id firstTowerId;
        private SubLevel.Id secondTowerId;
        private SubLevel.Id thirdTowerId;

        public static Screen End1 = null;
        public static Screen Middle1 = null;
        public static Screen Middle2 = null;
        public static Screen End2 = null;

        public Mist(WorldNumber number, byte[] content) : base(number, content)
        {
            Screens[MistEntranceScreen].Doors.Add(DoorId.MistEntrance);
            Screens[TowerOfSufferScreen].Doors.Add(DoorId.TowerOfSuffer);
            Screens[TowerOfSufferEntranceScreen].Doors.Add(DoorId.TowerOfSufferReturn);
            Screens[MasconTowerScreen].Doors.Add(DoorId.MasconTower);
            Screens[MasconTowerEntranceScreen].Doors.Add(DoorId.MasconTowerReturn);
            Screens[MistGuruScreen].Doors.Add(DoorId.MistSmallHouse);
            Screens[MistGuruScreen].Doors.Add(DoorId.MistGuru);
            Screens[MistGuruScreen].Doors.Add(DoorId.BirdHospital);
            Screens[MistSecretShopScreen].Doors.Add(DoorId.MistSecretShop);
            Screens[22].Doors.Add(DoorId.MistLargeHouse);
            Screens[TowerOfMistScreen].Doors.Add(DoorId.TowerOfMist);
            Screens[TowerOfMistEntranceScreen].Doors.Add(DoorId.TowerOfMistReturn);
            Screens[MistExitScreen].Doors.Add(DoorId.AceKeyHouse);
            Screens[MistExitScreen].Doors.Add(DoorId.MistExit);
            Screens[VictimTowerScreen].Doors.Add(DoorId.VictimTower);
            Screens[VictimTowerEntranceScreen].Doors.Add(DoorId.VictimTowerReturn);
            Screens[FireMageScreen].Doors.Add(DoorId.FireMage);
        }

        public override int GetStartOffset()
        {
            return 0x2C538;
        }

        public override int GetEndOffset()
        {
            return 0x2C759;
        }

        public override void CorrectScreenSprites()
        {
            var temp = Screens[Screens.Count - 1].Sprites;
            for (int i = Screens.Count - 1; i > 34; i--)
            {
                Screens[i].Sprites = Screens[i - 2].Sprites;
            }

            for (int i = 33; i > 11; i--)
            {
                Screens[i].Sprites = Screens[i - 1].Sprites;
            }

            Screens[11].Sprites = temp;
        }

        public override void GetEnds(ref List<Screen> startScreens, ref List<Screen> endScreens, Random random)
        {
            startScreens = new List<Screen>();
            startScreens.Add(Screens[17]);
            endScreens = new List<Screen>();
            if (random.Next(0, 2) == 0)
            {
                startScreens.Add(Screens[MasconRightScreen]);
                startScreens.Add(Screens[VictimRightScreen]);
                endScreens.Add(Screens[MasconLeftScreen]);
                endScreens.Add(Screens[VictimLeftScreen]);
            }
            else
            {
                startScreens.Add(Screens[VictimRightScreen]);
                startScreens.Add(Screens[MasconRightScreen]);
                endScreens.Add(Screens[VictimLeftScreen]);
                endScreens.Add(Screens[MasconLeftScreen]);
            }

            if (random.Next(0, 2) == 0)
            {
                (startScreens[1], endScreens[0]) = (endScreens[0], startScreens[1]);
            }

            if (random.Next(0, 2) == 0)
            {
                (startScreens[2], endScreens[1]) = (endScreens[1], startScreens[2]);
            }

            var possibleEnds = new List<Screen>() { Screens[BlackOnyxScreen] };
            possibleEnds.Add(Screens[PendantScreen]);
            possibleEnds.Add(Screens[TowerOfMistElixirScreen]);
            possibleEnds.Add(Screens[MasconTowerEndScreen]);
            possibleEnds.Add(Screens[FireMageScreen]);
            possibleEnds.Add(Screens[MistGuruScreen]);

            int shuffleIndex = ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Unchanged ? 2 : 0;
            Util.ShuffleList(possibleEnds, shuffleIndex, possibleEnds.Count - 1, random);

            startScreens.Add(Screens[VictimTowerEntranceScreen]);
            startScreens.Add(Screens[MasconTowerEntranceScreen]);
            startScreens.Add(Screens[TowerOfSufferEntranceScreen]);
            startScreens.Add(Screens[TowerOfMistEntranceScreen]);
            firstTowerId = SubLevel.Id.VictimTower;
            secondTowerId = SubLevel.Id.TowerOfSuffer;
            thirdTowerId = SubLevel.Id.TowerOfMist;

            possibleEnds.Insert(shuffleIndex, Screens[TowerOfMistScreen]);
            Util.ShuffleList(possibleEnds, shuffleIndex, possibleEnds.Count - 3, random);
            endScreens.AddRange(possibleEnds);
        }

        public override List<Screen> GetCandidates(Random random)
        {
            var candidates = new List<Screen>();
            candidates.Add(Screens[7]);
            candidates.Add(Screens[8]);
            candidates.AddRange(Screens.GetRange(14, 3));
            candidates.AddRange(Screens.GetRange(18, 2));
            candidates.Add(Screens[21]);
            candidates.AddRange(Screens.GetRange(24, 5));
            candidates.Add(Screens[29]);
            candidates.Add(Screens[30]);
            candidates.Add(Screens[38]);
            candidates.Add(Screens[40]);
            candidates.Add(Screens[44]);
            candidates.Add(Screens[45]);
            candidates.AddRange(Screens.GetRange(46, 3));
            candidates.Add(Screens[51]);
            candidates.Add(Screens[53]);
            candidates.Add(Screens[54]);
            candidates.Add(Screens[55]);
            candidates.Add(Screens[56]);
            candidates.Add(Screens[57]);
            candidates.Add(Screens[60]);
            candidates.Add(Screens[61]);
            candidates.Add(Screens[63]);
            candidates.AddRange(Screens.GetRange(66, 2));
            candidates.AddRange(Screens.GetRange(70, 6));

            Util.ShuffleList(candidates, 0, candidates.Count - 1, random);
            return candidates;
        }

        public override List<Screen> GetSpecialScreens(Random random)
        {
            var specialScreens = new List<Screen>();
            specialScreens.Add(Screens[22]);
            specialScreens.Add(Screens[MistExitScreen]);
            specialScreens.Add(Screens[5]);
            specialScreens.Add(Screens[39]);
            specialScreens.Add(Screens[20]);

            Util.ShuffleList(specialScreens, 0, specialScreens.Count - 1, random);
            return specialScreens;
        }

        public override bool CreateSublevels(List<Screen> startScreens, List<Screen> endScreens, List<Screen> candidates, List<Screen> specialScreens, Random random, uint attempts)
        {
            Screens[5].OpenTilesLeft = new HashSet<byte>();
            Screens[5].OpenTilesRight = new HashSet<byte>();
            if (random.Next(0, 2) == 0)
            {
                Screens[5].OpenTilesLeft.Add(2);
                Screens[5].OpenTilesLeft.Add(3);
                Screens[5].OpenTilesLeft.Add(4);
                Screens[5].OpenTilesLeft.Add(5);
                Screens[5].SecondOpenTilesLeft.Add(7);
                Screens[5].SecondOpenTilesLeft.Add(8);
                Screens[5].SecondOpenTilesLeft.Add(9);
                Screens[5].SecondOpenTilesLeft.Add(10);
                Screens[5].OpenTilesRight.Add(1);
                Screens[5].OpenTilesRight.Add(2);
                Screens[5].OpenTilesRight.Add(3);
                Screens[5].OpenTilesRight.Add(4);
                Screens[5].OpenTilesRight.Add(7);
                Screens[5].OpenTilesRight.Add(7);
                Screens[5].OpenTilesRight.Add(8);
                Screens[5].OpenTilesRight.Add(9);
                Screens[5].OpenTilesRight.Add(10);
            }
            else
            {
                Screens[5].OpenTilesLeft.Add(2);
                Screens[5].OpenTilesLeft.Add(3);
                Screens[5].OpenTilesLeft.Add(4);
                Screens[5].OpenTilesLeft.Add(5);
                Screens[5].OpenTilesLeft.Add(7);
                Screens[5].OpenTilesLeft.Add(8);
                Screens[5].OpenTilesLeft.Add(9);
                Screens[5].OpenTilesLeft.Add(10);
                Screens[5].OpenTilesRight.Add(1);
                Screens[5].OpenTilesRight.Add(2);
                Screens[5].OpenTilesRight.Add(3);
                Screens[5].OpenTilesRight.Add(4);
                Screens[5].SecondOpenTilesRight.Add(7);
                Screens[5].SecondOpenTilesRight.Add(8);
                Screens[5].SecondOpenTilesRight.Add(9);
                Screens[5].SecondOpenTilesRight.Add(10);
            }

            bool result = CreateSublevel(startScreens[0], endScreens[0], candidates, specialScreens, random, SubLevel.Id.EarlyMist, attempts);
            if (!result)
            {
                return result;
            }

            if (GeneralOptions.ShuffleSegments != GeneralOptions.SegmentShuffle.AllSegments)
            {
                result = CreateSublevel(startScreens[1], endScreens[1], candidates, specialScreens, random, SubLevel.Id.MiddleMist, attempts);
                if (!result)
                {
                    return result;
                }
            }

            result = CreateSublevel(startScreens[2], endScreens[2], candidates, specialScreens, random, SubLevel.Id.LateMist, attempts);
            if (!result)
            {
                return result;
            }

            if (GeneralOptions.ShuffleSegments == GeneralOptions.SegmentShuffle.AllSegments)
            {
                if (specialScreens.Contains(Screens[MistExitScreen]))
                {
                    return false;
                }

                result = CreateSublevel(startScreens[1], endScreens[1], candidates, specialScreens, random, SubLevel.Id.MiddleMist, attempts);
                if (!result)
                {
                    return result;
                }
            }

            result = CreateSublevel(Screens[3], Screens[MasconTowerScreen], candidates, specialScreens, random, Screens[MasconLeftScreen].ParentSublevel, attempts, false);
            if (!result)
            {
                return result;
            }

            if (specialScreens.Contains(Screens[MistExitScreen]))
            {
                return false;
            }

            result = CreateSublevel(startScreens[3], endScreens[3], candidates, specialScreens, random, firstTowerId, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[4], endScreens[4], candidates, specialScreens, random, SubLevel.Id.MasconTower, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[5], endScreens[5], candidates, specialScreens, random, secondTowerId, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[6], endScreens[6], candidates, specialScreens, random, thirdTowerId, attempts);
            if (!result)
            {
                return result;
            }

            if (specialScreens.Count > 0)
            {
                return false;
            }

            result = CreateSublevel(Screens[5], endScreens[7], candidates, specialScreens, random, Screens[5].ParentSublevel, attempts, false);
            if (!result)
            {
                return result;
            }

            if (candidates.Count > 10)
            {
                return false;
            }

            Screens[MasconTowerScreen].ScrollData.Down = MasconLeftScreen;
            Screens[29].ScrollData.Down = VictimTowerEntranceScreen;
            Screens[44].ScrollData.Down = 44;

            End1 = endScreens[0];
            Middle1 = startScreens[1];
            Middle2 = endScreens[1];
            End2 = startScreens[2];

            return result;
        }

        public override void SetupScreens()
        {
            startToSpecial[TowerOfSufferEntranceScreen] = 5;
            startToSpecial[VictimTowerEntranceScreen] = 39;

            Screens[MasconLeftScreen].FriendEnds[Direction.Up] = Screens[3];
            Screens[5].FriendEnds[Direction.Up] = Screens[TowerOfSufferScreen];

            Screens[20].FriendEnds[Direction.Up] = Screens[MistSecretShopScreen];
            Screens[39].FriendEnds[Direction.Up] = Screens[VictimTowerScreen];

            Screens[MasconTowerScreen].Directions.Add(Direction.Right);
            Screens[MasconTowerScreen].OpenTilesRight.Add(2);
            Screens[MasconTowerScreen].OpenTilesRight.Add(3);
            Screens[MasconTowerScreen].OpenTilesRight.Add(4);
            Screens[MasconTowerScreen].OpenTilesRight.Add(5);
            Screens[MasconTowerScreen].OpenTilesRight.Add(6);
            Screens[MasconTowerScreen].OpenTilesRight.Add(7);
            Screens[MasconTowerScreen].OpenTilesRight.Add(8);
            Screens[MasconTowerScreen].OpenTilesRight.Add(9);
            Screens[3].Directions.Add(Direction.Left);
            //Screens[3].Directions.Add(Direction.Down);
            Screens[3].OpenTilesLeft.Add(5);
            Screens[3].OpenTilesLeft.Add(6);
            Screens[3].OpenTilesLeft.Add(7);
            Screens[3].OpenTilesLeft.Add(8);
            Screens[3].OpenTilesDown.Add(12);
            Screens[5].Directions.Add(Direction.Left);
            Screens[5].Directions.Add(Direction.Right);
            Screens[5].Directions.Add(Direction.Down);
            Screens[5].OpenTilesDown.Add(2);
            Screens[6].Directions.Add(Direction.Left);
            Screens[6].OpenTilesLeft.Add(1);
            Screens[6].OpenTilesLeft.Add(2);
            Screens[6].OpenTilesLeft.Add(3);
            Screens[6].OpenTilesLeft.Add(4);
            Screens[6].OpenTilesLeft.Add(5);
            Screens[6].OpenTilesLeft.Add(6);
            Screens[6].OpenTilesLeft.Add(7);
            Screens[6].OpenTilesLeft.Add(8);
            Screens[6].OpenTilesLeft.Add(9);
            Screens[6].OpenTilesLeft.Add(10);
            Screens[7].Directions.Add(Direction.Right);
            Screens[7].Directions.Add(Direction.Down);
            Screens[7].OpenTilesRight.Add(1);
            Screens[7].OpenTilesRight.Add(2);
            Screens[7].OpenTilesRight.Add(3);
            Screens[7].OpenTilesRight.Add(4);
            Screens[7].OpenTilesRight.Add(7);
            Screens[7].OpenTilesRight.Add(8);
            Screens[7].OpenTilesRight.Add(9);
            Screens[7].OpenTilesRight.Add(10);
            Screens[7].OpenTilesDown.Add(1);
            Screens[8].Directions.Add(Direction.Left);
            Screens[8].Directions.Add(Direction.Right);
            Screens[8].OpenTilesLeft.Add(7);
            Screens[8].OpenTilesLeft.Add(8);
            Screens[8].OpenTilesLeft.Add(9);
            Screens[8].OpenTilesLeft.Add(10);
            Screens[8].OpenTilesRight.Add(7);
            Screens[8].OpenTilesRight.Add(8);
            Screens[8].OpenTilesRight.Add(9);
            Screens[8].OpenTilesRight.Add(10);
            Screens[9].Directions.Add(Direction.Left);
            //Screens[9].Directions.Add(Direction.Up);
            Screens[9].OpenTilesLeft.Add(0);
            Screens[9].OpenTilesLeft.Add(1);
            Screens[9].OpenTilesLeft.Add(2);
            Screens[9].OpenTilesLeft.Add(3);
            Screens[9].OpenTilesLeft.Add(4);
            Screens[9].OpenTilesLeft.Add(5);
            Screens[9].OpenTilesLeft.Add(6);
            Screens[9].OpenTilesLeft.Add(7);
            Screens[9].OpenTilesLeft.Add(8);
            Screens[9].OpenTilesLeft.Add(9);
            Screens[9].OpenTilesLeft.Add(10);
            Screens[9].OpenTilesUp.Add(12);
            Screens[12].Directions.Add(Direction.Down);
            Screens[12].OpenTilesDown.Add(3);
            Screens[14].Directions.Add(Direction.Right);
            Screens[14].Directions.Add(Direction.Down);
            Screens[14].OpenTilesRight.Add(7);
            Screens[14].OpenTilesRight.Add(8);
            Screens[14].OpenTilesRight.Add(9);
            Screens[14].OpenTilesRight.Add(10);
            Screens[14].OpenTilesDown.Add(1);
            Screens[15].Directions.Add(Direction.Left);
            Screens[15].Directions.Add(Direction.Up);
            Screens[15].OpenTilesLeft.Add(7);
            Screens[15].OpenTilesLeft.Add(8);
            Screens[15].OpenTilesLeft.Add(9);
            Screens[15].OpenTilesLeft.Add(10);
            Screens[15].OpenTilesUp.Add(2);
            Screens[16].Directions.Add(Direction.Right);
            Screens[16].Directions.Add(Direction.Up);
            Screens[16].OpenTilesRight.Add(0);
            Screens[16].OpenTilesRight.Add(1);
            Screens[16].OpenTilesRight.Add(2);
            Screens[16].OpenTilesRight.Add(3);
            Screens[16].OpenTilesRight.Add(7);
            Screens[16].OpenTilesRight.Add(8);
            Screens[16].OpenTilesRight.Add(9);
            Screens[16].OpenTilesRight.Add(10);
            Screens[16].OpenTilesUp.Add(1);
            Screens[17].Directions.Add(Direction.Left);
            Screens[17].Directions.Add(Direction.Right);
            Screens[17].OpenTilesLeft.Add(7);
            Screens[17].OpenTilesLeft.Add(8);
            Screens[17].OpenTilesLeft.Add(9);
            Screens[17].OpenTilesLeft.Add(10);
            Screens[17].OpenTilesRight.Add(9);
            Screens[17].OpenTilesRight.Add(10);
            Screens[18].Directions.Add(Direction.Right);
            Screens[18].Directions.Add(Direction.Up);
            Screens[18].OpenTilesRight.Add(7);
            Screens[18].OpenTilesRight.Add(8);
            Screens[18].OpenTilesRight.Add(9);
            Screens[18].OpenTilesRight.Add(10);
            Screens[18].OpenTilesUp.Add(3);
            Screens[19].Directions.Add(Direction.Left);
            Screens[19].Directions.Add(Direction.Right);
            Screens[19].OpenTilesLeft.Add(7);
            Screens[19].OpenTilesLeft.Add(8);
            Screens[19].OpenTilesLeft.Add(9);
            Screens[19].OpenTilesLeft.Add(10);
            Screens[19].OpenTilesRight.Add(7);
            Screens[19].OpenTilesRight.Add(8);
            Screens[19].OpenTilesRight.Add(9);
            Screens[19].OpenTilesRight.Add(10);
            Screens[20].Directions.Add(Direction.Left);
            Screens[20].Directions.Add(Direction.Right);
            Screens[20].OpenTilesLeft.Add(2);
            Screens[20].OpenTilesLeft.Add(3);
            Screens[20].OpenTilesLeft.Add(4);
            Screens[20].SecondOpenTilesLeft.Add(7);
            Screens[20].SecondOpenTilesLeft.Add(8);
            Screens[20].SecondOpenTilesLeft.Add(9);
            Screens[20].SecondOpenTilesLeft.Add(10);
            Screens[20].OpenTilesRight.Add(7);
            Screens[20].OpenTilesRight.Add(8);
            Screens[20].OpenTilesRight.Add(9);
            Screens[20].OpenTilesRight.Add(10);
            Screens[21].Directions.Add(Direction.Left);
            Screens[21].Directions.Add(Direction.Right);
            Screens[21].OpenTilesLeft.Add(7);
            Screens[21].OpenTilesLeft.Add(8);
            Screens[21].OpenTilesLeft.Add(9);
            Screens[21].OpenTilesLeft.Add(10);
            Screens[21].OpenTilesRight.Add(7);
            Screens[21].OpenTilesRight.Add(8);
            Screens[21].OpenTilesRight.Add(9);
            Screens[21].OpenTilesRight.Add(10);
            Screens[22].Directions.Add(Direction.Left);
            Screens[22].Directions.Add(Direction.Up);
            Screens[22].OpenTilesLeft.Add(7);
            Screens[22].OpenTilesLeft.Add(8);
            Screens[22].OpenTilesLeft.Add(9);
            Screens[22].OpenTilesLeft.Add(10);
            Screens[22].OpenTilesUp.Add(1);
            Screens[23].Directions.Add(Direction.Right);
            Screens[23].OpenTilesRight.Add(5);
            Screens[23].OpenTilesRight.Add(6);
            Screens[23].OpenTilesRight.Add(7);
            Screens[23].OpenTilesRight.Add(8);
            Screens[24].Directions.Add(Direction.Left);
            Screens[24].Directions.Add(Direction.Right);
            Screens[24].OpenTilesLeft.Add(5);
            Screens[24].OpenTilesLeft.Add(6);
            Screens[24].OpenTilesLeft.Add(7);
            Screens[24].OpenTilesLeft.Add(8);
            Screens[24].OpenTilesRight.Add(3);
            Screens[24].OpenTilesRight.Add(4);
            Screens[24].OpenTilesRight.Add(5);
            Screens[24].OpenTilesRight.Add(6);
            Screens[25].Directions.Add(Direction.Left);
            Screens[25].Directions.Add(Direction.Down);
            Screens[25].OpenTilesLeft.Add(3);
            Screens[25].OpenTilesLeft.Add(4);
            Screens[25].OpenTilesLeft.Add(5);
            Screens[25].OpenTilesLeft.Add(6);
            Screens[25].OpenTilesDown.Add(7);
            Screens[26].Directions.Add(Direction.Right);
            Screens[26].Directions.Add(Direction.Up);
            Screens[26].OpenTilesRight.Add(7);
            Screens[26].OpenTilesRight.Add(8);
            Screens[26].OpenTilesRight.Add(9);
            Screens[26].OpenTilesRight.Add(10);
            Screens[26].OpenTilesUp.Add(7);
            Screens[27].Directions.Add(Direction.Left);
            Screens[27].Directions.Add(Direction.Right);
            Screens[27].OpenTilesLeft.Add(7);
            Screens[27].OpenTilesLeft.Add(8);
            Screens[27].OpenTilesLeft.Add(9);
            Screens[27].OpenTilesLeft.Add(10);
            Screens[27].OpenTilesRight.Add(8);
            Screens[27].OpenTilesRight.Add(9);
            Screens[27].OpenTilesRight.Add(10);
            Screens[27].OpenTilesRight.Add(11);
            Screens[28].Directions.Add(Direction.Left);
            Screens[28].Directions.Add(Direction.Right);
            Screens[28].OpenTilesLeft.Add(7);
            Screens[28].OpenTilesLeft.Add(8);
            Screens[28].OpenTilesLeft.Add(9);
            Screens[28].OpenTilesLeft.Add(10);
            Screens[28].OpenTilesLeft.Add(11);
            Screens[28].OpenTilesRight.Add(7);
            Screens[28].OpenTilesRight.Add(8);
            Screens[28].OpenTilesRight.Add(9);
            Screens[28].OpenTilesRight.Add(10);
            Screens[29].Directions.Add(Direction.Left);
            Screens[29].Directions.Add(Direction.Right);
            Screens[29].OpenTilesLeft.Add(7);
            Screens[29].OpenTilesLeft.Add(8);
            Screens[29].OpenTilesLeft.Add(9);
            Screens[29].OpenTilesLeft.Add(10);
            Screens[29].OpenTilesRight.Add(8);
            Screens[29].OpenTilesRight.Add(9);
            Screens[29].OpenTilesRight.Add(10);
            Screens[30].Directions.Add(Direction.Left);
            Screens[30].Directions.Add(Direction.Right);
            Screens[30].OpenTilesLeft.Add(3);
            Screens[30].OpenTilesLeft.Add(4);
            Screens[30].OpenTilesLeft.Add(5);
            Screens[30].OpenTilesLeft.Add(6);
            Screens[30].OpenTilesLeft.Add(8);
            Screens[30].OpenTilesLeft.Add(9);
            Screens[30].OpenTilesLeft.Add(10);
            Screens[30].OpenTilesRight.Add(2);
            Screens[30].OpenTilesRight.Add(3);
            Screens[30].OpenTilesRight.Add(4);
            Screens[30].OpenTilesRight.Add(5);
            Screens[30].OpenTilesRight.Add(7);
            Screens[30].OpenTilesRight.Add(8);
            Screens[30].OpenTilesRight.Add(9);
            Screens[30].OpenTilesRight.Add(10);
            Screens[MistExitScreen].Directions.Add(Direction.Left);
            Screens[MistExitScreen].Directions.Add(Direction.Right);
            Screens[MistExitScreen].OpenTilesLeft.Add(7);
            Screens[MistExitScreen].OpenTilesLeft.Add(8);
            Screens[MistExitScreen].OpenTilesLeft.Add(9);
            Screens[MistExitScreen].OpenTilesLeft.Add(10);
            Screens[MistExitScreen].OpenTilesRight.Add(7);
            Screens[MistExitScreen].OpenTilesRight.Add(8);
            Screens[MistExitScreen].OpenTilesRight.Add(9);
            Screens[MistExitScreen].OpenTilesRight.Add(10);
            Screens[33].Directions.Add(Direction.Left);
            Screens[33].Directions.Add(Direction.Down);
            Screens[33].OpenTilesLeft.Add(5);
            Screens[33].OpenTilesLeft.Add(6);
            Screens[33].OpenTilesLeft.Add(7);
            Screens[33].OpenTilesLeft.Add(8);
            Screens[33].OpenTilesDown.Add(13);
            Screens[34].Directions.Add(Direction.Down);
            Screens[34].OpenTilesDown.Add(3);
            Screens[37].Directions.Add(Direction.Down);
            Screens[37].OpenTilesDown.Add(4);
            Screens[38].Directions.Add(Direction.Right);
            Screens[38].Directions.Add(Direction.Down);
            Screens[38].OpenTilesRight.Add(7);
            Screens[38].OpenTilesRight.Add(8);
            Screens[38].OpenTilesRight.Add(9);
            Screens[38].OpenTilesRight.Add(10);
            Screens[38].OpenTilesDown.Add(3);
            Screens[39].Directions.Add(Direction.Left);
            Screens[39].Directions.Add(Direction.Right);
            Screens[39].OpenTilesLeft.Add(7);
            Screens[39].OpenTilesLeft.Add(8);
            Screens[39].OpenTilesLeft.Add(9);
            Screens[39].OpenTilesLeft.Add(10);
            Screens[39].OpenTilesRight.Add(1);
            Screens[39].OpenTilesRight.Add(2);
            Screens[39].OpenTilesRight.Add(3);
            Screens[39].OpenTilesRight.Add(4);
            Screens[39].OpenTilesRight.Add(7);
            Screens[39].OpenTilesRight.Add(8);
            Screens[39].OpenTilesRight.Add(9);
            Screens[39].OpenTilesRight.Add(10);
            Screens[40].Directions.Add(Direction.Left);
            Screens[40].Directions.Add(Direction.Right);
            Screens[40].OpenTilesLeft.Add(7);
            Screens[40].OpenTilesLeft.Add(8);
            Screens[40].OpenTilesLeft.Add(9);
            Screens[40].OpenTilesLeft.Add(10);
            Screens[40].OpenTilesRight.Add(7);
            Screens[40].OpenTilesRight.Add(8);
            Screens[40].OpenTilesRight.Add(9);
            Screens[40].OpenTilesRight.Add(10);
            Screens[41].Directions.Add(Direction.Left);
            Screens[41].OpenTilesLeft.Add(7);
            Screens[41].OpenTilesLeft.Add(8);
            Screens[41].OpenTilesLeft.Add(9);
            Screens[41].OpenTilesLeft.Add(10);
            Screens[42].Directions.Add(Direction.Right);
            Screens[42].Directions.Add(Direction.Down);
            Screens[42].OpenTilesRight.Add(6);
            Screens[42].OpenTilesRight.Add(7);
            Screens[42].OpenTilesRight.Add(8);
            Screens[42].OpenTilesRight.Add(9);
            Screens[42].OpenTilesDown.Add(2);
            Screens[43].Directions.Add(Direction.Left);
            Screens[43].Directions.Add(Direction.Right);
            Screens[43].OpenTilesLeft.Add(6);
            Screens[43].OpenTilesLeft.Add(7);
            Screens[43].OpenTilesLeft.Add(8);
            Screens[43].OpenTilesLeft.Add(9);
            Screens[43].OpenTilesRight.Add(2);
            Screens[43].OpenTilesRight.Add(3);
            Screens[43].OpenTilesRight.Add(4);
            Screens[43].OpenTilesRight.Add(5);
            Screens[44].Directions.Add(Direction.Left);
            Screens[44].Directions.Add(Direction.Up);
            Screens[44].OpenTilesLeft.Add(2);
            Screens[44].OpenTilesLeft.Add(3);
            Screens[44].OpenTilesLeft.Add(4);
            Screens[44].OpenTilesLeft.Add(5);
            Screens[44].OpenTilesUp.Add(3);
            Screens[45].Directions.Add(Direction.Right);
            Screens[45].Directions.Add(Direction.Up);
            Screens[45].OpenTilesRight.Add(7);
            Screens[45].OpenTilesRight.Add(8);
            Screens[45].OpenTilesRight.Add(9);
            Screens[45].OpenTilesRight.Add(10);
            Screens[45].OpenTilesUp.Add(4);
            Screens[46].Directions.Add(Direction.Left);
            Screens[46].Directions.Add(Direction.Right);
            Screens[46].OpenTilesLeft.Add(7);
            Screens[46].OpenTilesLeft.Add(8);
            Screens[46].OpenTilesLeft.Add(9);
            Screens[46].OpenTilesLeft.Add(10);
            Screens[46].OpenTilesRight.Add(7);
            Screens[46].OpenTilesRight.Add(8);
            Screens[46].OpenTilesRight.Add(9);
            Screens[46].OpenTilesRight.Add(10);
            Screens[47].Directions.Add(Direction.Left);
            Screens[47].Directions.Add(Direction.Up);
            Screens[47].OpenTilesLeft.Add(2);
            Screens[47].OpenTilesLeft.Add(3);
            Screens[47].OpenTilesLeft.Add(4);
            Screens[47].OpenTilesLeft.Add(5);
            Screens[47].OpenTilesLeft.Add(7);
            Screens[47].OpenTilesLeft.Add(8);
            Screens[47].OpenTilesLeft.Add(9);
            Screens[47].OpenTilesLeft.Add(10);
            Screens[47].OpenTilesUp.Add(3);
            Screens[48].Directions.Add(Direction.Right);
            Screens[48].Directions.Add(Direction.Down);
            Screens[48].OpenTilesRight.Add(3);
            Screens[48].OpenTilesRight.Add(4);
            Screens[48].OpenTilesRight.Add(5);
            Screens[48].OpenTilesRight.Add(6);
            Screens[48].OpenTilesDown.Add(2);
            Screens[49].Directions.Add(Direction.Left);
            Screens[49].Directions.Add(Direction.Right);
            Screens[49].OpenTilesLeft.Add(3);
            Screens[49].OpenTilesLeft.Add(4);
            Screens[49].OpenTilesLeft.Add(5);
            Screens[49].OpenTilesLeft.Add(6);
            Screens[49].OpenTilesRight.Add(8);
            Screens[49].OpenTilesRight.Add(9);
            Screens[49].OpenTilesRight.Add(10);
            Screens[49].OpenTilesRight.Add(11);
            Screens[51].Directions.Add(Direction.Right);
            Screens[51].Directions.Add(Direction.Down);
            Screens[51].OpenTilesRight.Add(6);
            Screens[51].OpenTilesRight.Add(7);
            Screens[51].OpenTilesRight.Add(8);
            Screens[51].OpenTilesRight.Add(9);
            Screens[51].OpenTilesDown.Add(1);
            Screens[PendantScreen].Directions.Add(Direction.Left);
            Screens[PendantScreen].OpenTilesLeft.Add(6);
            Screens[PendantScreen].OpenTilesLeft.Add(7);
            Screens[PendantScreen].OpenTilesLeft.Add(8);
            Screens[PendantScreen].OpenTilesLeft.Add(9);
            Screens[53].Directions.Add(Direction.Right);
            Screens[53].Directions.Add(Direction.Up);
            Screens[53].Directions.Add(Direction.Down);
            Screens[53].OpenTilesRight.Add(10);
            Screens[53].OpenTilesRight.Add(11);
            Screens[53].OpenTilesUp.Add(2);
            Screens[53].OpenTilesDown.Add(2);
            Screens[54].Directions.Add(Direction.Right);
            Screens[54].Directions.Add(Direction.Up);
            Screens[54].OpenTilesRight.Add(1);
            Screens[54].OpenTilesRight.Add(2);
            Screens[54].OpenTilesRight.Add(3);
            Screens[54].OpenTilesRight.Add(4);
            Screens[54].OpenTilesRight.Add(5);
            Screens[54].OpenTilesRight.Add(6);
            Screens[54].OpenTilesRight.Add(7);
            Screens[54].OpenTilesRight.Add(8);
            Screens[54].OpenTilesRight.Add(9);
            Screens[54].OpenTilesRight.Add(10);
            Screens[54].OpenTilesRight.Add(11);
            Screens[54].OpenTilesUp.Add(1);
            Screens[55].Directions.Add(Direction.Right);
            Screens[55].Directions.Add(Direction.Down);
            Screens[55].OpenTilesRight.Add(8);
            Screens[55].OpenTilesRight.Add(9);
            Screens[55].OpenTilesRight.Add(10);
            Screens[55].OpenTilesRight.Add(11);
            Screens[55].OpenTilesDown.Add(5);
            Screens[56].Directions.Add(Direction.Left);
            Screens[56].Directions.Add(Direction.Right);
            Screens[56].OpenTilesLeft.Add(8);
            Screens[56].OpenTilesLeft.Add(9);
            Screens[56].OpenTilesLeft.Add(10);
            Screens[56].OpenTilesLeft.Add(11);
            Screens[56].OpenTilesRight.Add(8);
            Screens[56].OpenTilesRight.Add(9);
            Screens[56].OpenTilesRight.Add(10);
            Screens[56].OpenTilesRight.Add(11);
            Screens[57].Directions.Add(Direction.Left);
            Screens[57].Directions.Add(Direction.Right);
            Screens[57].OpenTilesLeft.Add(8);
            Screens[57].OpenTilesLeft.Add(9);
            Screens[57].OpenTilesLeft.Add(10);
            Screens[57].OpenTilesLeft.Add(11);
            Screens[57].OpenTilesRight.Add(8);
            Screens[57].OpenTilesRight.Add(9);
            Screens[57].OpenTilesRight.Add(10);
            Screens[57].OpenTilesRight.Add(11);
            Screens[59].Directions.Add(Direction.Right);
            Screens[59].Directions.Add(Direction.Up);
            Screens[59].OpenTilesRight.Add(2);
            Screens[59].OpenTilesRight.Add(3);
            Screens[59].OpenTilesRight.Add(4);
            Screens[59].OpenTilesRight.Add(5);
            Screens[59].OpenTilesUp.Add(2);
            Screens[59].OpenTilesUp.Add(3);
            Screens[59].OpenTilesUp.Add(4);
            Screens[59].OpenTilesUp.Add(5);
            Screens[59].OpenTilesUp.Add(6);
            Screens[60].Directions.Add(Direction.Left);
            Screens[60].Directions.Add(Direction.Down);
            Screens[60].OpenTilesLeft.Add(2);
            Screens[60].OpenTilesLeft.Add(3);
            Screens[60].OpenTilesLeft.Add(4);
            Screens[60].OpenTilesLeft.Add(5);
            Screens[60].OpenTilesDown.Add(1);
            Screens[61].Directions.Add(Direction.Left);
            Screens[61].Directions.Add(Direction.Up);
            Screens[61].OpenTilesLeft.Add(7);
            Screens[61].OpenTilesLeft.Add(8);
            Screens[61].OpenTilesLeft.Add(10);
            Screens[61].OpenTilesLeft.Add(11);
            Screens[61].OpenTilesUp.Add(5);
            Screens[TowerOfSufferEntranceScreen].Directions.Add(Direction.Left);
            Screens[TowerOfSufferEntranceScreen].Directions.Add(Direction.Up);
            Screens[TowerOfSufferEntranceScreen].OpenTilesLeft.Add(9);
            Screens[TowerOfSufferEntranceScreen].OpenTilesLeft.Add(10);
            Screens[TowerOfSufferEntranceScreen].OpenTilesUp.Add(1);
            Screens[TowerOfSufferEntranceScreen].OpenTilesUp.Add(14);
            Screens[63].Directions.Add(Direction.Right);
            Screens[63].Directions.Add(Direction.Down);
            Screens[63].OpenTilesRight.Add(7);
            Screens[63].OpenTilesRight.Add(8);
            Screens[63].OpenTilesRight.Add(9);
            Screens[63].OpenTilesRight.Add(10);
            Screens[63].OpenTilesDown.Add(1);
            Screens[TowerOfMistElixirScreen].Directions.Add(Direction.Right);
            Screens[TowerOfMistElixirScreen].OpenTilesRight.Add(8);
            Screens[TowerOfMistElixirScreen].OpenTilesRight.Add(9);
            Screens[TowerOfMistElixirScreen].OpenTilesRight.Add(10);
            Screens[TowerOfMistElixirScreen].OpenTilesRight.Add(11);
            Screens[66].Directions.Add(Direction.Left);
            Screens[66].Directions.Add(Direction.Right);
            Screens[66].OpenTilesLeft.Add(8);
            Screens[66].OpenTilesLeft.Add(9);
            Screens[66].OpenTilesLeft.Add(10);
            Screens[66].OpenTilesLeft.Add(11);
            Screens[66].OpenTilesRight.Add(8);
            Screens[66].OpenTilesRight.Add(9);
            Screens[66].OpenTilesRight.Add(10);
            Screens[66].OpenTilesRight.Add(11);
            Screens[67].Directions.Add(Direction.Left);
            Screens[67].Directions.Add(Direction.Right);
            Screens[67].OpenTilesLeft.Add(8);
            Screens[67].OpenTilesLeft.Add(9);
            Screens[67].OpenTilesLeft.Add(10);
            Screens[67].OpenTilesLeft.Add(11);
            Screens[67].OpenTilesRight.Add(8);
            Screens[67].OpenTilesRight.Add(9);
            Screens[67].OpenTilesRight.Add(10);
            Screens[67].OpenTilesRight.Add(11);
            Screens[TowerOfMistEntranceScreen].Directions.Add(Direction.Left);
            Screens[TowerOfMistEntranceScreen].Directions.Add(Direction.Right);
            Screens[TowerOfMistEntranceScreen].OpenTilesLeft.Add(10);
            Screens[TowerOfMistEntranceScreen].OpenTilesLeft.Add(11);
            Screens[TowerOfMistEntranceScreen].OpenTilesRight.Add(8);
            Screens[TowerOfMistEntranceScreen].OpenTilesRight.Add(9);
            Screens[TowerOfMistEntranceScreen].OpenTilesRight.Add(10);
            Screens[TowerOfMistEntranceScreen].OpenTilesRight.Add(11);
            Screens[70].Directions.Add(Direction.Left);
            Screens[70].Directions.Add(Direction.Right);
            Screens[70].OpenTilesLeft.Add(8);
            Screens[70].OpenTilesLeft.Add(9);
            Screens[70].OpenTilesLeft.Add(10);
            Screens[70].OpenTilesLeft.Add(11);
            Screens[70].OpenTilesRight.Add(8);
            Screens[70].OpenTilesRight.Add(9);
            Screens[70].OpenTilesRight.Add(10);
            Screens[70].OpenTilesRight.Add(11);
            Screens[71].Directions.Add(Direction.Left);
            Screens[71].Directions.Add(Direction.Right);
            Screens[71].OpenTilesLeft.Add(1);
            Screens[71].OpenTilesLeft.Add(2);
            Screens[71].OpenTilesLeft.Add(3);
            Screens[71].OpenTilesLeft.Add(4);
            Screens[71].OpenTilesLeft.Add(8);
            Screens[71].OpenTilesLeft.Add(9);
            Screens[71].OpenTilesLeft.Add(10);
            Screens[71].OpenTilesLeft.Add(11);
            Screens[71].OpenTilesRight.Add(1);
            Screens[71].OpenTilesRight.Add(2);
            Screens[71].OpenTilesRight.Add(3);
            Screens[71].OpenTilesRight.Add(4);
            Screens[72].Directions.Add(Direction.Left);
            Screens[72].Directions.Add(Direction.Right);
            Screens[72].Directions.Add(Direction.Up);
            Screens[72].OpenTilesLeft.Add(1);
            Screens[72].OpenTilesLeft.Add(2);
            Screens[72].OpenTilesLeft.Add(3);
            Screens[72].OpenTilesLeft.Add(4);
            Screens[72].OpenTilesRight.Add(8);
            Screens[72].OpenTilesRight.Add(9);
            Screens[72].OpenTilesRight.Add(10);
            Screens[72].OpenTilesRight.Add(11);
            Screens[72].OpenTilesUp.Add(1);
            Screens[73].Directions.Add(Direction.Left);
            Screens[73].Directions.Add(Direction.Right);
            Screens[73].OpenTilesLeft.Add(8);
            Screens[73].OpenTilesLeft.Add(9);
            Screens[73].OpenTilesLeft.Add(10);
            Screens[73].OpenTilesLeft.Add(11);
            Screens[73].OpenTilesRight.Add(5);
            Screens[73].OpenTilesRight.Add(6);
            Screens[73].OpenTilesRight.Add(7);
            Screens[74].Directions.Add(Direction.Left);
            Screens[74].Directions.Add(Direction.Right);
            Screens[74].OpenTilesLeft.Add(5);
            Screens[74].OpenTilesLeft.Add(6);
            Screens[74].OpenTilesLeft.Add(7);
            Screens[74].OpenTilesRight.Add(1);
            Screens[74].OpenTilesRight.Add(2);
            Screens[74].OpenTilesRight.Add(3);
            Screens[74].OpenTilesRight.Add(4);
            Screens[74].OpenTilesRight.Add(8);
            Screens[74].OpenTilesRight.Add(9);
            Screens[74].OpenTilesRight.Add(10);
            Screens[74].OpenTilesRight.Add(11);
            Screens[75].Directions.Add(Direction.Left);
            Screens[75].Directions.Add(Direction.Right);
            Screens[75].OpenTilesLeft.Add(8);
            Screens[75].OpenTilesLeft.Add(9);
            Screens[75].OpenTilesLeft.Add(10);
            Screens[75].OpenTilesLeft.Add(11);
            Screens[75].OpenTilesRight.Add(8);
            Screens[75].OpenTilesRight.Add(9);
            Screens[75].OpenTilesRight.Add(10);
            Screens[75].OpenTilesRight.Add(11);
            Screens[BlackOnyxScreen].Directions.Add(Direction.Left);
            Screens[BlackOnyxScreen].OpenTilesLeft.Add(8);
            Screens[BlackOnyxScreen].OpenTilesLeft.Add(9);
            Screens[BlackOnyxScreen].OpenTilesLeft.Add(10);
            Screens[BlackOnyxScreen].OpenTilesLeft.Add(11);
            Screens[MasconTowerEndScreen].Directions.Add(Direction.Right);
            Screens[MasconTowerEndScreen].OpenTilesRight.Add(8);
            Screens[MasconTowerEndScreen].OpenTilesRight.Add(9);
            Screens[MasconTowerEndScreen].OpenTilesRight.Add(10);
            Screens[MasconTowerEndScreen].OpenTilesRight.Add(11);
            Screens[MasconTowerEntranceScreen].Directions.Add(Direction.Left);
            Screens[MasconTowerEntranceScreen].Directions.Add(Direction.Right);
            Screens[MasconTowerEntranceScreen].OpenTilesLeft.Add(9);
            Screens[MasconTowerEntranceScreen].OpenTilesLeft.Add(10);
            Screens[MasconTowerEntranceScreen].OpenTilesLeft.Add(11);
            Screens[MasconTowerEntranceScreen].OpenTilesRight.Add(10);
            Screens[MasconTowerEntranceScreen].OpenTilesRight.Add(11);
            Screens[VictimTowerEntranceScreen].Directions.Add(Direction.Right);
            Screens[VictimTowerEntranceScreen].OpenTilesRight.Add(8);
            Screens[VictimTowerEntranceScreen].OpenTilesRight.Add(9);
            Screens[VictimTowerEntranceScreen].OpenTilesRight.Add(10);
            Screens[VictimTowerEntranceScreen].OpenTilesRight.Add(11);
        }
    }
}
