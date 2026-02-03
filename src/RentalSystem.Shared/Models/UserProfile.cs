using Google.Cloud.Firestore;

namespace RentalSystem.Shared.Models
{
    [FirestoreData]
    public class UserProfile
    {
        [FirestoreProperty]
        public double AverageRating { get; set; } = 0.0;

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Surname{ get; set; } = string.Empty;

        [FirestoreProperty]
        public string Email { get; set; } = string.Empty;

        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty]
        public bool IsBanned { get; set; } = false;

        [FirestoreProperty]
        public string PhoneNumber { get; set; } = string.Empty;

        [FirestoreProperty]
        public int RatingsCount { get; set; } = 0;

        [FirestoreProperty]
        public string Role { get; set; } = string.Empty;
    }
}