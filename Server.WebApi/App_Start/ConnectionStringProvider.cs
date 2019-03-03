namespace Server.WebApi
{
    using Server.Application;

    public sealed class ConnectionStringProvider : IConnectionStringProvider
    {
        public string GetConnectionString(DB db)
        {
            return DataSources.Jsons.AppSettings.ConnectionStrings.Default;
        }
    }
}
