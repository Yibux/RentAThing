using Google.Cloud.Firestore;
using RentalSystem.Shared.Converters;

namespace RentalSystem.Shared.Models
{
    [FirestoreData]
    public class Item
    {
        [FirestoreProperty]
        public string Category { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public string Currency { get; set; } = "PLN";

        [FirestoreProperty]
        public string Description { get; set; }

        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public ItemLocation Location { get; set; }

        [FirestoreProperty]
        public string OwnerId { get; set; }

        [FirestoreProperty]
        public List<string> Photos { get; set; } = new List<string>();

        [FirestoreProperty(ConverterType = typeof(DecimalConverter))]
        public decimal PricePerDay { get; set; }

        [FirestoreProperty]
        public string? RejectionReason { get; set; }

        [FirestoreProperty]
        public string Status { get; set; } = "PENDING";

        [FirestoreProperty]
        public string Title { get; set; }
    }

    [FirestoreData]
    public class ItemLocation
    {
        [FirestoreProperty]
        public string AddressLabel { get; set; }

        [FirestoreProperty]
        public string City { get; set; }

        [FirestoreProperty]
        public string Country { get; set; }

        [FirestoreProperty]
        public string HouseNumber { get; set; }

        [FirestoreProperty]
        public double Latitude { get; set; }

        [FirestoreProperty]
        public double Longitude { get; set; }

        [FirestoreProperty]
        public string PostalCode { get; set; }

        [FirestoreProperty]
        public string Street { get; set; }
    }
}