﻿using System;
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

        // public List<Channel> Channels { get; set; }
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
                Stats = new UserStats[4];
                // Channels = new List<Channel>();
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
            using (DbDataReader r = await Database.RunQuery($"SELECT ranked_score_{modes[mode]}, playcount_{modes[mode]}, total_score_{modes[mode]}, avg_accuracy_{modes[mode]}/100, pp_{modes[mode]} FROM users_stats WHERE id = {Id} LIMIT 1"))
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

            using (DbDataReader r = await Database.RunQuery($"SELECT COUNT(id) FROM users_stats WHERE pp_{modes[mode]} >= {Stats[mode].Performance}"))
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


    }
}