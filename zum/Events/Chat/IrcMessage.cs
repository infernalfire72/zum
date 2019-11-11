using zum.Objects;
using zum.Packets;

namespace zum.Events
{
    public static class IrcMessage
    {
        public static void Handle(Player p, byte[] Data)
        {
            using (CustomReader r = new CustomReader(Data))
            {
                if (Data[0] == 0)
                    r.BaseStream.Position += 2;
                else
                    r.ReadString();

                string Content = r.ReadString();
                string Target = r.ReadString();
                /*if (Content.StartsWith(CommandManager.Prefix))
                {
                    if (CommandManager.HandleCommands(p, msg.Content, msg.Target)) return;
                }*/

                if (Target == "#spectator")
                {
                    if (p.Spectating == null && p.Spectators.Count > 0)
                        p.MessageSpectators(p, Content);
                    else if (p.Spectating != null)
                        p.Spectating.MessageSpectators(p, Content);
                    return;
                }
                if (Target == "#multiplayer")
                {
                    if (p.Match == null) return;
                    if (!p.Match.Players.Contains(p)) return;
                    Packet pack = Packets.Packets.IrcMessage(Content, p.Username, Target, p.Id);
                    for (int i = 0; i < p.Match.Players.Count; i++)
                        if (p.Match.Players[i] != p)
                            p.Match.Players[i].AddQueue(pack);
                    return;
                }
                Global.FindChannel(Target).Message(p, Content);
            }
        }

        public static void HandlePrivate(Player p, byte[] Data)
        {
            using (CustomReader r = new CustomReader(Data))
            {
                if (Data[0] == 0)
                    r.BaseStream.Position += 2;
                else
                    r.ReadString();

                string Content = r.ReadString();
                string Target = r.ReadString();
                // TODO
                /* if (Target == Bot.Name)
                {
                    Target = p.Username;
                    return;
                } */
                Global.FindPlayerByName(Target)
                ?.AddQueue(Packets.Packets.IrcMessage(Content, p.Username, Target, p.Id));
            }
        }
    }
}
