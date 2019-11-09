using System;
using zum.Objects;

namespace zum.Events
{
    public static class JoinMatch
    {
        public static void Handle(Player p, byte[] Data)
        {
            if (p.Match != null) LeaveMatch.Handle(p);
            int MatchId = BitConverter.ToInt32(Data, 0);
            if (MatchId > short.MaxValue) return;
            MultiplayerLobby m = Global.FindMatch((short)MatchId);
            if (m?.AddPlayer(p, Tools.Ext.ReadStringFast(Data, 4)) ?? false)
            {
                p.AddQueue(m.Packet(36));
                m.Update();
                p.AddQueue(Packets.Packets.SingleStringPacket(64, "#multiplayer"));
                p.AddQueue(Packets.Packets.ChannelAvailable("#multiplayer", "Multiplayer Game Channel", (short)m.Players.Count));
                return;
            }
            p.AddQueue(Packets.Packets.NoDataPacket(37));
        }
    }
}
