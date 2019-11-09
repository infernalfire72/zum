using System.Threading.Tasks;
using zum.Objects;

namespace zum.Events
{
    public static class StatsUpdateRequest
    {
        public static async Task Handle(Player p)
        {
            await p.GetStatsFixed(p.Gamemode);
            p.AddQueue(Packets.Packets.StatsPacket(p));
        }
    }
}
