using System;
using zum.Objects;
using zum.Packets;

namespace zum.Events
{
    public static class StartSpectating
    {
        public static void Handle(Player p, byte[] Data)
        {
            if (p.Spectating != null) StopSpectating.Handle(p);
            int User = BitConverter.ToInt32(Data, 0);
            if (User == p.Id) return;
            Player Host = Global.FindPlayerById(User);
            if (Host == null || Host.Bot) return; // cursed cases (or funny people using custom bancho clients)
            p.Spectating = Host;
            Host.Spectators.Add(p);
            if (Host.Spectators.Count == 1) Host.AddQueue(Packets.Packets.SingleStringPacket(64, "#spectator"));
            p.AddQueue(Packets.Packets.SingleStringPacket(64, "#spectator"));
            Packet pack = Packets.Packets.ChannelAvailable("#spectator", $"Spectator Channel for {Host.Username}.", (short)(Host.Spectators.Count + 1));
            Host.AddQueue(pack);
            Host.SendSpectator(pack);
            Host.AddQueue(Packets.Packets.SingleIntPacket(13, p.Id));
            Host.SendSpectator(Packets.Packets.SingleIntPacket(42, p.Id));
        }
    }
}
