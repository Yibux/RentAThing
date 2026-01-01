using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalSystem.Backend.Services;
using RentalSystem.Shared.DTOs;
using RentalSystem.Shared.Models;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RentalSystem.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            // BEZPIECZEŃSTWO: Pobieramy ID użytkownika z Tokena (z nagłówka), a nie z JSONa, który ktoś mógł podrobić.
            var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdFromToken))
                return Unauthorized("Brak ID w tokenie.");

            request.Uid = userIdFromToken;

            var createdUser = await _usersService.AddUserAsync(request);

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var myId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (myId == null) return Unauthorized();

            var user = await _usersService.GetUserByIdAsync(myId);
            if (user == null) return NotFound("Profil nie istnieje.");

            return Ok(user);
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