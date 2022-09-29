using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class Towns : Level
    {
        public Towns(WorldNumber number, byte[] content) : base(number, content)
        {
        }

        public override int GetStartOffset()
        {
            return 0x2CCB9;
        }

        public override int GetEndOffset()
        {
            return 0x2CCED;
        }

        public override bool ShouldRandomizeScreens()
        {
            return false;
        }

        public override bool CreateSublevels(List<Screen> startScreens, List<Screen> endScreens, List<Screen> candidates, List<Screen> specialScreens, Random random)
        {
            return true;
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
            return new List<Screen>();
        }

        public override void SetupScreens()
        {
        }
    }
}
