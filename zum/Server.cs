using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        }
    }
}
