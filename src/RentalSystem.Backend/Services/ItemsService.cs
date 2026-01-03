using Google.Cloud.Firestore;
using RentalSystem.Shared.DTOs;
using RentalSystem.Shared.Models;

namespace RentalSystem.Backend.Services
{
    public class ItemsService : IItemsService
    {
        private readonly FirestoreDb _firestore;
        private const string CollectionName = "Items";

        public ItemsService(FirestoreDb firestore)
        {
            _firestore = firestore;
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            var snapshot = await _firestore.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(d =>
            {
                var item = d.ConvertTo<Item>();
                item.Id = d.Id;
                return item;
            }).ToList();
        }

        public async Task<Item?> GetItemByIdAsync(string id)
        {
            var doc = await _firestore.Collection(CollectionName).Document(id).GetSnapshotAsync();
            if (!doc.Exists) return null;

            var item = doc.ConvertTo<Item>();
            item.Id = doc.Id;
            return item;
        }

        public async Task<Item> AddItemAsync(string ownerId, CreateItemDto dto)
        {
            var item = new Item
            {
                OwnerId = ownerId,
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                PricePerDay = dto.PricePerDay,
                Currency = dto.Currency,
                Photos = dto.Photos,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow,

                Location = new ItemLocation
                {
                    City = dto.Location.City,
                    Street = dto.Location.Street,
                    HouseNumber = dto.Location.HouseNumber,
                    PostalCode = dto.Location.PostalCode,
                    Country = dto.Location.Country,
                    AddressLabel = dto.Location.AddressLabel,
                    Latitude = dto.Location.Latitude,
                    Longitude = dto.Location.Longitude
                }
            };

            var docRef = await _firestore.Collection(CollectionName).AddAsync(item);
            item.Id = docRef.Id;

            return item; 
        }

        public async Task<bool> ModerateItemAsync(string id, ModerateItemDto dto)
        {
            var docRef = _firestore.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists) return false;

            var updates = new Dictionary<string, object>
            {
                { "Status", dto.Status }
            };

            if (dto.Status == "REJECTED" && !string.IsNullOrEmpty(dto.RejectionReason))
            {
                updates.Add("RejectionReason", dto.RejectionReason);
            }
            else if (dto.Status == "APPROVED")
            {
                updates.Add("RejectionReason", null);
            }

            await docRef.UpdateAsync(updates);
            return true;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var docRef = _firestore.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync();
            return true;
        }
    }
}