using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Chair.Domain.Events;

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

            foreach (var message in response.Messages)
            {
                try
                {
                    Console.WriteLine($"Received message: {message.Body}");

                    // Process the message here
                    var bookingEvent = JsonSerializer.Deserialize<BookingEvent>(message.Body);
                    
                    Console.WriteLine($"Sending notification for AppointmentId {bookingEvent?.AppointmentId} of type {bookingEvent?.Type}");
                    
                    // TODO: send actual email/push here

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
}