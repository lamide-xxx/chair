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
    public IActionResult CreateService([FromBody] Service service)
    {
        var createdService = _serviceRepository.AddService(service);
        return CreatedAtAction(nameof(GetAllServices), new { id = createdService.Id }, createdService);
    }

    [HttpGet]
    public IActionResult GetAllServices()
    {
        var _services = _serviceRepository.GetAllServices();
        return Ok(_services);
    }

    [HttpGet("{id}")]
    public IActionResult GetServiceById(Guid id)
    {
        var service = _serviceRepository.GetServiceById(id);
        if (service == null)
        {
            return NotFound();
        }

        return Ok(service);
    }
}