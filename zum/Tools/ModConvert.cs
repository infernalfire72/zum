using System;
using System.Collections.Generic;
using System.Text;

namespace zum.Tools
{
    public enum Mods : int
    {
        None = 0,
        NoFail = 1,
        Easy = 2,
        TouchDevice = 4,
        Hidden = 8,
        HardRock = 16,
        SuddenDeath = 32,
        DoubleTime = 64,
        Relax = 128,
        HalfTime = 256,
        Nightcore = 512, // Only set along with DoubleTime. i.e: NC only gives 576
        Flashlight = 1024,
        Autoplay = 2048,
        SpunOut = 4096,
        Relax2 = 8192,    // Autopilot
        Perfect = 16384, // Only set along with SuddenDeath. i.e: PF only gives 16416  
        Key4 = 32768,
        Key5 = 65536,
        Key6 = 131072,
        Key7 = 262144,
        Key8 = 524288,
        FadeIn = 1048576,
        Random = 2097152,
        Cinema = 4194304,
        Target = 8388608,
        Key9 = 16777216,
        KeyCoop = 33554432,
        Key1 = 67108864,
        Key3 = 134217728,
        Key2 = 268435456,
        ScoreV2 = 536870912,
        Mirror = 1073741824,
        KeyMod = Key1 | Key2 | Key3 | Key4 | Key5 | Key6 | Key7 | Key8 | Key9 | KeyCoop,
        FreeModAllowed = NoFail | Easy | Hidden | HardRock | SuddenDeath | Flashlight | FadeIn | Relax | Relax2 | SpunOut | KeyMod,
        ScoreIncreaseMods = Hidden | HardRock | DoubleTime | Flashlight | FadeIn
    }

    public static class ModConvert
    {
        public static bool Has(int x, Mods y) => (x & (int)y) != 0;
        /// public static char Last(this string x) => x[x.Length - 1]; // idk what i needed this for lol
        public static string ToString(int mods)
        {
            if (mods == 0) return "";
            string mstr = "";
            if (Has(mods, Mods.Cinema)) return "~Cinema~";
            if (Has(mods, Mods.Easy)) mstr += "EZ";
            if (Has(mods, Mods.NoFail)) mstr += "NF";
            if (Has(mods, Mods.Hidden)) mstr += "HD";
            if (Has(mods, Mods.HardRock)) mstr += "HR";
            if (Has(mods, Mods.SuddenDeath)) mstr += Has(mods, Mods.Perfect) ? "PF" : "SD";
            if (Has(mods, Mods.DoubleTime)) mstr += Has(mods, Mods.Nightcore) ? "NC" : "DT";
            if (Has(mods, Mods.HalfTime)) mstr += "HT";
            if (Has(mods, Mods.Flashlight)) mstr += "FL";
            if (Has(mods, Mods.SpunOut)) mstr += "SO";

            if (Has(mods, Mods.Key1)) mstr += "1K";
            else if (Has(mods, Mods.Key2)) mstr += "2K";
            else if (Has(mods, Mods.Key3)) mstr += "3K";
            else if (Has(mods, Mods.Key4)) mstr += "4K";
            else if (Has(mods, Mods.Key5)) mstr += "5K";
            else if (Has(mods, Mods.Key6)) mstr += "6K";
            else if (Has(mods, Mods.Key7)) mstr += "7K";
            else if (Has(mods, Mods.Key8)) mstr += "8K";
            else if (Has(mods, Mods.Key9)) mstr += "9K";

            if (Has(mods, Mods.KeyCoop)) mstr += "~Coop~";

            if (Has(mods, Mods.Relax)) mstr += " ~Relax~";
            if (Has(mods, Mods.Relax2)) mstr += " ~AutoPilot~";

            return mstr;
        }
    }
}
