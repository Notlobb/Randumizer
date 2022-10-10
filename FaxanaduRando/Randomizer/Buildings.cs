using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    internal class Buildings : Level
    {
        public Buildings(WorldNumber number, byte[] content) : base(number, content)
        {
        }

        public override int GetStartOffset()
        {
            return 0x2CB12;
        }

        public override int GetEndOffset()
        {
            return 0x2CC9C;
        }

        public override bool ShouldRandomizeScreens()
        {
            return false;
        }

        public override bool CreateSublevels(List<Screen> startScreens, List<Screen> endScreens, List<Screen> candidates, List<Screen> specialScreens, Random random, uint attempts)
        {
            return true;
        }

        public override List<Screen> GetCandidates(Random random)
        {
            var candidates = new List<Screen>();
            return candidates;
        }

        public override void GetEnds(ref List<Screen> startScreens, ref List<Screen> endScreens, Random random)
        {
        }

        public override List<Screen> GetSpecialScreens(Random random)
        {
            var specialScreens = new List<Screen>();
            return specialScreens;
        }

        public override void SetupScreens()
        {
        }
    }
}
