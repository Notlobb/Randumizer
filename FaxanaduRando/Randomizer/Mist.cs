using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class Mist : Level
    {
        SubLevel.Id firstTowerId;
        SubLevel.Id secondTowerId;
        SubLevel.Id thirdTowerId;

        public Mist(WorldNumber number, byte[] content) : base(number, content)
        {
            Screens[0].Doors.Add(DoorId.TowerOfSuffer);
            Screens[1].Doors.Add(DoorId.MasconTower);
            Screens[6].Doors.Add(DoorId.MistSmallHouse);
            Screens[6].Doors.Add(DoorId.MistGuru);
            Screens[6].Doors.Add(DoorId.BirdHospital);
            Screens[13].Doors.Add(DoorId.MistSecretShop);
            Screens[22].Doors.Add(DoorId.MistLargeHouse);
            Screens[23].Doors.Add(DoorId.TowerOfMist);
            Screens[31].Doors.Add(DoorId.AceKeyHouse);
            Screens[32].Doors.Add(DoorId.VictimTower);
            Screens[41].Doors.Add(DoorId.FireMage);

            Screens[9].Doors.Add(DoorId.MasconBar);
            Screens[9].Doors.Add(DoorId.MasconHospital);
            Screens[9].Doors.Add(DoorId.MasconHouse);
            Screens[9].Doors.Add(DoorId.MasconItemShop);
            Screens[9].Doors.Add(DoorId.MasconKeyShop);
            Screens[9].Doors.Add(DoorId.MasconMeatShop);

            Screens[34].Doors.Add(DoorId.VictimBar);
            Screens[34].Doors.Add(DoorId.VictimGuru);
            Screens[34].Doors.Add(DoorId.VictimHospital);
            Screens[34].Doors.Add(DoorId.VictimHouse);
            Screens[34].Doors.Add(DoorId.VictimItemShop);
            Screens[34].Doors.Add(DoorId.VictimKeyShop);
            Screens[34].Doors.Add(DoorId.VictimMeatShop);
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
            startScreens.Add(Screens[12]);
            startScreens.Add(Screens[37]);

            var pairs = new List<List<Screen>>();
            pairs.Add(new List<Screen> { Screens[0], Screens[62] });
            pairs.Add(new List<Screen> { Screens[23], Screens[69] });
            pairs.Add(new List<Screen> { Screens[32], Screens[80] });
            Util.ShuffleList(pairs, 0, pairs.Count - 1, random);

            var towerIds = new Dictionary<byte, SubLevel.Id>()
            {
                { 0, SubLevel.Id.TowerOfSuffer },
                { 23, SubLevel.Id.TowerOfMist},
                { 32, SubLevel.Id.VictimTower },
            };

            firstTowerId = towerIds[pairs[0][0].Number];
            secondTowerId = towerIds[pairs[1][0].Number];
            thirdTowerId = towerIds[pairs[2][0].Number];

            startScreens.Add(pairs[0][1]);
            startScreens.Add(Screens[79]);
            startScreens.Add(pairs[1][1]);
            startScreens.Add(pairs[2][1]);

            endScreens = new List<Screen>();
            endScreens.Add(Screens[9]);
            endScreens.Add(Screens[34]);

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
            endScreens.Add(Screens[13]);

            var possibleEnds = new List<Screen>() { Screens[76] };
            if (ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Unchanged)
            {
                possibleEnds.Add(Screens[52]);
                possibleEnds.Add(Screens[65]);
                possibleEnds.Add(Screens[77]);
                Util.ShuffleList(possibleEnds, 0, possibleEnds.Count - 1, random);
            }

            endScreens.Add(possibleEnds[0]);
        }

        public override List<Screen> GetCandidates(Random random)
        {
            var candidates = new List<Screen>();
            candidates.AddRange(Screens.GetRange(2, 4));
            candidates.AddRange(Screens.GetRange(7, 2));
            candidates.AddRange(Screens.GetRange(14, 3));
            candidates.AddRange(Screens.GetRange(18, 4));
            candidates.AddRange(Screens.GetRange(24, 7));
            candidates.Add(Screens[33]);
            candidates.AddRange(Screens.GetRange(37, 4));
            candidates.AddRange(Screens.GetRange(42, 10));
            candidates.AddRange(Screens.GetRange(53, 4));
            candidates.AddRange(Screens.GetRange(59, 3));
            candidates.AddRange(Screens.GetRange(63, 2));
            candidates.AddRange(Screens.GetRange(66, 2));
            candidates.AddRange(Screens.GetRange(70, 6));
            candidates.Add(Screens[78]);
            Util.ShuffleList(candidates, 0, candidates.Count - 1, random);
            return candidates;
        }

        public override List<Screen> GetSpecialScreens(Random random)
        {
            var specialScreens = new List<Screen>();
            specialScreens.Add(Screens[1]);
            specialScreens.Add(Screens[6]);
            specialScreens.Add(Screens[22]);
            specialScreens.Add(Screens[31]);
            specialScreens.Add(Screens[41]);
            Util.ShuffleList(specialScreens, 0, specialScreens.Count - 1, random);
            return specialScreens;
        }

        public override bool CreateSublevels(List<Screen> startScreens, List<Screen> endScreens, List<Screen> candidates, List<Screen> specialScreens, Random random)
        {
            bool result = CreateSublevel(startScreens[0], endScreens[0], candidates, specialScreens, 30, 10, random, SubLevel.Id.EarlyMist);
            if (!result)
            {
                return result;
            }
            result = CreateSublevel(startScreens[1], endScreens[1], candidates, specialScreens, 30, 10, random, SubLevel.Id.MiddleMist);
            if (!result)
            {
                return result;
            }
            result = CreateSublevel(startScreens[2], endScreens[2], candidates, specialScreens, 30, 10, random, SubLevel.Id.LateMist);
            if (!result)
            {
                return result;
            }

            if (specialScreens.Contains(Screens[31]))
            {
                return false;
            }

            result = CreateSublevel(startScreens[3], endScreens[3], candidates, specialScreens, 30, 10, random, firstTowerId);
            if (!result)
            {
                return result;
            }

            if (specialScreens.Contains(Screens[1]))
            {
                return false;
            }

            result = CreateSublevel(startScreens[4], endScreens[4], candidates, specialScreens, 50, 10, random, SubLevel.Id.MasconTower);
            if (!result)
            {
                return result;
            }
            result = CreateSublevel(startScreens[5], endScreens[5], candidates, specialScreens, 50, 10, random, secondTowerId);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[6], endScreens[6], candidates, specialScreens, 95, 2, random, thirdTowerId);
            if (!result)
            {
                return result;
            }

            if (specialScreens.Count > 0)
            {
                return false;
            }

            if (candidates.Count > 15)
            {
                return false;
            }

            return result;
        }

        public override void SetupScreens()
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
            Screens[31].Directions.Add(Direction.Left); // Mist exit
            Screens[31].Directions.Add(Direction.Right);
            Screens[31].Directions.Add(Direction.Up);
            for (byte i = 2; i < 11; i++)
            {
                Screens[31].OpenTilesLeft.Add(i);
                Screens[31].OpenTilesRight.Add(i);
            }
            Screens[31].OpenTilesUp.Add(10);
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
            for (byte i = 1; i < 12; i++)
            {
                Screens[79].OpenTilesLeft.Add(i);
            }
            Screens[79].OpenTilesLeft.Remove(5);
            Screens[79].OpenTilesLeft.Remove(8);
            Screens[79].OpenTilesRight.Add(10);
            Screens[79].OpenTilesRight.Add(11);
            Screens[80].Directions.Add(Direction.Right);
            for (byte i = 1; i < 12; i++)
            {
                Screens[80].OpenTilesRight.Add(i);
            }
            Screens[80].OpenTilesRight.Remove(5);
        }
    }
}
