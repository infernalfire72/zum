using MySql.Data.MySqlClient;
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

            BinaryWriter w = new BinaryWriter(ctx.Response.OutputStream);
            Player p = new Player(Username, Id);

            p.Password = Password;
            ctx.Response.Headers["cho-token"] = p.Token;
            w.Write(Packets.Packets.SingleIntPacket(5, Id));
            w.Write(Packets.Packets.SingleIntPacket(75, 19));
            w.Write(Packets.Packets.SingleStringPacket(24, $"Welcome to osu!Bancho!\r\nUsing züm 0.1a"));
            await p.GetStats();

            ctx.Response.Close();
        }
    }
}
