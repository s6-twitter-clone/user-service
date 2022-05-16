namespace user_service.Interfaces;

public interface IEventService
{
    public void Publish<T>(string topic, T data);
    public void subscribe<T>(string topic, Action<T> handler);
}