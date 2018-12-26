namespace Server
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
