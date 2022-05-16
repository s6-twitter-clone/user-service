using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using user_service.Interfaces;

namespace user_service.Services;

public class EventService : IEventService, IDisposable
{
    private readonly IConnection connection;
    public EventService(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration.GetConnectionString("rabbitmq")
        };

        connection = factory.CreateConnection();
    }

    public void Dispose()
    {
        connection.Close();
    }

    public void Publish<T>(string topic, T data)
    {
        var channel = connection.CreateModel();

        channel.QueueDeclare(topic, exclusive: false, autoDelete: false);

        var json = JsonSerializer.Serialize(data);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: "", routingKey: topic, body: body);
    }

    public void subscribe<T>(string topic, Action<T> handler)
    {
        var channel = connection.CreateModel();

        channel.QueueDeclare(topic, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var data = JsonSerializer.Deserialize<T>(message);

            handler.Invoke(data);
        };

        channel.BasicConsume(queue: topic, autoAck: true, consumer: consumer);
    }

}
