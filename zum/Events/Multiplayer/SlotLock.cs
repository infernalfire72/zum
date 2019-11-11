using System;
using zum.Objects;

namespace zum.Events
{
    public static class SlotLock
    {
        public static void Handle(Player p, byte[] Data)
        {
            if (Data.Length != 4) return;
            if (p.Match == null) return;
            if (p.Match.Host != p.Id) return;
            int sId = BitConverter.ToInt32(Data, 0);
            if (sId > 15) return;
            Slot x = p.Match.Slots[sId];
            if (x.User != null || x.Status == 1)
            {
                x.Status = 2;
            } else
                x.Status = 1;
            p.Match.Update();
        }
    }
}
