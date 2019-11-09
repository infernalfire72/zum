using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using zum.Events;
using zum.Tools;

namespace zum
{
    public class Server
    {
        HttpListener listener;
        Thread listent;
        public bool Listening => listener.IsListening;
        private int _Port;
        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                if (Rebind(value))
                    _Port = value;
            }
        }

        public Server(int Port)
        {
            listener = new HttpListener();
            this.Port = Port;
        }

        bool Rebind(int Port)
        {
            if (listener == null) return false;
            if (listent != null && listent.ThreadState == ThreadState.Running) listent.Abort();
            if (listener.IsListening)
                listener.Stop();
            listener.Prefixes.Clear();
            listener.Prefixes.Add($"http://*:{Port}/");
            listener.Start();
            Log.LogFormat($"%#00FF00%> Now Listening on Port %#FFFF00%{Port}");
            Listen();
            return true;
        }

        public void Stop()
        {
            listent.Abort();
            listener.Stop();
        }

        void Listen()
        {
            listent = new Thread(async () =>
            {
                while (true)
                {
                    try
                    {
                        HttpListenerContext ctx = await listener.GetContextAsync();
                        new Thread(async () => await ProcessAsync(ctx)).Start();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });

            listent.Start();
        }

        async Task ProcessAsync(HttpListenerContext ctx)
        {
            if (ctx.Request.UserAgent != "osu!") return;
            if (ctx.Request.Url.AbsolutePath.StartsWith("/web/"))
            {
                return;
            }

            if (string.IsNullOrEmpty(ctx.Request.Headers["osu-token"]))
            {
                await Login.Handle(ctx);
                return;
            }

            var p = Global.FindPlayer(ctx.Request.Headers["osu-token"]);

            if (p == null) // if they arent part of the player collection, force them to relogin
            {
                ctx.Response.StatusCode = 403;
                ctx.Response.OutputStream.Write(Packets.Packets.SingleIntPacket(5, -5));
                ctx.Response.Close();
                return;
            }

            using (MemoryStream ms = new MemoryStream())
            using (BinaryReader r = new BinaryReader(ms))
            {
                await ctx.Request.InputStream.CopyToAsync(ms);
                ms.Position = 0;
                while (ms.Position < ctx.Request.ContentLength64 - 7)
                {
                    short Id = r.ReadInt16();
                    ms.Position += 1;
                    int Length = r.ReadInt32();
                    if (ms.Position + Length > ctx.Request.ContentLength64) break;
                    if (Id == 68 || Id == 79)
                    {
                        ms.Position += Length;
                        continue;
                    }

                    byte[] Data = r.ReadBytes(Length);

                    switch (Id)
                    {
                        case 4: p.Ping = DateTime.Now.Ticks; break;
                        case 0:
                            StatusUpdate.Handle(p, Data);
                            break;
                        case 63:
                            ChannelJoinEvent.Handle(p, Data);
                            break;
                        default:
                            Log.LogFormat($"%#FFFF%Unhandled Packet {Id} with {Length}");
                            break;
                    }
                }
            }

            if (p.StreamLength != 0) p.StreamCopyTo(ctx.Response.OutputStream);
            ctx.Response.Close();
        }
    }
}
