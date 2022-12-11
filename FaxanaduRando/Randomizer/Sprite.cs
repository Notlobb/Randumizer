using System;
using System.Collections.Generic;

namespace FaxanaduRando.Randomizer
{
    public class Sprite
    {
        public enum SpriteId
        {
            //TODO complete
            Undefined = -1,
            Luigi = 4,
            NecronAides = 5,
            Zombie = 6,
            Hornet = 7,
            Metroid = 8,
            SpawnedGhost = 9,
            Ghost = 11,
            Snowman = 12,
            Nash = 13,
            FireGiant = 14,
            BlueMage = 15,
            ExecutionHood = 16,
            Table = 17,
            RockSnake = 18,
            CloakedMage = 21,
            Raisin = 23,
            Psychic = 24,
            Mario = 25,
            GiantBees = 26,
            Myconid = 27,
            Naga = 28,
            StillKnight = 29,
            GiantStrider = 30,
            SirGawaine = 31,
            Jason = 32,
            Wolfman = 33,
            Yareeka = 34,
            TeleCreature = 35,
            Jouster = 36,
            Cricket = 38,
            Slug = 39,
            Lamprey = 40,
            Monodron = 42,
            Bat = 43,
            BurrowingCyclops = 44,
            Wyvern = 45,
            DogBoss = 46,
            BigSnake = 47,
            Nest = 48,
            RockLobster = 49,
            KingGrieve = 50,
            EvilOne = 51,
            RingDemon = 53,
            RingDworf = 54,
            KeyAce = 56,
            Eye = 70,
            Spiky = 71,
            Glove = 72,
            BlackOnyx = 73,
            Pendant = 74,
            RedPotion = 75,
            Poison = 76,
            Elixir = 77,
            Ointment = 78,
            Intro = 79,
            MattockOrRingRuby = 80,
            Wingboots = 85,
            Hourglass = 86,
            Rod = 87,
            Battlesuit = 88,
            Battlehelmet = 89,
            Dragonslayer = 90,
            MattockBossLocked = 91,
            WingbootsBossLocked = 92,
            RedPotion2 = 93,
            Poison2 = 94,
            Glove2OrKeyJoker = 95,
            Ointment2 = 96,
        }

        public static readonly HashSet<SpriteId> tallIds = new HashSet<SpriteId>
        {
            SpriteId.Luigi,
            SpriteId.NecronAides,
            SpriteId.Zombie,
            SpriteId.Snowman,
            SpriteId.Nash,
            SpriteId.FireGiant,
            SpriteId.BlueMage,
            SpriteId.ExecutionHood,
            SpriteId.CloakedMage,
            SpriteId.Raisin,
            SpriteId.Psychic,
            SpriteId.Mario,
            SpriteId.Myconid,
            SpriteId.Naga,
            SpriteId.StillKnight,
            SpriteId.GiantStrider,
            SpriteId.SirGawaine,
            SpriteId.Jason,
            SpriteId.Wolfman,
            SpriteId.Yareeka,
            SpriteId.TeleCreature,
            SpriteId.Jouster,
            SpriteId.Cricket,
            SpriteId.Slug,
            SpriteId.Lamprey,
            SpriteId.Monodron,
            SpriteId.BurrowingCyclops,
        };

        public static readonly HashSet<SpriteId> shortIds = new HashSet<SpriteId> { SpriteId.Spiky };

        public static readonly HashSet<SpriteId> flyingIds = new HashSet<SpriteId>
        {
            SpriteId.Hornet,
            SpriteId.Metroid,
            SpriteId.SpawnedGhost,
            SpriteId.Ghost,
            SpriteId.GiantBees,
            SpriteId.Bat,
            SpriteId.Eye,
        };

        public static readonly HashSet<SpriteId> bossIds = new HashSet<SpriteId>
        {
            SpriteId.Table,
            SpriteId.Wyvern,
            SpriteId.DogBoss,
            SpriteId.BigSnake,
            SpriteId.Nest,
            SpriteId.RockLobster,
        };

        public static readonly List<SpriteId> fallingBossIds = new List<SpriteId>
        {
            SpriteId.Table,
            SpriteId.Wyvern,
            SpriteId.DogBoss,
        };

        public static readonly HashSet<SpriteId> superBossIds = new HashSet<SpriteId>
        {
            SpriteId.KingGrieve,
            SpriteId.EvilOne,
        };

        public static readonly HashSet<SpriteId> vanillaItemIds = new HashSet<SpriteId>
        {
            SpriteId.Glove,
            SpriteId.BlackOnyx,
            SpriteId.Pendant,
            SpriteId.RedPotion,
            SpriteId.Poison,
            SpriteId.Elixir,
            SpriteId.Ointment,
            //SpriteId.Intro,
            SpriteId.MattockOrRingRuby,
            SpriteId.Wingboots,
            SpriteId.Hourglass,
            SpriteId.Rod,
            SpriteId.Battlesuit,
            SpriteId.Battlehelmet,
            SpriteId.Dragonslayer,
            SpriteId.MattockBossLocked,
            SpriteId.WingbootsBossLocked,
            SpriteId.RedPotion2,
            SpriteId.Poison2,
            SpriteId.Glove2OrKeyJoker,
            SpriteId.Ointment2,
        };

        public static readonly HashSet<SpriteId> itemIds = new HashSet<SpriteId>
        {
            SpriteId.Glove,
            SpriteId.BlackOnyx,
            SpriteId.Pendant,
            SpriteId.RedPotion,
            SpriteId.Poison,
            SpriteId.Elixir,
            SpriteId.Ointment,
            SpriteId.Intro,
            SpriteId.MattockOrRingRuby,
            SpriteId.Wingboots,
            SpriteId.Hourglass,
            SpriteId.Rod,
            SpriteId.Battlesuit,
            SpriteId.Battlehelmet,
            SpriteId.Dragonslayer,
            SpriteId.MattockBossLocked,
            SpriteId.WingbootsBossLocked,
            SpriteId.RedPotion2,
            SpriteId.Poison2,
            SpriteId.Glove2OrKeyJoker,
            SpriteId.Ointment2,
            SpriteId.KeyAce,
            SpriteId.RingDworf,
            SpriteId.RingDemon,
        };

        public static readonly HashSet<SpriteId> KeyItems = new HashSet<SpriteId>
        {
            SpriteId.Battlehelmet,
            SpriteId.Battlesuit,
            SpriteId.Dragonslayer,
            SpriteId.BlackOnyx,
            SpriteId.MattockBossLocked,
            SpriteId.RingDworf,
            SpriteId.RingDemon,
            SpriteId.KeyAce,
            SpriteId.MattockOrRingRuby,
            SpriteId.Glove2OrKeyJoker,
            SpriteId.Pendant,
            SpriteId.Rod,
            SpriteId.WingbootsBossLocked,
            SpriteId.Elixir,
        };

        public static readonly HashSet<SpriteId> enemies = GetEnemies();

        public static HashSet<SpriteId> GetEnemies()
        {
            var enemies = new HashSet<SpriteId>();
            enemies.UnionWith(tallIds);
            enemies.UnionWith(shortIds);
            enemies.UnionWith(flyingIds);
            enemies.UnionWith(bossIds);
            enemies.UnionWith(superBossIds);
            return enemies;
        }

        public Sprite(byte[] content, int offset)
        {
            Id = (SpriteId)content[offset];
            Location = content[offset + 1];
            Offset = offset;
        }

        public SpriteId Id { get; set; }
        public byte Location { get; set; }
        public int Offset { get; set; }
        public bool RequiresMattock { get; set; } = false;
        public bool RequiresWingBoots { get; set; } = false;
        public bool ShouldBeShuffled { get; set; } = true;

        public byte GetX()
        {
            return (byte)(Location & 0xF);
        }

        public byte GetY()
        {
            return (byte)((Location >> 4) & 0xF);
        }

        public void SetX(byte x)
        {
            byte y = GetY();
            SetLocation(x, y);
        }

        public void SetY(byte y)
        {
            byte x = GetX();
            SetLocation(x, y);
        }

        public void SetLocation(byte x, byte y)
        {
            Location = (byte)(x | (y << 4));
        }

        public void Update(SpriteId newId, Random random)
        {
            byte y = GetY();
            if (superBossIds.Contains(newId) && y > 0 && (shortIds.Contains(Id) ||
                                                          tallIds.Contains(Id) ||
                                                          bossIds.Contains(Id)))
            {
                y--;
            }
            if (newId == SpriteId.EvilOne && y > 2 && (shortIds.Contains(Id) ||
                                                       tallIds.Contains(Id)))
            {
                y-=2;
            }
            if (shortIds.Contains(Id) && y > 0 && (tallIds.Contains(newId) ||
                                                   bossIds.Contains(newId) ||
                                                   superBossIds.Contains(newId)))
            {
                y--;
            }

            if (EnemyOptions.TryToMoveBosses && y > 0 && bossIds.Contains(newId) &&
                (shortIds.Contains(Id) || tallIds.Contains(Id)) &&
                !(newId == SpriteId.RockLobster || newId == SpriteId.Nest))
            {
                y--;
            }

            if (EnemyOptions.TryToMoveBosses && fallingBossIds.Contains(newId) &&
                Id == SpriteId.RockLobster)
            {
                y--;
            }

            if (ItemOptions.BigItemSpawns == ItemOptions.BigItemSpawning.AlwaysLockBehindBosses &&
                (newId == SpriteId.RockLobster ||
                newId == SpriteId.Nest ||
                newId == SpriteId.BigSnake) &&
                (Id == SpriteId.Wyvern ||
                Id == SpriteId.DogBoss))
            {
                newId = fallingBossIds[random.Next(fallingBossIds.Count)];
            }

            SetY(y);
            Id = newId;
        }

        public void AddToContent(byte[] content)
        {
            content[Offset] = (byte)Id;
            content[Offset + 1] = Location;
        }
    }
}
