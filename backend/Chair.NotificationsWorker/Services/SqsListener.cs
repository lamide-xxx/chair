using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Chair.Domain.Events;
using Polly;

namespace Chair.NotificationsWorker.Services;

public class SqsListener : BackgroundService
{
    private readonly string _queueUrl;
    private readonly AmazonSQSClient _sqsClient;

    public SqsListener(IConfiguration configuration)
    {
        _queueUrl = configuration["SQS_QUEUE_URL"] ?? throw new ArgumentNullException("SQS_QUEUE_URL is not configured");
        _sqsClient = new AmazonSQSClient();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"SQS Listener started. Listening to queue {_queueUrl}");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var request = new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = 5,
                WaitTimeSeconds = 10 // Long polling
            };

            var response = await _sqsClient.ReceiveMessageAsync(request, stoppingToken);
            
            if (response.Messages == null || !response.Messages.Any())
            {
                Console.WriteLine("No messages this round.");
                await Task.Delay(5000, stoppingToken);
                continue;
            }

            
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry {retryCount} after: {timeSpan.TotalSeconds}s. Due to exception: {exception.Message}.");
                    });
            foreach (var message in response.Messages)
            {
                try
                {
                    Console.WriteLine($"Received message: {message.Body}");

                    // Process the message here
                    var bookingEvent = JsonSerializer.Deserialize<BookingEvent>(message.Body);

                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        await SendNotificationAsync(bookingEvent);
                    });

                    // Delete the message after processing
                    var deleteRequest = new DeleteMessageRequest
                    {
                        QueueUrl = _queueUrl,
                        ReceiptHandle = message.ReceiptHandle
                    };
                    _sqsClient.DeleteMessageAsync(deleteRequest, stoppingToken).Wait();
                    Console.WriteLine("Message processed and deleted from queue.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    // Optionally handle the error, e.g., log it or move the message to a dead-letter queue
                }
            }
        }
    }

    private async Task SendNotificationAsync(BookingEvent bookingEvent)
    {
        if (new Random().NextDouble() < 0.9) // 20% chance to simulate failure
        {
            throw new HttpRequestException("Simulated transient failure");
        }
        await Task.Delay(5000);
        Console.WriteLine($"Sending notification for AppointmentId {bookingEvent?.AppointmentId} of type {bookingEvent?.Type}");
                    
        // TODO: send actual email/push here
    }
}