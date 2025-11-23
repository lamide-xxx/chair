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

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("SQS Listener started. Listening to queue {QueueUrl}", _queueUrl);
        
        while (!cancellationToken.IsCancellationRequested)
        {
            var request = new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = 5,
                WaitTimeSeconds = 10, // Long polling
                MessageAttributeNames = new List<string> { "All" }   
            };

            var response = await _sqsClient.ReceiveMessageAsync(request, cancellationToken);
            
            if (response.Messages == null || !response.Messages.Any())
            {
                _logger.LogInformation("No messages this round.");
                await Task.Delay(5000, cancellationToken);
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
                // Process the message here
                var bookingEvent = JsonSerializer.Deserialize<BookingEvent>(message.Body);
                    
                // recreate parent context
                var parentContext = default(ActivityContext);
                if (message.MessageAttributes == null)
                {
                    _logger.LogWarning("Message Attributes are null for messageId: {MessageId}", message.MessageId);
                }
                if (message.MessageAttributes != null 
                    && message.MessageAttributes.TryGetValue("traceparent", out var traceparentAttribute)
                    && !string.IsNullOrWhiteSpace(traceparentAttribute.StringValue))
                {
                    parentContext = ActivityContext.Parse(traceparentAttribute.StringValue, null);
                }
                
                using var processActivity = Telemetry.ActivitySource.StartActivity("ProcessBookingMessage", ActivityKind.Consumer, parentContext);
                
                processActivity?.SetTag("messaging.system", "aws.sqs");
                processActivity?.SetTag("messaging.destination_kind", "queue");
                processActivity?.SetTag("messaging.destination", _queueUrl);
                processActivity?.SetTag("messaging.operation", "process");
                processActivity?.SetTag("messaging.message_id", message.MessageId);
                
                processActivity?.SetTag("booking.appointment_id", bookingEvent?.AppointmentId);
                processActivity?.SetTag("booking.event_type", bookingEvent?.Type);
                processActivity?.SetTag("sqs.receipt_handle", message.ReceiptHandle);
                
                try
                {
                    _logger.LogInformation("Received message: {MessageBody}", message.Body);

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
                    await _sqsClient.DeleteMessageAsync(deleteRequest, cancellationToken);
                    _logger.LogInformation("Message processed and deleted from queue.");
                }
                catch (Exception ex)
                {
                    processActivity?.SetStatus(ActivityStatusCode.Error);
                    processActivity?.SetTag("exception.type", ex.GetType().Name);
                    processActivity?.SetTag("exception.message", ex.Message);
                    processActivity?.SetTag("exception.stacktrace", ex.StackTrace);
                    
                    _logger.LogInformation(ex, "Error processing message for queueUrl: {QueueUrl}", _queueUrl);
                    // Optionally handle the error, e.g., log it or move the message to a dead-letter queue
                }
            }
        }
    }

    private async Task SendNotificationAsync(BookingEvent bookingEvent)
    {
        if (new Random().NextDouble() < 0.1) // 20% chance to simulate failure
        {
            throw new HttpRequestException("Simulated transient failure");
        }
        await Task.Delay(5000);
        _logger.LogInformation("Sending notification for AppointmentId {AppointmentId} of type {BookingEventType}", bookingEvent?.AppointmentId, bookingEvent?.Type);
                    
        // TODO: send actual email/push here
    }
}