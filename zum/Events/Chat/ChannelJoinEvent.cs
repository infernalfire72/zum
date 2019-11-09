using zum.Objects;

namespace zum.Events
{
    public static class ChannelJoinEvent
    {
        public static void Handle(Player p, byte[] Data)
        {
            string Channel = Tools.Ext.ReadStringFast(Data, 0); // this is usually in ANSI Encoding
            if (Channel == null || !Channel.StartsWith("#")) return;
            if (Global.FindChannel(Channel)?.PlayerJoin(p) ?? false)
            {
                p.AddQueue(Packets.Packets.SingleStringPacket(64, Channel));
                Log.LogFormat($"%#00FF44%{p.Username} joined {Channel}");
                return;
            }
            p.AddQueue(Packets.Packets.SingleStringPacket(66, Channel));
        }
    }
}
