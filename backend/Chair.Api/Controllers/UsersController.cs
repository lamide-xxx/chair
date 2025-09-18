using Chair.Domain.Entities;
using Chair.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Chair.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    
    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] User user)
    {
        var createdUser = await _userRepository.AddUserAsync(user);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    [HttpGet("{id}")]
    public async Task <ActionResult<User>> GetUserById(Guid id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}