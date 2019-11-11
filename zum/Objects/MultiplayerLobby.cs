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
        public int Host { get; set; }
        public int Creator { get; set; }
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

        public bool AddPlayer(Player p, string password)
        {
            if (password != Password) return false;
            for (int i = 0; i < 16; i++)
            {
                if ((Slots[i].Status & 124) == 0 && Slots[i].User == null)
                {
                    Players.Add(p);
                    Slots[i].User = p;
                    Slots[i].Status = 4;
                    p.Match = this;
                    Global.LeaveLobby(p);
                    return true;
                }
            }
            return false;
        }

        public void Update()
        {
            Packet p = this.Packet(26);
            Broadcast(p);
            for (int i = 0; i < Global.Lobby.Count; i++)
                 Global.Lobby[i].AddQueue(Packet(26));
        }

        public void Broadcast(Packet p)
        {
            for (int i = 0; i < Players.Count; i++)
                Players[i].AddQueue(p);
        }

        public void BroadcastPlaying(Packet p)
        {
            if (!MatchRunning) return;
            for (int i = 0; i < 16; i++)
                if (Slots[i].User != null && Slots[i].Status == 32)
                    Slots[i].User.AddQueue(p);
        }

        public Slot FindPlayerSlot(Player p) => FindPlayerSlotIndex(p) != -1 ? Slots[FindPlayerSlotIndex(p)] : null;

        public int FindPlayerSlotIndex(Player p)
        {
            for (int i = 0; i < 16; i++)
                if (Slots[i].User == p)
                    return i;
            return -1;
        }

        public bool CheckLoaded()
        {
            for (int i = 0; i < Slots.Length; i++)
                if (Slots[i].User != null && Slots[i].Status == 32 && !Slots[i].Loaded)
                    return false;
            return true;
        }

        public bool CheckSkip()
        {
            for (int i = 0; i < Slots.Length; i++)
                if (Slots[i].User != null && Slots[i].Status == 32 && !Slots[i].Skipped)
                    return false;
            return true;
        }

        public bool CheckDone()
        {
            for (int i = 0; i < Slots.Length; i++)
                if (Slots[i].User != null && Slots[i].Status == 32 && !Slots[i].Completed)
                    return false;
            return true;
        }

        public void DestroyMatch()
        {
            Global.Matches.Remove(this);
            Packet pack = Packets.Packets.SingleIntPacket(28, Id);
            for (int i = 0; i > Players.Count; i++)
            {
                Players[i].AddQueue(pack);
                Players[i].Match = null;
            }
            for (int i = 0; i < Global.Lobby.Count; i++)
                Global.Lobby[i].AddQueue(pack);
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

        public Packet Packet(short id)
        {
            Packet p = new Packet(id);
            using (MemoryStream ms = new MemoryStream())
            using (CustomWriter w = new CustomWriter(ms))
            {
                w.Write(Id);
                w.Write(MatchRunning);
                w.Write(MatchType);
                w.Write(Mods);
                w.Write(Name);
                w.Write(Password);
                w.Write(BeatmapName);
                w.Write(BeatmapId);
                w.Write(BeatmapMd5);

                for (int i = 0; i < 16; i++)
                    w.Write(Slots[i].Status);

                for (int i = 0; i < 16; i++)
                    w.Write(Slots[i].Team);

                for (int i = 0; i < 16; i++)
                    if ((Slots[i].Status & 124) != 0 && Slots[i].User != null)
                        w.Write(Slots[i].User.Id);

                w.Write(Host);
                w.Write(Gamemode);
                w.Write(ScoreType);
                w.Write(TeamType);
                w.Write(FreeMod);

                if (FreeMod)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        w.Write(Slots[i].Mods);
                    }
                }

                p.Data = ms.ToArray();
            }
            return p;
        }
    }
}
