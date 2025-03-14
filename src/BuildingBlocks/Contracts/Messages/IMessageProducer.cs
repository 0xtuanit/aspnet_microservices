namespace Contracts.Messages;

public interface IMessageProducer
{
    Task SendMessage<T>(T message);
}