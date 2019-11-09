using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using zum.Events;

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


        }
    }
}
