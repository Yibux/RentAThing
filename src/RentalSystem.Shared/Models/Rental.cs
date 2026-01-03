using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentalSystem.Shared.Converters;

using Google.Cloud.Firestore;

namespace RentalSystem.Shared.Models
{
    [FirestoreData]
    public class Rental
    {
        [FirestoreProperty]
        public string BorrowerId { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public DateTime EndDate { get; set; }

        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public bool IsRated { get; set; } = false;

        [FirestoreProperty]
        public string ItemId { get; set; }

        [FirestoreProperty]
        public string OwnerId { get; set; }

        [FirestoreProperty]
        public List<string> Photos { get; set; }

        [FirestoreProperty(ConverterType = typeof(DecimalConverter))]
        public decimal Price { get; set; }

        [FirestoreProperty]
        public DateTime StartDate { get; set; }

        [FirestoreProperty]
        public string Status { get; set; } = "REQUESTED";
    }
}