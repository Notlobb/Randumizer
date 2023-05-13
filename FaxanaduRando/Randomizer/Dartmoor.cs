using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    internal class Dartmoor : Level
    {
        public const byte DartmoorEntrance = 0;
        public const byte FraternalScreen = 12;
        public const byte FraternalEntranceScreen = 16;
        public const byte DartmoorCityLeftScreen = 3;
        public const byte DartmoorExitScreen = 14;
        public const byte GrieveScreen = 20;

        public Dartmoor(WorldNumber number, byte[] content) : base(number, content)
        {
            Screens[DartmoorEntrance].Doors.Add(DoorId.DartmoorEntrance);
            if (GeneralOptions.ShuffleTowers && GeneralOptions.IncludeEvilOnesFortress)
            {
                Screens[2].Doors.Add(DoorId.EvilOnesLair);
            }
            else
            {
                Screens[2].Doors.Add(DoorId.DartmoorHouse1);
            }

            Screens[2].Doors.Add(DoorId.DartmoorHouse2);
            Screens[2].Doors.Add(DoorId.DartmoorHouse3);
            Screens[2].Doors.Add(DoorId.DartmoorHouse4);

            Screens[8].Doors.Add(DoorId.FarLeftDartmoor);
            Screens[FraternalScreen].Doors.Add(DoorId.CastleFraternal);
            Screens[FraternalEntranceScreen].Doors.Add(DoorId.CastleFraternalReturn);
            Screens[DartmoorExitScreen].Doors.Add(DoorId.DartmoorExit);
            Screens[GrieveScreen].Doors.Add(DoorId.KingGrieve);
            Screens[30].Doors.Add(DoorId.FraternalGuru);
            Screens[31].Doors.Add(DoorId.FraternalHouse);

            if (ShouldRandomizeScreens())
            {
                Screens[FraternalScreen].Sprites[1].ShouldBeShuffled = false;
                Screens[FraternalScreen].Sprites[1].Id = Sprite.SpriteId.WingbootsBossLocked;
                Screens[FraternalScreen].Sprites[1].SetY(11);
            }
        }

        public override int GetStartOffset()
        {
            return 0x2C8F2;
        }

        public override int GetEndOffset()
        {
            return 0x2C9CB;
        }

        public override bool CreateSublevels(List<Screen> startScreens, List<Screen> endScreens, List<Screen> candidates, List<Screen> specialScreens, Random random, uint attempts)
        {
            bool result = CreateSublevel(startScreens[0], endScreens[0], candidates, specialScreens, random, SubLevel.Id.Dartmoor, attempts, highProbabilities: true);
            if (!result)
            {
                return result;
            }

            bool addedFirstFork = !specialScreens.Contains(Screens[4]);
            if (addedFirstFork)
            {
                result = CreateSublevel(Screens[1], endScreens[2], candidates, specialScreens, random, Screens[1].ParentSublevel, attempts, false, true);
                if (!result)
                {
                    return result;
                }
            }

            bool addedSecondFork = !specialScreens.Contains(Screens[GrieveScreen]);
            if (addedSecondFork)
            {
                result = CreateSublevel(Screens[26], endScreens[3], candidates, specialScreens, random, Screens[26].ParentSublevel, attempts, false, true);
                if (!result)
                {
                    return result;
                }
            }

            if (specialScreens.Contains(Screens[DartmoorExitScreen]))
            {
                return false;
            }

            result = CreateSublevel(startScreens[1], endScreens[1], candidates, specialScreens, random, SubLevel.Id.CastleFraternal, attempts, highProbabilities: true);
            if (!result)
            {
                return result;
            }

            if (specialScreens.Count > 0)
            {
                return false;
            }

            if (!addedFirstFork)
            {
                result = CreateSublevel(Screens[1], endScreens[2], candidates, specialScreens, random, Screens[1].ParentSublevel, attempts, false, true);
                if (!result)
                {
                    return result;
                }
            }

            if (!addedSecondFork)
            {
                result = CreateSublevel(Screens[26], endScreens[3], candidates, specialScreens, random, Screens[26].ParentSublevel, attempts, false, true);
                if (!result)
                {
                    return result;
                }
            }

            result = CreateSublevel(Screens[6], endScreens[4], candidates, specialScreens, random, Screens[6].ParentSublevel, attempts, false, true);
            if (!result)
            {
                return result;
            }

            if (candidates.Count > 10)
            {
                return false;
            }

            return true;
        }

        public override List<Screen> GetCandidates(Random random)
        {
            var candidates = new List<Screen>();
            candidates.Add(Screens[5]);
            candidates.Add(Screens[7]);
            candidates.AddRange(Screens.GetRange(9, 2));
            candidates.Add(Screens[11]);
            candidates.Add(Screens[13]);
            //candidates.Add(Screens[15]); //unused screen?
            candidates.Add(Screens[17]);
            candidates.Add(Screens[23]);
            candidates.Add(Screens[24]);
            //candidates.Add(Screens[26]);
            candidates.Add(Screens[27]);
            candidates.Add(Screens[29]);

            Util.ShuffleList(candidates, 0, candidates.Count - 1, random);
            return candidates;
        }

        public override void GetEnds(ref List<Screen> startScreens, ref List<Screen> endScreens, Random random)
        {
            startScreens = new List<Screen>();
            endScreens = new List<Screen>();

            startScreens.Add(Screens[DartmoorEntrance]);
            startScreens.Add(Screens[FraternalEntranceScreen]);

            endScreens.Add(Screens[2]);
            endScreens.Add(Screens[8]);
            endScreens.Add(Screens[18]);
            endScreens.Add(Screens[31]);
            endScreens.Add(Screens[19]);

            Util.ShuffleList(endScreens, 0, endScreens.Count - 1, random);
        }

        public override List<Screen> GetSpecialScreens(Random random)
        {
            var specialScreens = new List<Screen>();
            specialScreens.Add(Screens[FraternalScreen]);
            specialScreens.Add(Screens[DartmoorExitScreen]);
            specialScreens.Add(Screens[4]);
            specialScreens.Add(Screens[GrieveScreen]);
            specialScreens.Add(Screens[25]);
            specialScreens.Add(Screens[6]);

            Util.ShuffleList(specialScreens, 0, specialScreens.Count - 1, random);
            return specialScreens;
        }

        public override void SetupScreens()
        {
            startToSpecial[16] = FraternalScreen;

            Screens[GrieveScreen].FriendConnections[Direction.Down] = Screens[26];
            Screens[25].FriendEnds[Direction.Down] = Screens[30];

            Screens[4].FriendConnections[Direction.Up] = Screens[DartmoorCityLeftScreen];
            Screens[DartmoorCityLeftScreen].FriendConnections[Direction.Up] = Screens[1];

            Screens[DartmoorEntrance].Directions.Add(Direction.Left);
            Screens[DartmoorEntrance].Directions.Add(Direction.Right);
            Screens[DartmoorEntrance].OpenTilesLeft.Add(2);
            Screens[DartmoorEntrance].OpenTilesLeft.Add(3);
            Screens[DartmoorEntrance].OpenTilesLeft.Add(4);
            Screens[DartmoorEntrance].OpenTilesLeft.Add(5);
            Screens[DartmoorEntrance].OpenTilesLeft.Add(8);
            Screens[DartmoorEntrance].OpenTilesLeft.Add(9);
            Screens[DartmoorEntrance].OpenTilesLeft.Add(10);
            Screens[DartmoorEntrance].OpenTilesLeft.Add(11);
            Screens[DartmoorEntrance].OpenTilesRight.Add(10);
            Screens[DartmoorEntrance].OpenTilesRight.Add(11);
            Screens[1].Directions.Add(Direction.Left);
            Screens[1].Directions.Add(Direction.Right);
            Screens[1].OpenTilesLeft.Add(2);
            Screens[1].OpenTilesLeft.Add(3);
            Screens[1].OpenTilesLeft.Add(4);
            Screens[1].OpenTilesLeft.Add(5);
            Screens[1].SecondOpenTilesLeft.Add(9);
            Screens[1].SecondOpenTilesLeft.Add(10);
            Screens[1].SecondOpenTilesLeft.Add(11);
            Screens[1].OpenTilesRight.Add(2);
            Screens[1].OpenTilesRight.Add(3);
            Screens[1].OpenTilesRight.Add(4);
            Screens[1].OpenTilesRight.Add(5);
            Screens[2].Directions.Add(Direction.Right);
            Screens[2].OpenTilesRight.Add(2);
            Screens[2].OpenTilesRight.Add(3);
            Screens[2].OpenTilesRight.Add(4);
            Screens[2].OpenTilesRight.Add(5);
            Screens[2].OpenTilesRight.Add(8);
            Screens[2].OpenTilesRight.Add(9);
            Screens[2].OpenTilesRight.Add(10);
            Screens[2].OpenTilesRight.Add(11);
            //Screens[DartmoorCityLeftScreen].Directions.Add(Direction.Up);
            //Screens[DartmoorCityLeftScreen].Directions.Add(Direction.Down);
            //Screens[DartmoorCityLeftScreen].OpenTilesUp.Add(2);
            //Screens[DartmoorCityLeftScreen].OpenTilesDown.Add(5);
            Screens[4].Directions.Add(Direction.Left);
            //Screens[4].Directions.Add(Direction.Up);
            Screens[4].OpenTilesLeft.Add(2);
            Screens[4].OpenTilesLeft.Add(3);
            Screens[4].OpenTilesLeft.Add(4);
            Screens[4].OpenTilesLeft.Add(5);
            Screens[4].OpenTilesUp.Add(5);
            Screens[5].Directions.Add(Direction.Right);
            Screens[5].Directions.Add(Direction.Down);
            Screens[5].OpenTilesRight.Add(2);
            Screens[5].OpenTilesRight.Add(3);
            Screens[5].OpenTilesRight.Add(4);
            Screens[5].OpenTilesRight.Add(5);
            Screens[5].OpenTilesDown.Add(2);
            Screens[6].Directions.Add(Direction.Left);
            Screens[6].Directions.Add(Direction.Right);
            Screens[6].Directions.Add(Direction.Up);
            Screens[6].OpenTilesLeft.Add(2);
            Screens[6].OpenTilesLeft.Add(3);
            Screens[6].OpenTilesLeft.Add(4);
            Screens[6].OpenTilesLeft.Add(5);
            Screens[6].OpenTilesRight.Add(9);
            Screens[6].OpenTilesRight.Add(10);
            Screens[6].OpenTilesRight.Add(11);
            Screens[6].OpenTilesUp.Add(2);
            Screens[7].Directions.Add(Direction.Left);
            Screens[7].Directions.Add(Direction.Right);
            Screens[7].OpenTilesLeft.Add(2);
            Screens[7].OpenTilesLeft.Add(3);
            Screens[7].OpenTilesLeft.Add(4);
            Screens[7].OpenTilesLeft.Add(5);
            Screens[7].OpenTilesRight.Add(4);
            Screens[7].OpenTilesRight.Add(5);
            Screens[8].Directions.Add(Direction.Right);
            Screens[8].OpenTilesRight.Add(2);
            Screens[8].OpenTilesRight.Add(3);
            Screens[8].OpenTilesRight.Add(4);
            Screens[8].OpenTilesRight.Add(5);
            Screens[9].Directions.Add(Direction.Left);
            Screens[9].Directions.Add(Direction.Down);
            Screens[9].OpenTilesLeft.Add(1);
            Screens[9].OpenTilesLeft.Add(2);
            Screens[9].OpenTilesLeft.Add(3);
            Screens[9].OpenTilesDown.Add(13);
            Screens[10].Directions.Add(Direction.Right);
            Screens[10].Directions.Add(Direction.Down);
            Screens[10].OpenTilesRight.Add(1);
            Screens[10].OpenTilesRight.Add(2);
            Screens[10].OpenTilesRight.Add(3);
            Screens[10].OpenTilesDown.Add(2);
            Screens[11].Directions.Add(Direction.Left);
            Screens[11].Directions.Add(Direction.Right);
            Screens[11].OpenTilesLeft.Add(9);
            Screens[11].OpenTilesLeft.Add(10);
            Screens[11].OpenTilesLeft.Add(11);
            Screens[11].OpenTilesRight.Add(4);
            Screens[11].OpenTilesRight.Add(5);
            Screens[11].OpenTilesRight.Add(6);
            Screens[11].OpenTilesRight.Add(9);
            Screens[11].OpenTilesRight.Add(10);
            Screens[11].OpenTilesRight.Add(11);
            Screens[FraternalScreen].Directions.Add(Direction.Left);
            Screens[FraternalScreen].Directions.Add(Direction.Up);
            Screens[FraternalScreen].OpenTilesLeft.Add(6);
            Screens[FraternalScreen].OpenTilesLeft.Add(7);
            Screens[FraternalScreen].OpenTilesLeft.Add(8);
            Screens[FraternalScreen].OpenTilesLeft.Add(9);
            Screens[FraternalScreen].OpenTilesUp.Add(13);
            Screens[13].Directions.Add(Direction.Left);
            Screens[13].Directions.Add(Direction.Right);
            Screens[13].Directions.Add(Direction.Up);
            Screens[13].OpenTilesLeft.Add(2);
            Screens[13].OpenTilesLeft.Add(3);
            //Screens[13].OpenTilesRight.Add(5);
            //Screens[13].OpenTilesRight.Add(6);
            //Screens[13].OpenTilesRight.Add(7);
            //Screens[13].OpenTilesRight.Add(8);
            Screens[13].OpenTilesUp.Add(2);
            Screens[DartmoorExitScreen].Directions.Add(Direction.Left);
            Screens[DartmoorExitScreen].Directions.Add(Direction.Right);
            Screens[DartmoorExitScreen].OpenTilesLeft.Add(3);
            Screens[DartmoorExitScreen].OpenTilesLeft.Add(4);
            Screens[DartmoorExitScreen].OpenTilesLeft.Add(5);
            Screens[DartmoorExitScreen].OpenTilesLeft.Add(6);
            Screens[DartmoorExitScreen].OpenTilesRight.Add(2);
            Screens[DartmoorExitScreen].OpenTilesRight.Add(3);
            Screens[FraternalEntranceScreen].Directions.Add(Direction.Left);
            Screens[FraternalEntranceScreen].Directions.Add(Direction.Right);
            Screens[FraternalEntranceScreen].OpenTilesLeft.Add(7);
            Screens[FraternalEntranceScreen].OpenTilesLeft.Add(8);
            Screens[FraternalEntranceScreen].OpenTilesLeft.Add(9);
            Screens[FraternalEntranceScreen].OpenTilesLeft.Add(10);
            Screens[FraternalEntranceScreen].OpenTilesRight.Add(2);
            Screens[FraternalEntranceScreen].OpenTilesRight.Add(3);
            Screens[17].Directions.Add(Direction.Left);
            Screens[17].Directions.Add(Direction.Down);
            Screens[17].OpenTilesLeft.Add(1);
            Screens[17].OpenTilesLeft.Add(2);
            Screens[17].OpenTilesLeft.Add(3);
            Screens[17].OpenTilesLeft.Add(7);
            Screens[17].OpenTilesLeft.Add(8);
            Screens[17].OpenTilesLeft.Add(9);
            Screens[17].OpenTilesLeft.Add(10);
            Screens[17].OpenTilesDown.Add(7);
            Screens[18].Directions.Add(Direction.Left);
            Screens[18].OpenTilesLeft.Add(1);
            Screens[18].OpenTilesLeft.Add(2);
            Screens[18].OpenTilesLeft.Add(3);
            Screens[19].Directions.Add(Direction.Right);
            //Screens[19].Directions.Add(Direction.Down);
            Screens[19].OpenTilesRight.Add(1);
            Screens[19].OpenTilesRight.Add(2);
            Screens[19].OpenTilesRight.Add(3);
            //Screens[19].OpenTilesDown.Add(9);
            Screens[GrieveScreen].Directions.Add(Direction.Left);
            //Screens[GrieveScreen].Directions.Add(Direction.Down);
            Screens[GrieveScreen].OpenTilesLeft.Add(1);
            Screens[GrieveScreen].OpenTilesLeft.Add(2);
            Screens[GrieveScreen].OpenTilesLeft.Add(3);
            Screens[GrieveScreen].OpenTilesDown.Add(6);
            Screens[23].Directions.Add(Direction.Right);
            Screens[23].Directions.Add(Direction.Up);
            Screens[23].OpenTilesRight.Add(8);
            Screens[23].OpenTilesRight.Add(9);
            Screens[23].OpenTilesRight.Add(10);
            Screens[23].OpenTilesRight.Add(11);
            Screens[23].OpenTilesUp.Add(7);
            Screens[24].Directions.Add(Direction.Left);
            Screens[24].Directions.Add(Direction.Right);
            Screens[24].Directions.Add(Direction.Up);
            Screens[24].OpenTilesLeft.Add(8);
            Screens[24].OpenTilesLeft.Add(9);
            Screens[24].OpenTilesLeft.Add(10);
            Screens[24].OpenTilesLeft.Add(11);
            Screens[24].OpenTilesRight.Add(2);
            Screens[24].OpenTilesRight.Add(3);
            Screens[24].OpenTilesRight.Add(4);
            Screens[24].OpenTilesRight.Add(5);
            Screens[24].OpenTilesRight.Add(9);
            Screens[24].OpenTilesRight.Add(10);
            Screens[24].OpenTilesRight.Add(11);
            Screens[24].OpenTilesUp.Add(2);
            Screens[25].Directions.Add(Direction.Left);
            Screens[25].Directions.Add(Direction.Right);
            Screens[25].OpenTilesLeft.Add(8);
            Screens[25].OpenTilesLeft.Add(9);
            Screens[25].OpenTilesLeft.Add(10);
            Screens[25].OpenTilesLeft.Add(11);
            Screens[25].OpenTilesRight.Add(7);
            Screens[25].OpenTilesRight.Add(8);
            Screens[25].OpenTilesRight.Add(9);
            Screens[25].OpenTilesRight.Add(10);
            Screens[25].OpenTilesDown.Add(14);
            Screens[26].Directions.Add(Direction.Left);
            Screens[26].Directions.Add(Direction.Right);
            //Screens[26].Directions.Add(Direction.Up);
            Screens[26].OpenTilesLeft.Add(2);
            Screens[26].OpenTilesLeft.Add(3);
            Screens[26].OpenTilesLeft.Add(4);
            Screens[26].OpenTilesLeft.Add(5);
            Screens[26].SecondOpenTilesLeft.Add(7);
            Screens[26].SecondOpenTilesLeft.Add(8);
            Screens[26].SecondOpenTilesLeft.Add(9);
            Screens[26].SecondOpenTilesLeft.Add(10);
            Screens[26].OpenTilesRight.Add(2);
            Screens[26].OpenTilesRight.Add(3);
            Screens[26].OpenTilesRight.Add(4);
            Screens[26].OpenTilesRight.Add(5);
            Screens[26].OpenTilesUp.Add(6);
            Screens[27].Directions.Add(Direction.Left);
            Screens[27].Directions.Add(Direction.Right);
            Screens[27].Directions.Add(Direction.Down);
            Screens[27].OpenTilesLeft.Add(2);
            Screens[27].OpenTilesLeft.Add(3);
            Screens[27].OpenTilesLeft.Add(4);
            Screens[27].OpenTilesLeft.Add(5);
            Screens[27].OpenTilesRight.Add(2);
            Screens[27].OpenTilesRight.Add(3);
            Screens[27].OpenTilesRight.Add(4);
            Screens[27].OpenTilesRight.Add(5);
            Screens[27].OpenTilesDown.Add(2);
            Screens[29].Directions.Add(Direction.Left);
            Screens[29].Directions.Add(Direction.Right);
            Screens[29].Directions.Add(Direction.Up);
            Screens[29].OpenTilesLeft.Add(8);
            Screens[29].OpenTilesLeft.Add(9);
            Screens[29].OpenTilesLeft.Add(10);
            Screens[29].OpenTilesLeft.Add(11);
            Screens[29].OpenTilesRight.Add(2);
            Screens[29].OpenTilesRight.Add(3);
            Screens[29].OpenTilesRight.Add(4);
            Screens[29].OpenTilesRight.Add(5);
            Screens[29].OpenTilesUp.Add(7);
            Screens[30].Directions.Add(Direction.Up);
            Screens[30].OpenTilesUp.Add(14);
            Screens[31].Directions.Add(Direction.Up);
            Screens[31].OpenTilesUp.Add(2);
        }
    }
}
