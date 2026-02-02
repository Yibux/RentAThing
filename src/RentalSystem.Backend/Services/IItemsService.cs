using RentalSystem.Shared.DTOs;
using RentalSystem.Shared.Models;

namespace RentalSystem.Backend.Services
{
    public interface IItemsService
    {
        Task<List<Item>> GetAllItemsAsync();
        Task<List<Item>> GetAllUserItemsAsync(string id);
        Task<Item?> GetItemByIdAsync(string id);
        Task<Item> AddItemAsync(string ownerId, CreateItemDto dto);
        Task<bool> ModerateItemAsync(string id, ModerateItemDto dto);
        Task<bool> DeleteItemAsync(string id);
    }

    public interface IRentalsService
    {
        Task<List<Rental>> GetAllRentalsAsync();
        Task<string> CreateRentalAsync(string borrowerId, CreateRentalDto dto);
    }
}