using System.Diagnostics;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Chair.Domain.Messaging;

namespace Chair.Infrastructure.Messaging;

public class SqsPublisher : IMessagePublisher
{
    private readonly string _queueUrl;
    private readonly AmazonSQSClient _sqsClient;

    public SqsPublisher(string queueUrl)
    {
        _queueUrl = queueUrl;
        _sqsClient = new AmazonSQSClient();
    }

    public async Task PublishAsync(object message, CancellationToken cancellationToken, string? traceparent = null)
    {
        var json = JsonSerializer.Serialize(message);
        var request = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = json,
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                ["traceparent"] = new MessageAttributeValue
                {
                    DataType = "String",
                    StringValue = traceparent
                }
            }
        };
        await _sqsClient.SendMessageAsync(request, cancellationToken);
    }
}