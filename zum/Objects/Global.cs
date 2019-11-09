using System.Collections.Generic;
using zum.Objects;
namespace zum
{
    public static class Global
    {
        public static List<Player> Players = new List<Player>();
        public static Player FindPlayer(string Token)
        {
            for (int i = 0; i < Players.Count; i++)
                if (Players[i].Token == Token)
                    return Players[i];

            return null;
        }

        public static Player FindPlayerByName(string Name, bool Safe = false)
        {
            for (int i = 0; i < Players.Count; i++)
                if (Safe ? Players[i].SafeUsername == Name : Players[i].Username == Name)
                    return Players[i];

            return null;
        }

        public static Player FindPlayerById(int Id)
        {
            for (int i = 0; i < Players.Count; i++)
                if (Players[i].Id == Id)
                    return Players[i];

            return null;
        }

        public static List<Channel> Channels = new List<Channel>();
        public static Channel FindChannel(string Name)
        {
            for (int i = 0; i < Channels.Count; i++)
                if (Channels[i].Name == Name)
                    return Channels[i];
            return null;
        }

        public static void AddChannel(Channel c) => Channels.Add(c);
        public static void SetupChannels()
        {
            AddChannel(new Channel("#osu", "Main osu! Channel"));
            AddChannel(new Channel("#announce", "Announcements Channel", OpenWrite: false));
            AddChannel(new Channel("#admin", "Admin/Development Channel", false));
            AddChannel(new Channel("#lobby", "Multiplayer Lobby Channel/Discussion"));
        }
    }
}
