using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    internal class Branch : Level
    {
        public const byte BranchEntrance = 0;
        public const byte BattleHelmetWing = 10;
        public const byte BattleHelmetWingEntrance = 9;
        public const byte EastBranch = 14;
        public const byte EastBranchEntrance = 15;
        public const byte EastBranchLeft = 20;
        public const byte EastBranchLeftEntrance = 19;
        public const byte DropdownWing = 25;
        public const byte DropdownWingEntrance = 24;
        public const byte DoubleItemScreen = 30;
        public const byte ConflateLeftScreen = 13;
        public const byte DaybreakLeftScreen = 35;
        public const byte DaybreakRightScreen = 36;
        public const byte BranchExit = 39;

        public Branch(WorldNumber number, byte[] content) : base(number, content)
        {
            Screens[BranchEntrance].Doors.Add(DoorId.BranchEntrance);
            Screens[BattleHelmetWing].Doors.Add(DoorId.BattleHelmetWing);
            Screens[BattleHelmetWingEntrance].Doors.Add(DoorId.BattleHelmetWingReturn);
            Screens[EastBranch].Doors.Add(DoorId.EastBranch);
            Screens[EastBranchEntrance].Doors.Add(DoorId.EastBranchReturn);
            Screens[EastBranchLeft].Doors.Add(DoorId.EastBranchLeft);
            Screens[EastBranchLeftEntrance].Doors.Add(DoorId.EastBranchLeftReturn);
            Screens[DropdownWing].Doors.Add(DoorId.DropdownWing);
            Screens[DropdownWingEntrance].Doors.Add(DoorId.DropdownWingReturn);
            Screens[BranchExit].Doors.Add(DoorId.BranchExit);

            void AddDaybreakDoors(byte screenNumber)
            {
                Screens[screenNumber].Doors.Add(DoorId.DaybreakBar);
                Screens[screenNumber].Doors.Add(DoorId.DaybreakGuru);
                Screens[screenNumber].Doors.Add(DoorId.DaybreakHouse);
                Screens[screenNumber].Doors.Add(DoorId.DaybreakItemShop);
                Screens[screenNumber].Doors.Add(DoorId.DaybreakKeyShop);
                Screens[screenNumber].Doors.Add(DoorId.DaybreakMeatShop);
            }

            AddDaybreakDoors(DaybreakLeftScreen);
            AddDaybreakDoors(DaybreakRightScreen);

            Screens[ConflateLeftScreen].Doors.Add(DoorId.ConflateBar);
            Screens[ConflateLeftScreen].Doors.Add(DoorId.ConflateGuru);
            Screens[ConflateLeftScreen].Doors.Add(DoorId.ConflateHospital);
            Screens[ConflateLeftScreen].Doors.Add(DoorId.ConflateHouse);
            Screens[ConflateLeftScreen].Doors.Add(DoorId.ConflateItemShop);
            Screens[ConflateLeftScreen].Doors.Add(DoorId.ConflateMeatShop);
        }

        public override int GetStartOffset()
        {
            return 0x2C7AA;
        }

        public override int GetEndOffset()
        {
            return 0x2C8B1;
        }

        public override bool CreateSublevels(List<Screen> startScreens, List<Screen> endScreens, List<Screen> candidates, List<Screen> specialScreens, Random random, uint attempts)
        {
            Screens[28].OpenTilesLeft = new HashSet<byte>();
            Screens[28].OpenTilesRight = new HashSet<byte>();
            if (random.Next(0, 2) == 0)
            {
                Screens[28].OpenTilesLeft.Add(3);
                Screens[28].OpenTilesLeft.Add(4);
                Screens[28].OpenTilesLeft.Add(5);
                Screens[28].OpenTilesLeft.Add(6);
                Screens[28].SecondOpenTilesLeft.Add(8);
                Screens[28].SecondOpenTilesLeft.Add(9);
                Screens[28].SecondOpenTilesLeft.Add(10);
                Screens[28].SecondOpenTilesLeft.Add(11);
                Screens[28].OpenTilesRight.Add(3);
                Screens[28].OpenTilesRight.Add(4);
                Screens[28].OpenTilesRight.Add(5);
                Screens[28].OpenTilesRight.Add(6);
                Screens[28].OpenTilesRight.Add(8);
                Screens[28].OpenTilesRight.Add(9);
                Screens[28].OpenTilesRight.Add(10);
            }
            else
            {
                Screens[28].OpenTilesLeft.Add(3);
                Screens[28].OpenTilesLeft.Add(4);
                Screens[28].OpenTilesLeft.Add(5);
                Screens[28].OpenTilesLeft.Add(6);
                Screens[28].OpenTilesLeft.Add(8);
                Screens[28].OpenTilesLeft.Add(9);
                Screens[28].OpenTilesLeft.Add(10);
                Screens[28].OpenTilesLeft.Add(11);
                Screens[28].OpenTilesRight.Add(3);
                Screens[28].OpenTilesRight.Add(4);
                Screens[28].OpenTilesRight.Add(5);
                Screens[28].OpenTilesRight.Add(6);
                Screens[28].SecondOpenTilesRight.Add(8);
                Screens[28].SecondOpenTilesRight.Add(9);
                Screens[28].SecondOpenTilesRight.Add(10);
            }

            bool result = CreateSublevel(startScreens[0], endScreens[0], candidates, specialScreens, random, SubLevel.Id.EarlyBranch, attempts);
            if (!result)
            {
                return result;
            }
            result = CreateSublevel(startScreens[1], endScreens[1], candidates, specialScreens, random, SubLevel.Id.MiddleBranch, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[2], endScreens[2], candidates, specialScreens, random, SubLevel.Id.BattleHelmetWing, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[3], endScreens[3], candidates, specialScreens, random, SubLevel.Id.EastBranch, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[4], endScreens[4], candidates, specialScreens, random, SubLevel.Id.BackFromEastBranch, attempts);
            if (!result)
            {
                return result;
            }

            result = CreateSublevel(startScreens[5], endScreens[5], candidates, specialScreens, random, SubLevel.Id.DropDownWing, attempts);
            if (!result)
            {
                return result;
            }

            if (!GeneralOptions.ShuffleTowers &&
                specialScreens.Count > 0)
            {
                return false;
            }

            result = CreateSublevel(Screens[11], endScreens[6], candidates, specialScreens, random, Screens[11].ParentSublevel, attempts, false);
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

            Screens[17].ScrollData.Down = 17;
            Screens[33].ScrollData.Down = DoubleItemScreen;
            Screens[DoubleItemScreen].ScrollData.Up = 33;

            return result;
        }

        public override List<Screen> GetCandidates(Random random)
        {
            var candidates = new List<Screen>();
            candidates.AddRange(Screens.GetRange(1, 6));
            candidates.Add(Screens[8]);
            candidates.AddRange(Screens.GetRange(16, 3));
            candidates.Add(Screens[21]);
            candidates.Add(Screens[23]);
            candidates.Add(Screens[26]);
            candidates.Add(Screens[27]);
            candidates.AddRange(Screens.GetRange(31, 2));
            candidates.Add(Screens[33]);
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
            startScreens.Add(Screens[DaybreakRightScreen]);
            startScreens.Add(Screens[BattleHelmetWingEntrance]);
            startScreens.Add(Screens[EastBranchEntrance]);
            startScreens.Add(Screens[EastBranchLeftEntrance]);
            startScreens.Add(Screens[DropdownWingEntrance]);

            endScreens.Add(Screens[DaybreakLeftScreen]);
            endScreens.Add(Screens[39]);
            endScreens.Add(Screens[ConflateLeftScreen]);
            endScreens.Add(Screens[7]);
            endScreens.Add(Screens[22]);
            endScreens.Add(Screens[29]);
            endScreens.Add(Screens[10]);

            if (random.Next(0, 2) == 0)
            {
                (startScreens[1], endScreens[0]) = (endScreens[0], startScreens[1]);
            }

            if (GeneralOptions.ShuffleTowers)
            {
                Util.ShuffleList(endScreens, 2, endScreens.Count - 1, random);
            }
            else
            {
                Util.ShuffleList(endScreens, 2, endScreens.Count - 2, random);
                Util.ShuffleList(endScreens, 3, endScreens.Count - 1, random);
            }
        }

        public override List<Screen> GetSpecialScreens(Random random)
        {
            var specialScreens = new List<Screen>();
            specialScreens.Add(Screens[11]);
            specialScreens.Add(Screens[EastBranch]);
            specialScreens.Add(Screens[EastBranchLeft]);
            specialScreens.Add(Screens[28]);

            Util.ShuffleList(specialScreens, 0, specialScreens.Count - 1, random);
            return specialScreens;
        }

        public override void SetupScreens()
        {
            startToSpecial[EastBranchEntrance] = EastBranch;
            startToSpecial[EastBranchLeftEntrance] = EastBranchLeft;

            Screens[EastBranch].FriendConnections[Direction.Down] = Screens[12];
            Screens[EastBranchLeft].FriendEnds[Direction.Down] = Screens[DropdownWing];
            Screens[28].FriendEnds[Direction.Up] = Screens[DoubleItemScreen];

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
            Screens[8].OpenTilesLeft.Add(3);
            Screens[8].OpenTilesLeft.Add(4);
            Screens[8].OpenTilesLeft.Add(5);
            Screens[8].OpenTilesLeft.Add(6);
            Screens[8].OpenTilesLeft.Add(7);
            Screens[8].OpenTilesLeft.Add(8);
            Screens[8].OpenTilesLeft.Add(9);
            Screens[8].OpenTilesLeft.Add(10);
            Screens[8].OpenTilesRight.Add(1);
            Screens[8].OpenTilesRight.Add(2);
            Screens[8].OpenTilesRight.Add(3);
            Screens[8].OpenTilesRight.Add(4);
            Screens[8].OpenTilesRight.Add(6);
            Screens[8].OpenTilesRight.Add(7);
            Screens[8].OpenTilesRight.Add(8);
            Screens[8].OpenTilesRight.Add(9);
            Screens[BattleHelmetWingEntrance].Directions.Add(Direction.Left);
            Screens[BattleHelmetWingEntrance].Directions.Add(Direction.Right);
            Screens[BattleHelmetWingEntrance].OpenTilesLeft.Add(5);
            Screens[BattleHelmetWingEntrance].OpenTilesLeft.Add(6);
            Screens[BattleHelmetWingEntrance].OpenTilesLeft.Add(7);
            Screens[BattleHelmetWingEntrance].OpenTilesLeft.Add(8);
            Screens[BattleHelmetWingEntrance].OpenTilesLeft.Add(9);
            Screens[BattleHelmetWingEntrance].OpenTilesRight.Add(3);
            Screens[BattleHelmetWingEntrance].OpenTilesRight.Add(4);
            Screens[BattleHelmetWing].Directions.Add(Direction.Right);
            Screens[BattleHelmetWing].OpenTilesRight.Add(4);
            Screens[BattleHelmetWing].OpenTilesRight.Add(5);
            Screens[BattleHelmetWing].OpenTilesRight.Add(6);
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
            //Screens[12].Directions.Add(Direction.Up);
            Screens[12].OpenTilesLeft.Add(4);
            Screens[12].OpenTilesLeft.Add(5);
            Screens[12].OpenTilesLeft.Add(6);
            Screens[12].OpenTilesRight.Add(4);
            Screens[12].OpenTilesRight.Add(5);
            Screens[12].OpenTilesRight.Add(6);
            Screens[12].OpenTilesRight.Add(7);
            //Screens[12].OpenTilesUp.Add(8);
            Screens[ConflateLeftScreen].Directions.Add(Direction.Left);
            Screens[ConflateLeftScreen].OpenTilesLeft.Add(4);
            Screens[ConflateLeftScreen].OpenTilesLeft.Add(5);
            Screens[ConflateLeftScreen].OpenTilesLeft.Add(6);
            Screens[ConflateLeftScreen].OpenTilesLeft.Add(7);
            Screens[EastBranch].Directions.Add(Direction.Right);
            Screens[EastBranch].OpenTilesRight.Add(6);
            Screens[EastBranch].OpenTilesRight.Add(7);
            Screens[EastBranchEntrance].Directions.Add(Direction.Left);
            Screens[EastBranchEntrance].Directions.Add(Direction.Right);
            Screens[EastBranchEntrance].OpenTilesLeft.Add(6);
            Screens[EastBranchEntrance].OpenTilesLeft.Add(7);
            Screens[EastBranchEntrance].OpenTilesRight.Add(6);
            Screens[EastBranchEntrance].OpenTilesRight.Add(7);
            Screens[EastBranchEntrance].OpenTilesRight.Add(8);
            Screens[EastBranchEntrance].OpenTilesRight.Add(9);
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
            Screens[EastBranchLeftEntrance].Directions.Add(Direction.Left);
            Screens[EastBranchLeftEntrance].Directions.Add(Direction.Right);
            Screens[EastBranchLeftEntrance].OpenTilesLeft.Add(8);
            Screens[EastBranchLeftEntrance].OpenTilesLeft.Add(9);
            Screens[EastBranchLeftEntrance].OpenTilesLeft.Add(10);
            Screens[EastBranchLeftEntrance].OpenTilesLeft.Add(11);
            Screens[EastBranchLeftEntrance].OpenTilesRight.Add(7);
            Screens[EastBranchLeftEntrance].OpenTilesRight.Add(8);
            Screens[EastBranchLeftEntrance].OpenTilesRight.Add(9);
            Screens[EastBranchLeft].Directions.Add(Direction.Left);
            Screens[EastBranchLeft].Directions.Add(Direction.Right);
            Screens[EastBranchLeft].OpenTilesLeft.Add(7);
            Screens[EastBranchLeft].OpenTilesLeft.Add(8);
            Screens[EastBranchLeft].OpenTilesLeft.Add(9);
            Screens[EastBranchLeft].OpenTilesRight.Add(3);
            Screens[EastBranchLeft].OpenTilesRight.Add(4);
            Screens[EastBranchLeft].OpenTilesRight.Add(5);
            Screens[EastBranchLeft].OpenTilesRight.Add(6);
            Screens[EastBranchLeft].SecondOpenTilesRight.Add(8);
            Screens[EastBranchLeft].SecondOpenTilesRight.Add(9);
            Screens[21].Directions.Add(Direction.Left);
            Screens[21].Directions.Add(Direction.Up);
            Screens[21].Directions.Add(Direction.Down);
            Screens[21].OpenTilesLeft.Add(2);
            Screens[21].OpenTilesLeft.Add(3);
            Screens[21].OpenTilesLeft.Add(4);
            Screens[21].OpenTilesLeft.Add(5);
            Screens[21].SecondOpenTilesLeft.Add(7);
            Screens[21].SecondOpenTilesLeft.Add(8);
            Screens[21].SecondOpenTilesLeft.Add(9);
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
            Screens[DropdownWingEntrance].Directions.Add(Direction.Left);
            Screens[DropdownWingEntrance].Directions.Add(Direction.Right);
            Screens[DropdownWingEntrance].OpenTilesLeft.Add(6);
            Screens[DropdownWingEntrance].OpenTilesLeft.Add(7);
            Screens[DropdownWingEntrance].OpenTilesLeft.Add(8);
            Screens[DropdownWingEntrance].OpenTilesRight.Add(6);
            Screens[DropdownWingEntrance].OpenTilesRight.Add(7);
            Screens[DropdownWingEntrance].OpenTilesRight.Add(8);
            Screens[DropdownWingEntrance].OpenTilesRight.Add(9);
            Screens[DropdownWing].Directions.Add(Direction.Left);
            Screens[DropdownWing].Directions.Add(Direction.Up);
            Screens[DropdownWing].OpenTilesLeft.Add(6);
            Screens[DropdownWing].OpenTilesLeft.Add(7);
            Screens[DropdownWing].OpenTilesLeft.Add(8);
            Screens[DropdownWing].OpenTilesLeft.Add(9);
            Screens[DropdownWing].OpenTilesUp.Add(13);
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
            Screens[29].Directions.Add(Direction.Left);
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
            Screens[33].Directions.Add(Direction.Up);
            //Screens[33].Directions.Add(Direction.Down);
            Screens[33].OpenTilesLeft.Add(1);
            Screens[33].OpenTilesLeft.Add(2);
            Screens[33].OpenTilesLeft.Add(3);
            Screens[33].SecondOpenTilesLeft.Add(5);
            Screens[33].SecondOpenTilesLeft.Add(6);
            Screens[33].SecondOpenTilesLeft.Add(7);
            Screens[33].SecondOpenTilesLeft.Add(8);
            //Screens[33].OpenTilesDown.Add(6);
            Screens[33].OpenTilesUp.Add(12);
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
            Screens[38].OpenTilesLeft.Add(4);
            Screens[38].OpenTilesLeft.Add(5);
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
    }
}
