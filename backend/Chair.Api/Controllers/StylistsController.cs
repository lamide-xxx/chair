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
    public async Task<ActionResult<Stylist>> CreateStylist([FromBody] Stylist stylist)
    {
        var createdStylist = await _stylistRepository.AddStylistAsync(stylist);
        return CreatedAtAction(nameof(GetAllStylists), new { id = createdStylist.Id }, createdStylist);
    }

    [HttpGet]
    public async Task<ActionResult<Stylist>> GetAllStylists()
    {
        var stylists = await _stylistRepository.GetAllStylistsAsync();
        return Ok(stylists);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Stylist>> GetStylistsById(Guid id)
    {
        var stylist = await _stylistRepository.GetStylistByIdAsync(id);
        if (stylist == null)
        {
            return NotFound();
        }
        return Ok(stylist);
    }
    
}