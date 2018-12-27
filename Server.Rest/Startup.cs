namespace Server.Rest
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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //*ionixFactory.SetConnectionStringProviderType<ConnectionStringProvider>();
            IndexedRoles.IgnoreCase = true;
            //*TokenTableParams.SessionTimeout = Config.WebApiSessionTimeout;
            //*ionixFactory.InitMigration();

            StartNanoServices();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(opt =>
            {
                
            });

            //services.AddSingleton<IUtilsService, UtilsService>();

            //*services.AddSingleton<ServerMonitoringHubImpl, ServerMonitoringHubImpl>();

            //*services.AddScoped<Lazy<DbContext>, Lazy<DbContext>>((sp) => new Lazy<DbContext>(ionixFactory.CreateDbContext, true));//Bu Yapıda Gereksiz DbConetx ve connection nesnesi oluşturuluyor

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            })
            .AddJsonProtocol(config =>
            {
                config.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver();//to disable camelCase
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //*if (Config.WebApiAuthEnabled)
               //* app.UseTokenTableAuthentication(TokenTable.Instance, AuthorizationValidator.Instance);

            app.UseCors(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute("default", "api/{controller}/{action}/{id?}");
            //});

            app.UseMvc();

            //signalr
            app.UseSignalR(routes =>
            {
                //*routes.MapHubs(Assembly.GetExecutingAssembly());
                //routes.MapHub<ServerMonitoringHub>("/servermonitoring");
                //routes.MapHub<ImagesHub>("/images");
            });


            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("Hi From Aps.Net Core");
            //});
        }


        private static void StartNanoServices()
        {
           // ServerMonitoringService.Instance.Start();
        }

    }
}
