using System.IO;

namespace zum.Packets
{
    public class CustomWriter : BinaryWriter
    {
        public CustomWriter(Stream output) : base(output) { }

        public override void Write(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                base.Write(false);
                return;
            }

            base.Write((byte)11);
            base.Write(value);
        }
    }

    public class CustomReader : BinaryReader
    {
        public CustomReader(Stream input) : base(input) { }
        public CustomReader(byte[] input) : this(new MemoryStream(input)) { }

        public override string ReadString()
        {
            return base.ReadByte() == 0x0b ? base.ReadString() : null;
        }
    }
}
