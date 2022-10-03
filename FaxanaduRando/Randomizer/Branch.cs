using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    internal class Branch : Level
    {
        public Branch(WorldNumber number, byte[] content) : base(number, content)
        {
            Screens[10].Doors.Add(DoorId.BattleHelmetWing);
            Screens[14].Doors.Add(DoorId.EastBranch);
            Screens[20].Doors.Add(DoorId.BackFromEastBranch);
            Screens[25].Doors.Add(DoorId.DropdownWing);

            Screens[35].Doors.Add(DoorId.DaybreakBar);
            Screens[35].Doors.Add(DoorId.DaybreakGuru);
            Screens[35].Doors.Add(DoorId.DaybreakHouse);
            Screens[35].Doors.Add(DoorId.DaybreakItemShop);
            Screens[35].Doors.Add(DoorId.DaybreakKeyShop);
            Screens[35].Doors.Add(DoorId.DaybreakMeatShop);

            Screens[13].Doors.Add(DoorId.ConflateBar);
            Screens[13].Doors.Add(DoorId.ConflateGuru);
            Screens[13].Doors.Add(DoorId.ConflateHospital);
            Screens[13].Doors.Add(DoorId.ConflateHouse);
            Screens[13].Doors.Add(DoorId.ConflateItemShop);
            Screens[13].Doors.Add(DoorId.ConflateMeatShop);
        }

        public override int GetStartOffset()
        {
            return 0x2C7AA;
        }

        public override int GetEndOffset()
        {
            return 0x2C8B1;
        }

        public override bool CreateSublevels(List<Screen> startScreens, List<Screen> endScreens, List<Screen> candidates, List<Screen> specialScreens, Random random)
        {
            var secondSpecialScreens = GetSecondSpecialScreens(random);
            int count = secondSpecialScreens.Count;
            for (int i = 0; i < count; i++)
            {
                if (random.Next(0, 2) == 0)
                {
                    specialScreens.Add(secondSpecialScreens[0]);
                    secondSpecialScreens.RemoveAt(0);
                }
            }

            bool result = CreateSublevel(startScreens[0], endScreens[0], candidates, specialScreens, 30, 30, random, SubLevel.Id.EarlyBranch);
            if (!result)
            {
                return result;
            }
            result = CreateSublevel(startScreens[1], endScreens[1], candidates, specialScreens, 30, 30, random, SubLevel.Id.MiddleBranch);
            if (!result)
            {
                return result;
            }

            if (specialScreens.Count > 0)
            {
                return false;
            }

            result = CreateSublevel(startScreens[2], endScreens[2], candidates, secondSpecialScreens, 30, 20, random, SubLevel.Id.BattleHelmetWing);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[3], endScreens[3], candidates, secondSpecialScreens, 30, 20, random, SubLevel.Id.EastBranch);
            if (!result)
            {
                return result;
            }

            if (secondSpecialScreens.Count > 0)
            {
                return false;
            }

            result = CreateSublevel(startScreens[4], endScreens[4], candidates, secondSpecialScreens, 0, 10, random, SubLevel.Id.BackFromEastBranch);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[5], endScreens[5], candidates, secondSpecialScreens, 0, 2, random, SubLevel.Id.DropDownWing);
            if (!result)
            {
                return result;
            }

            if (candidates.Count > 8)
            {
                return false;
            }

            if (GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorldsLessWingboots)
            {
                Screens[20].Connect(Direction.Down, 25);
                Screens[25].Connect(Direction.Up, 20);
                bool found = false;

                foreach (var sublevel in SubLevels)
                {
                    foreach (var screen in sublevel.Screens)
                    {
                        if (screen.Number == 20)
                        {
                            sublevel.Screens.Add(Screens[25]);
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public override List<Screen> GetCandidates(Random random)
        {
            var candidates = new List<Screen>();
            candidates.AddRange(Screens.GetRange(1, 6));
            candidates.Add(Screens[8]);
            candidates.AddRange(Screens.GetRange(11, 2));
            candidates.AddRange(Screens.GetRange(16, 3));
            candidates.Add(Screens[21]);
            candidates.Add(Screens[23]);
            candidates.AddRange(Screens.GetRange(26, 3));
            candidates.AddRange(Screens.GetRange(31, 2));
            candidates.Add(Screens[34]);
            candidates.AddRange(Screens.GetRange(37, 2));
            Util.ShuffleList(candidates, 0, candidates.Count - 1, random);
            return candidates;
        }

        public override void GetEnds(ref List<Screen> startScreens, ref List<Screen> endScreens, Random random)
        {
            startScreens = new List<Screen>();
            endScreens = new List<Screen>();

            startScreens.Add(Screens[0]);
            startScreens.Add(Screens[36]);
            startScreens.Add(Screens[9]);
            startScreens.Add(Screens[15]);
            startScreens.Add(Screens[19]);
            startScreens.Add(Screens[24]);

            endScreens.Add(Screens[35]);
            endScreens.Add(Screens[39]);
            endScreens.Add(Screens[13]);
            endScreens.Add(Screens[29]);
            endScreens.Add(Screens[33]);
            endScreens.Add(Screens[7]);
            if (ItemOptions.ShuffleItems != ItemOptions.ItemShuffle.Unchanged)
            {
                endScreens.Add(Screens[22]);
            }

            Util.ShuffleList(endScreens, 3, endScreens.Count - 1, random);
            Util.ShuffleList(endScreens, 2, endScreens.Count - 2, random);
        }

        public override List<Screen> GetSpecialScreens(Random random)
        {
            var specialScreens = new List<Screen>();
            specialScreens.Add(Screens[10]);
            specialScreens.Add(Screens[14]);
            Util.ShuffleList(specialScreens, 0, specialScreens.Count - 1, random);
            return specialScreens;
        }

        public List<Screen> GetSecondSpecialScreens(Random random)
        {
            var specialScreens = new List<Screen>();
            specialScreens.Add(Screens[20]);

            if (GeneralOptions.RandomizeScreens != GeneralOptions.ScreenRandomization.AllWorldsLessWingboots)
            {
                specialScreens.Add(Screens[25]);
            }

            Util.ShuffleList(specialScreens, 0, specialScreens.Count - 1, random);
            return specialScreens;
        }

        public override void SetupScreens()
        {
            if (GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorldsLessWingboots)
            {
                Screens[0].Directions.Add(Direction.Right);
                Screens[0].OpenTilesRight.Add(8);
                Screens[0].OpenTilesRight.Add(9);
                Screens[0].OpenTilesRight.Add(10);
                Screens[1].Directions.Add(Direction.Left);
                Screens[1].Directions.Add(Direction.Right);
                Screens[1].OpenTilesLeft.Add(7);
                Screens[1].OpenTilesLeft.Add(8);
                Screens[1].OpenTilesLeft.Add(9);
                Screens[1].OpenTilesLeft.Add(10);
                Screens[1].OpenTilesRight.Add(6);
                Screens[1].OpenTilesRight.Add(7);
                Screens[1].OpenTilesRight.Add(8);
                Screens[2].Directions.Add(Direction.Left);
                Screens[2].Directions.Add(Direction.Right);
                Screens[2].OpenTilesLeft.Add(5);
                Screens[2].OpenTilesLeft.Add(6);
                Screens[2].OpenTilesLeft.Add(7);
                Screens[2].OpenTilesLeft.Add(8);
                Screens[2].OpenTilesRight.Add(5);
                Screens[2].OpenTilesRight.Add(6);
                Screens[2].OpenTilesRight.Add(7);
                Screens[2].OpenTilesRight.Add(8);
                Screens[3].Directions.Add(Direction.Left);
                Screens[3].Directions.Add(Direction.Right);
                Screens[3].OpenTilesLeft.Add(5);
                Screens[3].OpenTilesLeft.Add(6);
                Screens[3].OpenTilesLeft.Add(7);
                Screens[3].OpenTilesLeft.Add(8);
                Screens[3].OpenTilesRight.Add(4);
                Screens[3].OpenTilesRight.Add(5);
                Screens[3].OpenTilesRight.Add(6);
                Screens[3].OpenTilesRight.Add(7);
                Screens[4].Directions.Add(Direction.Left);
                Screens[4].Directions.Add(Direction.Up);
                Screens[4].OpenTilesLeft.Add(5);
                Screens[4].OpenTilesLeft.Add(6);
                Screens[4].OpenTilesLeft.Add(7);
                Screens[4].OpenTilesLeft.Add(8);
                Screens[4].OpenTilesUp.Add(2);
                Screens[5].Directions.Add(Direction.Right);
                Screens[5].Directions.Add(Direction.Up);
                Screens[5].OpenTilesRight.Add(4);
                Screens[5].OpenTilesRight.Add(5);
                Screens[5].OpenTilesRight.Add(6);
                Screens[5].OpenTilesUp.Add(7);
                Screens[6].Directions.Add(Direction.Left);
                Screens[6].Directions.Add(Direction.Down);
                Screens[6].OpenTilesLeft.Add(4);
                Screens[6].OpenTilesLeft.Add(5);
                Screens[6].OpenTilesLeft.Add(6);
                Screens[6].OpenTilesDown.Add(2);
                Screens[7].Directions.Add(Direction.Right);
                Screens[7].OpenTilesRight.Add(7);
                Screens[7].OpenTilesRight.Add(8);
                Screens[7].OpenTilesRight.Add(9);
                Screens[7].OpenTilesRight.Add(10);
                Screens[8].Directions.Add(Direction.Left);
                Screens[8].Directions.Add(Direction.Right);
                Screens[8].OpenTilesLeft.Add(7);
                Screens[8].OpenTilesLeft.Add(8);
                Screens[8].OpenTilesLeft.Add(9);
                Screens[8].OpenTilesLeft.Add(10);
                Screens[8].OpenTilesRight.Add(6);
                Screens[8].OpenTilesRight.Add(7);
                Screens[8].OpenTilesRight.Add(8);
                Screens[8].OpenTilesRight.Add(9);
                Screens[9].Directions.Add(Direction.Left);
                Screens[9].Directions.Add(Direction.Right);
                Screens[9].OpenTilesLeft.Add(6);
                Screens[9].OpenTilesLeft.Add(7);
                Screens[9].OpenTilesLeft.Add(8);
                Screens[9].OpenTilesLeft.Add(9);
                Screens[9].OpenTilesRight.Add(3);
                Screens[9].OpenTilesRight.Add(4);
                Screens[10].Directions.Add(Direction.Left);
                Screens[10].Directions.Add(Direction.Right);
                Screens[10].OpenTilesLeft.Add(3);
                Screens[10].OpenTilesLeft.Add(4);
                Screens[10].OpenTilesRight.Add(4);
                Screens[10].OpenTilesRight.Add(5);
                Screens[10].OpenTilesRight.Add(6);
                Screens[11].Directions.Add(Direction.Left);
                Screens[11].Directions.Add(Direction.Right);
                Screens[11].Directions.Add(Direction.Down);
                Screens[11].OpenTilesLeft.Add(4);
                Screens[11].OpenTilesLeft.Add(5);
                Screens[11].OpenTilesLeft.Add(6);
                Screens[11].OpenTilesRight.Add(4);
                Screens[11].OpenTilesRight.Add(5);
                Screens[11].OpenTilesRight.Add(6);
                Screens[11].OpenTilesDown.Add(7);
                Screens[12].Directions.Add(Direction.Left);
                Screens[12].Directions.Add(Direction.Right);
                Screens[12].Directions.Add(Direction.Up);
                Screens[12].OpenTilesLeft.Add(4);
                Screens[12].OpenTilesLeft.Add(5);
                Screens[12].OpenTilesLeft.Add(6);
                Screens[12].OpenTilesRight.Add(4);
                Screens[12].OpenTilesRight.Add(5);
                Screens[12].OpenTilesRight.Add(6);
                Screens[12].OpenTilesRight.Add(7);
                Screens[12].OpenTilesUp.Add(8);
                Screens[13].Directions.Add(Direction.Left);
                Screens[13].OpenTilesLeft.Add(4);
                Screens[13].OpenTilesLeft.Add(5);
                Screens[13].OpenTilesLeft.Add(6);
                Screens[13].OpenTilesLeft.Add(7);
                Screens[14].Directions.Add(Direction.Right);
                Screens[14].Directions.Add(Direction.Down);
                Screens[14].OpenTilesRight.Add(6);
                Screens[14].OpenTilesRight.Add(7);
                Screens[14].OpenTilesDown.Add(8);
                Screens[15].Directions.Add(Direction.Left);
                Screens[15].Directions.Add(Direction.Right);
                Screens[15].OpenTilesLeft.Add(6);
                Screens[15].OpenTilesLeft.Add(7);
                Screens[15].OpenTilesRight.Add(6);
                Screens[15].OpenTilesRight.Add(7);
                Screens[15].OpenTilesRight.Add(8);
                Screens[15].OpenTilesRight.Add(9);
                Screens[16].Directions.Add(Direction.Left);
                Screens[16].Directions.Add(Direction.Right);
                Screens[16].OpenTilesLeft.Add(6);
                Screens[16].OpenTilesLeft.Add(7);
                Screens[16].OpenTilesLeft.Add(8);
                Screens[16].OpenTilesLeft.Add(9);
                Screens[16].OpenTilesRight.Add(2);
                Screens[16].OpenTilesRight.Add(3);
                Screens[16].OpenTilesRight.Add(4);
                Screens[16].OpenTilesRight.Add(6);
                Screens[16].OpenTilesRight.Add(7);
                Screens[16].OpenTilesRight.Add(8);
                Screens[16].OpenTilesRight.Add(9);
                Screens[17].Directions.Add(Direction.Left);
                Screens[17].Directions.Add(Direction.Right);
                Screens[17].OpenTilesLeft.Add(6);
                Screens[17].OpenTilesLeft.Add(7);
                Screens[17].OpenTilesLeft.Add(8);
                Screens[17].OpenTilesRight.Add(3);
                Screens[17].OpenTilesRight.Add(4);
                Screens[17].OpenTilesRight.Add(5);
                Screens[18].Directions.Add(Direction.Left);
                Screens[18].Directions.Add(Direction.Right);
                Screens[18].OpenTilesLeft.Add(3);
                Screens[18].OpenTilesLeft.Add(4);
                Screens[18].OpenTilesLeft.Add(5);
                Screens[18].OpenTilesRight.Add(8);
                Screens[18].OpenTilesRight.Add(9);
                Screens[18].OpenTilesRight.Add(10);
                Screens[18].OpenTilesRight.Add(11);
                Screens[19].Directions.Add(Direction.Left);
                Screens[19].Directions.Add(Direction.Right);
                Screens[19].OpenTilesLeft.Add(8);
                Screens[19].OpenTilesLeft.Add(9);
                Screens[19].OpenTilesLeft.Add(10);
                Screens[19].OpenTilesLeft.Add(11);
                Screens[19].OpenTilesRight.Add(7);
                Screens[19].OpenTilesRight.Add(8);
                Screens[19].OpenTilesRight.Add(9);
                Screens[20].Directions.Add(Direction.Left);
                Screens[20].Directions.Add(Direction.Right);
                Screens[20].OpenTilesLeft.Add(7);
                Screens[20].OpenTilesLeft.Add(8);
                Screens[20].OpenTilesLeft.Add(9);
                Screens[20].OpenTilesRight.Add(3);
                Screens[20].OpenTilesRight.Add(4);
                Screens[20].OpenTilesRight.Add(5);
                Screens[20].OpenTilesRight.Add(6);
                Screens[21].Directions.Add(Direction.Left);
                Screens[21].Directions.Add(Direction.Up);
                Screens[21].Directions.Add(Direction.Down);
                Screens[21].OpenTilesLeft.Add(3);
                Screens[21].OpenTilesLeft.Add(4);
                Screens[21].OpenTilesLeft.Add(5);
                Screens[21].OpenTilesLeft.Add(6);
                Screens[21].OpenTilesUp.Add(5);
                Screens[21].OpenTilesDown.Add(7);
                Screens[22].Directions.Add(Direction.Right);
                Screens[22].OpenTilesRight.Add(8);
                Screens[22].OpenTilesRight.Add(9);
                Screens[22].OpenTilesRight.Add(10);
                Screens[22].OpenTilesRight.Add(11);
                Screens[23].Directions.Add(Direction.Left);
                Screens[23].Directions.Add(Direction.Right);
                Screens[23].OpenTilesLeft.Add(0);
                Screens[23].OpenTilesLeft.Add(1);
                Screens[23].OpenTilesLeft.Add(2);
                Screens[23].OpenTilesLeft.Add(3);
                Screens[23].OpenTilesRight.Add(1);
                Screens[23].OpenTilesRight.Add(2);
                Screens[23].OpenTilesRight.Add(3);
                Screens[23].OpenTilesRight.Add(4);
                Screens[24].Directions.Add(Direction.Left);
                Screens[24].Directions.Add(Direction.Right);
                Screens[24].OpenTilesLeft.Add(6);
                Screens[24].OpenTilesLeft.Add(7);
                Screens[24].OpenTilesLeft.Add(8);
                Screens[24].OpenTilesRight.Add(6);
                Screens[24].OpenTilesRight.Add(7);
                Screens[24].OpenTilesRight.Add(8);
                Screens[24].OpenTilesRight.Add(9);
                Screens[25].Directions.Add(Direction.Left);
                Screens[25].Directions.Add(Direction.Up);
                Screens[25].OpenTilesLeft.Add(6);
                Screens[25].OpenTilesLeft.Add(7);
                Screens[25].OpenTilesLeft.Add(8);
                Screens[25].OpenTilesLeft.Add(9);
                Screens[25].OpenTilesUp.Add(13);
                Screens[26].Directions.Add(Direction.Right);
                Screens[26].Directions.Add(Direction.Up);
                Screens[26].OpenTilesRight.Add(5);
                Screens[26].OpenTilesRight.Add(6);
                Screens[26].OpenTilesRight.Add(7);
                Screens[26].OpenTilesRight.Add(8);
                Screens[26].OpenTilesUp.Add(7);
                Screens[27].Directions.Add(Direction.Left);
                Screens[27].Directions.Add(Direction.Right);
                Screens[27].OpenTilesLeft.Add(5);
                Screens[27].OpenTilesLeft.Add(6);
                Screens[27].OpenTilesLeft.Add(7);
                Screens[27].OpenTilesLeft.Add(8);
                Screens[27].OpenTilesRight.Add(3);
                Screens[27].OpenTilesRight.Add(4);
                Screens[27].OpenTilesRight.Add(5);
                Screens[27].OpenTilesRight.Add(6);
                Screens[27].OpenTilesRight.Add(8);
                Screens[27].OpenTilesRight.Add(9);
                Screens[27].OpenTilesRight.Add(10);
                Screens[27].OpenTilesRight.Add(11);
                Screens[28].Directions.Add(Direction.Left);
                Screens[28].Directions.Add(Direction.Right);
                Screens[28].OpenTilesLeft.Add(8);
                Screens[28].OpenTilesLeft.Add(9);
                Screens[28].OpenTilesLeft.Add(10);
                Screens[28].OpenTilesLeft.Add(11);
                Screens[28].OpenTilesRight.Add(8);
                Screens[28].OpenTilesRight.Add(9);
                Screens[28].OpenTilesRight.Add(10);
                Screens[29].Directions.Add(Direction.Left);
                Screens[29].OpenTilesLeft.Add(3);
                Screens[29].OpenTilesLeft.Add(4);
                Screens[29].OpenTilesLeft.Add(5);
                Screens[29].OpenTilesLeft.Add(6);
                Screens[29].OpenTilesLeft.Add(8);
                Screens[29].OpenTilesLeft.Add(9);
                Screens[29].OpenTilesLeft.Add(10);
                Screens[31].Directions.Add(Direction.Right);
                Screens[31].Directions.Add(Direction.Down);
                Screens[31].OpenTilesRight.Add(2);
                Screens[31].OpenTilesRight.Add(3);
                Screens[31].OpenTilesRight.Add(4);
                Screens[31].OpenTilesRight.Add(7);
                Screens[31].OpenTilesRight.Add(8);
                Screens[31].OpenTilesRight.Add(9);
                Screens[31].OpenTilesRight.Add(10);
                Screens[31].OpenTilesDown.Add(5);
                Screens[32].Directions.Add(Direction.Left);
                Screens[32].Directions.Add(Direction.Right);
                Screens[32].OpenTilesLeft.Add(7);
                Screens[32].OpenTilesLeft.Add(8);
                Screens[32].OpenTilesLeft.Add(9);
                Screens[32].OpenTilesLeft.Add(10);
                Screens[32].OpenTilesRight.Add(5);
                Screens[32].OpenTilesRight.Add(6);
                Screens[32].OpenTilesRight.Add(7);
                Screens[32].OpenTilesRight.Add(8);
                Screens[33].Directions.Add(Direction.Left);
                Screens[33].OpenTilesLeft.Add(1);
                Screens[33].OpenTilesLeft.Add(2);
                Screens[33].OpenTilesLeft.Add(3);
                Screens[34].Directions.Add(Direction.Right);
                Screens[34].Directions.Add(Direction.Down);
                Screens[34].OpenTilesRight.Add(2);
                Screens[34].OpenTilesRight.Add(3);
                Screens[34].OpenTilesRight.Add(4);
                Screens[34].OpenTilesRight.Add(7);
                Screens[34].OpenTilesRight.Add(8);
                Screens[34].OpenTilesRight.Add(9);
                Screens[34].OpenTilesRight.Add(10);
                Screens[34].OpenTilesDown.Add(12);
                Screens[35].Directions.Add(Direction.Left);
                Screens[35].OpenTilesLeft.Add(2);
                Screens[35].OpenTilesLeft.Add(3);
                Screens[35].OpenTilesLeft.Add(4);
                Screens[36].Directions.Add(Direction.Right);
                Screens[36].OpenTilesRight.Add(6);
                Screens[36].OpenTilesRight.Add(7);
                Screens[36].OpenTilesRight.Add(8);
                Screens[36].OpenTilesRight.Add(9);
                Screens[37].Directions.Add(Direction.Left);
                Screens[37].Directions.Add(Direction.Right);
                Screens[37].OpenTilesLeft.Add(6);
                Screens[37].OpenTilesLeft.Add(7);
                Screens[37].OpenTilesLeft.Add(8);
                Screens[37].OpenTilesLeft.Add(9);
                Screens[37].OpenTilesRight.Add(6);
                Screens[37].OpenTilesRight.Add(7);
                Screens[37].OpenTilesRight.Add(8);
                Screens[37].OpenTilesRight.Add(9);
                Screens[38].Directions.Add(Direction.Left);
                Screens[38].Directions.Add(Direction.Right);
                Screens[38].OpenTilesLeft.Add(6);
                Screens[38].OpenTilesLeft.Add(7);
                Screens[38].OpenTilesLeft.Add(8);
                Screens[38].OpenTilesLeft.Add(9);
                Screens[38].OpenTilesRight.Add(4);
                Screens[38].OpenTilesRight.Add(5);
                Screens[38].OpenTilesRight.Add(6);
                Screens[38].OpenTilesRight.Add(7);
                Screens[39].Directions.Add(Direction.Left);
                Screens[39].OpenTilesLeft.Add(4);
                Screens[39].OpenTilesLeft.Add(5);
                Screens[39].OpenTilesLeft.Add(6);
                Screens[39].OpenTilesLeft.Add(7);
            }
            else
            {
                Screens[0].Directions.Add(Direction.Right);
                Screens[0].OpenTilesRight.Add(4);
                Screens[0].OpenTilesRight.Add(5);
                Screens[0].OpenTilesRight.Add(6);
                Screens[0].OpenTilesRight.Add(8);
                Screens[0].OpenTilesRight.Add(9);
                Screens[0].OpenTilesRight.Add(10);
                Screens[1].Directions.Add(Direction.Left);
                Screens[1].Directions.Add(Direction.Right);
                Screens[1].OpenTilesLeft.Add(4);
                Screens[1].OpenTilesLeft.Add(5);
                Screens[1].OpenTilesLeft.Add(7);
                Screens[1].OpenTilesLeft.Add(8);
                Screens[1].OpenTilesLeft.Add(9);
                Screens[1].OpenTilesLeft.Add(10);
                Screens[1].OpenTilesRight.Add(6);
                Screens[1].OpenTilesRight.Add(7);
                Screens[1].OpenTilesRight.Add(8);
                Screens[2].Directions.Add(Direction.Left);
                Screens[2].Directions.Add(Direction.Right);
                Screens[2].OpenTilesLeft.Add(5);
                Screens[2].OpenTilesLeft.Add(6);
                Screens[2].OpenTilesLeft.Add(7);
                Screens[2].OpenTilesLeft.Add(8);
                Screens[2].OpenTilesRight.Add(5);
                Screens[2].OpenTilesRight.Add(6);
                Screens[2].OpenTilesRight.Add(7);
                Screens[2].OpenTilesRight.Add(8);
                Screens[3].Directions.Add(Direction.Left);
                Screens[3].Directions.Add(Direction.Right);
                Screens[3].OpenTilesLeft.Add(5);
                Screens[3].OpenTilesLeft.Add(6);
                Screens[3].OpenTilesLeft.Add(7);
                Screens[3].OpenTilesLeft.Add(8);
                Screens[3].OpenTilesRight.Add(4);
                Screens[3].OpenTilesRight.Add(5);
                Screens[3].OpenTilesRight.Add(6);
                Screens[3].OpenTilesRight.Add(7);
                Screens[4].Directions.Add(Direction.Left);
                Screens[4].Directions.Add(Direction.Up);
                Screens[4].OpenTilesLeft.Add(4);
                Screens[4].OpenTilesLeft.Add(5);
                Screens[4].OpenTilesLeft.Add(6);
                Screens[4].OpenTilesLeft.Add(7);
                Screens[4].OpenTilesUp.Add(2);
                Screens[5].Directions.Add(Direction.Right);
                Screens[5].Directions.Add(Direction.Up);
                Screens[5].OpenTilesRight.Add(4);
                Screens[5].OpenTilesRight.Add(5);
                Screens[5].OpenTilesRight.Add(6);
                Screens[5].OpenTilesUp.Add(7);
                Screens[5].OpenTilesUp.Add(8);
                Screens[6].Directions.Add(Direction.Left);
                Screens[6].Directions.Add(Direction.Down);
                Screens[6].OpenTilesLeft.Add(4);
                Screens[6].OpenTilesLeft.Add(5);
                Screens[6].OpenTilesLeft.Add(6);
                Screens[6].OpenTilesDown.Add(2);
                Screens[7].Directions.Add(Direction.Right);
                for (byte i = 1; i < 11; i++)
                {
                    Screens[7].OpenTilesRight.Add(i);
                    Screens[8].OpenTilesLeft.Add(i);
                    Screens[8].OpenTilesRight.Add(i);
                    Screens[9].OpenTilesLeft.Add(i);
                }
                Screens[8].Directions.Add(Direction.Left);
                Screens[8].Directions.Add(Direction.Right);
                Screens[8].OpenTilesRight.Remove(11);
                Screens[9].OpenTilesLeft.Remove(11);
                Screens[9].Directions.Add(Direction.Left);
                Screens[9].Directions.Add(Direction.Right);
                Screens[9].OpenTilesRight.Add(3);
                Screens[9].OpenTilesRight.Add(4);
                Screens[10].Directions.Add(Direction.Left);
                Screens[10].Directions.Add(Direction.Right);
                Screens[10].OpenTilesLeft.Add(3);
                Screens[10].OpenTilesLeft.Add(4);
                Screens[10].OpenTilesRight.Add(4);
                Screens[10].OpenTilesRight.Add(5);
                Screens[10].OpenTilesRight.Add(6);
                Screens[11].Directions.Add(Direction.Left);
                Screens[11].Directions.Add(Direction.Right);
                Screens[11].Directions.Add(Direction.Down);
                Screens[11].OpenTilesLeft.Add(4);
                Screens[11].OpenTilesLeft.Add(5);
                Screens[11].OpenTilesLeft.Add(6);
                Screens[11].OpenTilesRight.Add(4);
                Screens[11].OpenTilesRight.Add(5);
                Screens[11].OpenTilesRight.Add(6);
                Screens[11].OpenTilesDown.Add(7);
                Screens[12].Directions.Add(Direction.Left);
                Screens[12].Directions.Add(Direction.Right);
                Screens[12].Directions.Add(Direction.Up);
                Screens[12].OpenTilesLeft.Add(4);
                Screens[12].OpenTilesLeft.Add(5);
                Screens[12].OpenTilesLeft.Add(6);
                Screens[12].OpenTilesRight.Add(4);
                Screens[12].OpenTilesRight.Add(5);
                Screens[12].OpenTilesRight.Add(6);
                Screens[12].OpenTilesRight.Add(7);
                Screens[12].OpenTilesUp.Add(6);
                Screens[12].OpenTilesUp.Add(7);
                Screens[12].OpenTilesUp.Add(8);
                Screens[13].Directions.Add(Direction.Left);
                Screens[13].OpenTilesLeft.Add(4);
                Screens[13].OpenTilesLeft.Add(5);
                Screens[13].OpenTilesLeft.Add(6);
                Screens[13].OpenTilesLeft.Add(7);
                Screens[14].Directions.Add(Direction.Right);
                Screens[14].Directions.Add(Direction.Down);
                Screens[14].OpenTilesRight.Add(6);
                Screens[14].OpenTilesRight.Add(7);
                Screens[14].OpenTilesDown.Add(5);
                Screens[14].OpenTilesDown.Add(6);
                Screens[14].OpenTilesDown.Add(7);
                Screens[14].OpenTilesDown.Add(8);
                Screens[15].Directions.Add(Direction.Left);
                Screens[15].Directions.Add(Direction.Right);
                Screens[15].OpenTilesLeft.Add(6);
                Screens[15].OpenTilesLeft.Add(7);
                Screens[15].OpenTilesRight.Add(5);
                Screens[15].OpenTilesRight.Add(6);
                Screens[15].OpenTilesRight.Add(7);
                Screens[15].OpenTilesRight.Add(8);
                Screens[15].OpenTilesRight.Add(9);
                Screens[16].Directions.Add(Direction.Left);
                Screens[16].Directions.Add(Direction.Right);
                Screens[16].OpenTilesLeft.Add(5);
                Screens[16].OpenTilesLeft.Add(6);
                Screens[16].OpenTilesLeft.Add(7);
                Screens[16].OpenTilesLeft.Add(8);
                Screens[16].OpenTilesLeft.Add(9);
                Screens[16].OpenTilesRight.Add(2);
                Screens[16].OpenTilesRight.Add(3);
                Screens[16].OpenTilesRight.Add(4);
                Screens[16].OpenTilesRight.Add(6);
                Screens[16].OpenTilesRight.Add(7);
                Screens[16].OpenTilesRight.Add(8);
                Screens[16].OpenTilesRight.Add(9);
                Screens[17].Directions.Add(Direction.Left);
                Screens[17].Directions.Add(Direction.Right);
                Screens[17].Directions.Add(Direction.Down);
                Screens[17].OpenTilesLeft.Add(2);
                Screens[17].OpenTilesLeft.Add(3);
                Screens[17].OpenTilesLeft.Add(4);
                Screens[17].OpenTilesLeft.Add(6);
                Screens[17].OpenTilesLeft.Add(7);
                Screens[17].OpenTilesLeft.Add(8);
                Screens[17].OpenTilesLeft.Add(11);
                Screens[17].OpenTilesLeft.Add(12);
                for (byte i = 0; i < 13; i++)
                {
                    Screens[17].OpenTilesRight.Add(i);
                    Screens[17].OpenTilesUp.Add(i);
                    Screens[17].OpenTilesDown.Add(i);
                }
                Screens[17].OpenTilesRight.Remove(2);
                Screens[17].OpenTilesRight.Remove(6);
                Screens[17].OpenTilesUp.Remove(0);
                Screens[17].OpenTilesUp.Remove(1);
                Screens[17].OpenTilesUp.Remove(2);
                Screens[17].OpenTilesUp.Remove(3);
                Screens[17].OpenTilesUp.Add(13);
                Screens[17].OpenTilesUp.Add(14);
                Screens[17].OpenTilesUp.Add(15);
                Screens[17].OpenTilesDown.Add(13);
                Screens[17].OpenTilesDown.Add(14);
                Screens[17].OpenTilesDown.Add(15);
                Screens[18].Directions.Add(Direction.Left);
                Screens[18].Directions.Add(Direction.Right);
                Screens[18].OpenTilesLeft.Add(3);
                Screens[18].OpenTilesLeft.Add(4);
                Screens[18].OpenTilesLeft.Add(5);
                Screens[18].OpenTilesRight.Add(6);
                Screens[18].OpenTilesRight.Add(7);
                Screens[18].OpenTilesRight.Add(8);
                Screens[18].OpenTilesRight.Add(9);
                Screens[18].OpenTilesRight.Add(10);
                Screens[18].OpenTilesRight.Add(11);
                Screens[19].Directions.Add(Direction.Left);
                Screens[19].Directions.Add(Direction.Right);
                Screens[19].OpenTilesLeft.Add(7);
                Screens[19].OpenTilesLeft.Add(8);
                Screens[19].OpenTilesLeft.Add(9);
                Screens[19].OpenTilesLeft.Add(10);
                Screens[19].OpenTilesLeft.Add(11);
                Screens[19].OpenTilesRight.Add(7);
                Screens[19].OpenTilesRight.Add(8);
                Screens[19].OpenTilesRight.Add(9);
                Screens[20].Directions.Add(Direction.Left);
                Screens[20].Directions.Add(Direction.Right);
                Screens[20].Directions.Add(Direction.Down);
                Screens[20].OpenTilesLeft.Add(7);
                Screens[20].OpenTilesLeft.Add(8);
                Screens[20].OpenTilesLeft.Add(9);
                Screens[20].OpenTilesRight.Add(2);
                Screens[20].OpenTilesRight.Add(3);
                Screens[20].OpenTilesRight.Add(4);
                Screens[20].OpenTilesRight.Add(5);
                Screens[20].OpenTilesRight.Add(6);
                Screens[20].OpenTilesRight.Add(8);
                Screens[20].OpenTilesRight.Add(9);
                Screens[21].Directions.Add(Direction.Left);
                Screens[21].Directions.Add(Direction.Up);
                Screens[21].Directions.Add(Direction.Down);
                for (byte i = 1; i < 12; i++)
                {
                    Screens[17].OpenTilesUp.Add(i);
                }
                Screens[21].OpenTilesLeft.Add(2);
                Screens[21].OpenTilesLeft.Add(3);
                Screens[21].OpenTilesLeft.Add(4);
                Screens[21].OpenTilesLeft.Add(5);
                Screens[21].OpenTilesLeft.Add(7);
                Screens[21].OpenTilesLeft.Add(8);
                Screens[21].OpenTilesLeft.Add(9);
                Screens[21].OpenTilesDown.Add(5);
                Screens[21].OpenTilesDown.Add(6);
                Screens[21].OpenTilesDown.Add(7);
                Screens[22].Directions.Add(Direction.Right);
                Screens[22].OpenTilesRight.Add(5);
                Screens[22].OpenTilesRight.Add(6);
                Screens[22].OpenTilesRight.Add(7);
                Screens[22].OpenTilesRight.Add(8);
                Screens[22].OpenTilesRight.Add(9);
                Screens[22].OpenTilesRight.Add(10);
                Screens[22].OpenTilesRight.Add(11);
                Screens[23].Directions.Add(Direction.Left);
                Screens[23].Directions.Add(Direction.Right);
                Screens[23].OpenTilesLeft.Add(0);
                Screens[23].OpenTilesLeft.Add(1);
                Screens[23].OpenTilesLeft.Add(2);
                Screens[23].OpenTilesLeft.Add(3);
                Screens[23].OpenTilesRight.Add(0);
                Screens[23].OpenTilesRight.Add(1);
                Screens[23].OpenTilesRight.Add(2);
                Screens[23].OpenTilesRight.Add(3);
                Screens[23].OpenTilesRight.Add(4);
                Screens[24].Directions.Add(Direction.Left);
                Screens[24].Directions.Add(Direction.Right);
                Screens[24].Directions.Add(Direction.Up);
                for (byte i = 0; i < 10; i++)
                {
                    Screens[24].OpenTilesLeft.Add(i);
                    Screens[24].OpenTilesRight.Add(i);
                    Screens[24].OpenTilesUp.Add(i);
                }
                Screens[24].OpenTilesLeft.Remove(5);
                Screens[24].OpenTilesLeft.Remove(9);
                Screens[24].OpenTilesRight.Remove(5);
                Screens[24].OpenTilesUp.Add(10);
                Screens[24].OpenTilesUp.Add(11);
                Screens[24].OpenTilesUp.Add(12);
                Screens[24].OpenTilesUp.Add(13);
                Screens[24].OpenTilesUp.Add(14);
                Screens[24].OpenTilesUp.Add(15);
                Screens[25].Directions.Add(Direction.Left);
                Screens[25].Directions.Add(Direction.Up);
                Screens[25].OpenTilesLeft.Add(6);
                Screens[25].OpenTilesLeft.Add(7);
                Screens[25].OpenTilesLeft.Add(8);
                Screens[25].OpenTilesLeft.Add(9);
                Screens[25].OpenTilesUp.Add(8);
                Screens[25].OpenTilesUp.Add(9);
                Screens[25].OpenTilesUp.Add(10);
                Screens[25].OpenTilesUp.Add(11);
                Screens[25].OpenTilesUp.Add(12);
                Screens[25].OpenTilesUp.Add(13);
                Screens[26].Directions.Add(Direction.Up);
                Screens[26].Directions.Add(Direction.Right);
                Screens[26].OpenTilesRight.Add(5);
                Screens[26].OpenTilesRight.Add(6);
                Screens[26].OpenTilesRight.Add(7);
                Screens[26].OpenTilesRight.Add(8);
                Screens[26].OpenTilesUp.Add(4);
                Screens[26].OpenTilesUp.Add(5);
                Screens[26].OpenTilesUp.Add(6);
                Screens[26].OpenTilesUp.Add(7);
                Screens[27].Directions.Add(Direction.Left);
                Screens[27].Directions.Add(Direction.Right);
                Screens[27].OpenTilesLeft.Add(5);
                Screens[27].OpenTilesLeft.Add(6);
                Screens[27].OpenTilesLeft.Add(7);
                Screens[27].OpenTilesLeft.Add(8);
                Screens[27].OpenTilesRight.Add(3);
                Screens[27].OpenTilesRight.Add(4);
                Screens[27].OpenTilesRight.Add(5);
                Screens[27].OpenTilesRight.Add(6);
                Screens[27].OpenTilesRight.Add(8);
                Screens[27].OpenTilesRight.Add(9);
                Screens[27].OpenTilesRight.Add(10);
                Screens[27].OpenTilesRight.Add(11);
                Screens[28].Directions.Add(Direction.Left);
                Screens[28].Directions.Add(Direction.Right);
                Screens[28].Directions.Add(Direction.Up);
                Screens[28].OpenTilesLeft.Add(2);
                Screens[28].OpenTilesLeft.Add(3);
                Screens[28].OpenTilesLeft.Add(4);
                Screens[28].OpenTilesLeft.Add(5);
                Screens[28].OpenTilesLeft.Add(6);
                Screens[28].OpenTilesLeft.Add(8);
                Screens[28].OpenTilesLeft.Add(9);
                Screens[28].OpenTilesLeft.Add(10);
                Screens[28].OpenTilesLeft.Add(11);
                for (byte i = 1; i < 11; i++)
                {
                    Screens[28].OpenTilesRight.Add(i);
                    Screens[29].OpenTilesLeft.Add(i);
                }
                for (byte i = 2; i < 16; i++)
                {
                    Screens[28].OpenTilesUp.Add(i);
                }
                Screens[28].OpenTilesRight.Add(0);
                Screens[28].OpenTilesRight.Remove(7);
                Screens[29].OpenTilesLeft.Remove(7);
                Screens[29].Directions.Add(Direction.Left);
                Screens[30].Directions.Add(Direction.Up); //Double item screen
                Screens[30].Directions.Add(Direction.Down);
                Screens[31].Directions.Add(Direction.Right);
                Screens[31].Directions.Add(Direction.Down);
                Screens[31].OpenTilesRight.Add(2);
                Screens[31].OpenTilesRight.Add(3);
                Screens[31].OpenTilesRight.Add(4);
                Screens[31].OpenTilesRight.Add(6);
                Screens[31].OpenTilesRight.Add(7);
                Screens[31].OpenTilesRight.Add(8);
                Screens[31].OpenTilesRight.Add(9);
                Screens[31].OpenTilesRight.Add(10);
                Screens[31].OpenTilesDown.Add(5);
                Screens[32].Directions.Add(Direction.Left);
                Screens[32].Directions.Add(Direction.Right);
                Screens[32].OpenTilesLeft.Add(2);
                Screens[32].OpenTilesLeft.Add(3);
                Screens[32].OpenTilesLeft.Add(4);
                Screens[32].OpenTilesLeft.Add(6);
                Screens[32].OpenTilesLeft.Add(7);
                Screens[32].OpenTilesLeft.Add(8);
                Screens[32].OpenTilesLeft.Add(9);
                Screens[32].OpenTilesLeft.Add(10);
                Screens[32].OpenTilesRight.Add(1);
                Screens[32].OpenTilesRight.Add(2);
                Screens[32].OpenTilesRight.Add(3);
                Screens[32].OpenTilesRight.Add(5);
                Screens[32].OpenTilesRight.Add(6);
                Screens[32].OpenTilesRight.Add(7);
                Screens[32].OpenTilesRight.Add(8);
                Screens[33].Directions.Add(Direction.Left);
                Screens[33].OpenTilesLeft.Add(1);
                Screens[33].OpenTilesLeft.Add(2);
                Screens[33].OpenTilesLeft.Add(3);
                Screens[34].Directions.Add(Direction.Right);
                Screens[34].Directions.Add(Direction.Down);
                Screens[34].OpenTilesRight.Add(2);
                Screens[34].OpenTilesRight.Add(3);
                Screens[34].OpenTilesRight.Add(4);
                Screens[34].OpenTilesRight.Add(6);
                Screens[34].OpenTilesRight.Add(7);
                Screens[34].OpenTilesRight.Add(8);
                Screens[34].OpenTilesRight.Add(9);
                Screens[34].OpenTilesRight.Add(10);
                Screens[34].OpenTilesDown.Add(12);
                Screens[35].Directions.Add(Direction.Left);
                Screens[35].OpenTilesLeft.Add(2);
                Screens[35].OpenTilesLeft.Add(3);
                Screens[35].OpenTilesLeft.Add(4);
                Screens[36].Directions.Add(Direction.Right);
                Screens[36].OpenTilesRight.Add(3);
                Screens[36].OpenTilesRight.Add(4);
                Screens[36].OpenTilesRight.Add(5);
                Screens[36].OpenTilesRight.Add(6);
                Screens[36].OpenTilesRight.Add(7);
                Screens[36].OpenTilesRight.Add(8);
                Screens[36].OpenTilesRight.Add(9);
                Screens[37].Directions.Add(Direction.Left);
                Screens[37].Directions.Add(Direction.Right);
                for (byte i = 2; i < 10; i++)
                {
                    Screens[37].OpenTilesLeft.Add(i);
                    Screens[37].OpenTilesRight.Add(i);
                    Screens[38].OpenTilesLeft.Add(i);
                    Screens[38].OpenTilesRight.Add(i);
                }
                Screens[38].Directions.Add(Direction.Left);
                Screens[38].Directions.Add(Direction.Right);
                Screens[38].OpenTilesRight.Remove(8);
                Screens[38].OpenTilesRight.Remove(9);
                Screens[39].Directions.Add(Direction.Left);
                Screens[39].OpenTilesLeft.Add(2);
                Screens[39].OpenTilesLeft.Add(3);
                Screens[39].OpenTilesLeft.Add(4);
                Screens[39].OpenTilesLeft.Add(5);
                Screens[39].OpenTilesLeft.Add(6);
                Screens[39].OpenTilesLeft.Add(7);

            }


        }
    }
}
