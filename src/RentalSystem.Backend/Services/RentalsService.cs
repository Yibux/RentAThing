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

        public async Task<bool> UpdateRentalAsync(string rentalId, string userId, UpdateRentalDto dto)
        {
            var docRef = _firestore.Collection(CollectionName).Document(rentalId);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return false;

            var rental = snapshot.ConvertTo<Rental>();

            if ((rental.BorrowerId != userId && rental.OwnerId != userId) || rental.Status != "REQUESTED")
            {
                return false;
            }

            var updates = new Dictionary<string, object>();
            if (dto.StartDate.HasValue) updates["StartDate"] = dto.StartDate.Value;
            if (dto.EndDate.HasValue) updates["EndDate"] = dto.EndDate.Value;
            if (!string.IsNullOrEmpty(dto.Status)) updates["Status"] = dto.Status;

            if (dto.StartDate.HasValue || dto.EndDate.HasValue)
            {
                var item = await _itemsService.GetItemByIdAsync(rental.ItemId);

                if (item == null) return false;

                var newStart = dto.StartDate ?? rental.StartDate;
                var newEnd = dto.EndDate ?? rental.EndDate;

                if (newEnd <= newStart)
                {
                    return false;
                }

                var days = (newEnd - newStart).Days;
                if (days < 1) days = 1;

                decimal newTotalPrice = item.PricePerDay * days;
                updates["Price"] = newTotalPrice;
            }

            if (updates.Any())
            {
                await docRef.UpdateAsync(updates);
            }

            return true;
        }

        public async Task<bool> RateOwnerAsync(string rentalId, string borrowerId, int rating)
        {
            if (rating < 1 || rating > 5) throw new ArgumentException("Rating must be 1-5");

            var rentalRef = _firestore.Collection(CollectionName).Document(rentalId);
            var rentalSnap = await rentalRef.GetSnapshotAsync();
            if (!rentalSnap.Exists) return false;

            var rental = rentalSnap.ConvertTo<Rental>();

            if (rental.BorrowerId != borrowerId || rental.IsRated) return false;

            var ownerRef = _firestore.Collection("users").Document(rental.OwnerId);

            await _firestore.RunTransactionAsync(async transaction =>
            {
                var ownerSnap = await transaction.GetSnapshotAsync(ownerRef);
                var owner = ownerSnap.ConvertTo<UserProfile>();

                double newAverage = ((owner.AverageRating * owner.RatingsCount) + rating) / (owner.RatingsCount + 1);
                int newCount = owner.RatingsCount + 1;

                transaction.Update(ownerRef, new Dictionary<string, object>
                {
                    { "AverageRating", newAverage },
                    { "RatingsCount", newCount }
                });

                transaction.Update(rentalRef, new Dictionary<string, object>
                {
                    { "IsRated", true }
                });
            });

            return true;
        }
    }
}