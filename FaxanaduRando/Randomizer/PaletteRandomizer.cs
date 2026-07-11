using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class PaletteRandomizer
    {
        public const byte DarkPalette = 4;
        public byte BranchPalette = 8;
        public byte FinalPalette = 15;

        public static readonly HashSet<byte> badPalettes = new HashSet<byte>
        {
            DarkPalette, 28,
        };

        private Dictionary<byte, byte> palettes = new Dictionary<byte, byte>();

        public PaletteRandomizer(Random random)
        {
            var paletteList = new List<byte>();
            // TODO: This should possibly stop at 31. Index 31 refers to HUD attribute-table
            // lookup data rather than a palette. It will still produce a usable result though,
            // because Faxanadu forces the background color of each subpalette to $0f
            for (byte i = 0; i < 32; i++)
            {
                if (badPalettes.Contains(i))
                {
                    paletteList.Add(++i);
                }
                else
                {
                    paletteList.Add(i);
                }
            }

            Util.ShuffleList(paletteList, 0, paletteList.Count - 1, random);
            for (byte i = 0; i < paletteList.Count; i++)
            {
                byte palette = paletteList[i];
                palettes[i] = palette;
            }

            palettes[DarkPalette] = DarkPalette;
        }

        public void RandomizePalettes(byte[] content, Random random)
        {
            var paletteSection = new Section();

            // randomize default palettes for the eight worlds,
            // plus an additional eight unused 0s following those palette indexes
            for (int i = 0; i < 16; i++)
            {
                var palette = content[Section.GetOffset(15, ROM.WorldToPaletteTable + i)];
                paletteSection.Db(GetRandomPalette(palette));
            }
            paletteSection.AddToContent(content, Section.GetOffset(15, ROM.WorldToPaletteTable));
            BranchPalette = content[Section.GetOffset(15, ROM.WorldToPaletteTable + 5)]; // world 5: branches

            if (GeneralOptions.DarkTowers)
            {
                content[Section.GetOffset(15, ROM.WorldToPaletteTable + 7)] = DarkPalette; // world 7: zenis
            }

            paletteSection = new Section();
            // randomize 10 palettes, one for each building screen
            for (int i = 0; i < 10; i++)
            {
                var palette = content[Section.GetOffset(15, ROM.BuildingToPaletteTable + i)];
                paletteSection.Db(GetRandomPalette(palette));
            }
            paletteSection.AddToContent(content, Section.GetOffset(15, ROM.BuildingToPaletteTable));

            // randomize some other palette references across banks
            // TODO: Discover these instead, by traversing the samewworld and otherworld transition ptr tables
            content[Section.GetOffset(15, ROM.SwTransTrunkScreen12To22PaletteIndex)] = GetRandomPalette(content[Section.GetOffset(15, ROM.SwTransTrunkScreen12To22PaletteIndex)]);
            content[Section.GetOffset(15, ROM.SwTransTrunkScreen22To12PaletteIndex)] = GetRandomPalette(content[Section.GetOffset(15, ROM.SwTransTrunkScreen22To12PaletteIndex)]);
            content[Section.GetOffset(15, ROM.OwTransTrunkScreen0Invalid)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransTrunkScreen0Invalid)]);
            content[Section.GetOffset(15, ROM.OwTransTrunkRightApolune)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransTrunkRightApolune)]);
            content[Section.GetOffset(15, ROM.OwTransTrunkLeftApolune)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransTrunkLeftApolune)]);
            content[Section.GetOffset(15, ROM.OwTransTrunkRightForepaw)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransTrunkRightForepaw)]);
            content[Section.GetOffset(15, ROM.OwTransTrunkLeftForepaw)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransTrunkLeftForepaw)]);
            content[Section.GetOffset(15, ROM.OwTransApoluneLeft)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransApoluneLeft)]);
            content[Section.GetOffset(15, ROM.OwTransApoluneRight)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransApoluneRight)]);
            content[Section.GetOffset(15, ROM.OwTransForepawLeft)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransForepawLeft)]);
            content[Section.GetOffset(15, ROM.OwTransForepawRight)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransForepawRight)]);
            content[Section.GetOffset(15, ROM.OwTransMasconLeft)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransMasconLeft)]);
            content[Section.GetOffset(15, ROM.OwTransMasconRight)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransMasconRight)]);
            content[Section.GetOffset(15, ROM.OwTransVictimLeft)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransVictimLeft)]);
            content[Section.GetOffset(15, ROM.OwTransVictimRight)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransVictimRight)]);
            content[Section.GetOffset(15, ROM.OwTransConflateLeft)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransConflateLeft)]);
            content[Section.GetOffset(15, ROM.OwTransDaybreakLeft)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransDaybreakLeft)]);
            content[Section.GetOffset(15, ROM.OwTransDaybreakRight)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransDaybreakRight)]);
            content[Section.GetOffset(15, ROM.OwTransDartmoorTownRight)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransDartmoorTownRight)]);
            content[Section.GetOffset(15, ROM.OwTransMistRightMascon)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransMistRightMascon)]);
            content[Section.GetOffset(15, ROM.OwTransMistLeftMascon)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransMistLeftMascon)]);
            content[Section.GetOffset(15, ROM.OwTransMistRightVictim)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransMistRightVictim)]);
            content[Section.GetOffset(15, ROM.OwTransMistLeftVictim)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransMistLeftVictim)]);
            content[Section.GetOffset(15, ROM.OwTransBranchesRightConflate)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransBranchesRightConflate)]);
            content[Section.GetOffset(15, ROM.OwTransBranchesRightDaybreak)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransBranchesRightDaybreak)]);
            content[Section.GetOffset(15, ROM.OwTransBranchesLeftDaybreak)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransBranchesLeftDaybreak)]);
            content[Section.GetOffset(15, ROM.OwTransDartmoorRightDartmoorTown)] = GetRandomPalette(content[Section.GetOffset(15, ROM.OwTransDartmoorRightDartmoorTown)]);
            content[Section.GetOffset(15, ROM.SpawnInTemplePaletteIndex)] = GetRandomPalette(content[Section.GetOffset(15, ROM.SpawnInTemplePaletteIndex)]);
            content[Section.GetOffset(15, ROM.GameEndKingsRoomPaletteIndex)] = GetRandomPalette(content[Section.GetOffset(15, ROM.GameEndKingsRoomPaletteIndex)]);

            // TODO: walkPalettes contains the high bytes of the intro and outro CHR data
            // addresses ($ACA0 and $B0A0 in bank 10), so this actually randomizes which CHR set
            // is used for each scene
            var walkPalettes = new List<byte>() { 0xAC, 0xB0, };
            // possible low bytes of the intro and outro palette addresses. the high byte remains $a6 for both
            // valid palettes will only exist for values $c8, $d8, $e8 and $f8 (intro and outro sprite and bg palettes)
            var walkPalettes2 = new List<byte>() { 0xB8, 0xC8, 0xD8, 0xE8, 0xF8 };
            var introWalkPalette = walkPalettes[random.Next(walkPalettes.Count)];
            var endingWalkPalette = walkPalettes[random.Next(walkPalettes.Count)];
            var introWalkPalette2 = walkPalettes2[random.Next(walkPalettes2.Count)];
            var endingWalkPalette2 = walkPalettes2[random.Next(walkPalettes2.Count)];
            content[Section.GetOffset(12, ROM.SceneIntroChrHi)] = introWalkPalette;
            content[Section.GetOffset(12, ROM.SceneOutroChrHi)] = endingWalkPalette;
            content[Section.GetOffset(12, ROM.SceneIntroPaletteLo)] = introWalkPalette2;
            content[Section.GetOffset(12, ROM.SceneOutroPaletteLo)] = endingWalkPalette2;
        }

        public void RandomizeMusic(byte[] content, Random random)
        {
            var paletteSection = new Section();
            // randomize default music for each world
            for (int i = 0; i < 8; i++)
            {
                paletteSection.Db(GetRandomMusic(random));
            }
            paletteSection.AddToContent(content, Section.GetOffset(15, ROM.WorldToMusicTable));

            paletteSection = new Section();
            // randomize default music for each building screen
            for (int i = 0; i < 10; i++)
            {
                paletteSection.Db(GetRandomMusic(random));
            }
            paletteSection.AddToContent(content, Section.GetOffset(15, ROM.BuildingToMusicTable));

            paletteSection = new Section();
            // randomize music for the palette to music table (vanilla has 7 entries)
            for (int i = 0; i < 7; i++)
            {
                paletteSection.Db(GetRandomMusic(random));
            }
            paletteSection.AddToContent(content, Section.GetOffset(15, ROM.Palette_To_Music_Table_Music));

            // update music index references in several banks
            content[Section.GetOffset(12, ROM.TitleScreenMusicIndex)] = GetRandomMusic(random);
            content[Section.GetOffset(14, ROM.SpriteBehaviorShadowEura_MusicIndex)] = GetRandomMusic(random);
            content[Section.GetOffset(12, ROM.IntroScreenMusicIndex)] = GetRandomMusic(random);
            content[Section.GetOffset(12, ROM.OutroScreenMusicIndex)] = GetRandomMusic(random);
            content[Section.GetOffset(15, ROM.HourGlassMusicIndex)] = GetRandomMusic(random);
            content[Section.GetOffset(15, ROM.DeathMusicIndex)] = GetRandomMusic(random);
            content[Section.GetOffset(15, ROM.BossMusicIndex)] = GetRandomMusic(random);
            content[Section.GetOffset(15, ROM.EndGameTransitionMusicIndex)] = GetRandomMusic(random);
            content[Section.GetOffset(15, ROM.MantraMusicIndex)] = GetRandomMusic(random);
            content[Section.GetOffset(15, ROM.SpawnInTempleMusicIndex)] = GetRandomMusic(random);
            content[Section.GetOffset(15, ROM.GameEndKingsRoomMusicIndex)] = GetRandomMusic(random);
        }

        public byte GetRandomPalette(byte palette)
        {
            if (palettes.ContainsKey(palette))
            {
                return palettes[palette];
            }

            return palette;
        }

        public void SetTowerPalettes(Table paletteTable)
        {
            for (byte i = 0; i < paletteTable.Entries.Count; i++)
            {
                var palette = paletteTable.Entries[i][0];
                if (palettes.ContainsKey(palette))
                {
                    paletteTable.Entries[i][0] = palettes[palette];
                }
            }
        }

        private byte GetRandomMusic(Random random)
        {
            if (ExtraOptions.MusicSetting == Music.None)
            {
                return 0;
            }

            return (byte)random.Next(1, 17);
        }
    }
}
