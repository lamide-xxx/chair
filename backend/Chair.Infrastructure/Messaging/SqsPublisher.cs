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

    public async Task PublishAsync(object message)
    {
        var json = JsonSerializer.Serialize(message);
        var request = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = json
        };
        await _sqsClient.SendMessageAsync(request);
    }
}