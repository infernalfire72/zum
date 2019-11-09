using System.Drawing;
using Colorful;

namespace zum
{
    public static class HexConverter
    {
        public static byte ToByte(string v)
        {
            return byte.TryParse(v, System.Globalization.NumberStyles.HexNumber, null, out byte result) ? result : (byte)0;
        }
        public static Color ToColor(string value)
        {
            if (value[0] == '#') value = value.Substring(1, value.Length - 1);
            if (value.Length > 6) value = value.Substring(0, 6);
            if (value.Length < 6) for (int i = value.Length; i < 6; i++) value += "0";
            return int.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out int result) ? Color.FromArgb(result) : Color.White;
        }
    }

    public static class Log
    {
        static char delim1 = '%';
        static char delim2 = '%';
        public static void LogFormat(string log)
        {
            Color pColor = Color.White;
            string[] d = log.Split(delim1, delim2);
            if (d.Length == 1)
            {
                Console.WriteLine(log);
                return;
            }
            for (int i = 1; i < d.Length; i += 2)
            {
                if (d[i][0] != '#') { Console.Write(delim1 + d[i] + delim2 + d[i + 1], pColor); continue; };
                //if (d[i + 1].Length == 0) continue;
                pColor = HexConverter.ToColor(d[i]);
                Console.Write(d[i + 1], pColor);
            }
            Console.WriteLine();
        }

        public static void Write(object value) => Write(value, Color.White);
        public static void Write(object value, Color color) => Console.Write("" + value, color);
        public static void Write(object value, int color) => Write(value, Color.FromArgb(color));
        public static void WriteLine(object value) => WriteLine(value, Color.White);
        public static void WriteLine(object value, Color color) => Console.WriteLine("" + value, color);
        public static void WriteLine(object value, int color) => WriteLine(value, Color.FromArgb(color));
    }
}
