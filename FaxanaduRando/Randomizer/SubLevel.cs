using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class SubLevel
    {
        public enum Id
        {
            EarlyTrunk,
            TowerOfTrunk,
            EastTrunk,
            TowerOfFortress,
            JokerHouse,
            EarlyMist,
            TowerOfSuffer,
            MasconTower,
            VictimTower,
            LateMist,
            TowerOfMist,
            EarlyBranch,
            BattleHelmetWing,
            MiddleBranch,
            DropDownWing,
            EastBranch,
            Dartmoor,
            CastleFraternal,
            KingGrieve,
            BirdHospital,
            DartmoorHouse,
            FraternalHouse,
            EvilOnesLair,
            Other,
        }

        public Id SubLevelId { get; set; }
        public List<Screen> Screens { get; set; } = new List<Screen>();

        public static Dictionary<Id, SubLevel> SubLevelDict { get; set; } = new Dictionary<Id, SubLevel>();

        public SubLevel(Id id, List<Screen> screens)
        {
            SubLevelId = id;
            Screens.AddRange(screens);
            foreach (var screen in Screens)
            {
                screen.ParentSubLevel = this;
            }
        }

        public bool ContainsItem(Sprite.SpriteId itemId)
        {
            foreach (var screen in Screens)
            {
                foreach (var sprite in screen.Sprites)
                {
                    if (sprite.Id == itemId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<Sprite> CollectItems()
        {
            var items = new List<Sprite>();
            foreach (var screen in Screens)
            {
                foreach (var sprite in screen.Sprites)
                {
                    if (Sprite.itemIds.Contains(sprite.Id))
                    {
                        items.Add(sprite);
                    }
                }
            }

            return items;
        }
    }
}
