using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Cloud.Firestore;

namespace RentalSystem.Shared.Models
{
    [FirestoreData]
    public class Item
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Title { get; set; }

        [FirestoreProperty]
        public string Description { get; set; }

        [FirestoreProperty]
        public string Category { get; set; }

        [FirestoreProperty]
        public decimal PricePerDay { get; set; }

        [FirestoreProperty]
        public string Currency { get; set; } = "PLN";

        [FirestoreProperty]
        public List<string> Photos { get; set; } = new List<string>();

        [FirestoreProperty]
        public string OwnerId { get; set; }

        [FirestoreProperty]
        public string OwnerName { get; set; }

        [FirestoreProperty]
        public string OwnerAvatarUrl { get; set; }

        [FirestoreProperty]
        public ItemLocation Location { get; set; }

        [FirestoreProperty]
        public string Status { get; set; } = "PENDING";

        [FirestoreProperty]
        public string? RejectionReason { get; set; }

        [FirestoreProperty]
        public double AverageRating { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [FirestoreData]
    public class ItemLocation
    {
        [FirestoreProperty]
        public double Latitude { get; set; }

        [FirestoreProperty]
        public double Longitude { get; set; }

        [FirestoreProperty]
        public string AddressLabel { get; set; }
    }
}