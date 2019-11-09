using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using zum.Objects;
using zum.Tools;

namespace zum.Events
{
    public static class Login
    {
        public static async Task Handle(HttpListenerContext ctx)
        {
            string Username;
            string Password;

            using (StreamReader r = new StreamReader(ctx.Request.InputStream))
            {
                Username = r.ReadLine();
                Password = r.ReadLine();
            }

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ctx.Response.Close();
                return;
            }

            int Id = 0;
            string DbPassword = null;

            using (DbDataReader r = await Database.RunQuery("SELECT username, id, password_md5 FROM users WHERE username = @user OR username_safe = @user LIMIT 1", new MySqlParameter("user", Username)))
            {
                if (r.Read())
                {
                    Username = r.GetString(0);
                    Id = r.GetInt32(1);
                    DbPassword = r.GetString(2);
                }
            }

            if (DbPassword == null || Id == 0 || !BCrypt.Net.BCrypt.Verify(Password, DbPassword))
            {
                ctx.Response.Headers["cho-token"] = "no";
                ctx.Response.OutputStream.Write(Packets.Packets.SingleIntPacket(5, -1));
                ctx.Response.Close();
                return;
            }

            if (Global.FindPlayerByName(Username) != null)
            {
                ctx.Response.Headers["cho-token"] = "no";
                ctx.Response.OutputStream.Write(Packets.Packets.SingleStringPacket(24, "You are already logged in from another Client."));
                ctx.Response.OutputStream.Write(Packets.Packets.SingleIntPacket(5, -2));
                ctx.Response.Close();
                return;
            }

            BinaryWriter w = new BinaryWriter(ctx.Response.OutputStream);
            Player p = new Player(Username, Id);

            p.Password = Password;
            ctx.Response.Headers["cho-token"] = p.Token;
            w.Write(Packets.Packets.SingleIntPacket(5, Id));
            w.Write(Packets.Packets.SingleIntPacket(75, 19));
            w.Write(Packets.Packets.SingleStringPacket(24, $"Welcome to osu!Bancho!\r\nUsing züm 0.1a"));
            await p.GetStats();
            w.Write(Packets.Packets.PresencePacket(p));
            w.Write(Packets.Packets.StatsPacket(p));

            w.Write(Packets.Packets.NoDataPacket(89));
            for (int i = 0; i < Global.Channels.Count; i++)
                if (Global.Channels[i].AdminRead && !p.IsAdmin) continue;
                else w.Write(Packets.Packets.ChannelAvailable(Global.Channels[i]));

            ctx.Response.Close();

            for (int i = 0; i < Global.Players.Count; i++)
            {
                w.Write(Packets.Packets.PresencePacket(Global.Players[i]));
                w.Write(Packets.Packets.StatsPacket(Global.Players[i]));
            }

            List<int> Friends = await p.GetFriends();
            if (Friends.Count > 0)
                p.AddQueue(Packets.Packets.FriendListPacket(Friends));

            Player.Broadcast(Packets.Packets.PresencePacket(p));
            Player.Broadcast(Packets.Packets.StatsPacket(p));
            Global.Players.Add(p);
            Log.LogFormat($"%#007cee%{p.Username} logged in with Token %#00FF33%{p.Token}");
        }
    }
}
