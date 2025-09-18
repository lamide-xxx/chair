using Chair.Api.DTOs;
using Chair.Domain.Entities;
using Chair.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Chair.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IStylistRepository _stylistRepository;
    private readonly IUserRepository _userRepository;
    
    public AppointmentsController(
        IAppointmentRepository appointmentRepository,
        IUserRepository userRepository,
        IServiceRepository serviceRepository,
        IStylistRepository stylistRepository)
        
    {
        _appointmentRepository = appointmentRepository;
        _userRepository = userRepository;
        _serviceRepository = serviceRepository;
        _stylistRepository = stylistRepository;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAllAppointments()
    {
        var appointments = await _appointmentRepository.GetAllAppointmentsAsync();
        
        var appointmentDtos = appointments.Select(async appointment =>
        {
            var user = await _userRepository.GetUserByIdAsync(appointment.UserId);
            var stylist = await _stylistRepository.GetStylistByIdAsync(appointment.StylistId);
            var service = await _serviceRepository.GetServiceByIdAsync(appointment.ServiceId);
            
            return new AppointmentDto()
            {
                Id = appointment.Id,
                User = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName
                },
                Stylist = new StylistDto
                {
                    Id = stylist.Id,
                    FullName = stylist.FullName
                },
                Service = new ServiceDto
                {
                    Id = service.Id,
                    Name = service.Name,
                    Price = service.Price
                },
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status
            };
        }).ToList();
        return Ok(appointmentDtos);
    }

    [HttpGet("{id}")]
    public async Task <ActionResult<Appointment>> GetAppointmentById(Guid id)
    {
        var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
        if (appointment == null)
        {
            return NotFound();
        }
        
        var user = await _userRepository.GetUserByIdAsync(appointment.UserId);
        var stylist = await _stylistRepository.GetStylistByIdAsync(appointment.StylistId);
        var service = await _serviceRepository.GetServiceByIdAsync(appointment.ServiceId);

        var appointmentDto = new AppointmentDto()
        {
            Id = appointment.Id,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName
            },
            Stylist = new StylistDto
            {
                Id = stylist.Id,
                FullName = stylist.FullName
            },
            Service = new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Price = service.Price
            },
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            Status = appointment.Status
        };
        
        return Ok(appointmentDto);
    }

    [HttpPost]
    public async Task<ActionResult<Appointment>> CreateAppointment([FromBody] Appointment appointment)
    {
        var user = await _userRepository.GetUserByIdAsync(appointment.UserId);
        if (user == null)
        {
            return BadRequest($"User {appointment.UserId} does not exist.");
        }
        
        var service = await _serviceRepository.GetServiceByIdAsync(appointment.ServiceId);
        if (service == null)
        {
            return BadRequest($"Service {appointment.ServiceId} does not exist.");
        }
        
        var stylist = await _stylistRepository.GetStylistByIdAsync(appointment.StylistId);
        if (stylist == null)
        {
            return BadRequest($"Stylist {appointment.StylistId} does not exist.");
        }
        
        var createdAppointment = await _appointmentRepository.AddAppointmentAsync(appointment);
        return CreatedAtAction(nameof(GetAppointmentById), new { id = createdAppointment.Id }, createdAppointment);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateAppointment(Guid id, [FromBody] Appointment updatedAppointment)
    {
        var existingAppointment = _appointmentRepository.GetAppointmentByIdAsync(id);
        if (existingAppointment == null)
        {
            return NotFound();
        }

        if (updatedAppointment.Id != id)
        {
            return BadRequest("ID in the URL must match ID in the body.");
        }
        _appointmentRepository.UpdateAppointmentAsync(updatedAppointment);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteAppointment(Guid id)
    {
        var existingAppointment = _appointmentRepository.GetAppointmentByIdAsync(id);
        if (existingAppointment == null)
        {
            return NotFound();
        }
        _appointmentRepository.DeleteAppointmentAsync(id);
        return NoContent();
    }
}