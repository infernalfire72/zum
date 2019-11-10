using zum.Objects;
using zum.Packets;

namespace zum.Events
{
    public static class MatchChangePassword
    {
        public static void Handle(Player p, byte[] Data)
        {
            if (p.Match.Host != p.Id) return;
            using (CustomReader r = new CustomReader(Data))
            {
                (r.BaseStream).Position = 8;
                r.ReadString();
                string newPassword = r.ReadString();
                p.Match.Broadcast(Packets.Packets.SingleStringPacket(91, newPassword));
            }
        }
    }
}
