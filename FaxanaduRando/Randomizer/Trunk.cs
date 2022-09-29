using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class Trunk : Level
    {
        SubLevel.Id firstTowerId;
        SubLevel.Id secondTowerId;

        public static byte MattockScreen = 12;
        public static byte SkySpringScreen = 37;
        public static byte FortressSpringScreen = 61;
        public static byte JokerSpringScreen = 63;

        public Trunk(WorldNumber number, byte[] content) : base(number, content)
        {
            Screens[6].Doors.Add(DoorId.TrunkSecretShop);
            Screens[11].Doors.Add(DoorId.TowerOfTrunk);
            Screens[30].Doors.Add(DoorId.JokerHouse);
            Screens[40].Doors.Add(DoorId.TowerOfFortress);
            Screens[45].Doors.Add(DoorId.FortressGuru);
            Screens[63].Gifts.Add(GiftItem.Id.JokerSpring);

            Screens[7].Doors.Add(DoorId.ApoluneBar);
            Screens[7].Doors.Add(DoorId.ApoluneGuru);
            Screens[7].Doors.Add(DoorId.ApoluneHospital);
            Screens[7].Doors.Add(DoorId.ApoluneHouse);
            Screens[7].Doors.Add(DoorId.ApoluneItemShop);
            Screens[7].Doors.Add(DoorId.ApoluneKeyShop);

            Screens[26].Doors.Add(DoorId.ForepawGuru);
            Screens[26].Doors.Add(DoorId.ForepawHospital);
            Screens[26].Doors.Add(DoorId.ForepawHouse);
            Screens[26].Doors.Add(DoorId.ForepawItemShop);
            Screens[26].Doors.Add(DoorId.ForepawKeyShop);
            Screens[26].Doors.Add(DoorId.ForepawMeatShop);
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

            Screens[61].Sprites = temp;
        }

        public override void GetEnds(ref List<Screen> startScreens, ref List<Screen> endScreens, Random random)
        {
            startScreens = new List<Screen>();
            startScreens.Add(Screens[29]);
            startScreens.Add(Screens[22]);
            startScreens.Add(Screens[8]);
            startScreens.Add(Screens[0]);
            startScreens.Add(Screens[41]);
            if (random.Next(0, 2) == 0)
            {
                startScreens.Add(Screens[13]);
                startScreens.Add(Screens[62]);
                firstTowerId = SubLevel.Id.TowerOfTrunk;
                secondTowerId = SubLevel.Id.JokerHouse;
            }
            else
            {
                startScreens.Add(Screens[62]);
                startScreens.Add(Screens[13]);
                firstTowerId = SubLevel.Id.JokerHouse;
                secondTowerId = SubLevel.Id.TowerOfTrunk;
            }

            endScreens = new List<Screen>();
            endScreens.Add(Screens[40]);
            endScreens.Add(Screens[26]);
            endScreens.Add(Screens[MattockScreen]);
            endScreens.Add(Screens[7]);
            if (random.Next(0, 2) == 0)
            {
                endScreens.Add(Screens[JokerSpringScreen]);
                endScreens.Add(Screens[FortressSpringScreen]);
            }
            else
            {
                endScreens.Add(Screens[FortressSpringScreen]);
                endScreens.Add(Screens[JokerSpringScreen]);
            }
            if ((random.Next(0, 2) == 0) ||
                ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Unchanged)
            {
                endScreens.Add(Screens[21]);
            }
            else
            {
                endScreens.Add(Screens[60]);
            }

            if (random.Next(0, 2) == 0)
            {
                var tmp = startScreens[0];
                startScreens[0] = startScreens[2];
                startScreens[2] = tmp;

                tmp = endScreens[1];
                endScreens[1] = endScreens[3];
                endScreens[3] = tmp;
            }

            if (ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Unchanged)
            {
                if (random.Next(0, 2) == 0)
                {
                    var tmp = startScreens[1];
                    startScreens[1] = startScreens[2];
                    startScreens[2] = tmp;

                    tmp = endScreens[1];
                    endScreens[1] = endScreens[2];
                    endScreens[2] = tmp;
                }
            }
        }

        public override List<Screen> GetCandidates(Random random)
        {
            var candidates = Screens.GetRange(1, 5);
            candidates.AddRange(Screens.GetRange(9, 2));
            candidates.AddRange(Screens.GetRange(14, 7));
            candidates.AddRange(Screens.GetRange(23, 3));
            candidates.AddRange(Screens.GetRange(31, 6));
            candidates.AddRange(Screens.GetRange(38, 2));
            candidates.Add(Screens[42]);
            candidates.Add(Screens[43]);
            candidates.AddRange(Screens.GetRange(46, 14));
            Util.ShuffleList(candidates, 0, candidates.Count - 1, random);
            return candidates;
        }

        public override List<Screen> GetSpecialScreens(Random random)
        {
            var specialScreens = new List<Screen>();
            specialScreens.Add(Screens[6]);
            specialScreens.Add(Screens[11]);
            specialScreens.Add(Screens[30]);
            specialScreens.Add(Screens[SkySpringScreen]);
            specialScreens.Add(Screens[45]);
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
                }
            }

            return result;
        }

        public override bool CreateSublevels(List<Screen> startScreens, List<Screen> endScreens, List<Screen> candidates, List<Screen> specialScreens, Random random)
        {
            int specialProbability = ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Unchanged ? 0 : 20;
            bool result = CreateSublevel(startScreens[0], endScreens[0], candidates, specialScreens, specialProbability, 100, random, SubLevel.Id.EastTrunk);
            if (!result)
            {
                return result;
            }
            result = CreateSublevel(startScreens[1], endScreens[1], candidates, specialScreens, specialProbability, 20, random, SubLevel.Id.LateTrunk);
            if (!result)
            {
                return result;
            }

            specialProbability = ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Unchanged ? 50 : 20;

            result = CreateSublevel(startScreens[2], endScreens[2], candidates, specialScreens, specialProbability, 20, random, SubLevel.Id.MiddleTrunk);
            if (!result)
            {
                return result;
            }
            result = CreateSublevel(startScreens[3], endScreens[3], candidates, specialScreens, specialProbability, 20, random, SubLevel.Id.EarlyTrunk);
            if (!result)
            {
                return result;
            }

            specialProbability = ItemOptions.ShuffleItems == ItemOptions.ItemShuffle.Unchanged ? 0 : 70;

            result = CreateSublevel(startScreens[4], endScreens[4], candidates, specialScreens, specialProbability, 15, random, SubLevel.Id.TowerOfFortress);
            if (!result)
            {
                return result;
            }

            if (specialScreens.Count > 0)
            {
                return false;
            }

            result = CreateSublevel(startScreens[5], endScreens[5], candidates, specialScreens, 0, 20, random, firstTowerId);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[6], endScreens[6], candidates, specialScreens, 0, 10, random, secondTowerId);
            if (!result)
            {
                return result;
            }

            if (candidates.Count > 8)
            {
                result = false;
            }

            return result;
        }

        public override void SetupScreens()
        {
            Screens[0].Directions.Add(Direction.Left);
            Screens[0].Directions.Add(Direction.Right);
            Screens[0].Directions.Add(Direction.Up);
            Screens[0].OpenTilesLeft.Add(0);
            Screens[0].OpenTilesLeft.Add(1);
            Screens[0].OpenTilesLeft.Add(2);
            Screens[0].OpenTilesLeft.Add(3);
            Screens[0].OpenTilesLeft.Add(8);
            Screens[0].OpenTilesLeft.Add(9);
            Screens[0].OpenTilesLeft.Add(10);
            Screens[0].OpenTilesLeft.Add(11);
            for (byte i = 0; i < 12; i++)
            {
                Screens[0].OpenTilesRight.Add(i);
                Screens[0].OpenTilesUp.Add(i);
            }
            Screens[0].OpenTilesUp.Add(12);
            Screens[0].OpenTilesUp.Add(13);
            Screens[0].OpenTilesUp.Add(14);
            Screens[0].OpenTilesUp.Add(15);
            Screens[1].Directions.Add(Direction.Left);
            Screens[1].Directions.Add(Direction.Right);
            Screens[1].Directions.Add(Direction.Up);
            for (byte i = 0; i < 12; i++)
            {
                Screens[1].OpenTilesLeft.Add(i);
                Screens[1].OpenTilesRight.Add(i);
                Screens[1].OpenTilesUp.Add(i);
            }
            Screens[1].OpenTilesUp.Add(12);
            Screens[1].OpenTilesUp.Add(13);
            Screens[1].OpenTilesUp.Add(14);
            Screens[1].OpenTilesUp.Add(15);
            Screens[1].OpenTilesUp.Remove(1);
            Screens[2].Directions.Add(Direction.Left);
            Screens[2].Directions.Add(Direction.Right);
            Screens[2].Directions.Add(Direction.Up);
            for (byte i = 0; i < 12; i++)
            {
                Screens[2].OpenTilesLeft.Add(i);
                Screens[2].OpenTilesRight.Add(i);
                Screens[2].OpenTilesUp.Add(i);
            }
            Screens[2].OpenTilesUp.Add(12);
            Screens[2].OpenTilesUp.Add(13);
            Screens[2].OpenTilesUp.Add(14);
            Screens[2].OpenTilesUp.Add(15);
            Screens[2].OpenTilesUp.Remove(3);
            Screens[2].OpenTilesUp.Remove(4);
            Screens[2].OpenTilesUp.Remove(5);
            Screens[3].Directions.Add(Direction.Left);
            Screens[3].Directions.Add(Direction.Up);
            for (byte i = 0; i < 12; i++)
            {
                Screens[3].OpenTilesLeft.Add(i);
            }
            Screens[3].OpenTilesUp.Add(0);
            Screens[3].OpenTilesUp.Add(1);
            Screens[3].OpenTilesUp.Add(2);
            Screens[3].OpenTilesUp.Add(3);
            Screens[3].OpenTilesUp.Add(4);
            Screens[3].OpenTilesUp.Add(5);
            Screens[3].OpenTilesUp.Add(6);
            Screens[3].OpenTilesUp.Add(7);
            Screens[3].OpenTilesUp.Add(9);
            Screens[4].Directions.Add(Direction.Left);
            Screens[4].Directions.Add(Direction.Right);
            Screens[4].Directions.Add(Direction.Up);
            Screens[4].Directions.Add(Direction.Down);
            for (byte i = 0; i < 12; i++)
            {
                Screens[4].OpenTilesLeft.Add(i);
                Screens[4].OpenTilesRight.Add(i);
                Screens[4].OpenTilesUp.Add(i);
            }
            Screens[4].OpenTilesDown.Add(0);
            Screens[4].OpenTilesDown.Add(1);
            Screens[4].OpenTilesDown.Add(2);
            Screens[4].OpenTilesDown.Add(9);
            Screens[4].OpenTilesUp.Add(12);
            Screens[4].OpenTilesUp.Add(13);
            Screens[4].OpenTilesUp.Add(14);
            Screens[4].OpenTilesUp.Add(15);
            Screens[5].Directions.Add(Direction.Left);
            Screens[5].Directions.Add(Direction.Right);
            Screens[5].Directions.Add(Direction.Up);
            for (byte i = 0; i < 12; i++)
            {
                Screens[5].OpenTilesLeft.Add(i);
                Screens[5].OpenTilesRight.Add(i);
            }
            Screens[5].OpenTilesUp.Add(0);
            Screens[5].OpenTilesUp.Add(1);
            Screens[5].OpenTilesUp.Add(2);
            Screens[5].OpenTilesUp.Add(3);
            Screens[5].OpenTilesUp.Add(4);
            Screens[5].OpenTilesUp.Add(5);
            Screens[5].OpenTilesUp.Add(6);
            Screens[5].OpenTilesUp.Add(7);
            Screens[5].OpenTilesRight.Remove(0);
            Screens[5].OpenTilesRight.Remove(1);
            Screens[5].OpenTilesRight.Remove(2);
            Screens[6].Directions.Add(Direction.Left);
            Screens[6].Directions.Add(Direction.Right);
            Screens[6].Directions.Add(Direction.Up);
            for (byte i = 3; i < 12; i++)
            {
                Screens[6].OpenTilesLeft.Add(i);
                Screens[6].OpenTilesRight.Add(i);
            }
            Screens[6].OpenTilesRight.Remove(3);
            Screens[6].OpenTilesRight.Remove(4);
            Screens[6].OpenTilesRight.Remove(7);
            Screens[6].OpenTilesUp.Add(12);
            Screens[6].OpenTilesUp.Add(13);
            Screens[6].OpenTilesUp.Add(14);
            Screens[7].Directions.Add(Direction.Left);
            Screens[7].Directions.Add(Direction.Up);
            for (byte i = 0; i < 12; i++)
            {
                Screens[7].OpenTilesLeft.Add(i);
            }
            Screens[7].OpenTilesLeft.Remove(7);
            Screens[7].OpenTilesUp.Add(0);
            Screens[7].OpenTilesUp.Add(1);
            Screens[7].OpenTilesUp.Add(3);
            Screens[7].OpenTilesUp.Add(4);
            Screens[7].OpenTilesUp.Add(6);
            Screens[7].OpenTilesUp.Add(7);
            Screens[7].OpenTilesUp.Add(8);
            Screens[7].OpenTilesUp.Add(9);
            Screens[8].Directions.Add(Direction.Right);
            Screens[8].Directions.Add(Direction.Up);
            for (byte i = 0; i < 12; i++)
            {
                Screens[8].OpenTilesRight.Add(i);
            }
            Screens[8].OpenTilesUp.Add(4);
            Screens[8].OpenTilesUp.Add(5);
            Screens[8].OpenTilesUp.Add(6);
            Screens[8].OpenTilesUp.Add(7);
            Screens[8].OpenTilesUp.Add(8);
            Screens[8].OpenTilesUp.Add(9);
            Screens[8].OpenTilesUp.Add(11);
            Screens[8].OpenTilesUp.Add(12);
            Screens[8].OpenTilesUp.Add(13);
            Screens[8].OpenTilesUp.Add(14);
            Screens[8].OpenTilesUp.Add(15);
            Screens[9].Directions.Add(Direction.Left);
            Screens[9].Directions.Add(Direction.Right);
            Screens[9].Directions.Add(Direction.Up);
            for (byte i = 0; i < 12; i++)
            {
                Screens[9].OpenTilesLeft.Add(i);
                Screens[9].OpenTilesRight.Add(i);
                Screens[9].OpenTilesUp.Add(i);
            }
            Screens[9].OpenTilesUp.Add(12);
            Screens[9].OpenTilesUp.Add(13);
            Screens[9].OpenTilesUp.Add(15);
            Screens[9].OpenTilesUp.Remove(6);
            Screens[10].Directions.Add(Direction.Left);
            Screens[10].Directions.Add(Direction.Right);
            Screens[10].Directions.Add(Direction.Up);
            for (byte i = 0; i < 12; i++)
            {
                Screens[10].OpenTilesLeft.Add(i);
                Screens[10].OpenTilesRight.Add(i);
                Screens[10].OpenTilesUp.Add(i);
            }
            Screens[10].OpenTilesUp.Add(12);
            Screens[10].OpenTilesUp.Add(13);
            Screens[10].OpenTilesUp.Add(15);
            Screens[10].OpenTilesUp.Remove(6);
            Screens[11].Directions.Add(Direction.Left);
            Screens[11].Directions.Add(Direction.Right);
            Screens[11].Directions.Add(Direction.Up);
            for (byte i = 0; i < 12; i++)
            {
                Screens[11].OpenTilesLeft.Add(i);
                Screens[11].OpenTilesRight.Add(i);
                Screens[11].OpenTilesUp.Add(i);
            }
            Screens[11].OpenTilesUp.Add(12);
            Screens[11].OpenTilesUp.Add(13);
            Screens[11].OpenTilesUp.Add(14);
            Screens[11].OpenTilesUp.Add(15);
            Screens[12].Directions.Add(Direction.Left);
            for (byte i = 0; i < 12; i++)
            {
                Screens[12].OpenTilesLeft.Add(i);
            }
            Screens[13].Directions.Add(Direction.Left);
            Screens[13].Directions.Add(Direction.Right);
            Screens[13].OpenTilesLeft.Add(9);
            Screens[13].OpenTilesLeft.Add(10);
            Screens[13].OpenTilesRight.Add(2);
            Screens[13].OpenTilesRight.Add(3);
            Screens[13].OpenTilesRight.Add(4);
            Screens[13].OpenTilesRight.Add(5);
            Screens[13].OpenTilesRight.Add(7);
            Screens[13].OpenTilesRight.Add(8);
            Screens[13].OpenTilesRight.Add(9);
            Screens[13].OpenTilesRight.Add(10);
            Screens[14].Directions.Add(Direction.Left);
            Screens[14].Directions.Add(Direction.Right);
            Screens[14].OpenTilesLeft.Add(1);
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
            for (byte i = 1; i < 11; i++)
            {
                Screens[16].OpenTilesRight.Add(i);
            }
            Screens[16].OpenTilesDown.Add(2);
            Screens[17].Directions.Add(Direction.Left);
            Screens[17].Directions.Add(Direction.Right);
            for (byte i = 1; i < 11; i++)
            {
                Screens[17].OpenTilesLeft.Add(i);
                Screens[17].OpenTilesRight.Add(i);
            }
            Screens[18].Directions.Add(Direction.Left);
            Screens[18].Directions.Add(Direction.Up);
            for (byte i = 1; i < 11; i++)
            {
                Screens[18].OpenTilesLeft.Add(i);
            }
            Screens[18].OpenTilesUp.Add(4);
            Screens[19].Directions.Add(Direction.Down);
            Screens[19].Directions.Add(Direction.Right);
            Screens[19].OpenTilesDown.Add(4);
            Screens[19].OpenTilesRight.Add(1);
            Screens[19].OpenTilesRight.Add(2);
            Screens[19].OpenTilesRight.Add(3);
            Screens[19].OpenTilesRight.Add(4);
            Screens[19].OpenTilesRight.Add(6);
            Screens[19].OpenTilesRight.Add(7);
            Screens[19].OpenTilesRight.Add(8);
            Screens[20].Directions.Add(Direction.Left);
            Screens[20].Directions.Add(Direction.Right);
            Screens[20].OpenTilesLeft.Add(1);
            Screens[20].OpenTilesLeft.Add(2);
            Screens[20].OpenTilesLeft.Add(3);
            Screens[20].OpenTilesLeft.Add(4);
            Screens[20].OpenTilesLeft.Add(6);
            Screens[20].OpenTilesLeft.Add(7);
            Screens[20].OpenTilesLeft.Add(8);
            Screens[20].OpenTilesRight.Add(1);
            Screens[20].OpenTilesRight.Add(2);
            Screens[20].OpenTilesRight.Add(3);
            Screens[20].OpenTilesRight.Add(4);
            Screens[20].OpenTilesRight.Add(5);
            Screens[20].OpenTilesRight.Add(6);
            Screens[20].OpenTilesRight.Add(7);
            Screens[20].OpenTilesRight.Add(8);
            Screens[21].Directions.Add(Direction.Left);
            Screens[21].OpenTilesLeft.Add(1);
            Screens[21].OpenTilesLeft.Add(2);
            Screens[21].OpenTilesLeft.Add(3);
            Screens[21].OpenTilesLeft.Add(4);
            Screens[21].OpenTilesLeft.Add(6);
            Screens[21].OpenTilesLeft.Add(7);
            Screens[21].OpenTilesLeft.Add(8);
            Screens[22].Directions.Add(Direction.Right);
            //Screens[22].Directions.Add(Direction.Up);
            Screens[22].OpenTilesRight.Add(7);
            Screens[22].OpenTilesRight.Add(8);
            Screens[22].OpenTilesRight.Add(9);
            Screens[22].OpenTilesRight.Add(10);
            Screens[23].Directions.Add(Direction.Left);
            Screens[23].Directions.Add(Direction.Up);
            for (byte i = 1; i < 11; i++)
            {
                Screens[23].OpenTilesLeft.Add(i);
                Screens[23].OpenTilesUp.Add(i);
            }
            Screens[23].OpenTilesLeft.Remove(6);
            Screens[23].OpenTilesUp.Add(11);
            Screens[23].OpenTilesUp.Add(12);
            Screens[23].OpenTilesUp.Add(13);
            Screens[23].OpenTilesUp.Add(14);
            Screens[23].OpenTilesUp.Add(15);
            Screens[24].Directions.Add(Direction.Right);
            Screens[24].Directions.Add(Direction.Down);
            Screens[24].OpenTilesDown.Add(1);
            Screens[24].OpenTilesRight.Add(1);
            Screens[24].OpenTilesRight.Add(2);
            Screens[24].OpenTilesRight.Add(3);
            Screens[24].OpenTilesRight.Add(4);
            Screens[24].OpenTilesRight.Add(5);
            Screens[24].OpenTilesRight.Add(7);
            Screens[24].OpenTilesRight.Add(8);
            Screens[24].OpenTilesRight.Add(9);
            Screens[24].OpenTilesRight.Add(10);
            Screens[25].Directions.Add(Direction.Left);
            Screens[25].Directions.Add(Direction.Right);
            Screens[25].OpenTilesLeft.Add(1);
            Screens[25].OpenTilesLeft.Add(2);
            Screens[25].OpenTilesLeft.Add(3);
            Screens[25].OpenTilesLeft.Add(4);
            Screens[25].OpenTilesLeft.Add(5);
            Screens[25].OpenTilesLeft.Add(7);
            Screens[25].OpenTilesLeft.Add(8);
            Screens[25].OpenTilesLeft.Add(9);
            Screens[25].OpenTilesLeft.Add(10);
            Screens[25].OpenTilesRight.Add(1);
            Screens[25].OpenTilesRight.Add(2);
            Screens[25].OpenTilesRight.Add(3);
            Screens[25].OpenTilesRight.Add(4);
            Screens[25].OpenTilesRight.Add(5);
            Screens[25].OpenTilesRight.Add(7);
            Screens[25].OpenTilesRight.Add(8);
            Screens[25].OpenTilesRight.Add(9);
            Screens[25].OpenTilesRight.Add(10);
            Screens[26].Directions.Add(Direction.Left);
            Screens[26].OpenTilesLeft.Add(1);
            Screens[26].OpenTilesLeft.Add(2);
            Screens[26].OpenTilesLeft.Add(3);
            Screens[26].OpenTilesLeft.Add(4);
            Screens[26].OpenTilesLeft.Add(5);
            Screens[26].OpenTilesLeft.Add(7);
            Screens[26].OpenTilesLeft.Add(8);
            Screens[26].OpenTilesLeft.Add(9);
            Screens[26].OpenTilesLeft.Add(10);
            Screens[28].Directions.Add(Direction.Right);
            Screens[28].Directions.Add(Direction.Up);
            for (byte i = 0; i < 12; i++)
            {
                Screens[28].OpenTilesRight.Add(i);
                Screens[28].OpenTilesUp.Add(i);
            }
            Screens[28].OpenTilesRight.Remove(5);
            Screens[28].OpenTilesRight.Remove(6);
            Screens[28].OpenTilesUp.Add(12);
            Screens[28].OpenTilesUp.Add(13);
            Screens[28].OpenTilesUp.Add(14);
            Screens[28].OpenTilesUp.Add(15);
            Screens[29].Directions.Add(Direction.Right);
            Screens[29].Directions.Add(Direction.Up);
            Screens[29].Directions.Add(Direction.Down);
            for (byte i = 0; i < 13; i++)
            {
                Screens[29].OpenTilesRight.Add(i);
            }
            Screens[29].OpenTilesUp.Add(4);
            Screens[29].OpenTilesUp.Add(9);
            Screens[29].OpenTilesUp.Add(10);
            Screens[29].OpenTilesUp.Add(11);
            Screens[29].OpenTilesUp.Add(12);
            Screens[29].OpenTilesUp.Add(13);
            Screens[29].OpenTilesUp.Add(14);
            Screens[29].OpenTilesUp.Add(15);
            Screens[29].OpenTilesDown.Add(2);
            Screens[29].OpenTilesDown.Add(4);
            Screens[29].OpenTilesDown.Add(5);
            Screens[29].OpenTilesDown.Add(7);
            Screens[29].OpenTilesDown.Add(8);
            Screens[29].OpenTilesDown.Add(9);
            Screens[29].OpenTilesDown.Add(10);
            Screens[29].OpenTilesDown.Add(11);
            Screens[29].OpenTilesDown.Add(12);
            Screens[29].OpenTilesDown.Add(13);
            Screens[29].OpenTilesDown.Add(14);
            Screens[29].OpenTilesDown.Add(15);
            Screens[30].Directions.Add(Direction.Left);
            Screens[30].Directions.Add(Direction.Right);
            Screens[30].Directions.Add(Direction.Up);
            for (byte i = 0; i < 11; i++)
            {
                Screens[30].OpenTilesLeft.Add(i);
                Screens[30].OpenTilesRight.Add(i);
            }
            Screens[30].OpenTilesLeft.Remove(0);
            Screens[30].OpenTilesLeft.Remove(1);
            Screens[30].OpenTilesLeft.Remove(2);
            Screens[30].OpenTilesLeft.Remove(3);
            Screens[30].OpenTilesLeft.Remove(4);
            Screens[30].OpenTilesLeft.Remove(5);
            Screens[30].OpenTilesLeft.Remove(6);
            Screens[30].OpenTilesUp.Add(7);
            Screens[30].OpenTilesUp.Add(8);
            Screens[30].OpenTilesUp.Add(9);
            Screens[30].OpenTilesUp.Add(10);
            Screens[30].OpenTilesUp.Add(11);
            Screens[30].OpenTilesUp.Add(12);
            Screens[30].OpenTilesUp.Add(13);
            Screens[30].OpenTilesUp.Add(14);
            Screens[30].OpenTilesUp.Add(15);
            Screens[31].Directions.Add(Direction.Left);
            Screens[31].Directions.Add(Direction.Right);
            Screens[31].Directions.Add(Direction.Up);
            for (byte i = 0; i < 11; i++)
            {
                Screens[31].OpenTilesLeft.Add(i);
                Screens[31].OpenTilesRight.Add(i);
                Screens[31].OpenTilesUp.Add(i);
            }
            Screens[31].OpenTilesUp.Add(11);
            Screens[31].OpenTilesUp.Add(12);
            Screens[31].OpenTilesUp.Add(13);
            Screens[31].OpenTilesUp.Add(14);
            Screens[31].OpenTilesUp.Add(15);
            Screens[32].Directions.Add(Direction.Left);
            Screens[32].Directions.Add(Direction.Right);
            Screens[32].Directions.Add(Direction.Up);
            for (byte i = 0; i < 11; i++)
            {
                Screens[32].OpenTilesLeft.Add(i);
                Screens[32].OpenTilesRight.Add(i);
                Screens[32].OpenTilesUp.Add(i);
            }
            Screens[32].OpenTilesUp.Add(11);
            Screens[32].OpenTilesUp.Add(12);
            Screens[32].OpenTilesUp.Add(13);
            Screens[32].OpenTilesUp.Add(14);
            Screens[32].OpenTilesUp.Add(15);
            Screens[32].OpenTilesRight.Remove(10);
            Screens[33].Directions.Add(Direction.Left);
            Screens[33].Directions.Add(Direction.Up);
            for (byte i = 0; i < 10; i++)
            {
                Screens[33].OpenTilesLeft.Add(i);
                Screens[33].OpenTilesUp.Add(i);
            }
            Screens[33].OpenTilesUp.Add(10);
            Screens[33].OpenTilesUp.Add(11);
            Screens[33].OpenTilesUp.Add(12);
            Screens[33].OpenTilesUp.Add(13);
            Screens[33].OpenTilesUp.Add(14);
            Screens[33].OpenTilesUp.Add(15);
            Screens[34].Directions.Add(Direction.Left);
            Screens[34].Directions.Add(Direction.Right);
            Screens[34].Directions.Add(Direction.Up);
            Screens[34].Directions.Add(Direction.Down);
            for (byte i = 0; i < 13; i++)
            {
                Screens[34].OpenTilesLeft.Add(i);
                Screens[34].OpenTilesUp.Add(i);
                Screens[34].OpenTilesDown.Add(i);
            }
            Screens[34].OpenTilesRight.Add(0);
            Screens[34].OpenTilesRight.Add(1);
            Screens[34].OpenTilesRight.Add(2);
            Screens[34].OpenTilesRight.Add(3);
            Screens[34].OpenTilesUp.Add(13);
            Screens[34].OpenTilesUp.Add(14);
            Screens[34].OpenTilesUp.Add(15);
            Screens[34].OpenTilesDown.Add(13);
            Screens[35].Directions.Add(Direction.Left);
            Screens[35].Directions.Add(Direction.Right);
            Screens[35].Directions.Add(Direction.Up);
            Screens[35].Directions.Add(Direction.Down);
            for (byte i = 0; i < 13; i++)
            {
                Screens[35].OpenTilesLeft.Add(i);
                Screens[35].OpenTilesRight.Add(i);
                Screens[35].OpenTilesDown.Add(i);
            }
            Screens[35].OpenTilesUp.Add(2);
            Screens[35].OpenTilesUp.Add(3);
            Screens[35].OpenTilesUp.Add(4);
            Screens[35].OpenTilesUp.Add(5);
            Screens[35].OpenTilesDown.Add(13);
            Screens[35].OpenTilesDown.Add(14);
            Screens[35].OpenTilesDown.Add(15);
            Screens[36].Directions.Add(Direction.Up);
            Screens[36].Directions.Add(Direction.Down);
            for (byte i = 1; i < 15; i++)
            {
                Screens[36].OpenTilesUp.Add(i);
            }
            Screens[36].OpenTilesDown.Add(2);
            Screens[36].OpenTilesDown.Add(3);
            Screens[36].OpenTilesDown.Add(4);
            Screens[36].OpenTilesDown.Add(5);
            Screens[37].Directions.Add(Direction.Left);
            Screens[37].Directions.Add(Direction.Right);
            Screens[37].Directions.Add(Direction.Up);
            Screens[37].Directions.Add(Direction.Down);
            for (byte i = 1; i < 15; i++)
            {
                Screens[37].OpenTilesUp.Add(i);
                Screens[37].OpenTilesDown.Add(i);
            }
            Screens[37].OpenTilesDown.Add(0);
            Screens[37].OpenTilesDown.Add(15);
            for (byte i = 0; i < 7; i++)
            {
                Screens[37].OpenTilesLeft.Add(i);
                Screens[37].OpenTilesRight.Add(i);
            }
            Screens[38].Directions.Add(Direction.Left);
            Screens[38].Directions.Add(Direction.Right);
            Screens[38].Directions.Add(Direction.Up);
            Screens[38].Directions.Add(Direction.Down);
            for (byte i = 0; i < 16; i++)
            {
                Screens[38].OpenTilesRight.Add(i);
            }
            Screens[38].OpenTilesLeft.Add(1);
            Screens[38].OpenTilesLeft.Add(2);
            Screens[38].OpenTilesLeft.Add(3);
            Screens[38].OpenTilesLeft.Add(4);
            Screens[38].OpenTilesLeft.Add(6);
            Screens[38].OpenTilesLeft.Add(7);
            Screens[38].OpenTilesLeft.Add(8);
            Screens[38].OpenTilesLeft.Add(9);
            Screens[38].OpenTilesLeft.Add(10);
            Screens[38].OpenTilesUp.Add(10);
            Screens[38].OpenTilesUp.Add(11);
            Screens[38].OpenTilesUp.Add(12);
            Screens[38].OpenTilesUp.Add(13);
            Screens[38].OpenTilesUp.Add(14);
            Screens[38].OpenTilesUp.Add(15);
            Screens[38].OpenTilesDown.Add(4);
            Screens[38].OpenTilesDown.Add(9);
            Screens[38].OpenTilesDown.Add(10);
            Screens[38].OpenTilesDown.Add(11);
            Screens[38].OpenTilesDown.Add(12);
            Screens[38].OpenTilesDown.Add(13);
            Screens[38].OpenTilesDown.Add(14);
            Screens[38].OpenTilesDown.Add(15);
            Screens[39].Directions.Add(Direction.Left);
            Screens[39].Directions.Add(Direction.Right);
            for (byte i = 0; i < 11; i++)
            {
                Screens[39].OpenTilesLeft.Add(i);
                Screens[39].OpenTilesRight.Add(i);
            }
            //Screens[40].Directions.Add(Direction.Left);
            Screens[40].Directions.Add(Direction.Right);
            for (byte i = 0; i < 11; i++)
            {
                Screens[40].OpenTilesRight.Add(i);
            }
            Screens[41].Directions.Add(Direction.Right);
            Screens[41].Directions.Add(Direction.Up);
            for (byte i = 3; i < 11; i++)
            {
                Screens[41].OpenTilesRight.Add(i);
            }
            Screens[41].OpenTilesUp.Add(2);
            Screens[42].Directions.Add(Direction.Left);
            Screens[42].Directions.Add(Direction.Right);
            for (byte i = 3; i < 11; i++)
            {
                Screens[42].OpenTilesLeft.Add(i);
                Screens[42].OpenTilesRight.Add(i);
            }
            Screens[42].OpenTilesRight.Add(2);
            Screens[43].Directions.Add(Direction.Left);
            Screens[43].Directions.Add(Direction.Right);
            Screens[43].Directions.Add(Direction.Up);
            for (byte i = 2; i < 11; i++)
            {
                Screens[43].OpenTilesLeft.Add(i);
                Screens[43].OpenTilesRight.Add(i);
            }
            Screens[43].OpenTilesRight.Add(2);
            Screens[43].OpenTilesRight.Remove(5);
            Screens[43].OpenTilesUp.Add(6);
            Screens[44].Directions.Add(Direction.Left);
            for (byte i = 2; i < 11; i++)
            {
                Screens[44].OpenTilesLeft.Add(i);
            }
            Screens[44].OpenTilesRight.Remove(5);
            Screens[45].Directions.Add(Direction.Right);
            Screens[45].Directions.Add(Direction.Up);
            Screens[45].Directions.Add(Direction.Down);
            for (byte i = 2; i < 11; i++)
            {
                Screens[45].OpenTilesRight.Add(i);
            }
            Screens[45].OpenTilesUp.Add(1);
            Screens[45].OpenTilesDown.Add(2);
            Screens[46].Directions.Add(Direction.Left);
            Screens[46].Directions.Add(Direction.Right);
            for (byte i = 2; i < 11; i++)
            {
                Screens[46].OpenTilesLeft.Add(i);
            }
            Screens[46].OpenTilesRight.Add(2);
            Screens[46].OpenTilesRight.Add(3);
            Screens[46].OpenTilesRight.Add(4);
            Screens[47].Directions.Add(Direction.Left);
            Screens[47].Directions.Add(Direction.Right);
            Screens[47].Directions.Add(Direction.Down);
            for (byte i = 1; i < 11; i++)
            {
                Screens[47].OpenTilesRight.Add(i);
            }
            Screens[47].OpenTilesLeft.Add(2);
            Screens[47].OpenTilesLeft.Add(3);
            Screens[47].OpenTilesLeft.Add(4);
            Screens[47].OpenTilesDown.Add(6);
            Screens[48].Directions.Add(Direction.Left);
            Screens[48].Directions.Add(Direction.Right);
            Screens[48].Directions.Add(Direction.Up);
            for (byte i = 1; i < 11; i++)
            {
                Screens[48].OpenTilesLeft.Add(i);
            }
            Screens[48].OpenTilesRight.Add(2);
            Screens[48].OpenTilesRight.Add(3);
            Screens[48].OpenTilesRight.Add(4);
            Screens[48].OpenTilesRight.Add(5);
            Screens[48].OpenTilesRight.Add(7);
            Screens[48].OpenTilesRight.Add(8);
            Screens[48].OpenTilesRight.Add(9);
            Screens[48].OpenTilesRight.Add(10);
            Screens[48].OpenTilesUp.Add(1);
            Screens[48].OpenTilesUp.Add(2);
            Screens[48].OpenTilesUp.Add(3);
            Screens[48].OpenTilesUp.Add(4);
            Screens[48].OpenTilesUp.Add(5);
            Screens[48].OpenTilesUp.Add(6);
            Screens[48].OpenTilesUp.Add(7);
            Screens[48].OpenTilesUp.Add(8);
            Screens[49].Directions.Add(Direction.Left);
            Screens[49].Directions.Add(Direction.Right);
            Screens[49].Directions.Add(Direction.Up);
            for (byte i = 0; i < 11; i++)
            {
                Screens[49].OpenTilesRight.Add(i);
            }
            Screens[49].OpenTilesLeft.Add(2);
            Screens[49].OpenTilesLeft.Add(3);
            Screens[49].OpenTilesLeft.Add(4);
            Screens[49].OpenTilesLeft.Add(5);
            Screens[49].OpenTilesLeft.Add(7);
            Screens[49].OpenTilesLeft.Add(8);
            Screens[49].OpenTilesLeft.Add(9);
            Screens[49].OpenTilesLeft.Add(10);
            Screens[50].Directions.Add(Direction.Left);
            Screens[50].Directions.Add(Direction.Up);
            for (byte i = 1; i < 15; i++)
            {
                Screens[50].OpenTilesUp.Add(i);
            }
            Screens[50].OpenTilesLeft.Add(5);
            Screens[50].OpenTilesLeft.Add(6);
            Screens[50].OpenTilesLeft.Add(7);
            Screens[50].OpenTilesLeft.Add(8);
            Screens[50].OpenTilesLeft.Add(9);
            Screens[50].OpenTilesLeft.Add(10);
            Screens[51].Directions.Add(Direction.Right);
            Screens[51].Directions.Add(Direction.Down);
            Screens[51].OpenTilesRight.Add(8);
            Screens[51].OpenTilesRight.Add(9);
            Screens[51].OpenTilesRight.Add(10);
            Screens[51].OpenTilesDown.Add(1);
            Screens[52].Directions.Add(Direction.Left);
            Screens[52].Directions.Add(Direction.Up);
            Screens[52].OpenTilesLeft.Add(8);
            Screens[52].OpenTilesLeft.Add(9);
            Screens[52].OpenTilesLeft.Add(10);
            Screens[52].OpenTilesUp.Add(2);
            Screens[52].OpenTilesUp.Add(4);
            Screens[52].OpenTilesUp.Add(6);
            Screens[52].OpenTilesUp.Add(8);
            Screens[52].OpenTilesUp.Add(10);
            Screens[52].OpenTilesUp.Add(12);
            Screens[53].Directions.Add(Direction.Right);
            Screens[53].Directions.Add(Direction.Up);
            Screens[53].OpenTilesRight.Add(1);
            Screens[53].OpenTilesRight.Add(2);
            Screens[53].OpenTilesRight.Add(3);
            Screens[53].OpenTilesRight.Add(4);
            Screens[53].OpenTilesRight.Add(5);
            Screens[53].OpenTilesUp.Add(1);
            Screens[53].OpenTilesUp.Add(2);
            Screens[53].OpenTilesUp.Add(4);
            Screens[53].OpenTilesUp.Add(5);
            Screens[53].OpenTilesUp.Add(6);
            Screens[53].OpenTilesUp.Add(8);
            Screens[53].OpenTilesUp.Add(9);
            Screens[53].OpenTilesUp.Add(10);
            Screens[53].OpenTilesUp.Add(12);
            Screens[53].OpenTilesUp.Add(13);
            Screens[53].OpenTilesUp.Add(15);
            Screens[54].Directions.Add(Direction.Left);
            Screens[54].Directions.Add(Direction.Down);
            Screens[54].OpenTilesLeft.Add(1);
            Screens[54].OpenTilesLeft.Add(2);
            Screens[54].OpenTilesLeft.Add(3);
            Screens[54].OpenTilesLeft.Add(4);
            Screens[54].OpenTilesLeft.Add(5);
            Screens[54].OpenTilesDown.Add(1);
            Screens[54].OpenTilesDown.Add(4);
            Screens[54].OpenTilesDown.Add(7);
            Screens[55].Directions.Add(Direction.Right);
            Screens[55].Directions.Add(Direction.Up);
            Screens[55].Directions.Add(Direction.Down);
            for (byte i = 1; i < 14; i++)
            {
                Screens[55].OpenTilesUp.Add(i);
            }
            for (byte i = 1; i < 9; i++)
            {
                Screens[55].OpenTilesRight.Add(i);
            }
            Screens[55].OpenTilesDown.Add(12);
            Screens[55].OpenTilesDown.Add(13);
            Screens[56].Directions.Add(Direction.Up);
            Screens[56].Directions.Add(Direction.Down);
            for (byte i = 1; i < 15; i++)
            {
                Screens[56].OpenTilesDown.Add(i);
            }
            Screens[56].OpenTilesUp.Add(14);
            Screens[57].Directions.Add(Direction.Right);
            //Screens[57].Directions.Add(Direction.Up);
            Screens[57].Directions.Add(Direction.Down);
            for (byte i = 2; i < 11; i++)
            {
                Screens[57].OpenTilesRight.Add(i);
            }
            Screens[57].OpenTilesDown.Add(4);
            Screens[58].Directions.Add(Direction.Left);
            Screens[58].Directions.Add(Direction.Right);
            Screens[58].Directions.Add(Direction.Down);
            for (byte i = 1; i < 11; i++)
            {
                Screens[58].OpenTilesLeft.Add(i);
            }
            for (byte i = 2; i < 7; i++)
            {
                Screens[58].OpenTilesRight.Add(i);
            }
            Screens[58].OpenTilesDown.Add(1);
            Screens[59].Directions.Add(Direction.Left);
            Screens[59].Directions.Add(Direction.Right);
            for (byte i = 2; i < 7; i++)
            {
                Screens[59].OpenTilesLeft.Add(i);
            }
            Screens[59].OpenTilesRight.Add(2);
            Screens[59].OpenTilesRight.Add(3);
            Screens[59].OpenTilesRight.Add(6);
            Screens[59].OpenTilesRight.Add(7);
            Screens[59].OpenTilesRight.Add(8);
            Screens[60].Directions.Add(Direction.Left);
            //Screens[60].Directions.Add(Direction.Right);
            //Screens[60].Directions.Add(Direction.Up);
            //Screens[60].Directions.Add(Direction.Down);
            Screens[60].OpenTilesLeft.Add(1);
            Screens[60].OpenTilesLeft.Add(2);
            Screens[60].OpenTilesLeft.Add(3);
            Screens[60].OpenTilesLeft.Add(5);
            Screens[60].OpenTilesLeft.Add(6);
            Screens[60].OpenTilesLeft.Add(7);
            Screens[60].OpenTilesLeft.Add(8);
            Screens[60].OpenTilesRight.Add(1);
            Screens[60].OpenTilesRight.Add(2);
            Screens[60].OpenTilesRight.Add(3);
            Screens[60].OpenTilesRight.Add(7);
            Screens[60].OpenTilesRight.Add(8);
            Screens[60].OpenTilesRight.Add(9);
            Screens[60].OpenTilesRight.Add(10);
            Screens[60].OpenTilesUp.Add(5);
            Screens[60].OpenTilesUp.Add(6);
            Screens[60].OpenTilesUp.Add(7);
            Screens[60].OpenTilesUp.Add(8);
            Screens[60].OpenTilesUp.Add(9);
            Screens[60].OpenTilesUp.Add(10);
            Screens[60].OpenTilesUp.Add(11);
            Screens[60].OpenTilesUp.Add(12);
            Screens[60].OpenTilesUp.Add(13);
            Screens[60].OpenTilesDown.Add(3);
            Screens[60].OpenTilesDown.Add(4);
            Screens[60].OpenTilesDown.Add(5);
            Screens[60].OpenTilesDown.Add(6);
            Screens[60].OpenTilesDown.Add(7);
            Screens[60].OpenTilesDown.Add(8);
            Screens[60].OpenTilesDown.Add(9);
            Screens[60].OpenTilesDown.Add(10);
            Screens[60].OpenTilesDown.Add(11);
            Screens[61].Directions.Add(Direction.Left);
            //Screens[61].Directions.Add(Direction.Down);
            Screens[61].OpenTilesLeft.Add(1);
            Screens[61].OpenTilesLeft.Add(2);
            Screens[61].OpenTilesLeft.Add(3);
            //Screens[61].OpenTilesLeft.Add(5);
            //Screens[61].OpenTilesLeft.Add(6);
            //Screens[61].OpenTilesLeft.Add(7);
            //Screens[61].OpenTilesLeft.Add(8);
            //Screens[61].OpenTilesLeft.Add(9);
            //Screens[61].OpenTilesLeft.Add(10);
            //Screens[61].OpenTilesDown.Add(14);
            Screens[62].Directions.Add(Direction.Right);
            Screens[62].OpenTilesRight.Add(2);
            Screens[62].OpenTilesRight.Add(3);
            Screens[62].OpenTilesRight.Add(4);
            Screens[62].OpenTilesRight.Add(6);
            Screens[62].OpenTilesRight.Add(7);
            Screens[62].OpenTilesRight.Add(8);
            Screens[62].OpenTilesRight.Add(9);
            Screens[62].OpenTilesRight.Add(10);
            Screens[63].Directions.Add(Direction.Left);
            Screens[63].OpenTilesLeft.Add(2);
            Screens[63].OpenTilesLeft.Add(3);
            Screens[63].OpenTilesLeft.Add(4);
            Screens[63].OpenTilesLeft.Add(6);
            Screens[63].OpenTilesLeft.Add(7);
            Screens[63].OpenTilesLeft.Add(8);
            Screens[63].OpenTilesLeft.Add(9);
            Screens[63].OpenTilesLeft.Add(10);
        }
    }
}
