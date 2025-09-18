using Chair.Domain.Entities;
using Chair.Domain.Repositories;
using Chair.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chair.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _context;
    
    public AppointmentRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Appointment> AddAppointmentAsync(Appointment appointment)
    {
        appointment.Id = Guid.NewGuid();
        appointment.StartTime = appointment.StartTime.ToUniversalTime();
        appointment.EndTime = appointment.EndTime.ToUniversalTime();
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
    {
        return await _context.Appointments
            .Include(a => a.User)
            .Include(a => a.Stylist)
            .Include(a => a.Service)
            .ToListAsync();
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(Guid id)
    {
        return await _context.Appointments
            .Include(a => a.User)
            .Include(a => a.Stylist)
            .Include(a => a.Service)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Appointment?> UpdateAppointmentAsync(Appointment appointment)
    {
        var existingAppointment = await GetAppointmentByIdAsync(appointment.Id);
        if (existingAppointment == null)
        {
            return null;
        }
        
        existingAppointment.UserId = appointment.UserId;
        existingAppointment.StylistId = appointment.StylistId;
        existingAppointment.ServiceId = appointment.ServiceId;
        existingAppointment.StartTime = appointment.StartTime;
        existingAppointment.EndTime = appointment.EndTime;
        existingAppointment.Status = appointment.Status;
        _context.Appointments.Update(existingAppointment);
        await _context.SaveChangesAsync();
        return await GetAppointmentByIdAsync(appointment.Id);
    }

    public async Task<bool> DeleteAppointmentAsync(Guid id)
    {
        var appointment = await GetAppointmentByIdAsync(id);
        if (appointment == null)
        {
            return false;
        }
        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return true;
    }
}