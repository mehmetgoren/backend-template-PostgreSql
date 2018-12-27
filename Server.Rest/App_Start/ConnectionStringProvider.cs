namespace Server.Rest
{
    public sealed class ConnectionStringProvider : IConnectionStringProvider
    {
        public string GetConnectionString(DB db)
        {
            return DataSources.Jsons.AppSettings.ConnectionStrings.Default;
        }
    }
}
