using Chair.Domain.Entities;
using Chair.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Chair.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceRepository _serviceRepository;

    public ServicesController(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    [HttpPost]
    public async Task<ActionResult<Service>> CreateService([FromBody] Service service)
    {
        var createdService = await _serviceRepository.AddServiceAsync(service);
        return CreatedAtAction(nameof(GetAllServices), new { id = createdService.Id }, createdService);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Service>>> GetAllServices()
    {
        var services = await _serviceRepository.GetAllServicesAsync();
        return Ok(services);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Service>> GetServiceById(Guid id)
    {
        var service = await _serviceRepository.GetServiceByIdAsync(id);
        if (service == null)
        {
            return NotFound();
        }

        return Ok(service);
    }
}