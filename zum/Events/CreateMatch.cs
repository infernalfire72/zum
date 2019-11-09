using zum.Objects;

namespace zum.Events
{
    public static class CreateMatch
    {
        public static void Handle(Player p, byte[] Data)
        {
            MultiplayerLobby m = new MultiplayerLobby(Data);
            m.Creator = m.Host = p.Id;
            Log.WriteLine($"{p.Username} created a new MultiplayerLobby {m.Name}");
            m.AddPlayer(p, m.Password);
            m.Update();
            p.AddQueue(Packets.Packets.SingleStringPacket(64, "#multiplayer"));
            p.AddQueue(Packets.Packets.ChannelAvailable("#multiplayer", "Multiplayer Game Channel", (short)m.Players.Count));
        }
    }
}
