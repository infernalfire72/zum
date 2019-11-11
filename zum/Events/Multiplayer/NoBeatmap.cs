using zum.Objects;

namespace zum.Events
{
    public static class NoBeatmap
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
            x.Status = 16;
            p.Match.Update();
        }
    }
}
