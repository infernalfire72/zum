using zum.Objects;

namespace zum.Events
{
    public static class MatchSkip
    {
        public static void Handle(Player p)
        {
            if (p.Match == null) return;
            int slotId = p.Match.FindPlayerSlotIndex(p);
            if (slotId == -1)
            {
                p.Match = null;
                return;
            }
            Slot x = p.Match.Slots[slotId];
            x.Skipped = true;
            p.Match.BroadcastPlaying(Packets.Packets.SingleIntPacket(81, slotId));
            if (p.Match.CheckSkip()) // if everyone has skipped, execute the skip
                p.Match.BroadcastPlaying(Packets.Packets.NoDataPacket(61));
        }
    }
}
