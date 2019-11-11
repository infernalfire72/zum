using zum.Objects;

namespace zum.Events
{
    public static class ChangeTeam
    {
        public static void Handle(Player p, byte[] Data)
        {
            if (p.Match == null) return;
            if (p.Match.TeamType == 0 || p.Match.TeamType == 1) return;
            Slot x = null;
            int cRed = 0;
            int cBlue = 0;
            for (int i = 0; i < 16; i++)
            {
                if (p.Match.Slots[i].User == p)
                    x = p.Match.Slots[i];
                if (p.Match.Slots[i].Team == 1) cBlue++;
                else cRed++;
            }
            if (x == null)
            {
                p.Match = null;
                return;
            }
            if (cRed == 1 && x.Team == 2) return; // only one player in their team which is themselves. we dont want no players in a team
            if (cBlue == 1 && x.Team == 1) return; // same
            x.Team = (byte)(x.Team == 1 ? 2 : 1);
        }
    }
}
