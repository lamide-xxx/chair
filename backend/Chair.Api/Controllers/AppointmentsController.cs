using Chair.Domain.Entities;
using Chair.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Chair.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentRepository _appointmentRepository;
    
    public AppointmentsController(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }
    
    [HttpGet]
    public IActionResult GetAllAppointments()
    {
        var appointments = _appointmentRepository.GetAllAppointments();
        return Ok(appointments);
    }

    [HttpGet("{id}")]
    public IActionResult GetAppointmentById(Guid id)
    {
        var appointment = _appointmentRepository.GetAppointmentById(id);
        if (appointment == null)
        {
            return NotFound();
        }
        return Ok(appointment);
    }

    [HttpPost]
    public IActionResult CreateAppointment([FromBody] Appointment appointment)
    {
        var _createdAppointment = _appointmentRepository.AddAppointment(appointment);
        return CreatedAtAction(nameof(GetAppointmentById), new { id = _createdAppointment.Id }, _createdAppointment);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateAppointment(Guid id, [FromBody] Appointment updatedAppointment)
    {
        var existingAppointment = _appointmentRepository.GetAppointmentById(id);
        if (existingAppointment == null)
        {
            return NotFound();
        }

        if (updatedAppointment.Id != id)
        {
            return BadRequest("ID in the URL must match ID in the body.");
        }
        _appointmentRepository.UpdateAppointment(updatedAppointment);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteAppointment(Guid id)
    {
        var existingAppointment = _appointmentRepository.GetAppointmentById(id);
        if (existingAppointment == null)
        {
            return NotFound();
        }
        _appointmentRepository.DeleteAppointment(id);
        return NoContent();
    }
}