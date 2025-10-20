namespace Chair.Domain.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync(object message);
}