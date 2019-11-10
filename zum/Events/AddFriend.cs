using System;
using zum.Objects;

namespace zum.Events
{
    public static class AddFriend
    {
        public async static void Handle(Player p, byte[] Data)
        {
            if (Data.Length != 4) return;
            int fId = BitConverter.ToInt32(Data, 0);
            await Database.Execute($"REPLACE INTO users_relationships VALUES ({p.Id}, {fId})");
        }
    }
}
