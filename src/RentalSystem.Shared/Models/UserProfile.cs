using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Name { get; set; }

        [FirestoreProperty]
        public string Surname{ get; set; }

        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public bool IsBanned { get; set; } = false;

        [FirestoreProperty]
        public string PhoneNumber { get; set; }

        [FirestoreProperty]
        public string Role { get; set; }
    }
}