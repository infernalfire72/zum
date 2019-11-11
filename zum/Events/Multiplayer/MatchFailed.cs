using zum.Objects;

namespace zum.Events
{
    public static class MatchFailed
    {
        public static void Handle(Player p)
        {
            if (p.Match == null) return;
            int slotId = p.Match.FindPlayerSlotIndex(p);
            if (slotId == -1 || p.Match.Slots[slotId].Status != 32) return;
            p.Match.BroadcastPlaying(Packets.Packets.SingleIntPacket(57, slotId));
        }
    }
}
