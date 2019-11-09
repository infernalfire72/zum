using System.Collections.Generic;
using System.Threading.Tasks;
using zum.Objects;
using zum.Tools;

namespace zum.Events
{
    public static class StatsUpdateRequest
    {
        public static async Task Handle(Player p)
        {
            await p.GetStatsFixed(p.Gamemode);
            p.AddQueue(Packets.Packets.StatsPacket(p));
        }

        public static void Handle(Player p, byte[] Data)
        {
            List<int> ids = Ext.ReadIntListFast(Data);
            for(int i = 0; i < ids.Count; i++)
            {
                Player t = Global.FindPlayerById(ids[i]);
                p.AddQueue(Packets.Packets.StatsPacket(t));
            }
        }
    }
}
