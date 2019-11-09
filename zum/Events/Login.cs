using MySql.Data.MySqlClient;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using zum.Objects;

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
        }
    }
}
