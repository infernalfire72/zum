using System;
using zum.Objects;

namespace zum.Events
{
    public static class RemoveFriend
    {
        public async static void Handle(Player p, byte[] Data)
        {
            if (Data.Length != 4) return;
            int fId = BitConverter.ToInt32(Data, 0);
            await Database.Execute($"DELETE FROM users_relationships WHERE user1 = {p.Id} AND user2 = {fId}");
        }
    }
}
