using Chair.Domain.Entities;

namespace Chair.Domain.Repositories;

public interface IAppointmentRepository
{
    Appointment AddAppointment(Appointment appointment);
    IEnumerable<Appointment> GetAllAppointments();
    Appointment? GetAppointmentById(Guid id);
    void UpdateAppointment(Appointment appointment);
    void DeleteAppointment(Guid id);
}