using System.Collections.Generic;

namespace FaxanaduRando
{
    public class GuruRandomizer
    {
        public Dictionary<Guru.GuruId, Guru> Gurus = new Dictionary<Guru.GuruId, Guru>();
        private Table worlds;
        private Table xPositions;
        private Table yPositions;
        private Table rooms;
        private Table worldSpawns;
        private Table screens;

        public GuruRandomizer(Table worlds, Table xPositions,
                              Table yPositions, Table rooms,
                              Table worldSpawns, Table screens)
        {
            this.worlds = worlds;
            this.xPositions = xPositions;
            this.yPositions = yPositions;
            this.rooms = rooms;
            this.worldSpawns = worldSpawns;
            this.screens = screens;
            AddGuru(Guru.GuruId.Eolis);
            AddGuru(Guru.GuruId.Apolune);
            AddGuru(Guru.GuruId.Forepaw);
            AddGuru(Guru.GuruId.Mascon);
            AddGuru(Guru.GuruId.Victim);
            AddGuru(Guru.GuruId.Conflate);
            AddGuru(Guru.GuruId.Daybreak);
            AddGuru(Guru.GuruId.Dartmoor);
        }

        public void AddToContent(byte[] content)
        {
            foreach (var key in Gurus.Keys)
            {
                if (key == Guru.GuruId.Eolis)
                {
                    continue;
                }
                else
                {
                    worlds.Entries[(int)key][0] = (byte)Gurus[key].World;
                    xPositions.Entries[(int)key][0] = Gurus[key].X;
                    yPositions.Entries[(int)key][0] = Gurus[key].Y;
                    worldSpawns.Entries[(int)key][0] = Gurus[key].WorldSpawn;
                    screens.Entries[(int)key][0] = Gurus[key].Screen;
                }
            }

            worlds.AddToContent(content);
            xPositions.AddToContent(content);
            yPositions.AddToContent(content);
            rooms.AddToContent(content);
            worldSpawns.AddToContent(content);
            screens.AddToContent(content);
        }

        private void AddGuru(Guru.GuruId id)
        {
            Gurus[id] = new Guru(id, (OtherWorldNumber)worlds.Entries[(int)id][0],
                                 xPositions.Entries[(int)id][0],
                                 yPositions.Entries[(int)id][0],
                                 worldSpawns.Entries[(int)id][0],
                                 screens.Entries[(int)id][0]);
        }

    }
}
