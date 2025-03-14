using System.Text;
using Contracts.Common.Interfaces;
using Contracts.Messages;
using RabbitMQ.Client;

namespace Infrastructure.Messages;

public class RabbitMQProducer : IMessageProducer
{
    private readonly ISerializeService _serializeService;

    public RabbitMQProducer(ISerializeService serializeService)
    {
        _serializeService = serializeService;
    }

    public async Task SendMessage<T>(T message)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "thomas",
            Password = "Admin3000",
        };

        using var connection = await connectionFactory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "orders",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var jsonData = _serializeService.Serialize(message); // Serialize the message to JSON
        var body = Encoding.UTF8.GetBytes(jsonData);

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "orders", body: body);
    }
}