using zum.Objects;

namespace zum.Events
{
    public static class MatchSkip
    {
        public static void Handle(Player p)
        {
            if (p.Match == null) return;
            Slot x = p.Match.FindPlayerSlot(p);
            if (x == null)
            {
                p.Match = null;
                return;
            }
            x.Skipped = true;
            if (p.Match.CheckSkip()) // if everyone has skipped, execute the skip
                p.Match.BroadcastPlaying(Packets.Packets.NoDataPacket(61));
        }
    }
}
