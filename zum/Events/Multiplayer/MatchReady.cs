using zum.Objects;

namespace zum.Events
{
    public static class MatchReady
    {
        public static void Handle(Player p)
        {
            if (p.Match == null) return;
            Slot x  = p.Match.FindPlayerSlot(p);
            if (x.Status == 4) x.Status = 8;
            else x.Status = 4;
            p.Match.Update();
        }
    }
}
