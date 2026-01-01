using Microsoft.AspNetCore.Mvc;
using RentalSystem.Backend.Services;
using RentalSystem.Shared.DTOs;
using RentalSystem.Shared.Models;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RentalSystem.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (string.IsNullOrEmpty(request.Uid) || string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Uid and Email are required.");
            }

            var createdUser = await _usersService.AddUserAsync(request);

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _usersService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _usersService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            var success = await _usersService.UpdateUserAsync(id, request);
            if (!success) return NotFound($"User with id {id} not found.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var success = await _usersService.DeleteUserAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}