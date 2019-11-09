using System;
using System.Collections.Generic;
using System.Text;

namespace zum.Objects
{
    public class Channel
    {
        public string Name { get; set; }
        public string Topic { get; set; }
        public short Users => (short)(Players?.Count ?? 0);
        public bool AdminRead { get; set; }
        public bool AdminWrite { get; set; }
        public List<Player> Players { get; set; }

        public Channel(string name, string topic, bool OpenRead = true, bool OpenWrite = true)
        {
            Name = name;
            Topic = topic;
            Players = new List<Player>();
            AdminRead = !OpenRead;
            AdminWrite = !OpenWrite;
        }

        public bool PlayerJoin(Player p)
        {
            if (AdminRead && !p.IsAdmin)
                return false;
            p.Channels.Add(this);
            this.Players.Add(p);
            return true;
        }

        public void PlayerKick(Player p)
        {
            PlayerLeave(p);
            p.AddQueue(Packets.Packets.SingleStringPacket(66, this.Name));
        }

        public void PlayerLeave(Player p)
        {
            p.Channels.Remove(this);
            this.Players.Remove(p);
        }
    }
}
