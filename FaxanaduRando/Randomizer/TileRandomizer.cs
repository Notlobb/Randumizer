using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class TileRandomizer
    {
        // ROM data offsets from chipx86.com/faxanadu documentation
        // Bank 3 ($8000-$BFFF) contains level data
        private const int TILE_DATA_BANK = 3;

        // Area-specific data structures
        private struct AreaData
        {
            public int BlockAttributes;    // Collision data
            public int BlockData1;         // Visual tile set 1
            public int BlockData2;         // Visual tile set 2
            public int BlockData3;         // Visual tile set 3
            public int BlockData4;         // Visual tile set 4
            public int BlockProperties;    // Gameplay properties
            public int TileCount;          // Number of tiles in this area
        }

        // Area definitions with exact ROM offsets
        private static readonly Dictionary<int, AreaData> AreaOffsets = new Dictionary<int, AreaData>
        {
            { 0, new AreaData { BlockAttributes = 0x8026, BlockData1 = 0x80a6, BlockData2 = 0x8126,
                                BlockData3 = 0x81a6, BlockData4 = 0x8226, BlockProperties = 0x82a6, TileCount = 128 } },
            { 1, new AreaData { BlockAttributes = 0x891e, BlockData1 = 0x89a6, BlockData2 = 0x8a2e,
                                BlockData3 = 0x8ab6, BlockData4 = 0x8b3e, BlockProperties = 0x8bc6, TileCount = 136 } },
            { 4, new AreaData { BlockAttributes = 0x8e8f, BlockData1 = 0x8eed, BlockData2 = 0x8f4b,
                                BlockData3 = 0x8fa9, BlockData4 = 0x9007, BlockProperties = 0x9065, TileCount = 94 } },
            { 6, new AreaData { BlockAttributes = 0x91c0, BlockData1 = 0x92c0, BlockData2 = 0x93c0,
                                BlockData3 = 0x94c0, BlockData4 = 0x95c0, BlockProperties = 0x96c0, TileCount = 256 } },
        };

        // Tile collision attributes (from ROM inspection)
        private const byte ATTR_PASSABLE = 0x00;          // Safe to randomize
        private const byte ATTR_SOLID_PARTIAL_1 = 0x55;   // Must preserve
        private const byte ATTR_SOLID_PARTIAL_2 = 0xAA;   // Must preserve
        private const byte ATTR_IMPASSABLE = 0xFF;        // Must preserve

        // These will be populated dynamically by reading block attributes
        private static HashSet<byte> CollisionTiles = new HashSet<byte>();
        private static HashSet<byte> DecorativeTiles = new HashSet<byte>();

        // World-specific tile sets
        private static readonly Dictionary<WorldNumber, List<byte>> WorldTileSets = new Dictionary<WorldNumber, List<byte>>
        {
            // TODO: Map each world to its appropriate tile set
            // { WorldNumber.Trunk, new List<byte> { /* tile IDs */ } },
            // { WorldNumber.Mist, new List<byte> { /* tile IDs */ } },
            // etc.
        };

        private Random random;

        public TileRandomizer(Random random)
        {
            this.random = random;
        }

        public void RandomizeTiles(List<Level> levels, byte[] content)
        {
            if (TileOptions.RandomizationMode == TileRandomizationMode.Unchanged)
            {
                return;
            }

            // First, analyze block attributes to categorize tiles
            AnalyzeBlockAttributes(content);

            // Then randomize each area's block data
            foreach (var areaId in AreaOffsets.Keys)
            {
                RandomizeAreaTiles(areaId, content);
            }
        }

        private void AnalyzeBlockAttributes(byte[] content)
        {
            // Read block attributes for each area and categorize tiles
            CollisionTiles.Clear();
            DecorativeTiles.Clear();

            foreach (var kvp in AreaOffsets)
            {
                int areaId = kvp.Key;
                AreaData area = kvp.Value;

                int offset = Section.GetOffset(TILE_DATA_BANK, area.BlockAttributes, 0x8000);

                for (byte tileId = 0; tileId < area.TileCount && offset + tileId < content.Length; tileId++)
                {
                    byte attribute = content[offset + tileId];

                    if (attribute == ATTR_PASSABLE)
                    {
                        // Passable tiles are safe to randomize
                        DecorativeTiles.Add(tileId);
                    }
                    else if (attribute == ATTR_SOLID_PARTIAL_1 ||
                             attribute == ATTR_SOLID_PARTIAL_2 ||
                             attribute == ATTR_IMPASSABLE)
                    {
                        // Collision tiles must be preserved
                        CollisionTiles.Add(tileId);
                    }
                }
            }
        }

        private void RandomizeAreaTiles(int areaId, byte[] content)
        {
            if (!AreaOffsets.ContainsKey(areaId))
            {
                return;
            }

            AreaData area = AreaOffsets[areaId];

            // Only randomize BlockData1 for now to test
            // BlockData2-4 provide visual variation but share same collision
            RandomizeBlockData(area.BlockData1, area.TileCount, content);
        }

        private void RandomizeBlockData(int blockDataAddress, int tileCount, byte[] content)
        {
            int offset = Section.GetOffset(TILE_DATA_BANK, blockDataAddress, 0x8000);

            // Build list of decorative tile indices for this area
            var decorativeTileList = new List<byte>(DecorativeTiles);
            if (decorativeTileList.Count == 0)
            {
                // No decorative tiles identified, skip randomization
                return;
            }

            // Randomize each decorative tile
            for (int i = 0; i < tileCount && offset + i < content.Length; i++)
            {
                byte originalTile = content[offset + i];

                // Only randomize if this is a decorative tile
                if (DecorativeTiles.Contains((byte)i))
                {
                    byte replacementTile = GetRandomTile(decorativeTileList);
                    content[offset + i] = replacementTile;
                }
            }
        }

        private void RandomizeLevelTiles(Level level, byte[] content)
        {
            foreach (var screen in level.Screens)
            {
                RandomizeScreenTiles(screen, content);
            }
        }

        private void RandomizeScreenTiles(Screen screen, byte[] content)
        {
            // TODO: Implement actual screen tile randomization
            // This requires:
            // 1. Finding where this screen's tile data is stored in the ROM
            // 2. Reading the tile layout (possibly RLE compressed)
            // 3. Identifying which tiles can be safely randomized
            // 4. Replacing decorative tiles based on the randomization mode
            // 5. Writing the modified data back to the ROM

            switch (TileOptions.RandomizationMode)
            {
                case TileRandomizationMode.DecorativeOnly:
                    RandomizeDecorativeTiles(screen, content);
                    break;

                case TileRandomizationMode.PerWorld:
                    RandomizeWithWorldTileSet(screen, content);
                    break;

                case TileRandomizationMode.AllWorlds:
                    RandomizeWithAllTiles(screen, content);
                    break;

                case TileRandomizationMode.Chaos:
                    RandomizeChaosTiles(screen, content);
                    break;
            }
        }

        private void RandomizeDecorativeTiles(Screen screen, byte[] content)
        {
            // Only randomize tiles that are purely decorative
            // Preserve all collision and functional tiles

            // TODO: Implement
            // - Read screen tile data from ROM
            // - For each decorative tile, replace with another random decorative tile
            // - Write back to ROM
        }

        private void RandomizeWithWorldTileSet(Screen screen, byte[] content)
        {
            // Randomize using tiles appropriate for this world
            if (!WorldTileSets.ContainsKey(screen.ParentWorld))
            {
                return;
            }

            var tileSet = WorldTileSets[screen.ParentWorld];

            // TODO: Implement
            // - Read screen tile data
            // - Replace decorative tiles with random tiles from this world's tile set
            // - Maintain the world's visual theme
        }

        private void RandomizeWithAllTiles(Screen screen, byte[] content)
        {
            // Mix tiles from all worlds
            var allTiles = new List<byte>();
            foreach (var tileSet in WorldTileSets.Values)
            {
                allTiles.AddRange(tileSet);
            }

            // TODO: Implement
            // - Similar to PerWorld but draw from all available tile sets
        }

        private void RandomizeChaosTiles(Screen screen, byte[] content)
        {
            // Complete chaos - any decorative tile can become any other tile
            // This mode creates the most visual variety but may look strange

            // TODO: Implement
            // - Maximum randomization while still preserving playability
            // - Could potentially randomize more aggressively than other modes
        }

        // Helper method to check if a tile can be safely randomized
        private bool IsTileSafeToRandomize(byte tileId)
        {
            if (!TileOptions.PreserveCollision)
            {
                // If not preserving collision, more tiles can be randomized
                // (This could break the game, so default should be true)
                return !IsEssentialTile(tileId);
            }

            return DecorativeTiles.Contains(tileId);
        }

        private bool IsEssentialTile(byte tileId)
        {
            // Essential tiles that should never be randomized
            // (doors, springs, special objects, etc.)

            // TODO: Identify essential tile IDs
            return false;
        }

        // Helper method to get a random replacement tile
        private byte GetRandomTile(List<byte> tilePool)
        {
            if (tilePool.Count == 0)
            {
                return 0; // Empty tile fallback
            }

            int intensity = TileOptions.RandomizationIntensity;

            // Lower intensity = more likely to keep similar tiles
            // Higher intensity = more variety in replacement
            if (random.Next(100) < intensity)
            {
                return tilePool[random.Next(tilePool.Count)];
            }

            // Return first tile (least randomization) if intensity check fails
            return tilePool[0];
        }
    }

    // ROM Structure Information (Source: https://chipx86.com/faxanadu)
    //
    // Bank 3 ($8000-$BFFF) contains level data organized by area:
    //
    // Area Data Organization:
    // - Block attributes pointer table (collision/property data)
    // - Block data arrays (4 variations: BLOCK_DATA_1 through BLOCK_DATA_4)
    // - Each array contains 128-256 tiles
    // - Block properties (single byte per tile)
    // - Scroll/screen mapping data (room layout references)
    // - Door location and destination tables
    //
    // Tile/Block Attributes:
    // - $00 = Passable (decorative tiles - SAFE TO RANDOMIZE)
    // - $55 or $AA = Solid/partial collision (platforms, walls - PRESERVE)
    // - $FF = Impassable/spike hazard (PRESERVE)
    //
    // Tile Properties:
    // - Each tile has a property byte controlling:
    //   - Item type
    //   - Enemy type
    //   - Hazard level
    //
    // Implementation Notes:
    // 1. Screens reference tile indices (0-255) from block data arrays
    // 2. Multiple block data sets provide visual variation
    // 3. Must preserve logical properties when randomizing
    // 4. Example: Area 1 has 68 doors across 136 tiles
    //
    // Next Steps:
    // - Map exact offsets in Bank 3 for each area's block data
    // - Create lookup tables for collision attributes
    // - Implement block data reading/writing
    // - Test with one area before expanding to all areas
}
