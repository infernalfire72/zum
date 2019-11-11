using zum.Objects;

namespace zum.Events
{
    public static class MatchLoaded
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
            x.Loaded = true;
            if (p.Match.CheckLoaded()) // if everyone has loaded, start the match
                p.Match.BroadcastPlaying(Packets.Packets.NoDataPacket(53));
        }
    }
}
