namespace Server.WebApi
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using ionix.Rest;
    using Microsoft.AspNetCore.SignalR;

    [TokenTableAuth]
    public class ImagesHub : BaseHub
    {
        public void FloodImages()
        {
            string path = Directory.GetCurrentDirectory();

            path += "/assets/images";

            foreach (string fileName in Directory.GetFiles(path))
            {
                this.Clients.All.SendAsync("notify", File.ReadAllBytes(fileName));
                Thread.Sleep(16);//60FPS
                if (this.closed)
                    break;
            }
        }

        private bool closed;
        public override Task OnDisconnectedAsync(Exception exception)
        {
            this.closed = true;
            return base.OnDisconnectedAsync(exception);
        }
    }

    public sealed class ImagesHubService//Timer döngü gibi durumlarda bu pattern i kullan.
    {
        internal static readonly Type ImagesHubServiceType = typeof(ImagesHubService);

        private readonly IHubContext<ImagesHub> _context;
        public ImagesHubService(IHubContext<ImagesHub> context)
        {
            this._context = context;
        }

        private static bool _started;
        private static readonly object _syncRoot = new object();
        internal void StartFloodImages()
        {
            if (!_started)
            {
                lock (_syncRoot)
                {
                    if (!_started)
                    {
                        _started = true;
                    }
                    else
                        return;
                }

                try
                {
                    string path = Directory.GetCurrentDirectory();

                    path += "/assets/images";

                    foreach (string fileName in Directory.GetFiles(path))
                    {
                        this._context.Clients.All.SendAsync("notify", File.ReadAllBytes(fileName));
                        Thread.Sleep(16);//60FPS
                    }
                }
                finally
                {
                    _started = false;
                }
            }
        }
    }
}
