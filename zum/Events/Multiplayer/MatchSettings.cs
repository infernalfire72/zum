using zum.Objects;

namespace zum.Events
{
    public static class MatchSettings
    {
        public static void Handle(Player p, byte[] Data)
        {
            if (p.Match == null) return;
            if (p.Match.Host != p.Id) return;
            p.Match.ReadMatch(Data);
            p.Match.Update();
        }
    }
}
