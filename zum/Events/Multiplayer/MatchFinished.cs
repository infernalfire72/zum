using zum.Objects;

namespace zum.Events
{
    public static class MatchFinished
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
            x.Completed = true;
            if (p.Match.CheckDone()) // if everyone has finished, end the match
            {
                p.Match.BroadcastPlaying(Packets.Packets.NoDataPacket(58));
                p.Match.MatchRunning = false;
            }
        }
    }
}
