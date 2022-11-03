using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    internal class Dartmoor : Level
    {
        public static byte FraternalScreen = 12;
        public static byte DartmoorExitScreen = 14;
        public static byte GrieveScreen = 20;

        public Dartmoor(WorldNumber number, byte[] content) : base(number, content)
        {
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

            Screens[3].Doors.Add(DoorId.LeftOfDartmoor);
            Screens[3].Doors.Add(DoorId.DartmoorBar);
            Screens[3].Doors.Add(DoorId.DartmoorGuru);
            Screens[3].Doors.Add(DoorId.DartmoorHospital);
            Screens[3].Doors.Add(DoorId.DartmoorItemShop);
            Screens[3].Doors.Add(DoorId.DartmoorKeyShop);
            Screens[3].Doors.Add(DoorId.DartmoorMeatShop);
            Screens[8].Doors.Add(DoorId.FarLeftDartmoor);
            Screens[FraternalScreen].Doors.Add(DoorId.CastleFraternal);
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
            bool result = CreateSublevel(startScreens[0], endScreens[0], candidates, specialScreens, 35, 10, random, SubLevel.Id.Dartmoor, attempts);
            if (!result)
            {
                return result;
            }

            if (specialScreens.Contains(Screens[DartmoorExitScreen]))
            {
                return false;
            }

            result = CreateSublevel(startScreens[1], endScreens[1], candidates, specialScreens, 30, 2, random, SubLevel.Id.CastleFraternal, attempts);
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

            return true;
        }

        public override List<Screen> GetCandidates(Random random)
        {
            var candidates = new List<Screen>();
            candidates.AddRange(Screens.GetRange(5, 3));
            candidates.AddRange(Screens.GetRange(9, 2));
            candidates.Add(Screens[13]);
            //candidates.Add(Screens[15]); //unused screen?
            candidates.Add(Screens[17]);
            candidates.Add(Screens[23]);
            candidates.Add(Screens[27]);
            candidates.Add(Screens[29]);

            if (GeneralOptions.RandomizeScreens != GeneralOptions.ScreenRandomization.AllWorldsLessWingboots)
            {
                candidates.Add(Screens[4]);
                candidates.Add(Screens[19]);
                candidates.Add(Screens[24]);
                candidates.Add(Screens[28]);
            }

            Util.ShuffleList(candidates, 0, candidates.Count - 1, random);
            return candidates;
        }

        public override void GetEnds(ref List<Screen> startScreens, ref List<Screen> endScreens, Random random)
        {
            startScreens = new List<Screen>();
            endScreens = new List<Screen>();

            startScreens.Add(Screens[0]);
            startScreens.Add(Screens[16]);

            endScreens.Add(Screens[18]);
            endScreens.Add(Screens[31]);

            Util.ShuffleList(endScreens, 0, endScreens.Count - 1, random);
        }

        public override List<Screen> GetSpecialScreens(Random random)
        {
            var specialScreens = new List<Screen>();
            specialScreens.Add(Screens[FraternalScreen]);
            specialScreens.Add(Screens[DartmoorExitScreen]);
            specialScreens.Add(Screens[25]);
            specialScreens.Add(Screens[26]);

            if (GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorldsLessWingboots)
            {
                specialScreens.Add(Screens[24]);
            }
            else
            {
                specialScreens.Add(Screens[1]);
                specialScreens.Add(Screens[3]);
            }

            Util.ShuffleList(specialScreens, 0, specialScreens.Count - 1, random);
            return specialScreens;
        }

        public override void SetupScreens()
        {
            startToSpecial[16] = FraternalScreen;

            Screens[1].FriendEnds[Direction.Left] = Screens[2];
            Screens[GrieveScreen].FriendEnds[Direction.Left] = Screens[8];
            Screens[25].FriendEnds[Direction.Down] = Screens[30];
            Screens[26].FriendConnections[Direction.Left] = Screens[11];
            Screens[26].FriendEnds[Direction.Up] = Screens[GrieveScreen];

            if (GeneralOptions.RandomizeScreens == GeneralOptions.ScreenRandomization.AllWorldsLessWingboots)
            {
                Screens[24].FriendConnections[Direction.Right] = Screens[4];
                Screens[4].FriendConnections[Direction.Up] = Screens[3];
                Screens[3].FriendConnections[Direction.Up] = Screens[1];

                Screens[0].Directions.Add(Direction.Left);
                Screens[0].Directions.Add(Direction.Right);
                Screens[0].OpenTilesLeft.Add(2);
                Screens[0].OpenTilesLeft.Add(3);
                Screens[0].OpenTilesLeft.Add(4);
                Screens[0].OpenTilesLeft.Add(5);
                Screens[0].OpenTilesLeft.Add(8);
                Screens[0].OpenTilesLeft.Add(9);
                Screens[0].OpenTilesLeft.Add(10);
                Screens[0].OpenTilesLeft.Add(11);
                Screens[0].OpenTilesRight.Add(10);
                Screens[0].OpenTilesRight.Add(11);
                //Screens[1].Directions.Add(Direction.Left);
                Screens[1].Directions.Add(Direction.Right);
                Screens[1].OpenTilesLeft.Add(8);
                Screens[1].OpenTilesLeft.Add(9);
                Screens[1].OpenTilesLeft.Add(10);
                Screens[1].OpenTilesLeft.Add(11);
                Screens[1].OpenTilesRight.Add(1);
                Screens[1].OpenTilesRight.Add(2);
                Screens[1].OpenTilesRight.Add(3);
                Screens[1].OpenTilesRight.Add(4);
                Screens[2].Directions.Add(Direction.Right);
                Screens[2].OpenTilesRight.Add(2);
                Screens[2].OpenTilesRight.Add(3);
                Screens[2].OpenTilesRight.Add(4);
                Screens[2].OpenTilesRight.Add(5);
                Screens[2].OpenTilesRight.Add(8);
                Screens[2].OpenTilesRight.Add(9);
                Screens[2].OpenTilesRight.Add(10);
                Screens[2].OpenTilesRight.Add(11);
                //Screens[3].Directions.Add(Direction.Up);
                Screens[3].Directions.Add(Direction.Down);
                Screens[3].OpenTilesUp.Add(2);
                Screens[3].OpenTilesDown.Add(5);
                //Screens[4].Directions.Add(Direction.Left);
                Screens[4].Directions.Add(Direction.Up);
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
                //Screens[11].Directions.Add(Direction.Right);
                Screens[11].OpenTilesLeft.Add(9);
                Screens[11].OpenTilesLeft.Add(10);
                Screens[11].OpenTilesLeft.Add(11);
                //Screens[11].OpenTilesRight.Add(4);
                //Screens[11].OpenTilesRight.Add(5);
                //Screens[11].OpenTilesRight.Add(6);
                //Screens[11].OpenTilesRight.Add(9);
                //Screens[11].OpenTilesRight.Add(10);
                //Screens[11].OpenTilesRight.Add(11);
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
                Screens[16].Directions.Add(Direction.Left);
                Screens[16].Directions.Add(Direction.Right);
                Screens[16].OpenTilesLeft.Add(7);
                Screens[16].OpenTilesLeft.Add(8);
                Screens[16].OpenTilesLeft.Add(9);
                Screens[16].OpenTilesLeft.Add(10);
                Screens[16].OpenTilesRight.Add(2);
                Screens[16].OpenTilesRight.Add(3);
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
                Screens[19].Directions.Add(Direction.Down);
                Screens[19].OpenTilesRight.Add(1);
                Screens[19].OpenTilesRight.Add(2);
                Screens[19].OpenTilesRight.Add(3);
                Screens[19].OpenTilesDown.Add(9);
                Screens[GrieveScreen].Directions.Add(Direction.Left);
                //Screens[GrieveScreen].Directions.Add(Direction.Down);
                Screens[GrieveScreen].OpenTilesLeft.Add(1);
                Screens[GrieveScreen].OpenTilesLeft.Add(2);
                Screens[GrieveScreen].OpenTilesLeft.Add(3);
                //Screens[GrieveScreen].OpenTilesDown.Add(6);
                Screens[23].Directions.Add(Direction.Right);
                Screens[23].Directions.Add(Direction.Up);
                Screens[23].OpenTilesRight.Add(8);
                Screens[23].OpenTilesRight.Add(9);
                Screens[23].OpenTilesRight.Add(10);
                Screens[23].OpenTilesRight.Add(11);
                Screens[23].OpenTilesUp.Add(7);
                Screens[24].Directions.Add(Direction.Left);
                //Screens[24].Directions.Add(Direction.Right);
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
                //Screens[26].Directions.Add(Direction.Left);
                Screens[26].Directions.Add(Direction.Right);
                //Screens[26].OpenTilesLeft.Add(7);
                //Screens[26].OpenTilesLeft.Add(8);
                //Screens[26].OpenTilesLeft.Add(9);
                //Screens[26].OpenTilesLeft.Add(10);
                Screens[26].OpenTilesRight.Add(2);
                Screens[26].OpenTilesRight.Add(3);
                Screens[26].OpenTilesRight.Add(4);
                Screens[26].OpenTilesRight.Add(5);
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
            else
            {
                Screens[0].Directions.Add(Direction.Left);
                Screens[0].Directions.Add(Direction.Right);
                Screens[0].OpenTilesLeft.Add(1);
                Screens[0].OpenTilesLeft.Add(2);
                Screens[0].OpenTilesLeft.Add(3);
                Screens[0].OpenTilesLeft.Add(4);
                Screens[0].OpenTilesLeft.Add(5);
                Screens[0].OpenTilesLeft.Add(8);
                Screens[0].OpenTilesLeft.Add(9);
                Screens[0].OpenTilesLeft.Add(10);
                Screens[0].OpenTilesLeft.Add(11);
                Screens[0].OpenTilesRight.Add(10);
                Screens[0].OpenTilesRight.Add(11);
                //Screens[1].Directions.Add(Direction.Left);
                Screens[1].Directions.Add(Direction.Right);
                Screens[1].Directions.Add(Direction.Down);
                Screens[1].OpenTilesLeft.Add(9);
                Screens[1].OpenTilesLeft.Add(10);
                Screens[1].OpenTilesLeft.Add(11);
                Screens[1].OpenTilesRight.Add(1);
                Screens[1].OpenTilesRight.Add(2);
                Screens[1].OpenTilesRight.Add(3);
                Screens[1].OpenTilesRight.Add(4);
                Screens[1].OpenTilesRight.Add(8);
                Screens[1].OpenTilesRight.Add(9);
                Screens[1].OpenTilesRight.Add(10);
                Screens[1].OpenTilesRight.Add(11);
                Screens[1].OpenTilesDown.Add(6);
                Screens[1].OpenTilesDown.Add(7);
                Screens[1].OpenTilesDown.Add(8);
                Screens[1].OpenTilesDown.Add(9);
                Screens[2].Directions.Add(Direction.Right);
                Screens[2].OpenTilesRight.Add(1);
                Screens[2].OpenTilesRight.Add(2);
                Screens[2].OpenTilesRight.Add(3);
                Screens[2].OpenTilesRight.Add(4);
                Screens[2].OpenTilesRight.Add(5);
                Screens[2].OpenTilesRight.Add(8);
                Screens[2].OpenTilesRight.Add(9);
                Screens[2].OpenTilesRight.Add(10);
                Screens[2].OpenTilesRight.Add(11);
                Screens[3].Directions.Add(Direction.Up);
                Screens[3].Directions.Add(Direction.Down);
                Screens[3].OpenTilesUp.Add(2);
                Screens[3].OpenTilesUp.Add(5);
                Screens[3].OpenTilesUp.Add(6);
                Screens[3].OpenTilesUp.Add(7);
                Screens[3].OpenTilesUp.Add(8);
                Screens[3].OpenTilesUp.Add(9);
                Screens[3].OpenTilesDown.Add(5);
                Screens[4].Directions.Add(Direction.Left);
                Screens[4].Directions.Add(Direction.Up);
                Screens[4].OpenTilesLeft.Add(2);
                Screens[4].OpenTilesLeft.Add(3);
                Screens[4].OpenTilesLeft.Add(4);
                Screens[4].OpenTilesLeft.Add(5);
                Screens[4].OpenTilesLeft.Add(9);
                Screens[4].OpenTilesLeft.Add(10);
                Screens[4].OpenTilesUp.Add(5);
                Screens[4].OpenTilesUp.Add(10);
                Screens[4].OpenTilesUp.Add(11);
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
                Screens[6].OpenTilesUp.Add(6);
                Screens[6].OpenTilesUp.Add(7);
                Screens[6].OpenTilesUp.Add(8);
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
                //Screens[11].Directions.Add(Direction.Right);
                Screens[11].OpenTilesLeft.Add(9);
                Screens[11].OpenTilesLeft.Add(10);
                Screens[11].OpenTilesLeft.Add(11);
                //Screens[11].OpenTilesRight.Add(4);
                //Screens[11].OpenTilesRight.Add(5);
                //Screens[11].OpenTilesRight.Add(6);
                //Screens[11].OpenTilesRight.Add(9);
                //Screens[11].OpenTilesRight.Add(10);
                //Screens[11].OpenTilesRight.Add(11);
                Screens[DartmoorExitScreen].Directions.Add(Direction.Left);
                Screens[DartmoorExitScreen].Directions.Add(Direction.Right);
                Screens[DartmoorExitScreen].OpenTilesLeft.Add(3);
                Screens[DartmoorExitScreen].OpenTilesLeft.Add(4);
                Screens[DartmoorExitScreen].OpenTilesLeft.Add(5);
                Screens[DartmoorExitScreen].OpenTilesLeft.Add(6);
                Screens[DartmoorExitScreen].OpenTilesLeft.Add(9);
                Screens[DartmoorExitScreen].OpenTilesLeft.Add(10);
                Screens[DartmoorExitScreen].OpenTilesLeft.Add(11);
                Screens[DartmoorExitScreen].OpenTilesRight.Add(2);
                Screens[DartmoorExitScreen].OpenTilesRight.Add(3);
                Screens[13].Directions.Add(Direction.Left);
                Screens[13].Directions.Add(Direction.Right);
                Screens[13].Directions.Add(Direction.Up);
                for (byte i = 2; i < 9; i++)
                {
                    Screens[13].OpenTilesRight.Add(i);
                    Screens[FraternalScreen].OpenTilesLeft.Add(i);
                }
                Screens[13].OpenTilesLeft.Add(2);
                Screens[13].OpenTilesLeft.Add(3);
                Screens[13].OpenTilesUp.Add(2);
                Screens[FraternalScreen].Directions.Add(Direction.Left);
                Screens[FraternalScreen].Directions.Add(Direction.Up);
                Screens[FraternalScreen].OpenTilesLeft.Add(9);
                Screens[FraternalScreen].OpenTilesUp.Add(13);
                Screens[16].Directions.Add(Direction.Left);
                Screens[16].Directions.Add(Direction.Right);
                Screens[16].OpenTilesLeft.Add(6);
                Screens[16].OpenTilesLeft.Add(7);
                Screens[16].OpenTilesLeft.Add(8);
                Screens[16].OpenTilesLeft.Add(9);
                Screens[16].OpenTilesLeft.Add(10);
                Screens[16].OpenTilesRight.Add(2);
                Screens[16].OpenTilesRight.Add(3);
                Screens[17].Directions.Add(Direction.Left);
                Screens[17].Directions.Add(Direction.Right);
                Screens[17].Directions.Add(Direction.Down);
                Screens[17].OpenTilesLeft.Add(1);
                Screens[17].OpenTilesLeft.Add(2);
                Screens[17].OpenTilesLeft.Add(3);
                Screens[17].OpenTilesLeft.Add(7);
                Screens[17].OpenTilesLeft.Add(8);
                Screens[17].OpenTilesLeft.Add(9);
                Screens[17].OpenTilesLeft.Add(10);
                Screens[17].OpenTilesRight.Add(1);
                Screens[17].OpenTilesRight.Add(2);
                Screens[17].OpenTilesRight.Add(3);
                Screens[17].OpenTilesDown.Add(7);
                Screens[18].Directions.Add(Direction.Left);
                Screens[18].OpenTilesLeft.Add(1);
                Screens[18].OpenTilesLeft.Add(2);
                Screens[18].OpenTilesLeft.Add(3);
                Screens[19].Directions.Add(Direction.Right);
                Screens[19].Directions.Add(Direction.Down);
                Screens[19].OpenTilesRight.Add(1);
                Screens[19].OpenTilesRight.Add(2);
                Screens[19].OpenTilesRight.Add(3);
                Screens[19].OpenTilesDown.Add(9);
                Screens[GrieveScreen].Directions.Add(Direction.Left);
                //Screens[GrieveScreen].Directions.Add(Direction.Down);
                Screens[GrieveScreen].OpenTilesLeft.Add(1);
                Screens[GrieveScreen].OpenTilesLeft.Add(2);
                Screens[GrieveScreen].OpenTilesLeft.Add(3);
                //Screens[GrieveScreen].OpenTilesDown.Add(6);
                Screens[23].Directions.Add(Direction.Left);
                Screens[23].Directions.Add(Direction.Right);
                Screens[23].Directions.Add(Direction.Up);
                Screens[23].OpenTilesLeft.Add(2);
                Screens[23].OpenTilesLeft.Add(3);
                Screens[23].OpenTilesLeft.Add(4);
                Screens[23].OpenTilesLeft.Add(5);
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
                Screens[25].Directions.Add(Direction.Up);
                //Screens[25].Directions.Add(Direction.Down);
                Screens[25].OpenTilesLeft.Add(2);
                Screens[25].OpenTilesLeft.Add(3);
                Screens[25].OpenTilesLeft.Add(4);
                Screens[25].OpenTilesLeft.Add(5);
                Screens[25].OpenTilesLeft.Add(8);
                Screens[25].OpenTilesLeft.Add(9);
                Screens[25].OpenTilesLeft.Add(10);
                Screens[25].OpenTilesLeft.Add(11);
                Screens[25].OpenTilesRight.Add(2);
                Screens[25].OpenTilesRight.Add(3);
                Screens[25].OpenTilesRight.Add(4);
                Screens[25].OpenTilesRight.Add(5);
                Screens[25].OpenTilesRight.Add(7);
                Screens[25].OpenTilesRight.Add(8);
                Screens[25].OpenTilesRight.Add(9);
                Screens[25].OpenTilesRight.Add(10);
                Screens[25].OpenTilesUp.Add(9);
                Screens[25].OpenTilesDown.Add(14);
                //Screens[26].Directions.Add(Direction.Left);
                Screens[26].Directions.Add(Direction.Right);
                //Screens[26].OpenTilesLeft.Add(7);
                //Screens[26].OpenTilesLeft.Add(8);
                //Screens[26].OpenTilesLeft.Add(9);
                //Screens[26].OpenTilesLeft.Add(10);
                Screens[26].OpenTilesRight.Add(2);
                Screens[26].OpenTilesRight.Add(3);
                Screens[26].OpenTilesRight.Add(4);
                Screens[26].OpenTilesRight.Add(5);
                Screens[27].Directions.Add(Direction.Left);
                Screens[27].Directions.Add(Direction.Right);
                Screens[27].Directions.Add(Direction.Down);
                Screens[27].OpenTilesLeft.Add(2);
                Screens[27].OpenTilesLeft.Add(3);
                Screens[27].OpenTilesLeft.Add(4);
                Screens[27].OpenTilesLeft.Add(5);
                Screens[27].OpenTilesRight.Add(1);
                Screens[27].OpenTilesRight.Add(2);
                Screens[27].OpenTilesRight.Add(3);
                Screens[27].OpenTilesRight.Add(4);
                Screens[27].OpenTilesRight.Add(5);
                Screens[27].OpenTilesDown.Add(2);
                Screens[28].Directions.Add(Direction.Left);
                Screens[28].Directions.Add(Direction.Right);
                for (byte i = 2; i < 12; i++)
                {
                    Screens[28].OpenTilesRight.Add(i);
                    Screens[29].OpenTilesLeft.Add(i);
                }
                Screens[28].OpenTilesRight.Add(1);
                Screens[28].OpenTilesRight.Add(2);
                Screens[28].OpenTilesRight.Add(3);
                Screens[28].OpenTilesRight.Add(4);
                Screens[28].OpenTilesRight.Add(5);
                Screens[29].Directions.Add(Direction.Left);
                Screens[29].Directions.Add(Direction.Right);
                Screens[29].Directions.Add(Direction.Up);
                Screens[29].OpenTilesRight.Add(2);
                Screens[29].OpenTilesRight.Add(3);
                Screens[29].OpenTilesRight.Add(4);
                Screens[29].OpenTilesRight.Add(5);
                Screens[29].OpenTilesUp.Add(7);
                Screens[30].Directions.Add(Direction.Up); //Fraternal guru
                Screens[30].OpenTilesUp.Add(14);
                Screens[31].Directions.Add(Direction.Up); //Fraternal house
                Screens[31].OpenTilesUp.Add(2);
            }
        }
    }
}
