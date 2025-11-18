using System.Diagnostics;
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
    private readonly ILogger<SqsListener> _logger;

    public SqsListener(IConfiguration configuration, ILogger<SqsListener> logger)
    {
        _logger = logger;
        _queueUrl = configuration["SQS_QUEUE_URL"] ?? throw new ArgumentNullException(nameof(configuration),"SQS_QUEUE_URL is not configured");
        _sqsClient = new AmazonSQSClient();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SQS Listener started. Listening to queue {QueueUrl}", _queueUrl);
        
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
                _logger.LogInformation("No messages this round.");
                await Task.Delay(5000, stoppingToken);
                continue;
            }

            
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogInformation("Retry {RetryCount} after: {TimeSpanTotalSeconds}s. Due to exception: {ExceptionMessage}.", retryCount, timeSpan.TotalSeconds, exception.Message);
                    });
            foreach (var message in response.Messages)
            {
                using var activity = Telemetry.ActivitySource.StartActivity("ProcessBookingMessage");
                try
                {
                    activity?.SetTag("sqs.message.id", message.MessageId);
                    activity?.SetTag("sqs.receipt.handle", message.ReceiptHandle);
                    activity?.SetTag("sqs.queue.url", _queueUrl);
                    
                    _logger.LogInformation("Received message: {MessageBody}", message.Body);

                    // Process the message here
                    var bookingEvent = JsonSerializer.Deserialize<BookingEvent>(message.Body);
                    
                    activity?.SetTag("booking.appointmentId", bookingEvent?.AppointmentId);
                    activity?.SetTag("booking.eventType", bookingEvent?.Type);

                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        using var sendActivity = Telemetry.ActivitySource.StartActivity("SendNotification");
            
                        sendActivity?.SetTag("notification.appointmentId", bookingEvent?.AppointmentId);
                        sendActivity?.SetTag("notification.type", bookingEvent?.Type);
                        
                        await SendNotificationAsync(bookingEvent);
                    });

                    // Delete the message after processing
                    var deleteRequest = new DeleteMessageRequest
                    {
                        QueueUrl = _queueUrl,
                        ReceiptHandle = message.ReceiptHandle
                    };
                    _sqsClient.DeleteMessageAsync(deleteRequest, stoppingToken).Wait();
                    _logger.LogInformation("Message processed and deleted from queue.");
                }
                catch (Exception ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error);
                    activity?.SetTag("exception.type", ex.GetType().Name);
                    activity?.SetTag("exception.message", ex.Message);
                    activity?.SetTag("exception.stacktrace", ex.StackTrace);
                    
                    _logger.LogInformation(ex, "Error processing message for queueUrl: {QueueUrl}", _queueUrl);
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
        _logger.LogInformation("Sending notification for AppointmentId {AppointmentId} of type {BookingEventType}", bookingEvent?.AppointmentId, bookingEvent?.Type);
                    
        // TODO: send actual email/push here
    }
}