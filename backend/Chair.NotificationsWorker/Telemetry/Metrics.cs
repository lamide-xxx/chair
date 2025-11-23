using System.Diagnostics.Metrics;

namespace Chair.NotificationsWorker.Telemetry;

public static class Metrics
{
    public static readonly Meter Meter = new Meter("Chair.NotificationsWorker");
    public static readonly Histogram<double> Queuelag = Meter.CreateHistogram<double>("sqs.queue_lag_ms", "ms", "Time spent in queue");
    public static readonly Histogram<double> NotificationProcessingTime = Meter.CreateHistogram<double>("notification.processing_time_ms", "ms", "Notification duration");
    public static readonly Counter<long> NotificationFailures = Meter.CreateCounter<long>("notification.failures", "count", "Number of notification failures");
}