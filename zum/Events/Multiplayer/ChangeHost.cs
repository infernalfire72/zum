using System;
using zum.Objects;

namespace zum.Events
{
    public static class ChangeHost
    {
        public static void Handle(Player p, byte[] Data)
        {
            if (Data.Length != 4) return;
            if (p.Match == null) return;
            if (p.Match.Host != p.Id) return;
            int nHost = BitConverter.ToInt32(Data, 0);
            if (nHost > 15) return;
            Slot x = p.Match.Slots[nHost];
            if (x.User == null || x.Status == 128) return;
            p.Match.Host = x.User.Id;
            p.Match.Broadcast(Packets.Packets.SingleIntPacket(50, nHost));
            p.Match.Update();
        }
    }
}
