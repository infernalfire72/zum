using zum.Objects;
using zum.Packets;

namespace zum.Events
{
    public static class MatchStart
    {
        public static void Handle(Player p, byte[] Data)
        {
            if (p.Match == null) return;
            if (p.Match.Host != p.Id) return;
            var pack = new Packet(46);
            pack.Data = Data;
            p.Match.ReadMatch(Data);
            p.Match.MatchRunning = true;
            for (int i = 0; i < 16; i++)
                if (p.Match.Slots[i].User != null && p.Match.Slots[i].Status == 8)
                    p.Match.Slots[i].Status = 32;
            p.Match.Update();
            p.Match.Broadcast(pack);
        }
    }
}
