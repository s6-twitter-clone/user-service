﻿using System.Text;
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

    public void Publish<T>(string exchange, string topic, T data)
    {
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable: true, autoDelete: false);

        var json = JsonSerializer.Serialize(data);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: exchange, routingKey: topic, body: body);
    }

    public void subscribe<T>(string exchange, string queue, string topic,  Action<T> handler)
    {
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct, durable: true, autoDelete: false);
        channel.QueueDeclare(queue, exclusive: false, autoDelete: false);
        channel.QueueBind(exchange: exchange, routingKey: topic, queue: queue);

        channel.BasicQos(0, 1, false);

        

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var data = JsonSerializer.Deserialize<T>(message);

            handler.Invoke(data);
        };

        channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);
    }

}
