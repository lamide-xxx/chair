namespace Chair.Domain.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync(object message, CancellationToken cancellationToken, string? traceparent = null);
}