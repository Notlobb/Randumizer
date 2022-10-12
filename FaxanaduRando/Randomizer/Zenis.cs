using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    internal class Zenis : Level
    {
        public Zenis(WorldNumber number, byte[] content) : base(number, content)
        {
            if (ShouldRandomizeScreens())
            {
                int pointerOffset = Section.GetOffset(11, 0x89CC, 0x8000);
                content[pointerOffset] = 0xE0;
                content[pointerOffset + 1] = 0xBC;
                int newOffset = Section.GetOffset(11, 0xBCE0, 0x8000);
                var sprite = new Sprite(content, newOffset);
                sprite.Id = Sprite.SpriteId.WingbootsBossLocked;
                sprite.SetX(13);
                sprite.SetY(9);
                sprite.ShouldBeShuffled = false;
                Screens[8].Sprites.Add(sprite);
            }
        }

        public override int GetStartOffset()
        {
            return 0x2C9F4;
        }

        public override int GetEndOffset()
        {
            return 0x2CA85;
        }

        public override bool ShouldRandomizeScreens()
        {
            return GeneralOptions.RandomizeScreens != GeneralOptions.ScreenRandomization.Unchanged;
        }

        public override bool CreateSublevels(List<Screen> startScreens, List<Screen> endScreens, List<Screen> candidates, List<Screen> specialScreens, Random random, uint attempts)
        {
            bool result = CreateSublevel(startScreens[0], endScreens[0], candidates, specialScreens, 0, 6, random, SubLevel.Id.EvilOnesLair, attempts);
            if (!result)
            {
                return result;
            }

            while (result)
            {
                result = AddMoreConnections(SubLevel.SubLevelDict[SubLevel.Id.EvilOnesLair], random);
            }

            return true;
        }

        public bool AddMoreConnections(SubLevel sublevel, Random random)
        {
            bool newConnection = false;
            var moreScreens = GetCandidates(random);
            var toBeAdded = new HashSet<byte>();

            foreach (var screen in sublevel.Screens)
            {
                var directions = new List<Direction>(screen.AvailableDirections);
                Util.ShuffleList(directions, 0, directions.Count - 1, random);
                foreach (var direction in directions)
                {
                    foreach (var otherScreen in moreScreens)
                    {
                        if (screen.CanConnect(direction, otherScreen))
                        {
                            screen.Connect(direction, otherScreen.Number);
                            var reverse = Screen.GetReverse(direction);
                            otherScreen.Connect(reverse, screen.Number);
                            toBeAdded.Add(otherScreen.Number);
                            newConnection = true;
                            break;
                        }
                    }
                }
            }

            foreach (var screen in sublevel.Screens)
            {
                toBeAdded.Remove(screen.Number);
            }

            foreach (var number in toBeAdded)
            {
                sublevel.Screens.Add(Screens[number]);
            }

            return newConnection;
        }

        public override List<Screen> GetCandidates(Random random)
        {
            var candidates = new List<Screen>();
            candidates.AddRange(Screens.GetRange(2, 6));
            candidates.AddRange(Screens.GetRange(9, 11));
            Util.ShuffleList(candidates, 0, candidates.Count - 1, random);
            return candidates;
        }

        public override void GetEnds(ref List<Screen> startScreens, ref List<Screen> endScreens, Random random)
        {
            startScreens = new List<Screen>();
            endScreens = new List<Screen>();

            startScreens.Add(Screens[8]);
            endScreens.Add(Screens[1]);
        }

        public override List<Screen> GetSpecialScreens(Random random)
        {
            var specialScreens = new List<Screen>();
            return specialScreens;
        }

        public override void SetupScreens()
        {
            Screens[1].Directions.Add(Direction.Left);
            Screens[1].Directions.Add(Direction.Down);
            Screens[1].OpenTilesLeft.Add(6);
            Screens[1].OpenTilesLeft.Add(7);
            Screens[1].OpenTilesDown.Add(2);
            Screens[2].Directions.Add(Direction.Left);
            Screens[2].Directions.Add(Direction.Right);
            Screens[2].Directions.Add(Direction.Up);
            Screens[2].Directions.Add(Direction.Down);
            for (byte i = 2; i < 12; i++)
            {
                Screens[2].OpenTilesRight.Add(i);
                Screens[3].OpenTilesLeft.Add(i);
                Screens[3].OpenTilesRight.Add(i);
                Screens[4].OpenTilesLeft.Add(i);
                Screens[4].OpenTilesRight.Add(i);
                Screens[5].OpenTilesLeft.Add(i);
            }
            Screens[2].OpenTilesLeft.Add(2);
            Screens[2].OpenTilesLeft.Add(3);
            Screens[2].OpenTilesLeft.Add(4);
            Screens[2].OpenTilesUp.Add(8);
            Screens[2].OpenTilesUp.Add(9);
            Screens[2].OpenTilesUp.Add(10);
            Screens[2].OpenTilesUp.Add(11);
            Screens[2].OpenTilesDown.Add(6);
            Screens[2].OpenTilesDown.Add(8);
            Screens[2].OpenTilesDown.Add(9);
            Screens[2].OpenTilesDown.Add(11);
            Screens[2].OpenTilesDown.Add(13);
            Screens[3].Directions.Add(Direction.Left);
            Screens[3].Directions.Add(Direction.Right);
            Screens[3].Directions.Add(Direction.Up);
            Screens[3].OpenTilesUp.Add(2);
            Screens[3].OpenTilesUp.Add(3);
            Screens[3].OpenTilesUp.Add(12);
            Screens[3].OpenTilesUp.Add(13);
            Screens[3].OpenTilesLeft.Remove(6);
            Screens[3].OpenTilesRight.Remove(6);
            Screens[4].Directions.Add(Direction.Left);
            Screens[4].Directions.Add(Direction.Right);
            Screens[4].Directions.Add(Direction.Up);
            Screens[4].Directions.Add(Direction.Down);
            Screens[3].OpenTilesLeft.Remove(6);
            Screens[4].OpenTilesLeft.Remove(6);
            Screens[4].OpenTilesLeft.Remove(7);
            Screens[4].OpenTilesRight.Add(1);
            Screens[4].OpenTilesRight.Remove(6);
            Screens[4].OpenTilesRight.Remove(7);
            Screens[4].OpenTilesLeft.Remove(6);
            Screens[4].OpenTilesUp.Add(6);
            Screens[4].OpenTilesUp.Add(7);
            Screens[4].OpenTilesUp.Add(8);
            Screens[4].OpenTilesUp.Add(9);
            Screens[4].OpenTilesUp.Add(10);
            Screens[4].OpenTilesDown.Add(7);
            Screens[4].OpenTilesDown.Add(8);
            Screens[4].OpenTilesDown.Add(9);
            Screens[5].Directions.Add(Direction.Left);
            Screens[5].Directions.Add(Direction.Right);
            Screens[5].Directions.Add(Direction.Up);
            Screens[5].Directions.Add(Direction.Down);
            Screens[5].OpenTilesLeft.Remove(6);
            Screens[5].OpenTilesRight.Add(1);
            Screens[5].OpenTilesRight.Add(2);
            Screens[5].OpenTilesRight.Add(3);
            Screens[5].OpenTilesRight.Add(4);
            Screens[5].OpenTilesRight.Add(5);
            Screens[5].OpenTilesUp.Add(4);
            Screens[5].OpenTilesUp.Add(5);
            Screens[5].OpenTilesUp.Add(6);
            Screens[5].OpenTilesUp.Add(7);
            Screens[5].OpenTilesUp.Add(8);
            Screens[5].OpenTilesDown.Add(13);
            Screens[6].Directions.Add(Direction.Left);
            Screens[6].Directions.Add(Direction.Right);
            Screens[6].Directions.Add(Direction.Up);
            Screens[6].Directions.Add(Direction.Down);
            for (byte i = 1; i < 10; i++)
            {
                Screens[6].OpenTilesLeft.Add(i);
            }
            Screens[6].OpenTilesRight.Add(1);
            Screens[6].OpenTilesRight.Add(2);
            Screens[6].OpenTilesRight.Add(3);
            Screens[6].OpenTilesRight.Add(4);
            Screens[6].OpenTilesRight.Add(5);
            Screens[6].OpenTilesRight.Add(6);
            Screens[6].OpenTilesUp.Add(2);
            Screens[6].OpenTilesDown.Add(6);
            Screens[7].Directions.Add(Direction.Left);
            Screens[7].Directions.Add(Direction.Up);
            Screens[7].Directions.Add(Direction.Down);
            Screens[7].OpenTilesLeft.Add(1);
            Screens[7].OpenTilesLeft.Add(2);
            Screens[7].OpenTilesLeft.Add(3);
            Screens[7].OpenTilesLeft.Add(4);
            Screens[7].OpenTilesLeft.Add(5);
            Screens[7].OpenTilesLeft.Add(6);
            Screens[7].OpenTilesUp.Add(6);
            Screens[7].OpenTilesUp.Add(8);
            Screens[7].OpenTilesUp.Add(9);
            Screens[7].OpenTilesUp.Add(11);
            Screens[7].OpenTilesUp.Add(13);
            Screens[7].OpenTilesDown.Add(10);
            Screens[8].Directions.Add(Direction.Right);
            Screens[8].Directions.Add(Direction.Up);
            Screens[8].Directions.Add(Direction.Down);
            Screens[8].OpenTilesRight.Add(3);
            Screens[8].OpenTilesRight.Add(4);
            Screens[8].OpenTilesRight.Add(5);
            Screens[8].OpenTilesUp.Add(6);
            Screens[8].OpenTilesUp.Add(7);
            Screens[8].OpenTilesUp.Add(8);
            Screens[8].OpenTilesUp.Add(9);
            Screens[8].OpenTilesUp.Add(10);
            Screens[8].OpenTilesDown.Add(11);
            Screens[8].OpenTilesDown.Add(14);
            Screens[9].Directions.Add(Direction.Left);
            Screens[9].Directions.Add(Direction.Right);
            Screens[9].Directions.Add(Direction.Up);
            Screens[9].Directions.Add(Direction.Down);
            Screens[9].OpenTilesLeft.Add(3);
            Screens[9].OpenTilesLeft.Add(4);
            Screens[9].OpenTilesLeft.Add(5);
            Screens[9].OpenTilesRight.Add(7);
            Screens[9].OpenTilesRight.Add(8);
            Screens[9].OpenTilesRight.Add(9);
            Screens[9].OpenTilesUp.Add(13);
            Screens[9].OpenTilesDown.Add(2);
            Screens[9].OpenTilesDown.Add(3);
            Screens[9].OpenTilesDown.Add(7);
            Screens[9].OpenTilesDown.Add(8);
            Screens[9].OpenTilesDown.Add(13);
            Screens[10].Directions.Add(Direction.Left);
            Screens[10].Directions.Add(Direction.Right);
            Screens[10].Directions.Add(Direction.Up);
            Screens[10].Directions.Add(Direction.Down);
            Screens[10].OpenTilesLeft.Add(4);
            Screens[10].OpenTilesLeft.Add(5);
            Screens[10].OpenTilesLeft.Add(6);
            Screens[10].OpenTilesLeft.Add(7);
            Screens[10].OpenTilesRight.Add(2);
            Screens[10].OpenTilesRight.Add(3);
            Screens[10].OpenTilesRight.Add(4);
            Screens[10].OpenTilesRight.Add(5);
            Screens[10].OpenTilesRight.Add(6);
            Screens[10].OpenTilesUp.Add(6);
            Screens[10].OpenTilesDown.Add(4);
            Screens[11].Directions.Add(Direction.Left);
            Screens[11].Directions.Add(Direction.Right);
            Screens[11].Directions.Add(Direction.Up);
            Screens[11].Directions.Add(Direction.Down);
            Screens[11].OpenTilesLeft.Add(2);
            Screens[11].OpenTilesLeft.Add(3);
            Screens[11].OpenTilesLeft.Add(4);
            Screens[11].OpenTilesLeft.Add(5);
            Screens[11].OpenTilesLeft.Add(6);
            Screens[11].OpenTilesRight.Add(2);
            Screens[11].OpenTilesRight.Add(3);
            Screens[11].OpenTilesRight.Add(4);
            Screens[11].OpenTilesRight.Add(5);
            Screens[11].OpenTilesRight.Add(8);
            Screens[11].OpenTilesRight.Add(9);
            Screens[11].OpenTilesRight.Add(10);
            Screens[11].OpenTilesRight.Add(11);
            Screens[11].OpenTilesUp.Add(6);
            Screens[11].OpenTilesUp.Add(7);
            Screens[11].OpenTilesUp.Add(8);
            Screens[11].OpenTilesUp.Add(9);
            Screens[11].OpenTilesUp.Add(10);
            Screens[11].OpenTilesDown.Add(6);
            Screens[11].OpenTilesDown.Add(7);
            Screens[11].OpenTilesDown.Add(8);
            Screens[11].OpenTilesDown.Add(9);
            Screens[12].Directions.Add(Direction.Left);
            Screens[12].Directions.Add(Direction.Right);
            Screens[12].Directions.Add(Direction.Up);
            Screens[12].Directions.Add(Direction.Down);
            for (byte i = 2; i < 12; i++)
            {
                Screens[12].OpenTilesRight.Add(i);
                Screens[13].OpenTilesLeft.Add(i);
            }
            Screens[12].OpenTilesLeft.Add(2);
            Screens[12].OpenTilesLeft.Add(3);
            Screens[12].OpenTilesLeft.Add(4);
            Screens[12].OpenTilesLeft.Add(5);
            Screens[12].OpenTilesLeft.Add(8);
            Screens[12].OpenTilesLeft.Add(9);
            Screens[12].OpenTilesLeft.Add(10);
            Screens[12].OpenTilesLeft.Add(11);
            Screens[12].OpenTilesUp.Add(5);
            Screens[12].OpenTilesUp.Add(6);
            Screens[12].OpenTilesUp.Add(7);
            Screens[12].OpenTilesUp.Add(8);
            Screens[12].OpenTilesUp.Add(9);
            Screens[12].OpenTilesUp.Add(10);
            Screens[12].OpenTilesDown.Add(4);
            Screens[12].OpenTilesDown.Add(7);
            Screens[12].OpenTilesDown.Add(8);
            Screens[12].OpenTilesDown.Add(11);
            Screens[13].Directions.Add(Direction.Left);
            Screens[13].Directions.Add(Direction.Right);
            Screens[13].Directions.Add(Direction.Up);
            Screens[13].Directions.Add(Direction.Down);
            Screens[13].OpenTilesRight.Add(5);
            Screens[13].OpenTilesRight.Add(6);
            Screens[13].OpenTilesRight.Add(7);
            Screens[13].OpenTilesUp.Add(11);
            Screens[13].OpenTilesUp.Add(12);
            Screens[13].OpenTilesUp.Add(13);
            Screens[13].OpenTilesUp.Add(14);
            Screens[13].OpenTilesDown.Add(4);
            Screens[13].OpenTilesDown.Add(6);
            Screens[13].OpenTilesDown.Add(7);
            Screens[14].Directions.Add(Direction.Left);
            Screens[14].Directions.Add(Direction.Up);
            Screens[14].OpenTilesLeft.Add(5);
            Screens[14].OpenTilesLeft.Add(6);
            Screens[14].OpenTilesLeft.Add(7);
            Screens[14].OpenTilesLeft.Add(8);
            Screens[14].OpenTilesUp.Add(2);
            Screens[14].OpenTilesUp.Add(3);
            Screens[14].OpenTilesUp.Add(4);
            Screens[14].OpenTilesUp.Add(5);
            Screens[14].OpenTilesUp.Add(6);
            Screens[14].OpenTilesUp.Add(7);
            Screens[14].OpenTilesUp.Add(8);
            Screens[15].Directions.Add(Direction.Left);
            Screens[15].Directions.Add(Direction.Right);
            Screens[15].Directions.Add(Direction.Up);
            Screens[15].Directions.Add(Direction.Down);
            for (byte i = 1; i < 11; i++)
            {
                Screens[15].OpenTilesRight.Add(i);
                Screens[16].OpenTilesLeft.Add(i);
            }
            Screens[15].OpenTilesLeft.Add(1);
            Screens[15].OpenTilesLeft.Add(2);
            Screens[15].OpenTilesLeft.Add(3);
            Screens[15].OpenTilesLeft.Add(4);
            Screens[15].OpenTilesLeft.Add(5);
            Screens[15].OpenTilesRight.Remove(7);
            Screens[15].OpenTilesUp.Add(4);
            Screens[15].OpenTilesDown.Add(4);
            Screens[15].OpenTilesDown.Add(5);
            Screens[15].OpenTilesDown.Add(7);
            Screens[15].OpenTilesDown.Add(8);
            Screens[16].Directions.Add(Direction.Left);
            Screens[16].Directions.Add(Direction.Right);
            Screens[16].Directions.Add(Direction.Up);
            Screens[16].Directions.Add(Direction.Down);
            Screens[16].OpenTilesLeft.Remove(7);
            Screens[16].OpenTilesRight.Add(8);
            Screens[16].OpenTilesRight.Add(9);
            Screens[16].OpenTilesRight.Add(10);
            Screens[16].OpenTilesRight.Add(11);
            Screens[16].OpenTilesUp.Add(6);
            Screens[16].OpenTilesUp.Add(7);
            Screens[16].OpenTilesUp.Add(8);
            Screens[16].OpenTilesUp.Add(9);
            Screens[16].OpenTilesDown.Add(2);
            Screens[16].OpenTilesDown.Add(3);
            Screens[16].OpenTilesDown.Add(12);
            Screens[16].OpenTilesDown.Add(13);
            Screens[17].Directions.Add(Direction.Left);
            Screens[17].Directions.Add(Direction.Right);
            Screens[17].Directions.Add(Direction.Up);
            Screens[17].Directions.Add(Direction.Down);
            for (byte i = 1; i < 12; i++)
            {
                Screens[17].OpenTilesLeft.Add(i);
                Screens[17].OpenTilesRight.Add(i);
                Screens[18].OpenTilesLeft.Add(i);
                Screens[18].OpenTilesRight.Add(i);
                Screens[19].OpenTilesLeft.Add(i);
            }
            Screens[17].OpenTilesLeft.Remove(7);
            Screens[17].OpenTilesRight.Remove(7);
            Screens[17].OpenTilesUp.Add(4);
            Screens[17].OpenTilesUp.Add(5);
            Screens[17].OpenTilesUp.Add(7);
            Screens[17].OpenTilesUp.Add(8);
            Screens[17].OpenTilesUp.Add(10);
            Screens[17].OpenTilesUp.Add(11);
            Screens[17].OpenTilesDown.Add(9);
            Screens[17].OpenTilesDown.Add(10);
            Screens[18].Directions.Add(Direction.Left);
            Screens[18].Directions.Add(Direction.Right);
            Screens[18].Directions.Add(Direction.Up);
            Screens[18].Directions.Add(Direction.Down);
            Screens[18].OpenTilesLeft.Remove(7);
            Screens[18].OpenTilesRight.Remove(7);
            Screens[18].OpenTilesUp.Add(3);
            Screens[18].OpenTilesUp.Add(4);
            Screens[18].OpenTilesUp.Add(6);
            Screens[18].OpenTilesUp.Add(7);
            Screens[18].OpenTilesDown.Add(7);
            Screens[18].OpenTilesDown.Add(8);
            Screens[18].OpenTilesDown.Add(9);
            Screens[19].Directions.Add(Direction.Left);
            Screens[19].Directions.Add(Direction.Right);
            Screens[19].Directions.Add(Direction.Up);
            Screens[19].Directions.Add(Direction.Down);
            for (byte i = 2; i < 9; i++)
            {
                Screens[19].OpenTilesRight.Add(i);
            }
            Screens[19].OpenTilesLeft.Remove(7);
            Screens[19].OpenTilesUp.Add(6);
            Screens[19].OpenTilesDown.Add(7);
            Screens[19].OpenTilesDown.Add(8);
            Screens[19].OpenTilesDown.Add(9);
        }
    }
}
