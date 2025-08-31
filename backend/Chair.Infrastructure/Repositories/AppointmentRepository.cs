using Chair.Domain.Entities;
using Chair.Domain.Repositories;

namespace Chair.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly List<Appointment> _appointments = new();
    
    public Appointment AddAppointment(Appointment appointment)
    {
        _appointments.Add(appointment);
        return appointment;
    }

    public IEnumerable<Appointment> GetAllAppointments()
    {
        return _appointments;
    }

    public Appointment? GetAppointmentById(Guid id)
    {
        return _appointments.FirstOrDefault(a => a.Id == id);
    }

    public void UpdateAppointment(Appointment appointment)
    {
        var index = _appointments.FindIndex(a => a.Id == appointment.Id);
        if (index != -1)
        {
            _appointments[index] = appointment;
        }
    }

    public void DeleteAppointment(Guid id)
    {
        var appointment = GetAppointmentById(id);
        if (appointment != null)
        {
            _appointments.Remove(appointment);
        }
    }
}