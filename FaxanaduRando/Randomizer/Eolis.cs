using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    internal class Eolis : Level
    {
        public Eolis(WorldNumber number, byte[] content) : base(number, content)
        {
        }

        public override bool IsEolis()
        {
            return true;
        }

        public override int GetStartOffset()
        {
            return 0x2C242;
        }

        public override int GetEndOffset()
        {
            return 0x2C26F;
        }

        public override bool ShouldRandomizeScreens()
        {
            return Util.AllEndWorldScreensRandomized();
        }

        public override bool CreateSublevels(List<Screen> startScreens, List<Screen> endScreens, List<Screen> candidates, List<Screen> specialScreens, Random random, uint attempts)
        {
            return CreateSublevel(Screens[7], Screens[8], new List<Screen>(), specialScreens, random, SubLevel.Id.Eolis, attempts);
        }

        public override List<Screen> GetCandidates(Random random)
        {
            return new List<Screen>();
        }

        public override void GetEnds(ref List<Screen> startScreens, ref List<Screen> endScreens, Random random)
        {
        }

        public override List<Screen> GetSpecialScreens(Random random)
        {
            var specialScreens = Screens.GetRange(1, 6);
            Util.ShuffleList(specialScreens, 0, specialScreens.Count - 1, random);
            return specialScreens;
        }

        public override void SetupScreens()
        {
            Screens[7].Directions.Add(Direction.Right);
            Screens[8].Directions.Add(Direction.Left);

            for (byte i = 0; i < 11; i++)
            {
                Screens[7].OpenTilesRight.Add(i);
                Screens[8].OpenTilesLeft.Add(i);
            }

            for (byte j = 1; j < 7; j++)
            {
                Screens[j].Directions.Add(Direction.Left);
                Screens[j].Directions.Add(Direction.Right);
                for (byte i = 0; i < 11; i++)
                {
                    Screens[j].OpenTilesLeft.Add(i);
                    Screens[j].OpenTilesRight.Add(i);
                }
            }
        }
    }
}
