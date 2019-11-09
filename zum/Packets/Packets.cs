using System;
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
    }
}
