using Chair.Domain.Entities;
using Chair.Domain.Repositories;
using Chair.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Chair.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StylistsController : ControllerBase
{
    private readonly IStylistRepository _stylistRepository;
    
    public StylistsController(IStylistRepository stylistRepository)
    {
        _stylistRepository = stylistRepository;
    }
    
    [HttpPost]
    public IActionResult CreateStylist([FromBody] Stylist stylist)
    {
        var createdStylist = _stylistRepository.AddStylist(stylist);
        return CreatedAtAction(nameof(GetAllStylists), new { id = createdStylist.Id }, createdStylist);
    }

    [HttpGet]
    public IActionResult GetAllStylists()
    {
        var stylists = _stylistRepository.GetAllStylists();
        return Ok(stylists);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetStylistsById(Guid id)
    {
        var stylist = _stylistRepository.GetStylistById(id);
        if (stylist == null)
        {
            return NotFound();
        }
        return Ok(stylist);
    }
    
}