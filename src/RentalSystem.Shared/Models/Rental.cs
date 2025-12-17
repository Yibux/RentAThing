using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Cloud.Firestore;

namespace RentalSystem.Shared.Models
{
    [FirestoreData]
    public class Rental
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string ItemId { get; set; }

        [FirestoreProperty]
        public string ItemTitleSnapshot { get; set; }

        [FirestoreProperty]
        public string ItemPhotoSnapshot { get; set; }

        [FirestoreProperty]
        public decimal PriceAtBooking { get; set; }

        [FirestoreProperty]
        public string BorrowerId { get; set; }

        [FirestoreProperty]
        public string OwnerId { get; set; }

        [FirestoreProperty]
        public DateTime StartDate { get; set; }

        [FirestoreProperty]
        public DateTime EndDate { get; set; }

        [FirestoreProperty]
        public decimal TotalPrice { get; set; }

        [FirestoreProperty]
        public string Status { get; set; } = "REQUESTED";

        [FirestoreProperty]
        public bool IsRated { get; set; } = false;

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}