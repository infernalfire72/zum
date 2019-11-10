using zum.Objects;

namespace zum.Events
{
    public static class LeaveMatch
    {
        public static void Handle(Player p)
        {
            if (p.Match == null) return;
            if (!p.Match.Players.Contains(p))
            {
                p.Match = null;
                return;
            }

            p.Match.Players.Remove(p);
            p.AddQueue(Packets.Packets.SingleStringPacket(66, "#multiplayer"));
            if (p.Match.Players.Count == 0)
            {
                p.AddQueue(Packets.Packets.SingleIntPacket(28, p.Match.Id));
                p.Match.DestroyMatch();
                return;
            }
            p.Match.FindPlayerSlot(p).Clear();
            p.Match.Update();
            p.Match = null;
        }
    }
}
