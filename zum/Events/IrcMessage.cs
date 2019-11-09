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
                Global.FindChannel(Target).Message(p, Content);
            }
        }
    }
}
