namespace Server.WebApi
{
    using System.Linq;
    using ionix.Utils.Extensions;
    using Microsoft.AspNetCore.SignalR;
    using Models;
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using ionix.Rest;
    using Server.Application;

    [TokenTableAuth]
    public sealed class ServerMonitoringHub : BaseHub
    {
        //private readonly ServerMonitoringHubImpl _impl;
        //public ServerMonitoringHub(ServerMonitoringHubImpl impl)
        //{
        //    this._impl = impl;
        //}

        //public void Start()
        //{
        //    this._impl.Start();
        //}

        private readonly IServiceProvider _serviceProvider;
        public ServerMonitoringHub(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public void Start()
        {
            ServerMonitoringHubImpl impl = this._serviceProvider.GetService<ServerMonitoringHubImpl>();

            impl.Start();
        }
    }

    public sealed class ServerMonitoringHubImpl : IObserver, IDisposable
    {
        private IHubContext<ServerMonitoringHub> HubContext { get; }

        public ServerMonitoringHubImpl(IHubContext<ServerMonitoringHub> hubContext)
        {
            this.HubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public void Notify(object args, string message)
        {
            this.HubContext.Clients.All.SendAsync("notify", args);
        }


        public void Start()
        {
            ServerMonitoringService.Instance.Start();

            ServerMonitoringService.Instance.Ticked -= this.OnTick;
            ServerMonitoringService.Instance.Ticked += this.OnTick;
        }

        public void Dispose()
        {
            ServerMonitoringService.Instance.Ticked -= this.OnTick;

            ServerMonitoringService.Instance.Stop();
        }


        private void OnTick(object sender, ServerMonitoringEventArgs e)
        {
            this.OnServerMonitoringTicked(e);
        }

        private void OnServerMonitoringTicked(ServerMonitoringEventArgs e)
        {
            ServerInfo info = e.ServerInfo;

            ChartModel cmCpu = CreateMonitoringChartModel("Cpu Usage %", info.CpuUsage, "Available Cpu %", 100.0);
            ChartModel cmRam = CreateMonitoringChartModel("Memory Usage (MB)", info.MemoryUsage, "Available Memory (MB)", 100.0 - info.MemoryUsage);
            ChartModel cmHdd = CreateMonitoringChartModel("HDD Usage (MB)", info.DiskUsage, "Available HDD Alanı (MB)", 100.0 - info.DiskUsage);

            this.Notify(new { info, cmCpu, cmRam, cmHdd }, null);
        }

        private static ChartModel CreateMonitoringChartModel(string usedText, object usedValue, string availableText, object availableValue)
        {
            ChartModel ret = new ChartModel();
            ChartModelDataSet dataSet = new ChartModelDataSet();
            ret.datasets = dataSet.ToSingleItemList().ToList();

            ret.labels.Add(usedText);
            dataSet.data.Add(usedValue);

            ret.labels.Add(availableText);
            dataSet.data.Add(availableValue);

            string color = Utility.CreateRandomColorCode();
            dataSet.backgroundColor.Add(color);
            dataSet.hoverBackgroundColor.Add(color);

            return ret;
        }
    }
}