using System.Collections.Generic;
using System.IO;
using System.Linq;
using zum.Packets;

namespace zum.Objects
{
    public class Slot
    {
        public bool Loaded, Skipped, Completed = false;
        public byte Status = 1;
        public byte Team = 0;
        public int Mods;
        public Player User;

        public bool Free => User == null && Status == 1;

        public void Clear()
        {
            User = null;
            Status = 1;
            Mods = 0;
            Team = 0;
        }

        public void Lock()
        {
            Clear();
            Status = 2;
        }

        public void Open() => Status = 1;

        public void Ready()
        {
            if (Status == 4) Status = 8;
            else Status = 4;
        }
    }

    public class MultiplayerLobby
    {
        public short Id { get; private set; }
        public int Host { get; private set; }
        public int Creator { get; private set; }
        public int BeatmapId { get; set; }
        public int Mods { get; set; }
        public int Seed { get; set; } // For Mania Random

        public string Name { get; set; }
        public string Password { get; set; }
        public string BeatmapName { get; set; }
        public string BeatmapMd5 { get; set; }
        public bool MatchRunning { get; set; }
        public bool FreeMod { get; set; }
        public byte Gamemode { get; set; }
        public byte MatchType { get; set; }
        public byte ScoreType { get; set; }
        public byte TeamType { get; set; }

        public List<Player> Players { get; set; }
        public Slot[] Slots { get; set; }

        public MultiplayerLobby()
        {
            Slots = new Slot[16];
            for (int i = 0; i < 16; i++) Slots[i] = new Slot();
            Players = new List<Player>();
            if (Global.Matches.Count == 0) Id = 1;
            else Id = (short)(Global.Matches.Last().Id + 1);
        }

        public MultiplayerLobby(byte[] Data) : this()
        {
            ReadMatch(Data);
            Global.Matches.Add(this);
        }

        public void ReadMatch(CustomReader r)
        {
            if (!(r.BaseStream is MemoryStream)) return; // We do not support Reading here
            (r.BaseStream).Position += 2;
            MatchRunning = r.ReadBoolean();
            MatchType = r.ReadByte();
            Mods = r.ReadInt32();
            Name = r.ReadString();
            Password = r.ReadString();
            BeatmapName = r.ReadString();
            BeatmapId = r.ReadInt32();
            BeatmapMd5 = r.ReadString();

            for (int i = 0; i < 16; i++)
                Slots[i].Status = r.ReadByte();

            for (int i = 0; i < 16; i++)
                Slots[i].Team = r.ReadByte();

            for (int i = 0; i < 16; i++)
                if ((Slots[i].Status & 124) != 0)
                    Slots[i].User = Global.FindPlayerById(r.ReadInt32());

            (r.BaseStream).Position += 4;
            Gamemode = r.ReadByte();
            ScoreType = r.ReadByte();
            TeamType = r.ReadByte();
            bool freeMod = r.ReadBoolean();

            if (freeMod != FreeMod) //freemod changed
            {
                if (freeMod)
                {
                    for (int i = 0; i < 16; i++)
                        if (Slots[i].User != null)
                            Slots[i].Mods = Mods;
                    Mods = 0;
                }
                else
                {
                    Mods = 0;
                    for (int i = 0; i < 16; i++)
                        Slots[i].Mods = 0;
                }
            }
            FreeMod = freeMod;
            Seed = r.ReadInt32();
        }

        public void ReadMatch(byte[] Data)
        {
            using (CustomReader r = new CustomReader(Data))
            {
                ReadMatch(r);
            }
        }
    }
}
