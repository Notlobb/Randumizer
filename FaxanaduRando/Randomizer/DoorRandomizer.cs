using System;
using System.Collections.Generic;
using System.Linq;

namespace FaxanaduRando.Randomizer
{
    public enum WorldNumber : byte
    {
        Eolis,
        Trunk,
        Mist,
        Branch,
        Dartmoor,
        EvilOnesLair,
        Buildings,
        Towns,
        Unknown,
    }

    public enum OtherWorldNumber
    {
        Eolis,
        Trunk,
        Mist,
        Towns,
        Buildings,
        Branch,
        Dartmoor,
        EvilOnesLair,
        Unknown,
    }

    public class DoorRandomizer
    {
        public Dictionary<DoorId, Door> Buildings { get; set; } = new Dictionary<DoorId, Door>();
        public Dictionary<DoorId, Door> TowerDoors { get; set; } = new Dictionary<DoorId, Door>();
        public Dictionary<DoorId, Door> TownDoors { get; set; } = new Dictionary<DoorId, Door>();
        public Dictionary<DoorId, Door> LevelDoors { get; set; } = new Dictionary<DoorId, Door>();
        public Dictionary<DoorId, Door> Doors { get; set; } = new Dictionary<DoorId, Door>();
        public Dictionary<WorldNumber, World> Worlds = new Dictionary<WorldNumber, World>();

        private List<ShopRandomizer.Id> keyRequirements;
        private Table doorRequirementTable;
        private Table levelTable;
        private Table screenTable;
        private List<World> worlds;
        private GuruRandomizer guruRandomizer;

        public DoorRandomizer(byte[] content, Random random)
        {
            doorRequirementTable = new Table(Section.GetOffset(15, 0xE5DB, 0xC000), 12, 1, content);
            levelTable = new Table(Section.GetOffset(15, 0xE5E7, 0xC000), 12, 1, content);
            screenTable = new Table(Section.GetOffset(15, 0xE5F3, 0xC000), 12, 1, content);

            var roomTable = new Table(Section.GetOffset(15, 0xDDC5, 0xC000), 8, 1, content);
            var rooms = new List<byte> { 0, 29 };
            if (Util.GurusShuffled())
            {
                roomTable.Entries[0][0] = rooms[random.Next(rooms.Count)];
            }

            guruRandomizer = new GuruRandomizer(new Table(Section.GetOffset(15, 0xDDAD, 0xC000), 8, 1, content),
                                                new Table(Section.GetOffset(15, 0xDDB5, 0xC000), 8, 1, content),
                                                new Table(Section.GetOffset(15, 0xDDBD, 0xC000), 8, 1, content),
                                                roomTable,
                                                new Table(Section.GetOffset(15, 0xDDCD, 0xC000), 8, 1, content),
                                                new Table(Section.GetOffset(15, 0xDDD5, 0xC000), 8, 1, content));

            keyRequirements = new List<ShopRandomizer.Id>
            {
                ShopRandomizer.Id.Book,
                ShopRandomizer.Id.AceKey,
                ShopRandomizer.Id.KingKey,
                ShopRandomizer.Id.QueenKey,
                ShopRandomizer.Id.JackKey,
                ShopRandomizer.Id.JokerKey,
                ShopRandomizer.Id.ElfRing,
                ShopRandomizer.Id.DworfRing,
                ShopRandomizer.Id.DemonRing,
            };

            worlds = new List<World>();
            worlds.Add(new World(WorldNumber.Eolis));
            Worlds[WorldNumber.Eolis] = worlds[0];
            worlds.Add(new World(WorldNumber.Trunk));
            Worlds[WorldNumber.Trunk] = worlds[1];
            worlds.Add(new World(WorldNumber.Mist));
            Worlds[WorldNumber.Mist] = worlds[2];
            worlds.Add(new World(WorldNumber.Branch));
            Worlds[WorldNumber.Branch] = worlds[3];
            worlds.Add(new World(WorldNumber.Dartmoor));
            Worlds[WorldNumber.Dartmoor] = worlds[4];
            worlds.Add(new World(WorldNumber.EvilOnesLair));
            Worlds[WorldNumber.EvilOnesLair] = worlds[5];

            for (int i = 1; i < worlds.Count; i++)
            {
                worlds[i - 1].forward = worlds[i];
                worlds[i].backward = worlds[i - 1];
            }

            var eolisPositions = GetPositions(0, content);
            var eolisOffset = eolisPositions[0].offset;
            var trunkPositions = GetPositions(OtherWorldNumber.Trunk, content);
            trunkPositions = UpdateTrunkPositions(trunkPositions, content, eolisOffset);
            eolisPositions = UpdateEolisPositions(eolisPositions, content);
            var eolisReqs = GetRequirements(0, content);

            bool includeTownBuildings = GeneralOptions.MiscDoorSetting != GeneralOptions.MiscDoors.ShuffleExcludeTowns &&
                                        GeneralOptions.MiscDoorSetting != GeneralOptions.MiscDoors.Unchanged;
            bool includeKeyShops = includeTownBuildings && GeneralOptions.MiscDoorSetting != GeneralOptions.MiscDoors.ShuffleIncludeTownsExceptKeyShops &&
                                   GeneralOptions.MiscDoorSetting != GeneralOptions.MiscDoors.ShuffleIncludeTownsExceptGurusAndKeyshops;
            bool includeGurus = includeTownBuildings && GeneralOptions.MiscDoorSetting != GeneralOptions.MiscDoors.ShuffleIncludeTownsExceptGurus &&
                                GeneralOptions.MiscDoorSetting != GeneralOptions.MiscDoors.ShuffleIncludeTownsExceptGurusAndKeyshops;
            bool shuffleTowers = GeneralOptions.ShuffleTowers || GeneralOptions.DoorTypeSetting != GeneralOptions.DoorTypeShuffle.Unchanged;
            bool shuffleBuildings = GeneralOptions.MiscDoorSetting != GeneralOptions.MiscDoors.Unchanged || GeneralOptions.DoorTypeSetting != GeneralOptions.DoorTypeShuffle.Unchanged;

            bool includeEolisGuru = includeGurus && (GeneralOptions.FastStart || ItemOptions.RandomizeKeys != ItemOptions.KeyRandomization.Unchanged);

            TownDoors[DoorId.EolisMeatShop] = new Door(DoorId.EolisMeatShop, OtherWorldNumber.Eolis, eolisPositions[1], eolisReqs, shouldShuffle: includeTownBuildings);
            TownDoors[DoorId.EolisHouse] = new Door(DoorId.EolisHouse, OtherWorldNumber.Eolis, eolisPositions[2], eolisReqs, shouldShuffle: includeTownBuildings);
            TownDoors[DoorId.EolisGuru] = new Door(DoorId.EolisGuru, OtherWorldNumber.Eolis, eolisPositions[3], eolisReqs, shouldShuffle: includeEolisGuru);
            TownDoors[DoorId.EolisKeyShop] = new Door(DoorId.EolisKeyShop, OtherWorldNumber.Eolis, eolisPositions[4], eolisReqs, shouldShuffle: includeKeyShops);
            TownDoors[DoorId.EolisItemShop] = new Door(DoorId.EolisItemShop, OtherWorldNumber.Eolis, eolisPositions[5], eolisReqs);
            TownDoors[DoorId.MartialArtsShop] = new Door(DoorId.MartialArtsShop, OtherWorldNumber.Eolis, eolisPositions[7], eolisReqs, shouldShuffle: includeTownBuildings);
            TownDoors[DoorId.EolisMagicShop] = new Door(DoorId.EolisMagicShop, OtherWorldNumber.Eolis, eolisPositions[6], eolisReqs, shouldShuffle: includeTownBuildings);
            TownDoors[DoorId.EolisKing] = new Door(DoorId.EolisKing, OtherWorldNumber.Eolis, eolisPositions[8], eolisReqs);

            if (GeneralOptions.ShuffleTowers)
            {
                TowerDoors[DoorId.FirstDoor] = new Door(DoorId.FirstDoor, OtherWorldNumber.Eolis, eolisPositions[10], eolisReqs, eolisPositions[0], shouldShuffle: shuffleTowers);
            }
            else
            {
                LevelDoors[DoorId.FirstDoor] = new Door(DoorId.FirstDoor, OtherWorldNumber.Eolis, eolisPositions[0], eolisReqs);
                LevelDoors[DoorId.FirstDoorReturn] = new Door(DoorId.FirstDoor, OtherWorldNumber.Eolis, eolisPositions[10], eolisReqs);
            }

            worlds[0].forwardPosition = eolisPositions[9];

            var trunkReqs = GetRequirements(OtherWorldNumber.Trunk, content);

            worlds[1].backwardPosition = trunkPositions[8];
            worlds[1].forwardPosition = trunkPositions[9];

            TowerDoors[DoorId.TowerOfTrunk] = new Door(DoorId.TowerOfTrunk, OtherWorldNumber.Trunk, trunkPositions[0], trunkReqs, trunkPositions[1], shouldShuffle: shuffleTowers);
            TowerDoors[DoorId.TowerOfFortress] = new Door(DoorId.TowerOfFortress, OtherWorldNumber.Trunk, trunkPositions[2], trunkReqs, trunkPositions[3], shouldShuffle: shuffleTowers);
            TowerDoors[DoorId.JokerHouse] = new Door(DoorId.JokerHouse, OtherWorldNumber.Trunk, trunkPositions[4], trunkReqs, trunkPositions[5], shouldShuffle: shuffleTowers);

            Buildings[DoorId.TrunkSecretShop] = new Door(DoorId.TrunkSecretShop, OtherWorldNumber.Trunk, trunkPositions[6], trunkReqs, shouldShuffle: shuffleBuildings);
            Buildings[DoorId.FortressGuru] = new Door(DoorId.FortressGuru, OtherWorldNumber.Trunk, trunkPositions[7], trunkReqs, shouldShuffle: shuffleBuildings);

            var mistPositions = GetPositions(OtherWorldNumber.Mist, content);
            var mistReqs = GetRequirements(OtherWorldNumber.Mist, content);

            TowerDoors[DoorId.TowerOfSuffer] = new Door(DoorId.TowerOfSuffer, OtherWorldNumber.Mist, mistPositions[0], mistReqs, mistPositions[4], shouldShuffle: shuffleTowers);
            TowerDoors[DoorId.MasconTower] = new Door(DoorId.MasconTower, OtherWorldNumber.Mist, mistPositions[3], mistReqs, mistPositions[6], shouldShuffle: shuffleTowers);
            TowerDoors[DoorId.TowerOfMist] = new Door(DoorId.TowerOfMist, OtherWorldNumber.Mist, mistPositions[1], mistReqs, mistPositions[5], shouldShuffle: shuffleTowers);
            TowerDoors[DoorId.VictimTower] = new Door(DoorId.VictimTower, OtherWorldNumber.Mist, mistPositions[2], mistReqs, mistPositions[7], shouldShuffle: shuffleTowers);

            Buildings[DoorId.FireMage] = new Door(DoorId.FireMage, OtherWorldNumber.Mist, mistPositions[8], mistReqs, shouldShuffle: shuffleBuildings);
            Buildings[DoorId.MistSmallHouse] = new Door(DoorId.MistSmallHouse, OtherWorldNumber.Mist, mistPositions[9], mistReqs, shouldShuffle: shuffleBuildings);
            Buildings[DoorId.MistGuru] = new Door(DoorId.MistGuru, OtherWorldNumber.Mist, mistPositions[10], mistReqs, shouldShuffle: includeGurus);
            Buildings[DoorId.BirdHospital] = new Door(DoorId.BirdHospital, OtherWorldNumber.Mist, mistPositions[11], mistReqs, shouldShuffle: shuffleBuildings);
            Buildings[DoorId.MistSecretShop] = new Door(DoorId.MistSecretShop, OtherWorldNumber.Mist, mistPositions[12], mistReqs, shouldShuffle: shuffleBuildings);
            Buildings[DoorId.MistLargeHouse] = new Door(DoorId.MistLargeHouse, OtherWorldNumber.Mist, mistPositions[13], mistReqs, shouldShuffle: shuffleBuildings);
            Buildings[DoorId.AceKeyHouse] = new Door(DoorId.AceKeyHouse, OtherWorldNumber.Mist, mistPositions[14], mistReqs, shouldShuffle: shuffleBuildings);

            worlds[2].backwardPosition = mistPositions[15];
            worlds[2].forwardPosition = mistPositions[16];

            var branchPositions = GetPositions(OtherWorldNumber.Branch, content);
            var branchReqs = GetRequirements(OtherWorldNumber.Branch, content);

            TowerDoors[DoorId.BattleHelmetWing] = new Door(DoorId.BattleHelmetWing, OtherWorldNumber.Branch, branchPositions[0], branchReqs, branchPositions[1], shouldShuffle: shuffleTowers);

            LevelDoors[DoorId.EastBranch] = new Door(DoorId.EastBranch, OtherWorldNumber.Branch, branchPositions[2], branchReqs, branchPositions[3]);
            LevelDoors[DoorId.EastBranchLeft] = new Door(DoorId.EastBranchLeft, OtherWorldNumber.Branch, branchPositions[5], branchReqs, branchPositions[4]);
            LevelDoors[DoorId.DropdownWing] = new Door(DoorId.DropdownWing, OtherWorldNumber.Branch, branchPositions[7], branchReqs, branchPositions[6]);

            worlds[3].backwardPosition = branchPositions[8];
            worlds[3].forwardPosition = branchPositions[9];

            var dartmoorPositions = GetPositions(OtherWorldNumber.Dartmoor, content);
            var dartmoorReqs = GetRequirements(OtherWorldNumber.Dartmoor, content);

            TowerDoors[DoorId.CastleFraternal] = new Door(DoorId.CastleFraternal, OtherWorldNumber.Dartmoor, dartmoorPositions[0], dartmoorReqs, dartmoorPositions[1], shouldShuffle: shuffleTowers);
            TowerDoors[DoorId.KingGrieve] = new Door(DoorId.KingGrieve, OtherWorldNumber.Dartmoor, dartmoorPositions[2], dartmoorReqs, dartmoorPositions[3], shouldShuffle: shuffleTowers);

            LevelDoors[DoorId.DartmoorLateDoor] = new Door(DoorId.DartmoorLateDoor, OtherWorldNumber.Dartmoor, dartmoorPositions[4], dartmoorReqs);
            LevelDoors[DoorId.DartmoorLateDoor2] = new Door(DoorId.DartmoorLateDoor2, OtherWorldNumber.Dartmoor, dartmoorPositions[5], dartmoorReqs);

            Buildings[DoorId.DartmoorHouse1] = new Door(DoorId.DartmoorHouse1, OtherWorldNumber.Dartmoor, dartmoorPositions[6], dartmoorReqs, shouldShuffle: shuffleBuildings);
            Buildings[DoorId.DartmoorHouse2] = new Door(DoorId.DartmoorHouse2, OtherWorldNumber.Dartmoor, dartmoorPositions[7], dartmoorReqs, shouldShuffle: shuffleBuildings);
            Buildings[DoorId.DartmoorHouse3] = new Door(DoorId.DartmoorHouse3, OtherWorldNumber.Dartmoor, dartmoorPositions[8], dartmoorReqs, shouldShuffle: shuffleBuildings);
            Buildings[DoorId.DartmoorHouse4] = new Door(DoorId.DartmoorHouse4, OtherWorldNumber.Dartmoor, dartmoorPositions[9], dartmoorReqs, shouldShuffle: shuffleBuildings);
            Buildings[DoorId.LeftOfDartmoor] = new Door(DoorId.LeftOfDartmoor, OtherWorldNumber.Dartmoor, dartmoorPositions[10], dartmoorReqs, shouldShuffle: shuffleBuildings);
            Buildings[DoorId.FarLeftDartmoor] = new Door(DoorId.FarLeftDartmoor, OtherWorldNumber.Dartmoor, dartmoorPositions[11], dartmoorReqs, shouldShuffle: shuffleBuildings);
            Buildings[DoorId.FraternalGuru] = new Door(DoorId.FraternalGuru, OtherWorldNumber.Dartmoor, dartmoorPositions[12], dartmoorReqs, shouldShuffle: shuffleBuildings);
            Buildings[DoorId.FraternalHouse] = new Door(DoorId.FraternalHouse, OtherWorldNumber.Dartmoor, dartmoorPositions[13], dartmoorReqs, shouldShuffle: shuffleBuildings);

            worlds[4].backwardPosition = dartmoorPositions[14];
            worlds[4].forwardPosition = dartmoorPositions[15];

            var evilPositions = GetPositions(OtherWorldNumber.EvilOnesLair, content);
            var evilReqs = GetRequirements(OtherWorldNumber.EvilOnesLair, content);

            LevelDoors[DoorId.FinalDoor] = new Door(DoorId.FinalDoor, OtherWorldNumber.EvilOnesLair, evilPositions[0], evilReqs);

            worlds[5].backwardPosition = evilPositions[1];

            if (GeneralOptions.ShuffleTowers)
            {
                if (GeneralOptions.IncludeEvilOnesFortress)
                {
                    var tmp = evilPositions[0].index;
                    evilPositions[0].index = evilPositions[1].index;
                    evilPositions[1].index = tmp;

                    tmp = evilPositions[0].pos;
                    evilPositions[0].pos = evilPositions[1].pos;
                    evilPositions[1].pos = tmp;

                    worlds[5].backwardPosition = evilPositions[0];
                    evilPositions[1].SetPos(12, 10);

                    TowerDoors[DoorId.EvilOnesLair] = new Door(DoorId.EvilOnesLair, OtherWorldNumber.Dartmoor, dartmoorPositions[6], dartmoorReqs, shouldShuffle: true);
                    Buildings.Remove(DoorId.DartmoorHouse1);
                    TowerDoors[DoorId.EvilOnesLair].Position.index = 6;
                    TowerDoors[DoorId.EvilOnesLair].OriginalId = DoorId.DartmoorHouse1;
                    TowerDoors[DoorId.EvilOnesLair].Position.pos = Worlds[WorldNumber.Dartmoor].forwardPosition.pos;
                    TowerDoors[DoorId.EvilOnesLair].Requirement = dartmoorReqs[6];

                    TowerDoors[DoorId.EvilOnesLair].ReturnPosition = evilPositions[1];

                    TowerDoors[DoorId.EvilOnesLair].ReturnRequirement = evilReqs[0];
                    TowerDoors[DoorId.EvilOnesLair].ReturnRequirement.level = WorldNumber.Dartmoor;
                    TowerDoors[DoorId.EvilOnesLair].ReturnRequirement.screen = 14;
                    TowerDoors[DoorId.EvilOnesLair].ReturnRequirement.palette = 12;

                    TowerDoors[DoorId.EvilOnesLair].Requirement.level = WorldNumber.EvilOnesLair;
                    TowerDoors[DoorId.EvilOnesLair].Requirement.palette = 15;
                    TowerDoors[DoorId.EvilOnesLair].Requirement.screen = 8;
                    TowerDoors[DoorId.EvilOnesLair].Requirement.key = DoorRequirement.Nothing;
                }

                LevelDoors[DoorId.DartmoorLateDoor].Requirement.level = WorldNumber.Dartmoor;
                LevelDoors[DoorId.DartmoorLateDoor2].Requirement.level = WorldNumber.Dartmoor;
                LevelDoors[DoorId.FinalDoor].Requirement.level = WorldNumber.EvilOnesLair;

                TowerDoors[DoorId.TowerOfTrunk].Requirement.level = WorldNumber.Trunk;
                TowerDoors[DoorId.TowerOfTrunk].ReturnRequirement.level = WorldNumber.Trunk;
                TowerDoors[DoorId.TowerOfFortress].Requirement.level = WorldNumber.Trunk;
                TowerDoors[DoorId.TowerOfFortress].ReturnRequirement.level = WorldNumber.Trunk;
                TowerDoors[DoorId.JokerHouse].Requirement.level = WorldNumber.Trunk;
                TowerDoors[DoorId.JokerHouse].ReturnRequirement.level = WorldNumber.Trunk;
                TowerDoors[DoorId.TowerOfSuffer].Requirement.level = WorldNumber.Mist;
                TowerDoors[DoorId.TowerOfSuffer].ReturnRequirement.level = WorldNumber.Mist;
                TowerDoors[DoorId.MasconTower].Requirement.level = WorldNumber.Mist;
                TowerDoors[DoorId.MasconTower].ReturnRequirement.level = WorldNumber.Mist;
                TowerDoors[DoorId.TowerOfMist].Requirement.level = WorldNumber.Mist;
                TowerDoors[DoorId.TowerOfMist].ReturnRequirement.level = WorldNumber.Mist;
                TowerDoors[DoorId.VictimTower].Requirement.level = WorldNumber.Mist;
                TowerDoors[DoorId.VictimTower].ReturnRequirement.level = WorldNumber.Mist;

                TowerDoors[DoorId.BattleHelmetWing].Requirement.level = WorldNumber.Branch;
                TowerDoors[DoorId.BattleHelmetWing].ReturnRequirement.level = WorldNumber.Branch;

                LevelDoors[DoorId.EastBranch].Requirement.level = WorldNumber.Branch;
                LevelDoors[DoorId.EastBranch].ReturnRequirement.level = WorldNumber.Branch;
                LevelDoors[DoorId.EastBranchLeft].Requirement.level = WorldNumber.Branch;
                LevelDoors[DoorId.EastBranchLeft].ReturnRequirement.level = WorldNumber.Branch;
                LevelDoors[DoorId.DropdownWing].Requirement.level = WorldNumber.Branch;
                LevelDoors[DoorId.DropdownWing].ReturnRequirement.level = WorldNumber.Branch;

                TowerDoors[DoorId.CastleFraternal].Requirement.level = WorldNumber.Dartmoor;
                TowerDoors[DoorId.CastleFraternal].ReturnRequirement.level = WorldNumber.Dartmoor;
                TowerDoors[DoorId.KingGrieve].Requirement.level = WorldNumber.Dartmoor;
                TowerDoors[DoorId.KingGrieve].ReturnRequirement.level = WorldNumber.Dartmoor;

                TowerDoors[DoorId.BattleHelmetWing].ReturnRequirement.palette = 8;
            }

            for (int i = 0; i < worlds.Count; i++)
            {
                worlds[i].backwardScreen = screenTable.Entries[i * 2][0];
                worlds[i].forwardScreen = screenTable.Entries[i * 2 + 1][0];
            }

            worlds[0].exitScreen = worlds[1].backwardScreen;
            for (int i = 1; i < worlds.Count - 1; i++)
            {
                worlds[i].entryScreen = worlds[i - 1].forwardScreen;
                worlds[i].exitScreen = worlds[i + 1].backwardScreen;
            }
            worlds[worlds.Count - 1].entryScreen = worlds[worlds.Count - 2].forwardScreen;

            worlds[0].exitPos = worlds[1].backwardPosition.pos;
            for (int i = 1; i < worlds.Count - 1; i++)
            {
                worlds[i].entryPos = worlds[i - 1].forwardPosition.pos;
                worlds[i].exitPos = worlds[i + 1].backwardPosition.pos;
            }
            worlds[worlds.Count - 1].entryPos = worlds[worlds.Count - 2].forwardPosition.pos;

            if (GeneralOptions.MoveFinalRequirements)
            {
                LevelDoors[DoorId.DartmoorLateDoor].Requirement.key = DoorRequirement.Nothing;
                LevelDoors[DoorId.DartmoorLateDoor2].Requirement.key = DoorRequirement.Nothing;
                doorRequirementTable.Entries[(int)ExitDoor.DartmoorExit][0] = 0;

                if (GeneralOptions.ShuffleTowers && GeneralOptions.IncludeEvilOnesFortress)
                {
                    doorRequirementTable.Entries[(int)ExitDoor.EvilLairExit2][0] = (byte)DoorRequirement.DemonRing;
                    LevelDoors[DoorId.FinalDoor].Requirement.key = DoorRequirement.Nothing;
                }
                else
                {
                    LevelDoors[DoorId.FinalDoor].Requirement.key = DoorRequirement.DemonRing;
                }
            }

            var townPositions = GetPositions(OtherWorldNumber.Towns, content);
            var townReqs = GetRequirements(OtherWorldNumber.Towns, content);
            TownDoors[DoorId.ApoluneItemShop] = new Door(DoorId.ApoluneItemShop, OtherWorldNumber.Towns, townPositions[0], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.ApoluneKeyShop] = new Door(DoorId.ApoluneKeyShop, OtherWorldNumber.Towns, townPositions[1], townReqs, shouldShuffle: includeKeyShops, town: true);
            TownDoors[DoorId.ApoluneBar] = new Door(DoorId.ApoluneBar, OtherWorldNumber.Towns, townPositions[2], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.ApoluneGuru] = new Door(DoorId.ApoluneGuru, OtherWorldNumber.Towns, townPositions[3], townReqs, shouldShuffle: includeGurus, town: true);
            TownDoors[DoorId.ApoluneHospital] = new Door(DoorId.ApoluneHospital, OtherWorldNumber.Towns, townPositions[4], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.ApoluneHouse] = new Door(DoorId.ApoluneHouse, OtherWorldNumber.Towns, townPositions[5], townReqs, shouldShuffle: includeTownBuildings, town: true);

            TownDoors[DoorId.ForepawMeatShop] = new Door(DoorId.ForepawMeatShop, OtherWorldNumber.Towns, townPositions[6], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.ForepawItemShop] = new Door(DoorId.ForepawItemShop, OtherWorldNumber.Towns, townPositions[7], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.ForepawGuru] = new Door(DoorId.ForepawGuru, OtherWorldNumber.Towns, townPositions[8], townReqs, shouldShuffle: includeGurus, town: true);
            TownDoors[DoorId.ForepawHospital] = new Door(DoorId.ForepawHospital, OtherWorldNumber.Towns, townPositions[9], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.ForepawHouse] = new Door(DoorId.ForepawKeyShop, OtherWorldNumber.Towns, townPositions[10], townReqs, shouldShuffle: includeKeyShops, town: true);
            TownDoors[DoorId.ForepawKeyShop] = new Door(DoorId.ForepawKeyShop, OtherWorldNumber.Towns, townPositions[11], townReqs, shouldShuffle: includeKeyShops, town: true);

            TownDoors[DoorId.MasconBar] = new Door(DoorId.MasconBar, OtherWorldNumber.Towns, townPositions[12], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.MasconMeatShop] = new Door(DoorId.MasconMeatShop, OtherWorldNumber.Towns, townPositions[13], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.MasconItemShop] = new Door(DoorId.MasconItemShop, OtherWorldNumber.Towns, townPositions[14], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.MasconKeyShop] = new Door(DoorId.MasconKeyShop, OtherWorldNumber.Towns, townPositions[15], townReqs, shouldShuffle: includeKeyShops, town: true);
            TownDoors[DoorId.MasconHouse] = new Door(DoorId.MasconHouse, OtherWorldNumber.Towns, townPositions[16], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.MasconHospital] = new Door(DoorId.MasconHospital, OtherWorldNumber.Towns, townPositions[17], townReqs, shouldShuffle: includeTownBuildings, town: true);

            TownDoors[DoorId.VictimGuru] = new Door(DoorId.VictimGuru, OtherWorldNumber.Towns, townPositions[18], townReqs, shouldShuffle: includeGurus, town: true);
            TownDoors[DoorId.VictimHospital] = new Door(DoorId.VictimHospital, OtherWorldNumber.Towns, townPositions[19], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.VictimHouse] = new Door(DoorId.VictimHouse, OtherWorldNumber.Towns, townPositions[20], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.VictimMeatShop] = new Door(DoorId.VictimMeatShop, OtherWorldNumber.Towns, townPositions[21], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.VictimKeyShop] = new Door(DoorId.VictimKeyShop, OtherWorldNumber.Towns, townPositions[22], townReqs, shouldShuffle: includeKeyShops, town: true);
            TownDoors[DoorId.VictimBar] = new Door(DoorId.VictimBar, OtherWorldNumber.Towns, townPositions[23], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.VictimItemShop] = new Door(DoorId.VictimItemShop, OtherWorldNumber.Towns, townPositions[24], townReqs, shouldShuffle: includeTownBuildings, town: true);

            TownDoors[DoorId.ConflateHouse] = new Door(DoorId.ConflateHouse, OtherWorldNumber.Towns, townPositions[25], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.ConflateMeatShop] = new Door(DoorId.ConflateMeatShop, OtherWorldNumber.Towns, townPositions[26], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.ConflateGuru] = new Door(DoorId.ConflateGuru, OtherWorldNumber.Towns, townPositions[27], townReqs, shouldShuffle: includeGurus, town: true);
            TownDoors[DoorId.ConflateHospital] = new Door(DoorId.ConflateHospital, OtherWorldNumber.Towns, townPositions[28], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.ConflateItemShop] = new Door(DoorId.ConflateItemShop, OtherWorldNumber.Towns, townPositions[29], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.ConflateBar] = new Door(DoorId.ConflateBar, OtherWorldNumber.Towns, townPositions[30], townReqs, shouldShuffle: includeTownBuildings, town: true);

            TownDoors[DoorId.DaybreakKeyShop] = new Door(DoorId.DaybreakKeyShop, OtherWorldNumber.Towns, townPositions[31], townReqs, shouldShuffle: includeKeyShops, town: true);
            TownDoors[DoorId.DaybreakItemShop] = new Door(DoorId.DaybreakItemShop, OtherWorldNumber.Towns, townPositions[32], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.DaybreakMeatShop] = new Door(DoorId.DaybreakMeatShop, OtherWorldNumber.Towns, townPositions[33], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.DaybreakBar] = new Door(DoorId.DaybreakBar, OtherWorldNumber.Towns, townPositions[34], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.DaybreakGuru] = new Door(DoorId.DaybreakGuru, OtherWorldNumber.Towns, townPositions[35], townReqs, shouldShuffle: includeGurus, town: true);
            TownDoors[DoorId.DaybreakHouse] = new Door(DoorId.DaybreakHouse, OtherWorldNumber.Towns, townPositions[36], townReqs, shouldShuffle: includeTownBuildings, town: true);

            TownDoors[DoorId.DartmoorGuru] = new Door(DoorId.DartmoorGuru, OtherWorldNumber.Towns, townPositions[37], townReqs, shouldShuffle: includeGurus, town: true);
            TownDoors[DoorId.DartmoorBar] = new Door(DoorId.DartmoorBar, OtherWorldNumber.Towns, townPositions[38], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.DartmoorMeatShop] = new Door(DoorId.DartmoorMeatShop, OtherWorldNumber.Towns, townPositions[39], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.DartmoorHospital] = new Door(DoorId.DartmoorHospital, OtherWorldNumber.Towns, townPositions[40], townReqs, shouldShuffle: includeTownBuildings, town: true);
            TownDoors[DoorId.DartmoorKeyShop] = new Door(DoorId.DartmoorKeyShop, OtherWorldNumber.Towns, townPositions[41], townReqs, shouldShuffle: includeKeyShops, town: true);
            TownDoors[DoorId.DartmoorItemShop] = new Door(DoorId.DartmoorItemShop, OtherWorldNumber.Towns, townPositions[42], townReqs, shouldShuffle: includeTownBuildings, town: true);

            foreach (var key in TowerDoors.Keys)
            {
                Doors[key] = TowerDoors[key];
            }

            foreach (var key in Buildings.Keys)
            {
                Doors[key] = Buildings[key];
            }

            foreach (var key in TownDoors.Keys)
            {
                Doors[key] = TownDoors[key];
            }

            foreach (var key in LevelDoors.Keys)
            {
                Doors[key] = LevelDoors[key];
            }

            foreach (var door in Doors.Values)
            {
                door.key = door.Requirement.key;
            }

            if (ItemOptions.RandomizeKeys != ItemOptions.KeyRandomization.Unchanged)
            {
                Doors[DoorId.EolisKing].key = DoorRequirement.Nothing;
                if (!ItemOptions.IncludeSomeEolisDoors)
                {
                    Doors[DoorId.MartialArtsShop].key = DoorRequirement.Nothing;
                    Doors[DoorId.EolisMagicShop].key = DoorRequirement.Nothing;
                    doorRequirementTable.Entries[(int)ExitDoor.EolisExit][0] = 0;
                }
            }

            if (Util.KeyShopsShuffled() ||
                ItemOptions.RandomizeKeys != ItemOptions.KeyRandomization.Unchanged ||
                GeneralOptions.ShuffleSegments != GeneralOptions.SegmentShuffle.Unchanged)
            {
                Doors[DoorId.EastBranchLeft].key = DoorRequirement.Nothing;
            }

            if (Util.AllCoreWorldScreensRandomized())
            {
                TowerDoors[DoorId.EastBranch] = LevelDoors[DoorId.EastBranch];
                TowerDoors[DoorId.EastBranchLeft] = LevelDoors[DoorId.EastBranchLeft];
                TowerDoors[DoorId.DropdownWing] = LevelDoors[DoorId.DropdownWing];
                LevelDoors.Remove(DoorId.EastBranch);
                LevelDoors.Remove(DoorId.EastBranchLeft);
                LevelDoors.Remove(DoorId.DropdownWing);

                if (GeneralOptions.ShuffleTowers)
                {
                    TowerDoors[DoorId.EastBranch].ShouldShuffle = true;
                    TowerDoors[DoorId.EastBranchLeft].ShouldShuffle = true;
                    TowerDoors[DoorId.DropdownWing].ShouldShuffle = true;
                }
            }

            foreach (var tower in TowerDoors.Values)
            {
                tower.pos = tower.Position.pos;
                tower.positionScreen = tower.Position.screen;
                tower.oldPos = tower.Position.oldPos;
                tower.screen = tower.Requirement.screen;
                tower.palette = tower.Requirement.palette;
                tower.level = tower.Requirement.level;
            }

            foreach (var building in Buildings.Values)
            {
                building.oldPos = building.Position.oldPos;
                building.positionScreen = building.Position.screen;
            }
        }

        public List<World> GetWorlds()
        {
            return worlds;
        }

        public void UpdateBuildings(GiftRandomizer giftRandomizer, ShopRandomizer shopRandomizer)
        {
            TowerDoors[DoorId.TowerOfTrunk].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.TowerOfTrunk];
            TowerDoors[DoorId.TowerOfFortress].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.TowerOfFortress];
            TowerDoors[DoorId.JokerHouse].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.JokerHouse];

            Buildings[DoorId.TrunkSecretShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.ApoluneSecretShop];
            Buildings[DoorId.FortressGuru].Gift = giftRandomizer.ItemDict[GiftItem.Id.FortressGuru];

            TowerDoors[DoorId.MasconTower].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.MasconTower];
            TowerDoors[DoorId.TowerOfSuffer].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.TowerOfSuffer];
            TowerDoors[DoorId.VictimTower].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.VictimTower];
            TowerDoors[DoorId.TowerOfMist].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.TowerOfMist];

            Buildings[DoorId.FireMage].Gift = giftRandomizer.ItemDict[GiftItem.Id.FireMage];
            Buildings[DoorId.MistGuru].Guru = guruRandomizer.Gurus[Guru.GuruId.Mascon];
            Buildings[DoorId.BirdHospital].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.BirdHospital];
            Buildings[DoorId.MistSecretShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.MasconSecretShop];
            Buildings[DoorId.AceKeyHouse].Gift = giftRandomizer.ItemDict[GiftItem.Id.AceKeyHouse];

            TowerDoors[DoorId.BattleHelmetWing].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.BattleHelmetWing];

            TowerDoors[DoorId.CastleFraternal].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.CastleFraternal];
            TowerDoors[DoorId.KingGrieve].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.KingGrieve];

            Doors[DoorId.EastBranch].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.EastBranch];
            Doors[DoorId.EastBranchLeft].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.BackFromEastBranch];
            Doors[DoorId.DropdownWing].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.DropDownWing];

            if (GeneralOptions.ShuffleTowers)
            {
                TowerDoors[DoorId.FirstDoor].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.OutsideEolis];
                if (GeneralOptions.IncludeEvilOnesFortress)
                {
                    TowerDoors[DoorId.EvilOnesLair].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.EvilOnesLair];
                }
            }

            Buildings[DoorId.DartmoorHouse3].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.DartmoorHouse];
            Buildings[DoorId.FraternalHouse].Sublevel = SubLevel.SubLevelDict[SubLevel.Id.FraternalHouse];
            Buildings[DoorId.FraternalGuru].Gift = giftRandomizer.ItemDict[GiftItem.Id.FraternalGuru];

            TownDoors[DoorId.EolisItemShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.EolisItemShop];
            TownDoors[DoorId.EolisKeyShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.EolisKeyShop];
            TownDoors[DoorId.EolisGuru].Gift = giftRandomizer.ItemDict[GiftItem.Id.EolisGuru];
            TownDoors[DoorId.EolisGuru].Guru = guruRandomizer.Gurus[Guru.GuruId.Eolis];

            TownDoors[DoorId.ApoluneItemShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.ApoluneItemShop];
            TownDoors[DoorId.ApoluneKeyShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.ApoluneKeyShop];
            TownDoors[DoorId.ApoluneGuru].Guru = guruRandomizer.Gurus[Guru.GuruId.Apolune];

            TownDoors[DoorId.ForepawItemShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.ForepawItemShop];
            TownDoors[DoorId.ForepawKeyShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.ForepawKeyShop];
            TownDoors[DoorId.ForepawGuru].Guru = guruRandomizer.Gurus[Guru.GuruId.Forepaw];

            TownDoors[DoorId.MasconItemShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.MasconItemShop];
            TownDoors[DoorId.MasconKeyShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.MasconKeyShop];

            TownDoors[DoorId.VictimItemShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.VictimItemShop];
            TownDoors[DoorId.VictimKeyShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.VictimKeyShop];
            TownDoors[DoorId.VictimBar].Gift = giftRandomizer.ItemDict[GiftItem.Id.VictimBar];
            TownDoors[DoorId.VictimGuru].Guru = guruRandomizer.Gurus[Guru.GuruId.Victim];

            TownDoors[DoorId.ConflateItemShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.ConflateItemShop];
            TownDoors[DoorId.ConflateGuru].Gift = giftRandomizer.ItemDict[GiftItem.Id.ConflateGuru];
            TownDoors[DoorId.ConflateGuru].Guru = guruRandomizer.Gurus[Guru.GuruId.Conflate];

            TownDoors[DoorId.DaybreakItemShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.DaybreakItemShop];
            TownDoors[DoorId.DaybreakKeyShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.DaybreakKeyShop];
            TownDoors[DoorId.DaybreakGuru].Guru = guruRandomizer.Gurus[Guru.GuruId.Daybreak];

            TownDoors[DoorId.DartmoorItemShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.DartmoorItemShop];
            TownDoors[DoorId.DartmoorKeyShop].BuildingShop = shopRandomizer.ShopDict[Shop.Id.DartmoorKeyShop];
            TownDoors[DoorId.DartmoorGuru].Guru = guruRandomizer.Gurus[Guru.GuruId.Dartmoor];
        }

        public void LimitKeys(Random random)
        {
            if (ItemOptions.SmallKeyLimit == ItemOptions.KeyLimit.NoLimit &&
                ItemOptions.BigKeyLimit == ItemOptions.KeyLimit.NoLimit)
            {
                return;
            }

            var counts = new Dictionary<DoorRequirement, int>();
            bool worldDoorsFirst = false;
            if (random.Next(0, 2) == 0)
            {
                worldDoorsFirst = true;
            }

            if (worldDoorsFirst)
            {
                LimitWorldDoors(counts, random);
            }

            var doors = new List<Door>(Doors.Values);
            Util.ShuffleList(doors, 0, doors.Count - 1, random);
            foreach (var door in doors)
            {
                int limit = GetKeyLimit(door.key);
                if(LimitKey(door.key, limit, counts))
                {
                    door.key = DoorRequirement.Nothing;
                }
            }

            if (!worldDoorsFirst)
            {
                LimitWorldDoors(counts, random);
            }
        }

        public void ShuffleTowers(Random random)
        {
            var towers = new List<Door>(Doors.Values.Where(t => IsTower(t.Id) &&
                                                           t.ShouldShuffle));
            var tmp = new List<Door>();
            foreach (var tower in towers)
            {
                tmp.Add(new Door(tower));
            }

            Util.ShuffleList(tmp, 0, tmp.Count - 1, random);

            for (int i = 0; i < towers.Count; i++)
            {
                SetDoorValues(towers[i], tmp[i], true);
            }
        }

        public void ShuffleDoorTypes(Random random)
        {
            ShuffleDoorTypes(OtherWorldNumber.Trunk, random);
            ShuffleDoorTypes(OtherWorldNumber.Mist, random);
            ShuffleDoorTypes(OtherWorldNumber.Dartmoor, random);
        }

        public void ShuffleDoorTypes(OtherWorldNumber world, Random random)
        {
            var towers = TowerDoors.Values.Where(d => d.World == world && d.ShouldShuffle);
            var buildings = Buildings.Values.Where(d => d.World == world && d.ShouldShuffle);
            var locations = new List<Door>(towers);
            locations.AddRange(buildings);

            var tempDoors = new List<Door>();
            for (int i = 0; i < locations.Count; i++)
            {
                tempDoors.Add(new Door(locations[i]));
            }

            Util.ShuffleList(tempDoors, 0, tempDoors.Count - 1, random);

            for (int i = 0; i < locations.Count; i++)
            {
                SetDoorValues(locations[i], tempDoors[i]);
            }

            for (int i = 0; i < locations.Count; i++)
            {
                locations[i].Position.oldPos = locations[i].oldPos;
                locations[i].Position.screen = locations[i].positionScreen;

                if (locations[i].ReturnRequirement != null)
                {
                    locations[i].ReturnPosition.pos = locations[i].Position.oldPos;
                    locations[i].ReturnRequirement.screen = locations[i].Position.screen;
                    locations[i].ReturnRequirement.palette = locations[i].GetPalette(locations[i].OriginalId);
                }

                if (GeneralOptions.DoorTypeSetting == GeneralOptions.DoorTypeShuffle.ShuffleMoveKeys)
                {
                    locations[i].key = tempDoors[i].key;
                }

                if (IsTower(locations[i].Id))
                {
                    locations[i].pos = tempDoors[i].pos;
                    locations[i].screen = tempDoors[i].screen;
                    locations[i].palette = tempDoors[i].palette;
                    locations[i].level = tempDoors[i].level;
                }
            }
        }

        public void ShuffleMiscDoors(Random random)
        {
            var buildingList = new List<Door>(Buildings.Values.Where(b => b.ShouldShuffle &&
                                                                     !IsTower(b.Id)));
            buildingList.AddRange(TowerDoors.Values.Where(t => t.ShouldShuffle && !IsTower(t.Id)));
            buildingList.AddRange(TownDoors.Values.Where(d => d.ShouldShuffle));

            var tempBuildings = new List<Door>();
            foreach (var building in buildingList)
            {
                tempBuildings.Add(new Door(building));
            }

            Util.ShuffleList(tempBuildings, 0, tempBuildings.Count - 1, random);

            for (int i = 0; i < buildingList.Count; i++)
            {
                buildingList[i].Id = tempBuildings[i].Id;
                buildingList[i].Sublevel = tempBuildings[i].Sublevel;
                buildingList[i].Gift = tempBuildings[i].Gift;
                buildingList[i].BuildingShop = tempBuildings[i].BuildingShop;
                buildingList[i].Guru = tempBuildings[i].Guru;
                buildingList[i].Requirement.screen = tempBuildings[i].reqScreen;
                buildingList[i].Requirement.palette = tempBuildings[i].reqPalette;
            }
        }

        public void ShuffleWorlds(Random random)
        {
            Util.ShuffleList(worlds, 1, worlds.Count - 2, random);

            for (int i = 1; i < worlds.Count; i++)
            {
                worlds[i].index = (WorldNumber)i;
                worlds[i - 1].forward = worlds[i];
                worlds[i].backward = worlds[i - 1];
            }
        }

        public bool IsTower(DoorId id)
        {
            return TowerDoors.Keys.Contains(id);
        }

        public void RandomizeKeys(byte[] content, Random random)
        {
            var requirements = new List<DoorRequirement>();
            var counts = new Dictionary<DoorRequirement, int>();
            var possibleKeys = new List<DoorRequirement>();
            for (int i = 0; i < (int)DoorRequirement.DemonRing; i++)
            {
                possibleKeys.Add((DoorRequirement)i);
            }

            var doorTableindices = new List<ExitDoor>()
            {
                ExitDoor.TrunkExit,
                ExitDoor.MistExit,
                ExitDoor.BranchExit,
            };

            if (GeneralOptions.MoveFinalRequirements)
            {
                doorTableindices.Add(ExitDoor.DartmoorExit);
            }

            if (ItemOptions.IncludeSomeEolisDoors)
            {
                doorTableindices.Add(ExitDoor.EolisExit);
            }

            foreach (var door in doorTableindices)
            {
                requirements.Add((DoorRequirement)doorRequirementTable.Entries[(int)door][0]);
            }

            var towerDoorsToRandomize = new List<Door>();
            if (GeneralOptions.DoorTypeSetting == GeneralOptions.DoorTypeShuffle.ShuffleMoveKeys)
            {
                towerDoorsToRandomize = new List<Door>(Doors.Values.Where(t => IsTower(t.Id)));
            }
            else
            {
                towerDoorsToRandomize = new List<Door>(Doors.Values.Where(t => IsTower(t.OriginalId)));
            }

            for (int i = 0; i < towerDoorsToRandomize.Count; i++)
            {
                requirements.Add(towerDoorsToRandomize[i].key);
            }

            if (ItemOptions.IncludeSomeEolisDoors)
            {
                requirements.Add(Doors[DoorId.EolisMagicShop].key);
                requirements.Add(Doors[DoorId.MartialArtsShop].key);
            }

            if (!Util.AllCoreWorldScreensRandomized())
            {
                requirements.Add(Doors[DoorId.EastBranch].key); 
                requirements.Add(Doors[DoorId.DropdownWing].key);
            }

            requirements.Add(Doors[DoorId.DartmoorLateDoor].key);

            if (ItemOptions.RandomizeKeys == ItemOptions.KeyRandomization.Shuffled)
            {
                Util.ShuffleList(requirements, 0, requirements.Count - 1, random);
            }
            else
            {
                for(int i = 0; i < requirements.Count; i++)
                {
                    DoorRequirement requirement = DoorRequirement.Nothing;
                    bool finished = false;
                    while(!finished)
                    {
                        requirement = possibleKeys[random.Next(0, possibleKeys.Count)];
                        int limit = GetKeyLimit(requirement);
                        if (LimitKey(requirement, limit, counts))
                        {
                            possibleKeys.Remove(requirement);
                            continue;
                        }

                        finished = true;
                    }

                    requirements[i] = requirement;
                }
            }

            int index = 0;
            for (int i = 0; i < doorTableindices.Count; i++)
            {
                doorRequirementTable.Entries[(int)doorTableindices[i]][0] = (byte)requirements[index];
                index++;
            }

            for (int i = 0; i < towerDoorsToRandomize.Count; i++)
            {
                towerDoorsToRandomize[i].key = requirements[index];
                index++;
            }

            if (ItemOptions.IncludeSomeEolisDoors)
            {
                Doors[DoorId.EolisMagicShop].key = requirements[index];
                index++;
                Doors[DoorId.MartialArtsShop].key = requirements[index];
                index++;
            }

            if (!Util.AllCoreWorldScreensRandomized())
            {
                Doors[DoorId.EastBranch].key = requirements[index];
                index++;
                Doors[DoorId.DropdownWing].key = requirements[index];
                index++;
            }

            LevelDoors[DoorId.DartmoorLateDoor].key = requirements[index];
        }

        public enum DoorRequirement
        {
            Nothing = 0,
            AceKey = 1,
            KingKey = 2,
            QueenKey = 3,
            JackKey = 4,
            JokerKey = 5,
            ElfRing = 6,
            DworfRing = 7,
            DemonRing = 8,
        }

        public enum ExitDoor
        {
            IntroExit = 0,
            EolisExit = 1,
            TrunkExitBackwards = 2,
            TrunkExit = 3,
            MistExitBackwards = 4,
            MistExit = 5,
            BranchExitBackwards = 6,
            BranchExit = 7,
            DartmoorExitBackwards = 8,
            DartmoorExit = 9,
            EvilLairExit2 = 10,
            Unknown2 = 11,
        }

        public ShopRandomizer.Id GetExitKey(ExitDoor door)
        {
            int index = doorRequirementTable.Entries[(int)door][0];
            return keyRequirements[index];
        }

        public ShopRandomizer.Id GetLevelKey(Door building)
        {
            return keyRequirements[(int)building.key];
        }

        public List<Requirement> GetRequirements(OtherWorldNumber world, byte[] content)
        {
            int bankOffset = Section.GetOffset(3, 0x8000, 0x8000);
            int pointer = Util.GetPointer((byte)world, content, 3);
            var newPointer = Util.GetPointer(content, bankOffset + pointer + 8);

            var requirements = new List<Requirement>();
            for (int i = 0; i < 64; i++)
            {
                var data = new Requirement(bankOffset + newPointer + i * 4 + 2, content);
                requirements.Add(data);
            }

            return requirements;
        }

        public List<PositionData> GetPositions(OtherWorldNumber world, byte[] content)
        {
            int bankOffset = Section.GetOffset(3, 0x8000, 0x8000);
            int pointer = Util.GetPointer((byte)world, content, 3);
            var newPointer = Util.GetPointer(content, bankOffset + pointer + 6);
            var positions = new List<PositionData>();

            for (int i = 0; i < 64; i++)
            {
                var data = new PositionData(bankOffset + newPointer + i * 4, content);
                if (data.screen == 0xFF)
                {
                    break;
                }

                positions.Add(data);
            }

            return positions;
        }

        public List<PositionData> UpdateEolisPositions(List<PositionData> positions, byte[] content)
        {
            int newOffset = Section.GetOffset(3, 0xBF00, 0x8000);
            int bankOffset = Section.GetOffset(3, 0x8000, 0x8000);
            int pointer = Util.GetPointer((byte)OtherWorldNumber.Eolis, content, 3);
            byte b1 = 0x00;
            byte b2 = 0x3F;
            content[bankOffset + pointer + 6] = b1;
            content[bankOffset + pointer + 7] = b2;

            var newPositions = new List<PositionData>();
            for (int i = 0; i < positions.Count; i++)
            {
                positions[i].offset = newOffset;
                newOffset += 4;
                newPositions.Add(positions[i]);
            }
            newPositions.Add(new PositionData(newOffset, 1, 1, 9, 9, 12, 9));
            newPositions.Add(new PositionData(newOffset + 4, 255, 0, 0, 0, 0, 0));
            return newPositions;
        }

        public List<PositionData> UpdateTrunkPositions(List<PositionData> positions, byte[] content, int newOffset)
        {
            int bankOffset = Section.GetOffset(3, 0x8000, 0x8000);
            int eolisPointer = Util.GetPointer((byte)OtherWorldNumber.Eolis, content, 3);
            int trunkPointer = Util.GetPointer((byte)OtherWorldNumber.Trunk, content, 3);
            content[bankOffset + trunkPointer + 6] = content[bankOffset + eolisPointer + 6];
            content[bankOffset + trunkPointer + 7] = content[bankOffset + eolisPointer + 7];

            var newPositions = new List<PositionData>();
            int i = 0;
            for (; i < positions.Count - 1; i++)
            {
                positions[i + 1].offset = newOffset + i * 4;
                newPositions.Add(positions[i + 1]);
            }
            newPositions.Add(new PositionData(newOffset + i * 4, 0, 254, 0, 10, 6, 8));
            i++;
            positions[0].offset = newOffset + i * 4;
            newPositions.Add(positions[0]);
            return newPositions;
        }

        public void AddToContent(byte[] content)
        {
            doorRequirementTable.AddToContent(content);
            levelTable.AddToContent(content);
            screenTable.AddToContent(content);

            foreach (var door in Doors.Values)
            {
                door.AddToContent(content);
            }

            foreach (var world in worlds)
            {
                world.AddToContent(content);
            }

            guruRandomizer.AddToContent(content);
        }

        public class Requirement
        {
            public DoorRequirement key = 0;
            public int offset;
            public byte palette;
            public byte screen;
            public WorldNumber level = 0;

            public Requirement(int offset, byte[] content)
            {
                this.offset = offset;
                key = (DoorRequirement)content[offset];
                palette = content[offset - 1];
                screen = content[offset - 2];
            }

            public void AddToContent(byte[] content)
            {
                if (GeneralOptions.ShuffleTowers && level > 0)
                {
                    content[offset] = (byte)((int)key | ((byte)level << 4));
                }
                else
                {
                    content[offset] = (byte)key;
                }

                content[offset - 1] = palette;
                content[offset - 2] = screen;
            }
        }

        public class PositionData
        {
            public int offset = 0;
            public byte screen = 0;
            public byte oldPos = 0;
            public byte index = 0;
            public byte pos = 0;
            public byte X
            {
                get
                {
                    return (byte)(oldPos & 0xF);
                }
                set
                {
                    SetDoorPos(value, Y);
                }
            }
            public byte Y
            {
                get
                {
                    return (byte)(oldPos >> 4);
                }
                set
                {
                    SetDoorPos(X, value);
                }
            }

            public void SetDoorPos(byte x, byte y)
            {
                oldPos = (byte)(x | (y << 4));
            }

            public void SetPos(byte x, byte y)
            {
                pos = (byte)(x | (y << 4));
            }

            public PositionData(int offset, byte[] content)
            {
                this.offset = offset;
                screen = content[offset];
                oldPos = content[offset + 1];
                index = content[offset + 2];
                pos = content[offset + 3];
            }

            public PositionData(int offset, byte screen, byte index, byte doorX, byte doorY, byte posX, byte posY)
            {
                this.offset = offset;
                this.screen = screen;
                this.index = index;
                X = doorX;
                Y = doorY;
                SetPos(posX, posY);
            }

            public void AddToContent(byte[] content)
            {
                content[offset] = screen;
                content[offset + 1] = oldPos;
                content[offset + 2] = index;
                content[offset + 3] = pos;
            }
        }

        public class World
        {
            public WorldNumber number;
            public WorldNumber index;
            public World backward;
            public World forward;
            public byte backwardScreen;
            public byte forwardScreen;
            public byte entryScreen;
            public byte exitScreen;
            public byte entryPos;
            public byte exitPos;
            public PositionData backwardPosition;
            public PositionData forwardPosition;

            public World(WorldNumber number)
            {
                this.number = number;
                index = number;
                backward = this;
                forward = this;
            }

            public void AddToContent(byte[] content)
            {
                if (backwardPosition != null)
                {
                    backwardPosition.AddToContent(content);
                }
                if (forwardPosition != null)
                {
                    forwardPosition.AddToContent(content);
                }
            }
        }

        public void FinalizeWorlds(List<Level> levels, Random random, byte[] content)
        {
            for (int i = 1; i < worlds.Count; i++)
            {
                for (int j = 1; j < worlds.Count; j++)
                {
                    if (levels[i].Number == worlds[j].number)
                    {
                        levels[i].AdjustedNumber = (WorldNumber)j;
                        break;
                    }
                }
            }

            foreach (var level in levels)
            {
                foreach (var sublevel in level.SubLevels)
                {
                    foreach (var screen in sublevel.Screens)
                    {
                        foreach (var doorId in screen.Doors)
                        {
                            if (Doors.ContainsKey(doorId))
                            {
                                var door = Doors[doorId];
                                door.ParentSublevel = sublevel;

                                var subSublevel = door.Sublevel;
                                if (subSublevel != null)
                                {
                                    if (subSublevel.SubLevelId != sublevel.SubLevelId)
                                    {
                                        foreach (var subScreen in subSublevel.Screens)
                                        {
                                            subScreen.ParentWorld = level.AdjustedNumber;
                                        }
                                    }
                                }
                            }
                        }

                        if (level.Number == WorldNumber.Branch &&
                            screen.Number == Branch.ConflateLeftScreen &&
                            sublevel.Palette != SubLevel.UndefinedPalette)
                        {
                            content[Section.GetOffset(15, 0xEAF2, 0xC000)] = sublevel.Palette;
                        }

                        if (level.Number == WorldNumber.Dartmoor &&
                            screen.Number == Dartmoor.DartmoorCityLeftScreen &&
                            sublevel.Palette != SubLevel.UndefinedPalette)
                        {
                            content[Section.GetOffset(15, 0xEB01, 0xC000)] = sublevel.Palette;
                        }

                        if (screen.ParentWorld == WorldNumber.Unknown)
                        {
                            screen.ParentWorld = level.AdjustedNumber;
                        }
                    }
                }
            }

            if (GeneralOptions.ShuffleWorlds)
            {
                if (GeneralOptions.QuickSeed &&
                    !(GeneralOptions.ShuffleTowers && GeneralOptions.IncludeEvilOnesFortress))
                {
                    var tmp = new List<World>();
                    tmp.Add(worlds[0]);
                    tmp.Add(worlds[1]);
                    tmp.Add(worlds[2]);
                    tmp.Add(worlds[worlds.Count - 1]);
                    tmp.Add(worlds[3]);
                    tmp.Add(worlds[4]);
                    worlds = tmp;
                    worlds[3].forward = worlds[4];
                    worlds[4].backward = worlds[3];
                    worlds[5].forward = worlds[5];

                    for (int i = 1; i < worlds.Count; i++)
                    {
                        worlds[i].index = (WorldNumber)i;
                        worlds[i - 1].forward = worlds[i];
                        worlds[i].backward = worlds[i - 1];
                    }
                }

                for (int i = 0; i < worlds.Count; i++)
                {
                    if (GeneralOptions.ShuffleTowers && GeneralOptions.IncludeEvilOnesFortress && i == worlds.Count - 1)
                    {
                        continue;
                    }

                    worlds[i].backwardScreen = worlds[i].backward.exitScreen;
                    worlds[i].forwardScreen = worlds[i].forward.entryScreen;
                    if (worlds[i].backwardPosition != null)
                    {
                        worlds[i].backwardPosition.pos = worlds[i].backward.exitPos;
                    }
                    if (worlds[i].forwardPosition != null)
                    {
                        worlds[i].forwardPosition.pos = worlds[i].forward.entryPos;
                    }
                }

                for (int i = 0; i < worlds.Count; i++)
                {
                    if (GeneralOptions.ShuffleTowers && GeneralOptions.IncludeEvilOnesFortress && i == worlds.Count - 1)
                    {
                        continue;
                    }

                    levelTable.Entries[(int)worlds[i].number * 2][0] = (byte)worlds[i].backward.number;
                    levelTable.Entries[(int)worlds[i].number * 2 + 1][0] = (byte)worlds[i].forward.number;
                    screenTable.Entries[(int)worlds[i].number * 2][0] = worlds[i].backwardScreen;
                    screenTable.Entries[(int)worlds[i].number * 2 + 1][0] = worlds[i].forwardScreen;
                }
            }

            foreach (var door in Doors.Values)
            {
                door.Requirement.key = door.key;
            }

            if (GeneralOptions.ShuffleTowers)
            {
                var towers = new List<Door>(Doors.Values.Where(t => IsTower(t.Id) &&
                                            t.ShouldShuffle));

                for (int i = 0; i < towers.Count; i++)
                {
                    var tower = towers[i];
                    tower.Position.pos = tower.pos;
                    tower.Requirement.screen = tower.screen;
                    tower.Requirement.level = tower.level;
                    tower.Requirement.palette = tower.palette;
                }

                for (int i = 0; i < towers.Count; i++)
                {
                    var tower = towers[i];
                    tower.ReturnPosition.pos = tower.Position.oldPos;
                    tower.ReturnRequirement.screen = tower.Position.screen;
                    tower.ReturnRequirement.level = tower.GetWorld();
                }

                if (GeneralOptions.IncludeEvilOnesFortress)
                {
                    levelTable.Entries[(int)ExitDoor.EvilLairExit2][0] = 5;
                    screenTable.Entries[(int)ExitDoor.EvilLairExit2][0] = 0;
                    worlds[5].backwardPosition.SetPos(14, 6);
                    levelTable.Entries[(byte)worlds[4].number * 2 + 1][0] = 0;
                    screenTable.Entries[(byte)worlds[4].number * 2 + 1][0] = 0;
                    worlds[4].forward = worlds[0];
                    worlds[4].forwardPosition.SetPos(2, 8);
                }
            }

            var allTowers = new List<Door>(Doors.Values.Where(t => IsTower(t.Id)));
            for (int i = 0; i < allTowers.Count; i++)
            {
                var tower = allTowers[i];
                tower.ReturnRequirement.palette = tower.GetPalette(tower.OriginalId);
            }

            if (GeneralOptions.DarkTowers)
            {
                var towers = new List<Door>(Doors.Values.Where(t => IsTower(t.Id)));
                foreach (var door in towers)
                {
                    door.Requirement.palette = PaletteRandomizer.DarkPalette;
                }

                foreach (var level in Level.LevelDict.Values)
                {
                    foreach (var sublevel in level.SubLevels)
                    {
                        if (sublevel.IsTower())
                        {
                            foreach (var screen in sublevel.Screens)
                            {
                                foreach (var door in screen.Doors)
                                {
                                    CheckDarkReturn(door);
                                }
                            }
                        }
                    }
                }
            }

            foreach (var door in Doors.Values)
            {
                door.FinalizeGuru();
            }
        }

        private void CheckDarkReturn(DoorId id)
        {
            var subDoor = Doors[id];
            if (subDoor.ReturnRequirement != null)
            {
                subDoor.ReturnRequirement.palette = PaletteRandomizer.DarkPalette;
            }
        }

        public void RandomizeTowerPalettes(PaletteRandomizer paletteRandomizer, byte[] content)
        {
            var doors = new List<Door>(Doors.Values.Where(t => IsTower(t.Id)));
            doors.AddRange(LevelDoors.Values);

            foreach (var door in doors)
            {
                if (!GeneralOptions.DarkTowers)
                {
                    door.Requirement.palette = paletteRandomizer.GetRandomPalette(door.Requirement.palette);
                }

                if (door.Id == DoorId.EvilOnesLair)
                {
                    paletteRandomizer.FinalPalette = door.Requirement.palette;
                }

                if (door.ReturnRequirement != null &&
                    door.ReturnRequirement.palette != PaletteRandomizer.DarkPalette)
                {
                    door.ReturnRequirement.palette = paletteRandomizer.GetRandomPalette(door.ReturnRequirement.palette);
                }
            }

            if (ExtraOptions.MusicSetting == Music.Unchanged)
            {
                var paletteTable = new Table(Section.GetOffset(15, 0xE569, 0xC000), 7, 1, content);
                paletteRandomizer.SetTowerPalettes(paletteTable);
                paletteTable.AddToContent(content);
            }

            foreach (var door in Doors.Values)
            {
                door.AddToContent(content);
            }
        }

        private void SetDoorValues(Door destination, Door source, bool tower=false)
        {
            destination.Id = source.Id;
            destination.Sublevel = source.Sublevel;
            destination.Gift = source.Gift;
            destination.BuildingShop = source.BuildingShop;
            destination.Guru = source.Guru;
            destination.ReturnPosition = source.ReturnPosition;
            destination.ReturnRequirement = source.ReturnRequirement;

            if (tower)
            {
                destination.palette = source.palette;
                destination.pos = source.pos;
                destination.screen = source.screen;
                destination.level = source.level;
            }
            else
            {
                destination.Position = source.Position;
                destination.Requirement = source.Requirement;
            }
        }

        private void LimitWorldDoors(Dictionary<DoorRequirement, int> counts, Random random)
        {
            var entries = new List<byte[]>(doorRequirementTable.Entries);
            Util.ShuffleList(entries, 0, entries.Count - 1, random);

            foreach (var entry in entries)
            {
                var key = (DoorRequirement)entry[0];
                int limit = GetKeyLimit(key);
                if (LimitKey(key, limit, counts))
                {
                    entry[0] = (byte)DoorRequirement.Nothing;
                }
            }
        }

        private bool LimitKey(DoorRequirement key, int limit, Dictionary<DoorRequirement, int> counts)
        {
            if (counts.ContainsKey(key))
            {
                counts[key]++;
            }
            else
            {
                counts.Add(key, 1);
            }

            if (counts[key] > limit)
            {
                return true;
            }

            return false;
        }

        private int GetKeyLimit(DoorRequirement key)
        {
            if (key == DoorRequirement.JackKey ||
                key == DoorRequirement.QueenKey ||
                key == DoorRequirement.KingKey)
            {
                if (ItemOptions.SmallKeyLimit == ItemOptions.KeyLimit.NoLimit)
                {
                    return int.MaxValue;
                }

                return (int)ItemOptions.SmallKeyLimit;
            }
            else if (key == DoorRequirement.AceKey ||
                     key == DoorRequirement.JokerKey)
            {
                if (ItemOptions.BigKeyLimit == ItemOptions.KeyLimit.NoLimit)
                {
                    return int.MaxValue;
                }

                return (int)ItemOptions.BigKeyLimit;
            }

            return int.MaxValue;
        }
    }
}
