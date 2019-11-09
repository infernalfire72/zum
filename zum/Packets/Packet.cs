using System.IO;

namespace zum.Packets
{
    public class Packet
    {
        public short Id;
        public byte[] Data;

        public Packet(short id)
        {
            Id = id;
        }

        public void Write(BinaryWriter w)
        {
            w.Write(Id);
            w.Write(false);
            w.Write(Data?.Length ?? 0);
            if (Data != null) w.Write(Data);
        }
    }
}
