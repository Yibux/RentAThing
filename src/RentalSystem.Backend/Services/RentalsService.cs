using Google.Cloud.Firestore;
using RentalSystem.Shared.DTOs;
using RentalSystem.Shared.Models;

namespace RentalSystem.Backend.Services
{
    public class RentalsService : IRentalsService
    {
        private readonly FirestoreDb _firestore;
        private readonly IItemsService _itemsService;
        private const string CollectionName = "Rentals";

        public RentalsService(FirestoreDb firestore, IItemsService itemsService)
        {
            _firestore = firestore;
            _itemsService = itemsService;
        }

        public async Task<List<Rental>> GetAllRentalsAsync()
        {
            var snapshot = await _firestore.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(d =>
            {
                var rental = d.ConvertTo<Rental>();
                rental.Id = d.Id;
                return rental;
            }).ToList();
        }

        public async Task<List<Rental>> GetUserRentalsAsync(string userId)
        {
            var query = _firestore.Collection(CollectionName)
                .WhereEqualTo("BorrowerId", userId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(d =>
            {
                var rental = d.ConvertTo<Rental>();
                rental.Id = d.Id;
                return rental;
            }).ToList();
        }

        public async Task<string> CreateRentalAsync(string borrowerId, CreateRentalDto dto)
        {
            var item = await _itemsService.GetItemByIdAsync(dto.ItemId);
            if (item == null) throw new Exception("Item does not exist.");

            var days = (dto.EndDate - dto.StartDate).Days;
            if (days < 1) days = 1;

            decimal totalPrice = item.PricePerDay * days;

            var rental = new Rental
            {
                ItemId = dto.ItemId,
                OwnerId = item.OwnerId,
                BorrowerId = borrowerId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Price = totalPrice,
                Photos = item.Photos,
                Status = "REQUESTED",
                IsRated = false,
                CreatedAt = DateTime.UtcNow
            };

            var docRef = await _firestore.Collection(CollectionName).AddAsync(rental);
            return docRef.Id;
        }
    }
}