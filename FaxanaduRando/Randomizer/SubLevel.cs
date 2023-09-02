using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class SubLevel
    {
        public enum Id
        {
            Eolis,
            OutsideEolis,
            EarlyTrunk,
            MiddleTrunk,
            LateTrunk,
            TowerOfTrunk,
            EastTrunk,
            TowerOfFortress,
            JokerHouse,
            EarlyMist,
            MiddleMist,
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
            BackFromEastBranch,
            LateBranch,
            Dartmoor,
            CastleFraternal,
            KingGrieve,
            BirdHospital,
            DartmoorHouse,
            FraternalHouse,
            EvilOnesLair,
            Apolune,
            Forepaw,
            Mascon,
            Victim,
            Conflate,
            Daybreak,
            DartmoorCity,
            Other,
        }

        public const byte UndefinedPalette = 0xFF;

        public Id SubLevelId { get; set; }
        public List<Screen> Screens { get; set; } = new List<Screen>();
        public bool RequiresMattock { get; set; } = false;
        public bool RequiresWingboots { get; set; } = false;
        public byte Palette { get; set; } = UndefinedPalette;

        public static Dictionary<Id, SubLevel> SubLevelDict { get; set; } = new Dictionary<Id, SubLevel>();

        public static Id SkySpringSublevel { get; set; } = Id.EastTrunk;
        public static Id FortressSpringSublevel { get; set; } = Id.TowerOfFortress;
        public static Id JokerSpringSublevel { get; set; } = Id.JokerHouse;

        public SubLevel(Id id, List<Screen> screens)
        {
            SubLevelId = id;
            Screens = screens;
            foreach (var screen in screens)
            {
                screen.ParentSublevel = SubLevelId;
            }
        }

        public SubLevel(Id id, List<Screen> screens, byte palette) : this(id, screens)
        {
            Palette = palette;
        }

        public void AddScreen(Screen screen)
        {
            Screens.Add(screen);
            screen.ParentSublevel = SubLevelId;
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

        private static HashSet<Id> towerIds = new HashSet<Id>()
        {
            Id.TowerOfTrunk,
            Id.TowerOfFortress,
            Id.JokerHouse,
            Id.MasconTower,
            Id.TowerOfSuffer,
            Id.VictimTower,
            Id.TowerOfMist,
            Id.BattleHelmetWing,
            Id.CastleFraternal,
            Id.EvilOnesLair,
        };

        public bool IsTower()
        {
            return towerIds.Contains(SubLevelId);
        }
    }
}
