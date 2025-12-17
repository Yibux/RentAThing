using Google.Cloud.Firestore;
using RentalSystem.Shared.DTOs;
using RentalSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalSystem.Backend.Services
{
    public interface IUsersService
    {
        Task<UserProfile> AddUserAsync(CreateUserRequest request);
    }

    public class UsersService : IUsersService
    {
        private readonly FirestoreDb _db;

        public UsersService(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<UserProfile> AddUserAsync(CreateUserRequest request)
        {
            var newUser = new UserProfile
            {
                Id = request.Uid,
                Email = request.Email,
                DisplayName = request.DisplayName,
                AvatarUrl = request.AvatarUrl ?? "",
                PhoneNumber = request.PhoneNumber ?? "",
                Role = "USER",
                IsBanned = false,
                CreatedAt = DateTime.UtcNow
            };

            var docRef = _db.Collection("users").Document(newUser.Id);
            await docRef.SetAsync(newUser);

            return newUser;
        }
    }
}