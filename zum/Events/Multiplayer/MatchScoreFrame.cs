using zum.Objects;
using zum.Packets;

namespace zum.Events
{
    public static class MatchScoreFrame
    {
        public static void Handle(Player p, byte[] Data)
        {
            if (p.Match == null) return;
            int x = p.Match.FindPlayerSlotIndex(p);
            if (x == -1)
            {
                p.Match = null;
                return;
            }
            Packet pack = new Packet(48);
            pack.Data = Data;
            pack.Data[5] = (byte)x;
            p.Match.BroadcastPlaying(pack);
        }
    }
}
