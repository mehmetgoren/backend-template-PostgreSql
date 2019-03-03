namespace Server.Application
{
    public enum DB
    {
        Default
    }

    public interface IConnectionStringProvider
    {
        string GetConnectionString(DB db);
    }
}
