using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalSystem.Backend.Services;
using RentalSystem.Shared.DTOs;
using System.Security.Claims;

namespace RentalSystem.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalsService _rentalsService;

        public RentalsController(IRentalsService rentalsService)
        {
            _rentalsService = rentalsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rentals = await _rentalsService.GetAllRentalsAsync();
            return Ok(rentals);
        }

        [HttpGet("my-rentals")]
        public async Task<IActionResult> GetMyRentals()
        {
            var borrowerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(borrowerId))
            {
                return Unauthorized("User not found.");
            }
            var rentals = await _rentalsService.GetUserRentalsAsync(borrowerId);
            return Ok(rentals);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRentalDto dto)
        {
            var borrowerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                var id = await _rentalsService.CreateRentalAsync(borrowerId, dto);
                return Ok(new { id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateRental(string id, [FromBody] UpdateRentalDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var success = await _rentalsService.UpdateRentalAsync(id, userId, dto);

            if (!success) return BadRequest("Cannot update rental (wrong user, status or id).");

            return NoContent();
        }

        [HttpPost("{id}/rate")]
        public async Task<IActionResult> RateOwner(string id, [FromBody] RateUserDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var success = await _rentalsService.RateOwnerAsync(id, userId, dto.Rating);
                if (!success) return BadRequest("Cannot rate this rental (already rated or not yours).");
                return Ok("User rated successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}