namespace Server.Rest
{
    public interface IObserver
    {
        void Notify(object args,  string message);
    }
}
