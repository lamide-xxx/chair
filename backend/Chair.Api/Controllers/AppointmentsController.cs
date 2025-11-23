using System.Diagnostics;
using Chair.Api.DTOs;
using Chair.Domain.Entities;
using Chair.Domain.Events;
using Chair.Domain.Messaging;
using Chair.Domain.Repositories;
using Chair.Infrastructure.Messaging;
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
    private readonly IMessagePublisher _sqsPublisher;

    public AppointmentsController(
        IAppointmentRepository appointmentRepository,
        IUserRepository userRepository,
        IServiceRepository serviceRepository,
        IStylistRepository stylistRepository,
        IMessagePublisher sqsPublisher)
        
    {
        _appointmentRepository = appointmentRepository;
        _userRepository = userRepository;
        _serviceRepository = serviceRepository;
        _stylistRepository = stylistRepository;
        _sqsPublisher = sqsPublisher;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAllAppointments()
    {
        var appointments = await _appointmentRepository.GetAllAppointmentsAsync();
        
        var appointmentDtos = appointments.Select( appointment =>
        {
            return new AppointmentDto()
            {
                Id = appointment.Id,
                User = new UserDto
                {
                    Id = appointment.User.Id,
                    FullName = appointment.User.FullName
                },
                Stylist = new StylistDto
                {
                    Id = appointment.Stylist.Id,
                    FullName = appointment.Stylist.FullName
                },
                Service = new ServiceDto
                {
                    Id = appointment.Service.Id,
                    Name = appointment.Service.Name,
                    Price = appointment.Service.Price
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

        var appointmentDto = new AppointmentDto()
        {
            Id = appointment.Id,
            User = new UserDto
            {
                Id = appointment.User.Id,
                FullName = appointment.User.FullName
            },
            Stylist = new StylistDto
            {
                Id = appointment.Stylist.Id,
                FullName = appointment.Stylist.FullName
            },
            Service = new ServiceDto
            {
                Id = appointment.Service.Id,
                Name = appointment.Service.Name,
                Price = appointment.Service.Price
            },
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            Status = appointment.Status
        };
        
        return Ok(appointmentDto);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppoinmentsByUserId(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            return BadRequest($"User {userId} does not exist.");
        }
        
        var appointments = await _appointmentRepository.GetAppointmentsByUserIdAsync(userId);
        if (!appointments.Any())
        {
            return NotFound();
        }
        
        var appointmentDtos = appointments.Select(appointment =>
        {
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
                    Id = appointment.Stylist.Id,
                    FullName = appointment.Stylist.FullName
                },
                Service = new ServiceDto
                {
                    Id = appointment.Service.Id,
                    Name = appointment.Service.Name,
                    Price = appointment.Service.Price
                },
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status
            };
        }).ToList();
        return Ok(appointmentDtos);

    }

    [HttpPost]
    public async Task<ActionResult<Appointment>> CreateAppointment([FromBody] Appointment appointment, CancellationToken cancellationToken)
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
        var traceparent = Activity.Current?.Id;
        await _sqsPublisher.PublishAsync(
            new BookingEvent{
            Type = EventType.BookingCreated.ToString(),
            AppointmentId = createdAppointment.Id,
            StylistId = createdAppointment.StylistId,
            UserId = createdAppointment.UserId,
            Timestamp = DateTime.UtcNow
            },
            cancellationToken,
            traceparent);
        
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