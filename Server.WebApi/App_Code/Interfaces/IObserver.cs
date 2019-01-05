namespace Server.WebApi
{
    public interface IObserver
    {
        void Notify(object args, string message);
    }
}
