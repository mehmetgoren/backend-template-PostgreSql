namespace Server.WebApi
{
    using System;
    using System.Reflection;
    using ionix.Rest;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Serialization;

    public class Startup
    {
        internal static IServiceCollection ServiceCollection { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            IndexedRoles.IgnoreCase = true;

            OnStartup.Instance
                .SetConnectionStringProviderType<ConnectionStringProvider>()
                .InitMigration();

            TokenTableParams.SessionTimeout = Config.WebApiSessionTimeout;

            StartNanoServices();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(opt =>
            {
                
            });

            services.AddSingleton<IUtilsService, UtilsService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IAdminPanelService, AdminPanelService>();
            services.AddSingleton<IUnauthorizedService, UnauthorizedService>();

            services.AddSingleton<ServerMonitoringHubImpl, ServerMonitoringHubImpl>();

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            })
            .AddJsonProtocol(config =>
            {
                config.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver();//to disable camelCase
            });

            ServiceCollection = services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (Config.WebApiAuthEnabled)
                app.UseTokenTableAuthentication(TokenTable.Instance, AuthorizationValidator.Instance);

            app.UseCors(builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins("http://192.168.0.21:51")
                    .AllowCredentials();
            });

            app.UseMvc();

            //signalr
            app.UseSignalR(routes =>
            {
                routes.MapHubs(Assembly.GetExecutingAssembly());
                //routes.MapHub<ServerMonitoringHub>("/servermonitoring");
                //routes.MapHub<ImagesHub>("/images");
            });
        }


        private static void StartNanoServices()
        {
           // ServerMonitoringService.Instance.Start();
        }
    }
}
