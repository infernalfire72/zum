using System.Text;

namespace zum.Tools
{
    public static class Ext
    {
        public static byte[] WriteStringFast(string v, Encoding enc) // 140ns/op
        {
            if (v == null || v.Length == 0) return new byte[] { 0 };
            int b = enc.GetByteCount(v);
            int vsize = b;
            byte[] r = new byte[vsize + ((vsize / 128) + 2)];
            int i = 0;
            r[i++] = 11;
            while ((vsize -= 127) > 0)
                r[i++] = 128;
            r[i++] = (byte)(vsize + 127);
            enc.GetBytes(v, 0, v.Length, r, i);
            return r;
        }
    }
}
