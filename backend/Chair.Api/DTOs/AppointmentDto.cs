using Chair.Domain.Entities;

namespace Chair.Api.DTOs;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public UserDto User { get; set; }
    public StylistDto Stylist { get; set; }
    public ServiceDto Service { get; set; }
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public Appointment.AppointmentStatus Status { get; set; }
}