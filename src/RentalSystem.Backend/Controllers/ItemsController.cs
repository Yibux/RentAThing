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
    public class ItemsController : ControllerBase
    {
        private readonly IItemsService _itemsService;

        public ItemsController(IItemsService itemsService)
        {
            _itemsService = itemsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _itemsService.GetAllItemsAsync();
            return Ok(items);
        }

        [HttpGet("my-items")]
        public async Task<IActionResult> GetMyItems()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }

            var items = await _itemsService.GetAllUserItemsAsync(userId);

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _itemsService.GetItemByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateItemDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var item = await _itemsService.AddItemAsync(userId, dto);

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPatch("{id}/moderate")]
        public async Task<IActionResult> Moderate(string id, [FromBody] ModerateItemDto dto)
        {
            var success = await _itemsService.ModerateItemAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _itemsService.DeleteItemAsync(id);
            return NoContent();
        }
    }
}