using System.Collections.Generic;
using System.IO;
using System.Text;
using zum.Packets;

namespace zum.Tools
{
    public static class Ext
    {
        public static void Write(this Stream s, Packet p) {
            using (BinaryWriter w = new BinaryWriter(s))
                p.Write(w);
        }
        public static void Write(this BinaryWriter w, Packet p) => p.Write(w);
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
        public static string ReadStringFast(byte[] v, int i) // 82ns/op 
        {
            if (v[i++] != 11) return null;
            int l = 0;
            do
            {
                l += v[i] == 128 ? 127 : v[i];
            } while ((v[i++] & 128) != 0);
            unsafe
            {
                fixed (byte* ptr = &v[i])
                {
                    return new string((sbyte*)ptr, 0, l);
                }
            }
        }

        public static byte[] WriteIntListFast(List<int> list) // 72ns/op old method: 250ns/op
        {
            if (list.Count == 0) return new byte[2];
            byte[] res = new byte[2 + 4 * list.Count];
            unsafe
            {
                fixed (byte* b = res)
                    *((short*)b) = (short)list.Count;

                for (int i = 2; i < res.Length; i += 4)
                    fixed (byte* xb = &res[i])
                        *((int*)xb) = list[(i - 2) / 4];
            }
            return res;
        }

        public static List<int> ReadIntListFast(byte[] Data) // 54ns/op old method: 170ns/op
        {
            if (Data[0] == 0 && Data[1] == 0) return new List<int>();
            List<int> newl = new List<int>();
            unsafe
            {
                for (int i = 2; i < Data.Length; i += 4)
                    fixed (byte* xb = &Data[i])
                        newl.Add(*((int*)xb));
            }
            return newl;
        }
    }
}
