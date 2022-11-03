using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class Mist : Level
    {
        public static byte TowerOfSufferScreen = 0;
        public static byte MasconTowerScreen = 1;
        public static byte MistGuruScreen = 6;
        public static byte MasconLeftScreen = 9;
        public static byte MasconRightScreen = 12;
        public static byte MistSecretShopScreen = 13;
        public static byte TowerOfMistScreen = 23;
        public static byte MistExitScreen = 31;
        public static byte VictimTowerScreen = 32;
        public static byte VictimLeftScreen = 34;
        public static byte VictimRightScreen = 37;
        public static byte FireMageScreen = 41;
        public static byte TowerOfSufferEntranceScreen = 62;
        public static byte TowerOfMistEntranceScreen = 69;
        public static byte MasconTowerEntranceScreen = 79;
        public static byte VictimTowerEntranceScreen = 80;

        private SubLevel.Id firstTowerId;
        private SubLevel.Id secondTowerId;
        private SubLevel.Id thirdTowerId;

        public Mist(WorldNumber number, byte[] content) : base(number, content)
        {
            Screens[TowerOfSufferScreen].Doors.Add(DoorId.TowerOfSuffer);
            Screens[MasconTowerScreen].Doors.Add(DoorId.MasconTower);
            Screens[MistGuruScreen].Doors.Add(DoorId.MistSmallHouse);
            Screens[MistGuruScreen].Doors.Add(DoorId.MistGuru);
            Screens[MistGuruScreen].Doors.Add(DoorId.BirdHospital);
            Screens[MistSecretShopScreen].Doors.Add(DoorId.MistSecretShop);
            Screens[22].Doors.Add(DoorId.MistLargeHouse);
            Screens[TowerOfMistScreen].Doors.Add(DoorId.TowerOfMist);
            Screens[MistExitScreen].Doors.Add(DoorId.AceKeyHouse);
            Screens[32].Doors.Add(DoorId.VictimTower);
            Screens[FireMageScreen].Doors.Add(DoorId.FireMage);

            void AddMasconDoors(byte screenNumber)
            {
                Screens[screenNumber].Doors.Add(DoorId.MasconBar);
                Screens[screenNumber].Doors.Add(DoorId.MasconHospital);
                Screens[screenNumber].Doors.Add(DoorId.MasconHouse);
                Screens[screenNumber].Doors.Add(DoorId.MasconItemShop);
                Screens[screenNumber].Doors.Add(DoorId.MasconKeyShop);
                Screens[screenNumber].Doors.Add(DoorId.MasconMeatShop);
            }

            AddMasconDoors(MasconLeftScreen);
            AddMasconDoors(MasconRightScreen);

            void AddVictimDoors(byte screenNumber)
            {
                Screens[screenNumber].Doors.Add(DoorId.VictimBar);
                Screens[screenNumber].Doors.Add(DoorId.VictimGuru);
                Screens[screenNumber].Doors.Add(DoorId.VictimHospital);
                Screens[screenNumber].Doors.Add(DoorId.VictimHouse);
                Screens[screenNumber].Doors.Add(DoorId.VictimItemShop);
                Screens[screenNumber].Doors.Add(DoorId.VictimKeyShop);
                Screens[screenNumber].Doors.Add(DoorId.VictimMeatShop);
            }

            AddVictimDoors(VictimLeftScreen);
            AddVictimDoors(VictimRightScreen);
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

            var possibleEnds = new List<Screen>() { Screens[76] };
            possibleEnds.Add(Screens[52]);
            possibleEnds.Add(Screens[65]);
            possibleEnds.Add(Screens[77]);

            if (GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorldsLessWingboots)
            {
                possibleEnds.Add(Screens[FireMageScreen]);
            }

            int shuffleIndex = ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Unchanged ? 2 : 0;
            Util.ShuffleList(possibleEnds, shuffleIndex, possibleEnds.Count - 1, random);

            if (GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorldsLessWingboots)
            {
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
            else
            {
                var pairs = new List<List<Screen>>();
                pairs.Add(new List<Screen> { Screens[TowerOfSufferScreen], Screens[TowerOfSufferEntranceScreen] });
                pairs.Add(new List<Screen> { Screens[TowerOfMistScreen], Screens[TowerOfMistEntranceScreen] });
                pairs.Add(new List<Screen> { Screens[VictimTowerScreen], Screens[VictimTowerEntranceScreen] });
                Util.ShuffleList(pairs, 0, pairs.Count - 1, random);

                var towerIds = new Dictionary<byte, SubLevel.Id>()
                {
                    { TowerOfSufferScreen, SubLevel.Id.TowerOfSuffer },
                    { 23, SubLevel.Id.TowerOfMist},
                    { 32, SubLevel.Id.VictimTower },
                };

                firstTowerId = towerIds[pairs[0][0].Number];
                secondTowerId = towerIds[pairs[1][0].Number];
                thirdTowerId = towerIds[pairs[2][0].Number];

                startScreens.Add(pairs[0][1]);
                startScreens.Add(Screens[MasconTowerEntranceScreen]);
                startScreens.Add(pairs[1][1]);
                startScreens.Add(pairs[2][1]);

                if (random.Next(0, 2) == 0)
                {
                    var tmp = startScreens[1];
                    startScreens[1] = startScreens[2];
                    startScreens[2] = tmp;

                    tmp = endScreens[0];
                    endScreens[0] = endScreens[1];
                    endScreens[1] = tmp;
                }

                endScreens.Add(pairs[0][0]);
                endScreens.Add(pairs[1][0]);
                endScreens.Add(pairs[2][0]);
                endScreens.Add(possibleEnds[0]);

                if (ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Unchanged)
                {
                    endScreens.Add(possibleEnds[1]);
                }
                else
                {
                    endScreens.Add(Screens[MistSecretShopScreen]);
                }
            }
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
            candidates.Add(Screens[30]);
            candidates.Add(Screens[38]);
            candidates.Add(Screens[40]);
            candidates.Add(Screens[44]);
            candidates.Add(Screens[45]);
            candidates.AddRange(Screens.GetRange(46, 3));
            candidates.Add(Screens[51]);
            candidates.Add(Screens[53]);
            candidates.Add(Screens[54]);
            candidates.Add(Screens[56]);
            candidates.Add(Screens[60]);
            candidates.Add(Screens[63]);
            candidates.AddRange(Screens.GetRange(66, 2));
            candidates.AddRange(Screens.GetRange(70, 6));

            if (GeneralOptions.RandomizeScreens != GeneralOptions.ScreenRandomization.AllWorldsLessWingboots)
            {
                candidates.Add(Screens[2]);
                candidates.Add(Screens[3]);
                candidates.Add(Screens[4]);
                candidates.Add(Screens[5]);
                candidates.Add(Screens[20]);
                candidates.Add(Screens[29]);
                candidates.Add(Screens[33]);
                candidates.Add(Screens[39]);
                candidates.Add(Screens[42]);
                candidates.Add(Screens[43]);
                candidates.Add(Screens[50]);
                candidates.Add(Screens[55]);
                candidates.Add(Screens[59]);
                candidates.Add(Screens[61]);
                candidates.Add(Screens[64]);
                candidates.Add(Screens[78]);
            }

            Util.ShuffleList(candidates, 0, candidates.Count - 1, random);
            return candidates;
        }

        public override List<Screen> GetSpecialScreens(Random random)
        {
            var specialScreens = new List<Screen>();
            specialScreens.Add(Screens[22]);
            specialScreens.Add(Screens[MistExitScreen]);

            if (GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorldsLessWingboots)
            {
                specialScreens.Add(Screens[5]);
                specialScreens.Add(Screens[39]);
            }
            else
            {
                specialScreens.Add(Screens[MasconTowerScreen]);
                specialScreens.Add(Screens[FireMageScreen]);
            }

            Util.ShuffleList(specialScreens, 0, specialScreens.Count - 1, random);
            return specialScreens;
        }

        public override bool CreateSublevels(List<Screen> startScreens, List<Screen> endScreens, List<Screen> candidates, List<Screen> specialScreens, Random random, uint attempts)
        {
            int initialProbability = GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorldsLessWingboots ? 30 : 10;
            int specialProbability = GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorldsLessWingboots ? 40 : 30;

            bool result = CreateSublevel(startScreens[0], endScreens[0], candidates, specialScreens, specialProbability, initialProbability, random, SubLevel.Id.EarlyMist, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[1], endScreens[1], candidates, specialScreens, specialProbability, initialProbability, random, SubLevel.Id.MiddleMist, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[2], endScreens[2], candidates, specialScreens, specialProbability, initialProbability, random, SubLevel.Id.LateMist, attempts);
            if (!result)
            {
                return result;
            }

            if (specialScreens.Contains(Screens[MistExitScreen]))
            {
                return false;
            }

            result = CreateSublevel(startScreens[3], endScreens[3], candidates, specialScreens, specialProbability, initialProbability, random, firstTowerId, attempts);
            if (!result)
            {
                return result;
            }

            if (GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorldsLessWingboots)
            {
                if (specialScreens.Contains(Screens[7]))
                {
                    return false;
                }
            }
            else
            {
                if (specialScreens.Contains(Screens[MasconTowerScreen]))
                {
                    return false;
                }
            }

            result = CreateSublevel(startScreens[4], endScreens[4], candidates, specialScreens, 50, initialProbability, random, SubLevel.Id.MasconTower, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[5], endScreens[5], candidates, specialScreens, 50, initialProbability, random, secondTowerId, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[6], endScreens[6], candidates, specialScreens, 95, 10, random, thirdTowerId, attempts);
            if (!result)
            {
                return result;
            }

            if (specialScreens.Count > 0)
            {
                return false;
            }

            if (candidates.Count > 10)
            {
                return false;
            }

            if (GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorldsLessWingboots)
            {
                Screens[MasconTowerScreen].ScrollData.Down = MasconLeftScreen;
            }

            return result;
        }

        public override void SetupScreens()
        {
            if (GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorldsLessWingboots)
            {
                startToSpecial[TowerOfSufferEntranceScreen] = 5;
                startToSpecial[VictimTowerEntranceScreen] = 39;

                Screens[MasconLeftScreen].FriendEnds[Direction.Up] = Screens[3];
                Screens[3].FriendEnds[Direction.Left] = Screens[MasconTowerScreen];
                Screens[5].FriendEnds[Direction.Up] = Screens[TowerOfSufferScreen];
                Screens[5].FriendEnds[Direction.Right] = Screens[MistGuruScreen];

                Screens[20].FriendEnds[Direction.Up] = Screens[MistSecretShopScreen];
                Screens[39].FriendEnds[Direction.Up] = Screens[VictimTowerScreen];
                Screens[39].FriendConnections[Direction.Right] = Screens[20];

                Screens[3].Directions.Add(Direction.Left);
                Screens[3].Directions.Add(Direction.Down);
                Screens[3].OpenTilesLeft.Add(5);
                Screens[3].OpenTilesLeft.Add(6);
                Screens[3].OpenTilesLeft.Add(7);
                Screens[3].OpenTilesLeft.Add(8);
                Screens[3].OpenTilesDown.Add(12);
                Screens[5].Directions.Add(Direction.Left);
                //Screens[5].Directions.Add(Direction.Right);
                Screens[5].Directions.Add(Direction.Down);
                Screens[5].OpenTilesLeft.Add(7);
                Screens[5].OpenTilesLeft.Add(8);
                Screens[5].OpenTilesLeft.Add(9);
                Screens[5].OpenTilesLeft.Add(10);
                Screens[5].OpenTilesRight.Add(7);
                Screens[5].OpenTilesRight.Add(8);
                Screens[5].OpenTilesRight.Add(9);
                Screens[5].OpenTilesRight.Add(10);
                Screens[5].OpenTilesDown.Add(2);
                Screens[6].Directions.Add(Direction.Left);
                Screens[6].OpenTilesLeft.Add(1);
                Screens[6].OpenTilesLeft.Add(2);
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
                //Screens[20].Directions.Add(Direction.Left);
                Screens[20].Directions.Add(Direction.Right);
                Screens[20].OpenTilesLeft.Add(7);
                Screens[20].OpenTilesLeft.Add(8);
                Screens[20].OpenTilesLeft.Add(9);
                Screens[20].OpenTilesLeft.Add(10);
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
                //Screens[39].Directions.Add(Direction.Right);
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
                Screens[52].Directions.Add(Direction.Left);
                Screens[52].OpenTilesLeft.Add(6);
                Screens[52].OpenTilesLeft.Add(7);
                Screens[52].OpenTilesLeft.Add(8);
                Screens[52].OpenTilesLeft.Add(9);
                Screens[53].Directions.Add(Direction.Right);
                Screens[53].Directions.Add(Direction.Up);
                Screens[53].Directions.Add(Direction.Down);
                Screens[53].OpenTilesRight.Add(10);
                Screens[53].OpenTilesRight.Add(11);
                Screens[53].OpenTilesUp.Add(2);
                Screens[53].OpenTilesDown.Add(2);
                Screens[54].Directions.Add(Direction.Right);
                Screens[54].Directions.Add(Direction.Up);
                Screens[54].OpenTilesRight.Add(8);
                Screens[54].OpenTilesRight.Add(9);
                Screens[54].OpenTilesRight.Add(10);
                Screens[54].OpenTilesRight.Add(11);
                Screens[54].OpenTilesUp.Add(1);
                Screens[55].Directions.Add(Direction.Right);
                Screens[55].Directions.Add(Direction.Up);
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
                Screens[62].Directions.Add(Direction.Left);
                Screens[62].Directions.Add(Direction.Up);
                Screens[62].OpenTilesLeft.Add(9);
                Screens[62].OpenTilesLeft.Add(10);
                Screens[62].OpenTilesUp.Add(1);
                Screens[62].OpenTilesUp.Add(14);
                Screens[63].Directions.Add(Direction.Right);
                Screens[63].Directions.Add(Direction.Down);
                Screens[63].OpenTilesRight.Add(7);
                Screens[63].OpenTilesRight.Add(8);
                Screens[63].OpenTilesRight.Add(9);
                Screens[63].OpenTilesRight.Add(10);
                Screens[63].OpenTilesDown.Add(1);
                Screens[65].Directions.Add(Direction.Right);
                Screens[65].OpenTilesRight.Add(8);
                Screens[65].OpenTilesRight.Add(9);
                Screens[65].OpenTilesRight.Add(10);
                Screens[65].OpenTilesRight.Add(11);
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
                Screens[69].Directions.Add(Direction.Left);
                Screens[69].Directions.Add(Direction.Right);
                Screens[69].OpenTilesLeft.Add(10);
                Screens[69].OpenTilesLeft.Add(11);
                Screens[69].OpenTilesRight.Add(8);
                Screens[69].OpenTilesRight.Add(9);
                Screens[69].OpenTilesRight.Add(10);
                Screens[69].OpenTilesRight.Add(11);
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
                Screens[76].Directions.Add(Direction.Left);
                Screens[76].OpenTilesLeft.Add(1);
                Screens[76].OpenTilesLeft.Add(2);
                Screens[76].OpenTilesLeft.Add(3);
                Screens[76].OpenTilesLeft.Add(4);
                Screens[76].OpenTilesLeft.Add(8);
                Screens[76].OpenTilesLeft.Add(9);
                Screens[76].OpenTilesLeft.Add(10);
                Screens[76].OpenTilesLeft.Add(11);
                Screens[77].Directions.Add(Direction.Right);
                Screens[77].OpenTilesRight.Add(1);
                Screens[77].OpenTilesRight.Add(2);
                Screens[77].OpenTilesRight.Add(3);
                Screens[77].OpenTilesRight.Add(8);
                Screens[77].OpenTilesRight.Add(9);
                Screens[77].OpenTilesRight.Add(10);
                Screens[77].OpenTilesRight.Add(11);
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
            else
            {
                Screens[0].Directions.Add(Direction.Down);
                Screens[0].OpenTilesDown.Add(2);
                Screens[0].OpenTilesDown.Add(8);
                Screens[1].Directions.Add(Direction.Right);
                Screens[1].Directions.Add(Direction.Down);
                for (byte i = 2; i < 10; i++)
                {
                    Screens[1].OpenTilesRight.Add(i);
                }
                Screens[1].OpenTilesRight.Add(11);
                Screens[1].OpenTilesRight.Add(12);
                Screens[1].OpenTilesDown.Add(3);
                Screens[1].OpenTilesDown.Add(4);
                Screens[1].OpenTilesDown.Add(9);
                Screens[1].OpenTilesDown.Add(12);
                Screens[1].OpenTilesDown.Add(13);
                Screens[2].Directions.Add(Direction.Left);
                Screens[2].Directions.Add(Direction.Right);
                Screens[2].Directions.Add(Direction.Down);
                for (byte i = 0; i < 10; i++)
                {
                    Screens[2].OpenTilesLeft.Add(i);
                    Screens[2].OpenTilesRight.Add(i);
                }
                Screens[2].OpenTilesLeft.Remove(0);
                Screens[2].OpenTilesRight.Remove(0);
                Screens[2].OpenTilesRight.Remove(1);
                Screens[2].OpenTilesRight.Remove(2);
                Screens[2].OpenTilesRight.Remove(9);
                Screens[2].OpenTilesDown.Add(6);
                Screens[2].OpenTilesDown.Add(7);
                Screens[2].OpenTilesDown.Add(8);
                Screens[2].OpenTilesDown.Add(9);
                Screens[2].OpenTilesDown.Add(10);
                Screens[2].OpenTilesDown.Add(11);
                Screens[2].OpenTilesDown.Add(12);
                Screens[2].OpenTilesDown.Add(13);
                Screens[2].OpenTilesDown.Add(14);
                Screens[2].OpenTilesDown.Add(15);
                Screens[3].Directions.Add(Direction.Left);
                Screens[3].Directions.Add(Direction.Down);
                for (byte i = 0; i < 13; i++)
                {
                    Screens[3].OpenTilesLeft.Add(i);
                }
                Screens[3].OpenTilesLeft.Remove(0);
                Screens[3].OpenTilesLeft.Remove(1);
                Screens[3].OpenTilesLeft.Remove(2);
                Screens[3].OpenTilesLeft.Remove(3);
                Screens[3].OpenTilesLeft.Remove(9);
                Screens[3].OpenTilesDown.Add(0);
                Screens[3].OpenTilesDown.Add(1);
                Screens[3].OpenTilesDown.Add(2);
                Screens[3].OpenTilesDown.Add(3);
                Screens[3].OpenTilesDown.Add(4);
                Screens[3].OpenTilesDown.Add(5);
                Screens[3].OpenTilesDown.Add(12);
                Screens[4].Directions.Add(Direction.Right);
                Screens[4].Directions.Add(Direction.Up);
                Screens[4].OpenTilesUp.Add(2);
                Screens[4].OpenTilesUp.Add(3);
                Screens[4].OpenTilesUp.Add(4);
                Screens[4].OpenTilesRight.Add(2);
                Screens[4].OpenTilesRight.Add(3);
                Screens[4].OpenTilesRight.Add(4);
                Screens[4].OpenTilesRight.Add(5);
                Screens[4].OpenTilesRight.Remove(6);
                Screens[5].Directions.Add(Direction.Left);
                Screens[5].Directions.Add(Direction.Right);
                Screens[5].Directions.Add(Direction.Up);
                Screens[5].Directions.Add(Direction.Down);
                for (byte i = 0; i < 11; i++)
                {
                    Screens[5].OpenTilesRight.Add(i);
                }
                Screens[5].OpenTilesRight.Remove(5);
                Screens[5].OpenTilesLeft.Add(2);
                Screens[5].OpenTilesLeft.Add(3);
                Screens[5].OpenTilesLeft.Add(4);
                Screens[5].OpenTilesLeft.Add(5);
                Screens[5].OpenTilesLeft.Add(7);
                Screens[5].OpenTilesLeft.Add(8);
                Screens[5].OpenTilesLeft.Add(9);
                Screens[5].OpenTilesLeft.Add(10);
                Screens[5].OpenTilesLeft.Add(14);
                Screens[5].OpenTilesLeft.Add(15);
                Screens[5].OpenTilesUp.Add(2);
                Screens[5].OpenTilesUp.Add(5);
                Screens[5].OpenTilesUp.Add(6);
                Screens[5].OpenTilesUp.Add(7);
                Screens[5].OpenTilesUp.Add(8);
                Screens[5].OpenTilesDown.Add(2);
                Screens[5].OpenTilesDown.Add(4);
                Screens[5].OpenTilesDown.Add(11);
                Screens[5].OpenTilesDown.Add(12);
                Screens[6].Directions.Add(Direction.Left);
                Screens[6].Directions.Add(Direction.Up);
                for (byte i = 0; i < 11; i++)
                {
                    Screens[6].OpenTilesLeft.Add(i);
                    Screens[6].OpenTilesUp.Add(i);
                }
                Screens[6].OpenTilesUp.Add(11);
                Screens[6].OpenTilesUp.Add(12);
                Screens[6].OpenTilesUp.Add(13);
                Screens[7].Directions.Add(Direction.Right);
                Screens[7].Directions.Add(Direction.Up);
                Screens[7].Directions.Add(Direction.Down);
                for (byte i = 0; i < 11; i++)
                {
                    Screens[7].OpenTilesRight.Add(i);
                    Screens[7].OpenTilesUp.Add(i);
                }
                Screens[7].OpenTilesRight.Remove(5);
                Screens[7].OpenTilesRight.Remove(6);
                Screens[7].OpenTilesUp.Remove(0);
                Screens[7].OpenTilesUp.Remove(1);
                Screens[7].OpenTilesUp.Remove(2);
                Screens[7].OpenTilesUp.Remove(3);
                Screens[7].OpenTilesUp.Add(13);
                Screens[7].OpenTilesUp.Add(14);
                Screens[7].OpenTilesUp.Add(15);
                Screens[7].OpenTilesDown.Add(1);
                Screens[8].Directions.Add(Direction.Left);
                Screens[8].Directions.Add(Direction.Right);
                Screens[8].Directions.Add(Direction.Up);
                for (byte i = 0; i < 11; i++)
                {
                    Screens[8].OpenTilesLeft.Add(i);
                    Screens[8].OpenTilesRight.Add(i);
                    Screens[8].OpenTilesUp.Add(i);
                }
                Screens[8].OpenTilesUp.Add(11);
                Screens[8].OpenTilesUp.Add(12);
                Screens[8].OpenTilesUp.Add(13);
                Screens[8].OpenTilesUp.Add(14);
                Screens[8].OpenTilesUp.Add(15);
                Screens[9].Directions.Add(Direction.Left); //Mascon entrance
                Screens[9].Directions.Add(Direction.Up);
                for (byte i = 0; i < 11; i++)
                {
                    Screens[9].OpenTilesLeft.Add(i);
                    Screens[9].OpenTilesUp.Add(i);
                }
                Screens[9].OpenTilesUp.Add(11);
                Screens[9].OpenTilesUp.Add(12);
                Screens[9].OpenTilesUp.Add(13);
                Screens[12].Directions.Add(Direction.Down); //Mascon exit
                Screens[12].OpenTilesDown.Add(3);
                Screens[12].OpenTilesDown.Add(6);
                Screens[12].OpenTilesDown.Add(8);
                Screens[12].OpenTilesDown.Add(10);
                Screens[12].OpenTilesDown.Add(11);
                Screens[12].OpenTilesDown.Add(13);
                Screens[12].OpenTilesDown.Add(14);
                Screens[13].Directions.Add(Direction.Down);
                for (byte i = 1; i < 11; i++)
                {
                    Screens[13].OpenTilesDown.Add(i);
                }
                Screens[13].OpenTilesDown.Add(12);
                Screens[14].Directions.Add(Direction.Right);
                //Screens[14].Directions.Add(Direction.Up);
                Screens[14].Directions.Add(Direction.Down);
                for (byte i = 3; i < 12; i++)
                {
                    Screens[14].OpenTilesRight.Add(i);
                }
                //Screens[14].OpenTilesUp.Add(4);
                Screens[14].OpenTilesDown.Add(1);
                Screens[15].Directions.Add(Direction.Left);
                Screens[15].Directions.Add(Direction.Up);
                Screens[15].OpenTilesLeft.Add(5);
                Screens[15].OpenTilesLeft.Add(6);
                Screens[15].OpenTilesLeft.Add(7);
                Screens[15].OpenTilesLeft.Add(8);
                Screens[15].OpenTilesLeft.Add(9);
                Screens[15].OpenTilesLeft.Add(10);
                Screens[15].OpenTilesUp.Add(2);
                Screens[15].OpenTilesUp.Add(3);
                Screens[15].OpenTilesUp.Add(4);
                Screens[15].OpenTilesUp.Add(11);
                Screens[15].OpenTilesUp.Add(12);
                Screens[15].OpenTilesUp.Add(13);
                Screens[16].Directions.Add(Direction.Right);
                Screens[16].Directions.Add(Direction.Up);
                for (byte i = 0; i < 11; i++)
                {
                    Screens[16].OpenTilesLeft.Add(i);
                    Screens[16].OpenTilesRight.Add(i);
                }
                Screens[16].OpenTilesRight.Remove(5);
                Screens[16].OpenTilesUp.Remove(2);
                Screens[16].OpenTilesUp.Remove(3);
                Screens[16].OpenTilesUp.Add(11);
                Screens[16].OpenTilesUp.Add(12);
                Screens[16].OpenTilesUp.Add(13);
                Screens[16].OpenTilesUp.Add(14);
                Screens[16].OpenTilesUp.Add(15);
                Screens[17].Directions.Add(Direction.Left);
                Screens[17].Directions.Add(Direction.Right);
                Screens[17].Directions.Add(Direction.Up);
                for (byte i = 0; i < 11; i++)
                {
                    Screens[17].OpenTilesLeft.Add(i);
                    Screens[17].OpenTilesUp.Add(i);
                }
                Screens[17].OpenTilesLeft.Remove(5);
                Screens[17].OpenTilesRight.Add(10);
                Screens[17].OpenTilesRight.Add(9);
                Screens[17].OpenTilesUp.Add(11);
                Screens[17].OpenTilesUp.Add(12);
                Screens[17].OpenTilesUp.Add(13);
                Screens[17].OpenTilesUp.Add(14);
                Screens[17].OpenTilesUp.Add(15);
                Screens[18].Directions.Add(Direction.Up);
                Screens[18].Directions.Add(Direction.Right);
                for (byte i = 1; i < 11; i++)
                {
                    Screens[18].OpenTilesRight.Add(i);
                }
                Screens[18].OpenTilesRight.Remove(6);
                Screens[18].OpenTilesUp.Add(3);
                Screens[18].OpenTilesUp.Add(6);
                Screens[18].OpenTilesUp.Add(8);
                Screens[18].OpenTilesUp.Add(10);
                Screens[18].OpenTilesUp.Add(11);
                Screens[18].OpenTilesUp.Add(13);
                Screens[18].OpenTilesUp.Add(14);
                Screens[19].Directions.Add(Direction.Left);
                Screens[19].Directions.Add(Direction.Right);
                Screens[19].Directions.Add(Direction.Up);
                for (byte i = 2; i < 11; i++)
                {
                    Screens[19].OpenTilesLeft.Add(i);
                    Screens[19].OpenTilesRight.Add(i);
                }
                Screens[19].OpenTilesLeft.Add(0);
                Screens[19].OpenTilesLeft.Add(1);
                Screens[19].OpenTilesLeft.Remove(6);
                Screens[19].OpenTilesRight.Remove(5);
                Screens[19].OpenTilesUp.Add(0);
                Screens[19].OpenTilesUp.Add(1);
                Screens[20].Directions.Add(Direction.Left);
                Screens[20].Directions.Add(Direction.Right);
                Screens[20].Directions.Add(Direction.Up);
                for (byte i = 2; i < 11; i++)
                {
                    Screens[20].OpenTilesLeft.Add(i);
                    Screens[20].OpenTilesRight.Add(i);
                    Screens[20].OpenTilesUp.Add(i);
                }
                Screens[20].OpenTilesLeft.Remove(5);
                Screens[20].OpenTilesUp.Add(1);
                Screens[20].OpenTilesUp.Add(11);
                Screens[20].OpenTilesUp.Add(12);
                Screens[20].OpenTilesUp.Add(13);
                Screens[20].OpenTilesUp.Add(14);
                Screens[21].Directions.Add(Direction.Left);
                Screens[21].Directions.Add(Direction.Right);
                for (byte i = 4; i < 11; i++)
                {
                    Screens[21].OpenTilesLeft.Add(i);
                    Screens[21].OpenTilesRight.Add(i);
                }
                Screens[21].OpenTilesRight.Add(3);
                Screens[22].Directions.Add(Direction.Left);
                Screens[22].Directions.Add(Direction.Up);
                for (byte i = 1; i < 14; i++)
                {
                    Screens[22].OpenTilesUp.Add(i);
                }
                Screens[22].OpenTilesLeft.Add(5);
                Screens[22].OpenTilesLeft.Add(6);
                Screens[22].OpenTilesLeft.Add(7);
                Screens[22].OpenTilesLeft.Add(8);
                Screens[22].OpenTilesLeft.Add(9);
                Screens[22].OpenTilesLeft.Add(10);
                Screens[23].Directions.Add(Direction.Right); // Tower of Mist entrance
                for (byte i = 2; i < 9; i++)
                {
                    Screens[23].OpenTilesRight.Add(i);
                }
                Screens[24].Directions.Add(Direction.Left);
                Screens[24].Directions.Add(Direction.Right);
                Screens[24].Directions.Add(Direction.Up);
                for (byte i = 2; i < 9; i++)
                {
                    Screens[24].OpenTilesLeft.Add(i);
                }
                for (byte i = 0; i < 7; i++)
                {
                    Screens[24].OpenTilesRight.Add(i);
                }
                Screens[24].OpenTilesUp.Add(15);
                Screens[25].Directions.Add(Direction.Left);
                Screens[25].Directions.Add(Direction.Up);
                Screens[25].Directions.Add(Direction.Down);
                for (byte i = 0; i < 7; i++)
                {
                    Screens[25].OpenTilesLeft.Add(i);
                }
                Screens[25].OpenTilesUp.Add(0);
                Screens[25].OpenTilesUp.Add(3);
                Screens[25].OpenTilesUp.Add(8);
                Screens[25].OpenTilesDown.Add(2);
                Screens[25].OpenTilesDown.Add(3);
                Screens[25].OpenTilesDown.Add(4);
                Screens[25].OpenTilesDown.Add(5);
                Screens[25].OpenTilesDown.Add(6);
                Screens[25].OpenTilesDown.Add(7);
                Screens[26].Directions.Add(Direction.Up);
                Screens[26].Directions.Add(Direction.Right);
                for (byte i = 1; i < 8; i++)
                {
                    Screens[26].OpenTilesUp.Add(i);
                }
                Screens[26].OpenTilesRight.Add(7);
                Screens[26].OpenTilesRight.Add(8);
                Screens[26].OpenTilesRight.Add(9);
                Screens[26].OpenTilesRight.Add(10);
                Screens[27].Directions.Add(Direction.Left);
                Screens[27].Directions.Add(Direction.Right);
                Screens[27].Directions.Add(Direction.Up);
                Screens[27].OpenTilesLeft.Add(7);
                Screens[27].OpenTilesLeft.Add(8);
                Screens[27].OpenTilesLeft.Add(9);
                Screens[27].OpenTilesLeft.Add(10);
                Screens[27].OpenTilesRight.Add(7);
                Screens[27].OpenTilesRight.Add(8);
                Screens[27].OpenTilesRight.Add(9);
                Screens[27].OpenTilesRight.Add(10);
                Screens[27].OpenTilesRight.Add(11);
                Screens[27].OpenTilesUp.Add(1);
                Screens[27].OpenTilesUp.Add(2);
                Screens[27].OpenTilesUp.Add(3);
                Screens[27].OpenTilesUp.Add(4);
                Screens[27].OpenTilesUp.Add(5);
                Screens[27].OpenTilesUp.Add(6);
                Screens[27].OpenTilesUp.Add(7);
                Screens[27].OpenTilesUp.Add(8);
                Screens[27].OpenTilesUp.Add(9);
                Screens[28].Directions.Add(Direction.Left);
                Screens[28].Directions.Add(Direction.Right);
                for (byte i = 3; i < 12; i++)
                {
                    Screens[28].OpenTilesLeft.Add(i);
                }
                for (byte i = 2; i < 11; i++)
                {
                    Screens[28].OpenTilesRight.Add(i);
                }
                Screens[29].Directions.Add(Direction.Left);
                Screens[29].Directions.Add(Direction.Right);
                Screens[29].Directions.Add(Direction.Down);
                for (byte i = 2; i < 11; i++)
                {
                    Screens[29].OpenTilesLeft.Add(i);
                    Screens[29].OpenTilesRight.Add(i);
                }
                Screens[29].OpenTilesRight.Remove(7);
                Screens[29].OpenTilesDown.Add(4);
                Screens[29].OpenTilesDown.Add(5);
                Screens[29].OpenTilesDown.Add(7);
                Screens[29].OpenTilesDown.Add(8);
                Screens[29].OpenTilesDown.Add(10);
                Screens[29].OpenTilesDown.Add(11);
                Screens[30].Directions.Add(Direction.Left);
                Screens[30].Directions.Add(Direction.Right);
                for (byte i = 2; i < 11; i++)
                {
                    Screens[30].OpenTilesLeft.Add(i);
                    Screens[30].OpenTilesRight.Add(i);
                }
                Screens[30].OpenTilesLeft.Remove(2);
                Screens[30].OpenTilesLeft.Remove(7);
                Screens[30].OpenTilesRight.Remove(6);
                Screens[MistExitScreen].Directions.Add(Direction.Left);
                Screens[MistExitScreen].Directions.Add(Direction.Right);
                Screens[MistExitScreen].Directions.Add(Direction.Up);
                for (byte i = 2; i < 11; i++)
                {
                    Screens[MistExitScreen].OpenTilesLeft.Add(i);
                    Screens[MistExitScreen].OpenTilesRight.Add(i);
                }
                Screens[MistExitScreen].OpenTilesUp.Add(10);
                //Screens[32].Directions.Add(Direction.Left); // Victim tower entrance
                //Screens[32].Directions.Add(Direction.Right);
                Screens[32].Directions.Add(Direction.Down);
                Screens[32].OpenTilesDown.Add(4);
                Screens[33].Directions.Add(Direction.Left);
                Screens[33].Directions.Add(Direction.Down);
                for (byte i = 1; i < 9; i++)
                {
                    Screens[33].OpenTilesLeft.Add(i);
                }
                for (byte i = 0; i < 6; i++)
                {
                    Screens[33].OpenTilesDown.Add(i);
                }
                Screens[33].OpenTilesLeft.Remove(7);
                Screens[33].OpenTilesDown.Add(13);
                Screens[34].Directions.Add(Direction.Left); // Victim entrance
                Screens[34].Directions.Add(Direction.Up);
                Screens[34].Directions.Add(Direction.Down);
                for (byte i = 1; i < 13; i++)
                {
                    Screens[34].OpenTilesLeft.Add(i);
                }
                Screens[34].OpenTilesUp.Add(13);
                Screens[34].OpenTilesDown.Add(0);
                Screens[34].OpenTilesDown.Add(1);
                Screens[34].OpenTilesDown.Add(2);
                Screens[34].OpenTilesDown.Add(3);
                Screens[37].Directions.Add(Direction.Up); //Victim exit
                Screens[37].Directions.Add(Direction.Down);
                Screens[37].OpenTilesUp.Add(3);
                Screens[37].OpenTilesUp.Add(4);
                Screens[37].OpenTilesUp.Add(7);
                Screens[37].OpenTilesUp.Add(8);
                Screens[37].OpenTilesUp.Add(10);
                Screens[37].OpenTilesUp.Add(11);
                Screens[37].OpenTilesUp.Add(13);
                Screens[37].OpenTilesDown.Add(3);
                Screens[37].OpenTilesDown.Add(4);
                Screens[37].OpenTilesDown.Add(8);
                Screens[38].Directions.Add(Direction.Right);
                Screens[38].Directions.Add(Direction.Down);
                for (byte i = 2; i < 11; i++)
                {
                    Screens[38].OpenTilesRight.Add(i);
                }
                Screens[38].OpenTilesDown.Add(3);
                Screens[38].OpenTilesDown.Add(11);
                Screens[39].Directions.Add(Direction.Left);
                Screens[39].Directions.Add(Direction.Right);
                Screens[39].Directions.Add(Direction.Up);
                for (byte i = 4; i < 11; i++)
                {
                    Screens[39].OpenTilesLeft.Add(i);
                    Screens[39].OpenTilesRight.Add(i);
                    Screens[39].OpenTilesUp.Add(i);
                }
                Screens[39].OpenTilesLeft.Add(3);
                Screens[39].OpenTilesRight.Add(0);
                Screens[39].OpenTilesRight.Add(1);
                Screens[39].OpenTilesRight.Add(2);
                Screens[39].OpenTilesRight.Add(3);
                Screens[39].OpenTilesRight.Remove(5);
                Screens[39].OpenTilesRight.Remove(6);
                Screens[39].OpenTilesUp.Add(11);
                Screens[39].OpenTilesUp.Add(12);
                Screens[39].OpenTilesUp.Add(13);
                Screens[39].OpenTilesUp.Add(14);
                Screens[39].OpenTilesUp.Add(15);
                Screens[40].Directions.Add(Direction.Left);
                Screens[40].Directions.Add(Direction.Right);
                Screens[40].Directions.Add(Direction.Up);
                for (byte i = 0; i < 11; i++)
                {
                    Screens[40].OpenTilesLeft.Add(i);
                }
                Screens[40].OpenTilesLeft.Remove(5);
                Screens[40].OpenTilesRight.Add(5);
                Screens[40].OpenTilesRight.Add(6);
                Screens[40].OpenTilesRight.Add(7);
                Screens[40].OpenTilesRight.Add(8);
                Screens[40].OpenTilesRight.Add(9);
                Screens[40].OpenTilesRight.Add(10);
                Screens[40].OpenTilesRight.Add(11);
                Screens[40].OpenTilesUp.Add(0);
                Screens[40].OpenTilesUp.Add(1);
                Screens[40].OpenTilesUp.Add(2);
                Screens[40].OpenTilesUp.Add(3);
                Screens[40].OpenTilesUp.Add(4);
                Screens[40].OpenTilesUp.Add(5);
                Screens[40].OpenTilesUp.Add(6);
                Screens[40].OpenTilesUp.Add(7);
                Screens[41].Directions.Add(Direction.Left);
                Screens[41].Directions.Add(Direction.Up);
                for (byte i = 3; i < 11; i++)
                {
                    Screens[41].OpenTilesLeft.Add(i);
                }
                Screens[41].OpenTilesUp.Add(11);
                Screens[41].OpenTilesUp.Add(12);
                Screens[41].OpenTilesUp.Add(13);
                Screens[42].Directions.Add(Direction.Right);
                //Screens[42].Directions.Add(Direction.Up);
                Screens[42].Directions.Add(Direction.Down);
                for (byte i = 4; i < 11; i++)
                {
                    Screens[42].OpenTilesRight.Add(i);
                }
                //Screens[42].OpenTilesUp.Add(12);
                //Screens[42].OpenTilesUp.Add(13);
                //Screens[42].OpenTilesUp.Add(14);
                Screens[42].OpenTilesDown.Add(2);
                Screens[43].Directions.Add(Direction.Left);
                Screens[43].Directions.Add(Direction.Right);
                for (byte i = 3; i < 10; i++)
                {
                    Screens[43].OpenTilesLeft.Add(i);
                }
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
                Screens[44].OpenTilesUp.Add(1);
                Screens[44].OpenTilesUp.Add(2);
                Screens[44].OpenTilesUp.Add(3);
                Screens[45].Directions.Add(Direction.Up);
                Screens[45].Directions.Add(Direction.Right);
                for (byte i = 1; i < 11; i++)
                {
                    Screens[45].OpenTilesRight.Add(i);
                    Screens[45].OpenTilesUp.Add(i);
                }
                Screens[45].OpenTilesUp.Add(11);
                Screens[45].OpenTilesUp.Add(12);
                Screens[45].OpenTilesUp.Add(13);
                Screens[45].OpenTilesUp.Add(14);
                Screens[45].OpenTilesUp.Add(15);
                Screens[46].Directions.Add(Direction.Left);
                Screens[46].Directions.Add(Direction.Right);
                for (byte i = 1; i < 11; i++)
                {
                    Screens[46].OpenTilesLeft.Add(i);
                    Screens[46].OpenTilesRight.Add(i);
                }
                Screens[46].OpenTilesRight.Remove(1);
                Screens[46].OpenTilesRight.Remove(2);
                Screens[46].OpenTilesRight.Remove(6);
                Screens[47].Directions.Add(Direction.Left);
                Screens[47].Directions.Add(Direction.Up);
                for (byte i = 2; i < 11; i++)
                {
                    Screens[47].OpenTilesLeft.Add(i);
                    Screens[47].OpenTilesUp.Add(i);
                }
                Screens[47].OpenTilesRight.Remove(6);
                Screens[47].OpenTilesUp.Add(11);
                Screens[47].OpenTilesUp.Add(12);
                Screens[47].OpenTilesUp.Add(13);
                Screens[47].OpenTilesUp.Add(14);
                Screens[48].Directions.Add(Direction.Right); // tower of suffer?
                Screens[48].Directions.Add(Direction.Down);
                for (byte i = 1; i < 12; i++)
                {
                    Screens[48].OpenTilesRight.Add(i);
                }
                Screens[48].OpenTilesRight.Remove(7);
                Screens[48].OpenTilesDown.Add(2);
                Screens[49].Directions.Add(Direction.Left);
                Screens[49].Directions.Add(Direction.Right);
                for (byte i = 1; i < 7; i++)
                {
                    Screens[49].OpenTilesLeft.Add(i);
                }
                for (byte i = 1; i < 12; i++)
                {
                    Screens[49].OpenTilesRight.Add(i);
                }
                Screens[50].Directions.Add(Direction.Left);
                Screens[50].Directions.Add(Direction.Down);
                for (byte i = 2; i < 12; i++)
                {
                    Screens[50].OpenTilesLeft.Add(i);
                }
                Screens[50].OpenTilesDown.Add(3);
                Screens[50].OpenTilesDown.Add(4);
                Screens[50].OpenTilesDown.Add(9);
                Screens[50].OpenTilesDown.Add(10);
                Screens[51].Directions.Add(Direction.Right);
                Screens[51].Directions.Add(Direction.Down);
                for (byte i = 1; i < 10; i++)
                {
                    Screens[51].OpenTilesRight.Add(i);
                }
                Screens[51].OpenTilesDown.Add(1);
                Screens[52].Directions.Add(Direction.Left);
                for (byte i = 1; i < 10; i++)
                {
                    Screens[52].OpenTilesLeft.Add(i);
                }
                Screens[53].Directions.Add(Direction.Right);
                Screens[53].Directions.Add(Direction.Up);
                Screens[53].Directions.Add(Direction.Down);
                Screens[53].OpenTilesRight.Add(10);
                Screens[53].OpenTilesRight.Add(11);
                Screens[53].OpenTilesUp.Add(2);
                Screens[53].OpenTilesDown.Add(2);
                Screens[54].Directions.Add(Direction.Right);
                Screens[54].Directions.Add(Direction.Up);
                for (byte i = 1; i < 12; i++)
                {
                    Screens[54].OpenTilesRight.Add(i);
                }
                Screens[54].OpenTilesUp.Add(1);
                Screens[55].Directions.Add(Direction.Left);
                Screens[55].Directions.Add(Direction.Right);
                Screens[55].Directions.Add(Direction.Up);
                Screens[55].Directions.Add(Direction.Down);
                for (byte i = 1; i < 12; i++)
                {
                    Screens[55].OpenTilesLeft.Add(i);
                    Screens[55].OpenTilesRight.Add(i);
                }
                Screens[55].OpenTilesRight.Remove(5);
                Screens[55].OpenTilesUp.Add(3);
                Screens[55].OpenTilesUp.Add(4);
                Screens[55].OpenTilesUp.Add(9);
                Screens[55].OpenTilesUp.Add(10);
                Screens[55].OpenTilesDown.Add(2);
                Screens[55].OpenTilesDown.Add(5);
                Screens[56].Directions.Add(Direction.Left);
                Screens[56].Directions.Add(Direction.Right);
                Screens[56].Directions.Add(Direction.Up);
                for (byte i = 1; i < 12; i++)
                {
                    Screens[56].OpenTilesLeft.Add(i);
                }
                Screens[56].OpenTilesLeft.Remove(5);
                for (byte i = 0; i < 12; i++)
                {
                    Screens[56].OpenTilesRight.Add(i);
                }
                Screens[56].OpenTilesRight.Remove(5);
                for (byte i = 1; i < 16; i++)
                {
                    Screens[56].OpenTilesUp.Add(i);
                }
                Screens[57].Directions.Add(Direction.Left);
                Screens[57].Directions.Add(Direction.Right);
                Screens[57].Directions.Add(Direction.Up);
                for (byte i = 0; i < 15; i++)
                {
                    Screens[57].OpenTilesUp.Add(i);
                }
                Screens[57].OpenTilesLeft.Add(0);
                Screens[57].OpenTilesLeft.Add(1);
                Screens[57].OpenTilesLeft.Add(2);
                Screens[57].OpenTilesLeft.Add(3);
                Screens[57].OpenTilesLeft.Add(4);
                Screens[57].OpenTilesRight.Add(1);
                Screens[57].OpenTilesRight.Add(2);
                Screens[57].OpenTilesRight.Add(3);
                Screens[57].OpenTilesRight.Add(4);
                Screens[59].Directions.Add(Direction.Right);
                Screens[59].Directions.Add(Direction.Up);
                for (byte i = 1; i < 16; i++)
                {
                    Screens[59].OpenTilesUp.Add(i);
                }
                Screens[59].OpenTilesRight.Add(0);
                Screens[59].OpenTilesRight.Add(1);
                Screens[59].OpenTilesRight.Add(2);
                Screens[59].OpenTilesRight.Add(3);
                Screens[59].OpenTilesRight.Add(4);
                Screens[59].OpenTilesRight.Add(5);
                Screens[60].Directions.Add(Direction.Left);
                Screens[60].Directions.Add(Direction.Up);
                Screens[60].Directions.Add(Direction.Down);
                for (byte i = 1; i < 10; i++)
                {
                    Screens[60].OpenTilesUp.Add(i);
                }
                Screens[60].OpenTilesLeft.Add(1);
                Screens[60].OpenTilesLeft.Add(2);
                Screens[60].OpenTilesLeft.Add(3);
                Screens[60].OpenTilesLeft.Add(4);
                Screens[60].OpenTilesLeft.Add(5);
                Screens[60].OpenTilesDown.Add(1);
                Screens[61].Directions.Add(Direction.Left);
                Screens[61].Directions.Add(Direction.Up);
                for (byte i = 5; i < 15; i++)
                {
                    Screens[61].OpenTilesUp.Add(i);
                }
                Screens[61].OpenTilesLeft.Add(7);
                Screens[61].OpenTilesLeft.Add(8);
                Screens[61].OpenTilesLeft.Add(10);
                Screens[61].OpenTilesLeft.Add(11);
                Screens[62].Directions.Add(Direction.Left);
                Screens[62].Directions.Add(Direction.Up);
                for (byte i = 1; i < 15; i++)
                {
                    Screens[62].OpenTilesUp.Add(i);
                }
                Screens[62].OpenTilesLeft.Add(9);
                Screens[62].OpenTilesLeft.Add(10);
                Screens[63].Directions.Add(Direction.Right);
                Screens[63].Directions.Add(Direction.Down);
                for (byte i = 1; i < 11; i++)
                {
                    Screens[63].OpenTilesRight.Add(i);
                }
                Screens[63].OpenTilesDown.Add(1);
                Screens[63].OpenTilesDown.Add(6);
                Screens[63].OpenTilesDown.Add(10);
                Screens[63].OpenTilesDown.Add(14);
                Screens[64].Directions.Add(Direction.Left);
                Screens[64].Directions.Add(Direction.Right);
                Screens[64].Directions.Add(Direction.Down);
                for (byte i = 1; i < 11; i++)
                {
                    Screens[64].OpenTilesLeft.Add(i);
                }
                Screens[64].OpenTilesRight.Add(1);
                Screens[64].OpenTilesRight.Add(2);
                Screens[64].OpenTilesRight.Add(3);
                Screens[64].OpenTilesDown.Add(2);
                Screens[64].OpenTilesDown.Add(6);
                Screens[64].OpenTilesDown.Add(10);
                Screens[64].OpenTilesDown.Add(14);
                Screens[65].Directions.Add(Direction.Right);
                for (byte i = 5; i < 12; i++)
                {
                    Screens[65].OpenTilesRight.Add(i);
                }
                Screens[66].Directions.Add(Direction.Left);
                Screens[66].Directions.Add(Direction.Right);
                for (byte i = 5; i < 12; i++)
                {
                    Screens[66].OpenTilesLeft.Add(i);
                }
                Screens[66].OpenTilesRight.Add(8);
                Screens[66].OpenTilesRight.Add(9);
                Screens[66].OpenTilesRight.Add(10);
                Screens[66].OpenTilesRight.Add(11);
                Screens[67].Directions.Add(Direction.Left);
                Screens[67].Directions.Add(Direction.Right);
                for (byte i = 5; i < 12; i++)
                {
                    Screens[67].OpenTilesRight.Add(i);
                }
                Screens[67].OpenTilesLeft.Add(8);
                Screens[67].OpenTilesLeft.Add(9);
                Screens[67].OpenTilesLeft.Add(10);
                Screens[67].OpenTilesLeft.Add(11);
                Screens[69].Directions.Add(Direction.Left); //Tower of mist?
                Screens[69].Directions.Add(Direction.Right);
                for (byte i = 1; i < 12; i++)
                {
                    Screens[69].OpenTilesRight.Add(i);
                }
                Screens[69].OpenTilesLeft.Add(10);
                Screens[69].OpenTilesLeft.Add(11);
                Screens[69].OpenTilesRight.Remove(5);
                Screens[70].Directions.Add(Direction.Left);
                Screens[70].Directions.Add(Direction.Right);
                for (byte i = 1; i < 12; i++)
                {
                    Screens[70].OpenTilesLeft.Add(i);
                    Screens[70].OpenTilesRight.Add(i);
                }
                Screens[70].OpenTilesLeft.Remove(5);
                Screens[70].OpenTilesRight.Remove(5);
                Screens[71].Directions.Add(Direction.Left);
                Screens[71].Directions.Add(Direction.Right);
                for (byte i = 1; i < 12; i++)
                {
                    Screens[71].OpenTilesLeft.Add(i);
                }
                Screens[71].OpenTilesLeft.Remove(5);
                Screens[71].OpenTilesRight.Add(1);
                Screens[71].OpenTilesRight.Add(2);
                Screens[71].OpenTilesRight.Add(3);
                Screens[71].OpenTilesRight.Add(4);
                Screens[72].Directions.Add(Direction.Left);
                Screens[72].Directions.Add(Direction.Right);
                Screens[72].Directions.Add(Direction.Up);
                for (byte i = 0; i < 12; i++)
                {
                    Screens[72].OpenTilesRight.Add(i);
                }
                for (byte i = 3; i < 16; i++)
                {
                    Screens[72].OpenTilesUp.Add(i);
                }
                Screens[72].OpenTilesLeft.Add(1);
                Screens[72].OpenTilesLeft.Add(2);
                Screens[72].OpenTilesLeft.Add(3);
                Screens[72].OpenTilesLeft.Add(4);
                Screens[72].OpenTilesUp.Add(1);
                Screens[73].Directions.Add(Direction.Left);
                Screens[73].Directions.Add(Direction.Right);
                Screens[73].Directions.Add(Direction.Up);
                for (byte i = 0; i < 12; i++)
                {
                    Screens[73].OpenTilesLeft.Add(i);
                }
                for (byte i = 0; i < 16; i++)
                {
                    Screens[73].OpenTilesUp.Add(i);
                }
                Screens[73].OpenTilesRight.Add(5);
                Screens[73].OpenTilesRight.Add(6);
                Screens[73].OpenTilesRight.Add(7);
                Screens[74].Directions.Add(Direction.Left);
                Screens[74].Directions.Add(Direction.Right);
                for (byte i = 1; i < 12; i++)
                {
                    Screens[74].OpenTilesRight.Add(i);
                }
                Screens[74].OpenTilesRight.Remove(5);
                Screens[74].OpenTilesLeft.Add(5);
                Screens[74].OpenTilesLeft.Add(6);
                Screens[74].OpenTilesLeft.Add(7);
                Screens[75].Directions.Add(Direction.Left);
                Screens[75].Directions.Add(Direction.Right);
                for (byte i = 1; i < 12; i++)
                {
                    Screens[75].OpenTilesLeft.Add(i);
                    Screens[75].OpenTilesRight.Add(i);
                }
                Screens[75].OpenTilesLeft.Remove(5);
                Screens[75].OpenTilesRight.Remove(5);
                Screens[76].Directions.Add(Direction.Left);
                for (byte i = 1; i < 12; i++)
                {
                    Screens[76].OpenTilesLeft.Add(i);
                }
                Screens[76].OpenTilesLeft.Remove(5);
                Screens[77].Directions.Add(Direction.Right); //Mascon tower?
                for (byte i = 1; i < 12; i++)
                {
                    Screens[77].OpenTilesRight.Add(i);
                }
                Screens[77].OpenTilesRight.Remove(4);
                Screens[78].Directions.Add(Direction.Left);
                Screens[78].Directions.Add(Direction.Right);
                for (byte i = 1; i < 12; i++)
                {
                    Screens[78].OpenTilesRight.Add(i);
                }
                Screens[78].OpenTilesLeft.Remove(5);
                Screens[78].OpenTilesLeft.Remove(8);
                Screens[78].OpenTilesRight.Add(1);
                Screens[78].OpenTilesRight.Add(2);
                Screens[78].OpenTilesRight.Add(3);
                Screens[78].OpenTilesRight.Add(4);
                Screens[78].OpenTilesRight.Add(6);
                Screens[78].OpenTilesRight.Add(7);
                Screens[78].OpenTilesRight.Add(9);
                Screens[78].OpenTilesRight.Add(10);
                Screens[78].OpenTilesRight.Add(11);
                Screens[79].Directions.Add(Direction.Left);
                Screens[79].Directions.Add(Direction.Right);
                Screens[79].OpenTilesRight.Add(9);
                Screens[79].OpenTilesRight.Add(10);
                Screens[79].OpenTilesRight.Add(11);
                Screens[80].Directions.Add(Direction.Right);
                for (byte i = 1; i < 12; i++)
                {
                    Screens[80].OpenTilesRight.Add(i);
                }
                Screens[80].OpenTilesRight.Remove(5);

                startToSpecial[MasconTowerEntranceScreen] = MasconTowerScreen;
                startToSpecial[TowerOfSufferEntranceScreen] = TowerOfSufferScreen;
                startToSpecial[VictimTowerEntranceScreen] = VictimTowerScreen;
            }
        }
    }
}
