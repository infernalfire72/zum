using System;
using zum.Objects;

namespace zum.Events
{
    public static class ModsChange
    {
        public static void Handle(Player p, byte[] Data)
        {
            if (Data.Length != 4) return;
            if (p.Match == null) return;
            int mods = BitConverter.ToInt32(Data, 0);
            bool rx = (mods & 128) != 0;
            if (p.Match.Host == p.Id && !p.Match.FreeMod)
            {
                bool mrx = (p.Match.Mods & 128) != 0;
                if (mrx != rx) // if rx state switches
                    for (int i = 0; i < p.Match.Slots.Length; i++)
                        if (p.Match.Slots[i].User != null && p.Match.Slots[i].Status != 128)
                            p.Match.Slots[i].User.SetRelax(rx);
                p.Match.Mods = mods;
            } else if (p.Match.FreeMod)
            {
                if ((mods & 64) != 0 && p.Match.Host == p.Id)
                    p.Match.Mods |= (mods & 512) != 0 ? 576 : 64;
                else if (p.Match.Host == p.Id)
                    p.Match.Mods &= ~(576);
                mods &= ~(576); // remove dt/nc from mods (singular slot)
                Slot x = p.Match.FindPlayerSlot(p);
                p.SetRelax(rx);
                x.Mods = mods;
            }

            p.Match.Update();
        }
    }
}
