using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public enum DoorId
    {
        FirstDoor,
        EolisExit,
        TrunkEntrance,
        TowerOfTrunk,
        TowerOfTrunkReturn,
        JokerHouse,
        JokerHouseReturn,
        TowerOfFortress,
        TowerOfFortressReturn,
        TrunkExit,
        MistEntrance,
        TowerOfSuffer,
        TowerOfSufferReturn,
        MasconTower,
        MasconTowerReturn,
        TowerOfMist,
        TowerOfMistReturn,
        VictimTower,
        VictimTowerReturn,
        MistExit,
        BranchEntrance,
        BattleHelmetWing,
        BattleHelmetWingReturn,
        EastBranch,
        EastBranchReturn,
        EastBranchLeft,
        EastBranchLeftReturn,
        DropdownWing,
        DropdownWingReturn,
        BranchExit,
        DartmoorEntrance,
        CastleFraternal,
        CastleFraternalReturn,
        KingGrieve,
        DartmoorLateDoor,
        DartmoorLateDoor2,
        DartmoorExit,
        ZenisReturn,
        FinalDoor,
        EvilOnesLair,
        EolisMeatShop,
        EolisHouse,
        EolisGuru,
        EolisKeyShop,
        EolisItemShop,
        EolisKing,
        MartialArtsShop,
        EolisMagicShop,
        TrunkSecretShop,
        FortressGuru,
        FireMage,
        MistSmallHouse,
        MistGuru,
        BirdHospital,
        MistSecretShop,
        MistLargeHouse,
        AceKeyHouse,
        DartmoorHouse1,
        DartmoorHouse2,
        DartmoorHouse3,
        DartmoorHouse4,
        LeftOfDartmoor,
        FarLeftDartmoor,
        FraternalGuru,
        FraternalHouse,
        ApoluneItemShop,
        ApoluneKeyShop,
        ApoluneBar,
        ApoluneGuru,
        ApoluneHospital,
        ApoluneHouse,
        ForepawMeatShop,
        ForepawItemShop,
        ForepawGuru,
        ForepawHospital,
        ForepawHouse,
        ForepawKeyShop,
        MasconBar,
        MasconMeatShop,
        MasconItemShop,
        MasconKeyShop,
        MasconHouse,
        MasconHospital,
        VictimGuru,
        VictimHospital,
        VictimHouse,
        VictimMeatShop,
        VictimKeyShop,
        VictimItemShop,
        VictimBar,
        ConflateHouse,
        ConflateMeatShop,
        ConflateGuru,
        ConflateHospital,
        ConflateItemShop,
        ConflateBar,
        DaybreakKeyShop,
        DaybreakItemShop,
        DaybreakMeatShop,
        DaybreakBar,
        DaybreakGuru,
        DaybreakHouse,
        DartmoorGuru,
        DartmoorBar,
        DartmoorMeatShop,
        DartmoorHospital,
        DartmoorKeyShop,
        DartmoorItemShop,
    }

    public class Door
    {
        public DoorId Id { get; set; }
        public DoorId OriginalId { get; set; }
        public OtherWorldNumber World { get; set; }
        public DoorRandomizer.PositionData Position { get; set; }
        public DoorRandomizer.Requirement Requirement { get; set; }
        public DoorRandomizer.PositionData ReturnPosition { get; set; }
        public DoorRandomizer.Requirement ReturnRequirement { get; set; }
        public SubLevel Sublevel { get; set; } = null;
        public GiftItem Gift { get; set; } = null;
        public Shop BuildingShop { get; set; } = null;
        public Guru Guru { get; set; } = null;
        public SubLevel ParentSublevel { get; set; } = null;

        public bool ShouldShuffle { get; set; } = false;

        public byte x;
        public byte y;
        public byte pos;
        public byte positionScreen;
        public byte oldPos;

        public byte screen;
        public byte palette;
        public WorldNumber level;
        public DoorRandomizer.DoorRequirement key = DoorRandomizer.DoorRequirement.Nothing;

        public byte reqScreen;
        public byte reqPalette;

        public Door(DoorId id, OtherWorldNumber world, DoorRandomizer.PositionData position, List<DoorRandomizer.Requirement> requirements, DoorRandomizer.PositionData returnPosition=null, bool shouldShuffle=false, bool town=false)
        {
            Id = id;
            OriginalId = id;
            World = world;
            Position = position;
            if (town)
            {
                Requirement = requirements[position.index - 32];
            }
            else
            {
                Requirement = requirements[position.index];
            }
            ReturnPosition = returnPosition;
            if (ReturnPosition != null)
            {
                if (town)
                {
                    ReturnRequirement = requirements[ReturnPosition.index - 32];
                }
                else
                {
                    ReturnRequirement = requirements[ReturnPosition.index];
                }
            }

            ShouldShuffle = shouldShuffle;
        }

        public Door(Door building)
        {
            Id = building.Id;
            Sublevel = building.Sublevel;
            Gift = building.Gift;
            BuildingShop = building.BuildingShop;
            Guru = building.Guru;
            World = building.World;
            x = building.Position.X;
            y = building.Position.Y;
            pos = building.pos;
            positionScreen = building.positionScreen;
            oldPos = building.oldPos;
            screen = building.screen;
            palette = building.palette;
            key = building.key;
            level = building.level;
            reqScreen = building.Requirement.screen;
            reqPalette = building.Requirement.palette;
            Position = building.Position;
            Requirement = building.Requirement;
            ReturnPosition = building.ReturnPosition;
            ReturnRequirement = building.ReturnRequirement;
        }

        public void AddToContent(byte[] content)
        {
            Position.AddToContent(content);
            Requirement.AddToContent(content);
            if (ReturnPosition != null)
            {
                ReturnPosition.AddToContent(content);
            }
            if (ReturnRequirement != null)
            {
                ReturnRequirement.AddToContent(content);
            }
        }

        public void FinalizeGuru()
        {
            if (Guru != null)
            {
                Guru.World = World;
                Guru.X = (byte)(Position.X * 16);
                Guru.Y = (byte)(Position.Y * 16);
                Guru.Screen = Position.screen;
                Guru.WorldSpawn = worldSpawns[OriginalId];
            }
        }

        public WorldNumber GetWorld()
        {
            return worldDict[World];
        }

        public byte GetPalette(DoorId id)
        {
            if (ParentSublevel != null &&
                ParentSublevel.Palette != SubLevel.UndefinedPalette)
            {
                return ParentSublevel.Palette;
            }

            if (doorPalettes.ContainsKey(id))
            {
                return doorPalettes[id];
            }

            return 10;
        }

        public static Dictionary<OtherWorldNumber, WorldNumber> worldDict = new Dictionary<OtherWorldNumber, WorldNumber>()
        {
            { OtherWorldNumber.Eolis, WorldNumber.Eolis },
            { OtherWorldNumber.Trunk, WorldNumber.Trunk },
            { OtherWorldNumber.Mist, WorldNumber.Mist },
            { OtherWorldNumber.Branch, WorldNumber.Branch },
            { OtherWorldNumber.Dartmoor, WorldNumber.Dartmoor },
            { OtherWorldNumber.EvilOnesLair, WorldNumber.EvilOnesLair },
            { OtherWorldNumber.Towns, WorldNumber.Towns },
            { OtherWorldNumber.Buildings, WorldNumber.Buildings },
        };

        public static Dictionary<WorldNumber, OtherWorldNumber> OtherWorldDict = Util.Reverse(worldDict);

        private static Dictionary<DoorId, byte> doorPalettes = new Dictionary<DoorId, byte>()
        {
            { DoorId.TowerOfTrunk, 6},
            { DoorId.TowerOfFortress, 6},
            { DoorId.JokerHouse, 6},
            { DoorId.TrunkSecretShop, 6},
            { DoorId.FortressGuru, 7},
            { DoorId.MasconTower, 10},
            { DoorId.TowerOfSuffer, 10},
            { DoorId.VictimTower, 10},
            { DoorId.TowerOfMist, 10},
            { DoorId.MistSmallHouse, 10},
            { DoorId.MistGuru, 10},
            { DoorId.BirdHospital, 10},
            { DoorId.MistLargeHouse, 10},
            { DoorId.MistSecretShop, 10},
            { DoorId.FireMage, 10},
            { DoorId.AceKeyHouse, 10},
            { DoorId.BattleHelmetWing, 8},
            { DoorId.EastBranch, 8},
            { DoorId.EastBranchLeft, 8},
            { DoorId.DropdownWing, 8},
            { DoorId.CastleFraternal, 12},
            { DoorId.DartmoorLateDoor, 12},
            { DoorId.KingGrieve, 13},
            { DoorId.EvilOnesLair, 12},
            { DoorId.FraternalGuru, 13},
            { DoorId.FraternalHouse, 13},
            { DoorId.DartmoorHouse1, 12},
            { DoorId.DartmoorHouse2, 12},
            { DoorId.DartmoorHouse3, 12},
            { DoorId.DartmoorHouse4, 12},
            { DoorId.LeftOfDartmoor, 12},
            { DoorId.FarLeftDartmoor, 12},
        };

        private static Dictionary<DoorId, byte> worldSpawns = new Dictionary<DoorId, byte>()
        {
            { DoorId.TowerOfTrunk, 1 },
            { DoorId.TowerOfFortress, 1 },
            { DoorId.JokerHouse, 1 },
            { DoorId.TowerOfSuffer, 2 },
            { DoorId.MasconTower, 2 },
            { DoorId.TowerOfMist, 2 },
            { DoorId.VictimTower, 2 },
            { DoorId.BattleHelmetWing, 3 },
            { DoorId.EastBranch, 3 },
            { DoorId.EastBranchLeft, 3 },
            { DoorId.DropdownWing, 3 },
            { DoorId.CastleFraternal, 4 },
            { DoorId.KingGrieve, 4 },
            { DoorId.DartmoorLateDoor, 4 },
            { DoorId.DartmoorLateDoor2, 4 },
            { DoorId.EvilOnesLair, 4 },
            { DoorId.FinalDoor, 4 },
            { DoorId.EolisMeatShop, 0 },
            { DoorId.EolisHouse, 0 },
            { DoorId.EolisGuru, 0 },
            { DoorId.EolisKeyShop, 0 },
            { DoorId.EolisItemShop, 0 },
            { DoorId.EolisKing, 0 },
            { DoorId.MartialArtsShop, 0 },
            { DoorId.EolisMagicShop, 0 },
            { DoorId.TrunkSecretShop, 1 },
            { DoorId.FortressGuru, 1 },
            { DoorId.FireMage, 2 },
            { DoorId.MistSmallHouse, 2 },
            { DoorId.MistGuru, 2 },
            { DoorId.BirdHospital, 2 },
            { DoorId.MistSecretShop, 2 },
            { DoorId.MistLargeHouse, 2 },
            { DoorId.AceKeyHouse, 2 },
            { DoorId.DartmoorHouse1, 4 },
            { DoorId.DartmoorHouse2, 4 },
            { DoorId.DartmoorHouse3, 4 },
            { DoorId.DartmoorHouse4, 4 },
            { DoorId.LeftOfDartmoor, 4 },
            { DoorId.FarLeftDartmoor, 4 },
            { DoorId.FraternalGuru, 4 },
            { DoorId.FraternalHouse, 4 },
            { DoorId.ApoluneItemShop, 1 },
            { DoorId.ApoluneKeyShop, 1 },
            { DoorId.ApoluneBar, 1 },
            { DoorId.ApoluneGuru, 1 },
            { DoorId.ApoluneHospital, 1 },
            { DoorId.ApoluneHouse, 1 },
            { DoorId.ForepawMeatShop, 1 },
            { DoorId.ForepawItemShop, 1 },
            { DoorId.ForepawGuru, 1 },
            { DoorId.ForepawHospital, 1 },
            { DoorId.ForepawHouse, 1 },
            { DoorId.ForepawKeyShop, 1 },
            { DoorId.MasconBar, 2 },
            { DoorId.MasconMeatShop, 2 },
            { DoorId.MasconItemShop, 2 },
            { DoorId.MasconKeyShop, 2 },
            { DoorId.MasconHouse, 2 },
            { DoorId.MasconHospital, 2 },
            { DoorId.VictimGuru, 2 },
            { DoorId.VictimHospital, 2 },
            { DoorId.VictimHouse, 2 },
            { DoorId.VictimMeatShop, 2 },
            { DoorId.VictimKeyShop, 2 },
            { DoorId.VictimItemShop, 2 },
            { DoorId.VictimBar, 2 },
            { DoorId.ConflateHouse, 3 },
            { DoorId.ConflateMeatShop, 3 },
            { DoorId.ConflateGuru, 3 },
            { DoorId.ConflateHospital, 3 },
            { DoorId.ConflateItemShop, 3 },
            { DoorId.ConflateBar, 3 },
            { DoorId.DaybreakKeyShop, 3 },
            { DoorId.DaybreakItemShop, 3 },
            { DoorId.DaybreakMeatShop, 3 },
            { DoorId.DaybreakBar, 3 },
            { DoorId.DaybreakGuru, 3 },
            { DoorId.DaybreakHouse, 3 },
            { DoorId.DartmoorGuru, 4 },
            { DoorId.DartmoorBar, 4 },
            { DoorId.DartmoorMeatShop, 4 },
            { DoorId.DartmoorHospital, 4 },
            { DoorId.DartmoorKeyShop, 4 },
            { DoorId.DartmoorItemShop, 4 },
        };
    }
}
