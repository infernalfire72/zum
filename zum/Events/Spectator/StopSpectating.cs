using zum.Objects;

namespace zum.Events
{
    public static class StopSpectating
    {
        public static void Handle(Player p)
        {
            if (p.Spectating == null) return;
            if (p.Spectating.Bot || !p.Spectating.Spectators.Contains(p))
            {
                p.Spectating = null;
                return;
            }
            p.Spectating.Spectators.Remove(p);
            if (p.Spectating.Spectators.Count == 0) p.Spectating.AddQueue(Packets.Packets.SingleStringPacket(66, "#spectator"));
            p.Spectating.AddQueue(Packets.Packets.SingleIntPacket(14, p.Id));
            p.Spectating.SendSpectator(Packets.Packets.SingleIntPacket(43, p.Id));
            p.Spectating = null;
            p.AddQueue(Packets.Packets.SingleStringPacket(66, "#spectator"));
        }
    }
}
