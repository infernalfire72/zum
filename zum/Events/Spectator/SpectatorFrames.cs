using zum.Objects;
using zum.Packets;

namespace zum.Events
{
    public static class SpectatorFrames
    {
        public static void Handle(Player p, byte[] Data)
        {
            if (p.Spectators == null || p.Spectators.Count == 0) return;
            p.SendSpectator(new Packet(15) { Data = Data });
        }
    }
}
