using zum.Objects;

namespace zum.Events
{
    public static class HasBeatmap
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
            if (x.Status != 16) return;
            x.Status = 4;
            p.Match.Update();
        }
    }
}
