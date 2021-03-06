﻿using zum.Objects;
using zum.Packets;

namespace zum.Events
{
    public static class CreateMatch
    {
        public static void Handle(Player p, byte[] Data)
        {
            MultiplayerLobby m = new MultiplayerLobby(Data);
            m.Creator = m.Host = p.Id;
            Log.LogFormat($"%#00FF44%{p.Username} %#007cee%created a new MultiplayerLobby {m.Name}");
            m.AddPlayer(p, m.Password);

            Packet pack = m.Packet(26);
            m.Broadcast(pack);
            for (int i = 0; i < Global.Lobby.Count; i++)
                Global.Lobby[i].AddQueue(pack);
            pack.Id = 36;
            p.AddQueue(pack);
            p.AddQueue(Packets.Packets.SingleStringPacket(64, "#multiplayer"));
            p.AddQueue(Packets.Packets.ChannelAvailable("#multiplayer", "Multiplayer Game Channel", (short)m.Players.Count));
        }
    }
}
