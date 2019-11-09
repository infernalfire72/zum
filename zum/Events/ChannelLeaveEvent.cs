using zum.Objects;

namespace zum.Events
{
    public static class ChannelLeaveEvent
    {
        public static void Handle(Player p, byte[] Data)
        {
            string Name = Tools.Ext.ReadStringFast(Data, 0);
            if (!Name.StartsWith("#")) return;
            if (Name == "#highlight" || Name == "#userlog") return;
            Global.FindChannel(Name)?.PlayerLeave(p);
        }
    }
}
