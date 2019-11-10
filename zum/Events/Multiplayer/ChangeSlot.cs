using System;
using zum.Objects;

namespace zum.Events
{
    public static class ChangeSlot
    {
        public static void Handle(Player p, byte[] Data)
        {
            if (Data.Length != 4) return;
            int nSlot = BitConverter.ToInt32(Data);
            if (nSlot > 15) return;
            Slot newSlot = p.Match.Slots[nSlot];
            if ((newSlot.Status & 124) != 0 || newSlot.User != null) return;
            int x = p.Match.FindPlayerSlotIndex(p);
            p.Match.Slots[nSlot] = p.Match.Slots[x];
            p.Match.Slots[x] = newSlot;
            p.Match.Update();
        }
    }
}
