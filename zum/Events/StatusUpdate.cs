using zum.Objects;
using zum.Packets;

namespace zum.Events
{
    public static class StatusUpdate
    {
        public static void Handle(Player p, byte[] Data)
        {
            using (CustomReader r = new CustomReader(Data))
            {
                p.Action = (ActionType)r.ReadByte();
                p.ActionText = r.ReadString();
                p.ActionMd5 = r.ReadString();
                p.ActionMods = r.ReadInt32();
                p.Relax = (p.ActionMods & 128) != 0;
                p.Gamemode = r.ReadByte();
                p.ActionBeatmap = r.ReadInt32();

                if (p.Action == ActionType.Playing && p.ActionMods != 0) p.ActionText += " +" + Tools.ModConvert.ToString(p.ActionMods);

                Player.Broadcast(Packets.Packets.StatsPacket(p));
            }
        }
    }
}
