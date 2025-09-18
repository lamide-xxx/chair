using Chair.Domain.Entities;

namespace Chair.Domain.Repositories;

public interface IAppointmentRepository
{
    Task<Appointment> AddAppointmentAsync(Appointment appointment);
    Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
    Task<Appointment?> GetAppointmentByIdAsync(Guid id);
    Task<Appointment?> UpdateAppointmentAsync(Appointment appointment);
    Task<bool> DeleteAppointmentAsync(Guid id);
}