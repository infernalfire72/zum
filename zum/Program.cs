using System;
using System.Threading.Tasks;

namespace zum
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Database.ConnectionString = "Server=localhost;Database=dbname;Uid=root;Pwd=password;";
            await Database.connection.PingAsync();
            Global.SetupChannels();
            new Server(5001);
            await Task.Delay(-1);
        }
    }
}
