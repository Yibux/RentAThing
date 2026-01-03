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
    }
}