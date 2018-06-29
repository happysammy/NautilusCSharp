namespace Nautilus.Database.Interfaces
{
    public interface IChannelPublisher
    {
        void Publish(string channel, string message);
    }
}
