namespace Chair.Domain.Events;

public class BookingEvent
{
    public string Type { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid StylistId { get; set; }
    public Guid UserId { get; set; }
    public DateTime Timestamp { get; set; }
}