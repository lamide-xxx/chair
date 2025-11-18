using System.Diagnostics;

namespace Chair.NotificationsWorker;

public static class Telemetry
{
    public static readonly ActivitySource ActivitySource = new ActivitySource("Chair.NotificationsWorker");
}