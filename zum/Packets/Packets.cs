using System;
using System.Collections.Generic;
using System.IO;
using zum.Objects;
using zum.Tools;

namespace zum.Packets
{
    public static class Packets
    {
        #region Generic Packets
        public static Packet NoDataPacket(short id) =>
            new Packet(id);

        public static Packet SingleStringPacket(short id, string v)
        {
            Packet p = new Packet(id);
            p.Data = Ext.WriteStringFast(v, System.Text.Encoding.UTF8);
            return p;
        }

        public static Packet SingleIntPacket(short id, int v)
        {
            Packet p = new Packet(id);
            p.Data = BitConverter.GetBytes(v);
            return p;
        }
        #endregion

        #region Chat

        #endregion

        #region Presences and User Info
        public static Packet StatsPacket(Player p)
        {
            Packet pack = new Packet(11);
            using (MemoryStream ms = new MemoryStream())
            using (CustomWriter w = new CustomWriter(ms))
            {
                w.Write(p.Id);
                w.Write((byte)p.Action);
                w.Write(p.ActionText);
                w.Write(p.ActionMd5);
                w.Write(p.ActionMods);
                w.Write(p.Gamemode);
                w.Write(p.ActionBeatmap);
                if (p.Bot)
                {
                    w.Write(0L);
                    w.Write(0);
                    w.Write(0);
                    w.Write(0L);
                    w.Write(0);
                    w.Write((short)0);
                }
                else
                {
                    w.Write(p.Stats[p.Gamemode].RankedScore); // Ranked Score
                    w.Write(p.Stats[p.Gamemode].Accuracy); // Acc
                    w.Write(p.Stats[p.Gamemode].Playcount); // Playcount
                    w.Write(p.Stats[p.Gamemode].TotalScore); // Total Score
                    w.Write(p.Stats[p.Gamemode].Rank); // Rank
                    w.Write((short)(p.Stats[p.Gamemode].Performance > short.MaxValue ? 0 : p.Stats[p.Gamemode].Performance)); // pp
                }

                pack.Data = ms.ToArray();
            }
            return pack;
        }

        public static Packet LogoutPacket(int Id)
        {
            Packet p = new Packet(12);
            p.Data = BitConverter.GetBytes((long)unchecked((uint)Id)); // wtf omegabrain time lol (unchecked = omega safety cast lol)
            return p;
        }

        public static Packet FriendListPacket(List<int> ids)
        {
            Packet p = new Packet(72);
            p.Data = Ext.WriteIntListFast(ids);
            return p;
        }

        public static Packet PresencePacket(Player p)
        {
            Packet pack = new Packet(83);
            using (MemoryStream ms = new MemoryStream())
            using (CustomWriter w = new CustomWriter(ms))
            {
                w.Write(p.Id);
                w.Write(p.Username);
                w.Write((byte)24);
                w.Write(p.Country);
                w.Write((byte)((p.IngamePrivileges & 0x1f) | ((p.Gamemode & 0x7) << 5)));
                w.Write(0.0f);
                w.Write(0.0f);
                w.Write(p.Bot ? -1337 : p.Stats[p.Gamemode].Rank); // rank
                pack.Data = ms.ToArray();
            }
            return pack;
        }
        #endregion
    }
}
