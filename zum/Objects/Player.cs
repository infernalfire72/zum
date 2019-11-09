using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using zum.Packets;

namespace zum.Objects
{
    public enum ActionType : byte
    {
        Idle,
        AFK,
        Playing,
        Editing,
        Modding,
        Multiplayer,
        Watching,
        Ranking,
        Testing,
        Submitting,
        Paused,
        Lobby,
        Multiplaying,
        Direct,
        None
    }

    public struct UserStats
    {
        public int Performance, Playcount, Rank, MaxCombo;
        public long TotalScore, RankedScore;
        public float Accuracy;
    }

    public class Player
    {
        // discord.gg/csharp made me use auto props
        public string Username { get; private set; }
        public string SafeUsername => Username.Replace(" ", "_").ToLower();
        public string Password { get; set; }
        public string Token { get; }
        public int Id { get; }

        public byte IngamePrivileges { get; set; } = 21; // Developer Perms for now
        public bool IsAdmin => (IngamePrivileges & 16) != 0;
        public byte Country { get; set; } = 245;
        public byte Gamemode { get; set; } = 0;

        public UserStats[] Stats { get; set; }

        public ActionType Action { get; set; } = ActionType.Idle;
        public string ActionText { get; set; }
        public string ActionMd5 { get; set; }
        public int ActionMods { get; set; } = 0;
        public int ActionBeatmap { get; set; } = 0;

        public List<Channel> Channels { get; set; }
        public List<Player> Spectators { get; set; }
        public Player Spectating { get; set; }
        // public MultiplayerLobby Match { get; set; }

        public long Ping { get; set; }
        public bool Bot { get; private set; }

        private BinaryWriter w;
        private MemoryStream Stream { get; set; }
        public long StreamLength => Stream?.Length ?? 0;

        public Player(string username, int id, bool bot = false)
        {
            Username = username;
            Id = id;
            Bot = bot;
            if (!Bot)
            {
                Token = Guid.NewGuid().ToString();
                Stats = new UserStats[7];
                Channels = new List<Channel>();
                Spectators = new List<Player>();
                Ping = DateTime.Now.Ticks;
                Stream = new MemoryStream();
                w = new BinaryWriter(Stream);
            }
        }

        public void StreamCopyTo(Stream output)
        {
            if (Stream == null) return;
            Stream.Flush();
            Stream.Position = 0;
            Stream.CopyTo(output);
            Stream.SetLength(0);
        }

        public void AddQueue(Packet p)
        {
            if (Bot) return;
            p.Write(w);
        }

        private static string[] modes = { "std", "taiko", "ctb", "mania" };
        public async Task GetStatsFixed(byte mode)
        {
            string table = mode > 3 ? "rx" : "users";
            mode -= (byte)(mode > 3 ? 4 : 0);
            using (DbDataReader r = await Database.RunQuery($"SELECT ranked_score_{modes[mode]}, playcount_{modes[mode]}, total_score_{modes[mode]}, avg_accuracy_{modes[mode]}/100, pp_{modes[mode]} FROM {table}_stats WHERE id = {Id} LIMIT 1"))
            {
                if (r.Read())
                {
                    Stats[mode].RankedScore = r.GetInt64(0);
                    Stats[mode].Playcount = r.GetInt32(1);
                    Stats[mode].TotalScore = r.GetInt64(2);
                    Stats[mode].Accuracy = r.GetFloat(3);
                    Stats[mode].Performance = r.GetInt32(4);
                }
            }

            using (DbDataReader r = await Database.RunQuery($"SELECT COUNT(id) FROM {table}_stats WHERE pp_{modes[mode]} >= {Stats[mode].Performance}"))
            {
                if (r.Read())
                    Stats[mode].Rank = r.GetInt32(0);
            }
        }

        public async Task GetStats()
        {
            for (byte i = 0; i < modes.Length; i++)
            {
                await GetStatsFixed(i);
            }
        }
        public async Task<List<int>> GetFriends()
        {
            List<int> ids = new List<int>();
            using (DbDataReader r = await Database.RunQuery($"SELECT user2 FROM users_relationships WHERE user1 = {Id};"))
                while (r.Read())
                    ids.Add(r.GetInt32(0));
            return ids;
        }

        public static void RemovePlayer(Player p)
        {
            Log.WriteLine($"{p.Username} logged out.");
            Global.Players.Remove(p);
            // Remove from Channels
            for (int i = 0; i < p.Channels.Count; i++) p.Channels[i].PlayerLeave(p);
            // Remove from Spectators
            for (int i = 0; i < p.Spectators.Count; i++)
                if (p.Spectators[i] != null)
                    p.Spectators[i].Spectating = null;
            // Remove from People spectated
        // if (p.Spectating != null) StopSpectateEvent.Handle(p);
            // Remove from Lobbies
        // Global.LeaveLobby(p);
        // if (p.Match != null) LeaveMatch.Handle(p);
            // Tell other Players we went away
            Broadcast(Packets.Packets.LogoutPacket(p.Id));
        }

        public static void Broadcast(Packet p)
        {
            for(int i = 0; i < Global.Players.Count; i++)
                Global.Players[i].AddQueue(p);
        }
    }
}
