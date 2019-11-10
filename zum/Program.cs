using System.Drawing;
using System.Threading.Tasks;

namespace zum
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Database.ConnectionString = "Server=localhost;Database=dbname;Uid=root;Pwd=password;";
            if (!await Database.connection.PingAsync()) return;
            Log.WriteLine("Connected to MySQL Server.", Color.LightGreen);
            Global.SetupChannels();
            new Server(5001);
            await Task.Delay(-1);
        }
    }
}
