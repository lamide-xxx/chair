namespace Chair.Domain.Entities;

public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid UserId { get; set; }
    public Guid StylistId { get; set; }
    public Guid ServiceId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    
    // Navigation properties — EF Core uses these for Include()
    public User? User { get; set; }
    public Stylist? Stylist { get; set; }
    public Service? Service { get; set; }

    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }
}