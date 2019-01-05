namespace Server.Rest
{
    using ionix.Data;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using System;
    using System.IO;
    using ionix.Data.PostgreSql;

    public static class Program
    {
        public static void Main(string[] args)
        {
            OnStartup.Instance.LogSqlScript(LogSqlScript);
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls(DataSources.Jsons.AppSettings.Server.Address)
                .UseStartup<Startup>()
                .Build();


        private static void LogSqlScript(ExecuteSqlCompleteEventArgs e)
        {
            try
            {
                string path = e.Succeeded ? DataSources.Jsons.AppSettings.Server.SqlPath : DataSources.Jsons.AppSettings.Server.SqlErrorPath;

                if (!String.IsNullOrEmpty(path))
                {
                    using (Stream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            if (e.Query != null)
                            {
                                sw.WriteLine(e.Query.ToParameterlessQuery());
                                sw.WriteLine();
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}
