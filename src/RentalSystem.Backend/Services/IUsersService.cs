using Google.Cloud.Firestore;
using RentalSystem.Shared.AppConstants;
using RentalSystem.Shared.DTOs;
using RentalSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalSystem.Backend.Services
{
    public interface IUsersService
    {
        Task<UserProfile> AddUserAsync(string uid, CreateUserRequest request);

        Task<UserProfile?> GetUserByIdAsync(string uid);
        Task<List<UserProfile>> GetAllUsersAsync();
        Task<bool> UpdateUserAsync(string uid, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(string uid);
    }

    public class UsersService : IUsersService
    {
        private readonly FirestoreDb _db;
        private const string CollectionName = "users";

        public UsersService(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<UserProfile> AddUserAsync(string uid, CreateUserRequest request)
        {
            var newUser = new UserProfile
            {
                Email = request.Email,
                Id = uid,
                Name = request.Name,
                PhoneNumber = request.PhoneNumber ?? "",
                Role = string.IsNullOrEmpty(request.Role) ? "USER" : request.Role,
                Surname = request.Surname
            };

            var docRef = _db.Collection(CollectionName).Document(newUser.Id);
            await docRef.SetAsync(newUser);

            return newUser;
        }

        public async Task<UserProfile?> GetUserByIdAsync(string uid)
        {
            var docRef = _db.Collection(CollectionName).Document(uid);
            var snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<UserProfile>();
            }

            return null;
        }

        public async Task<List<UserProfile>> GetAllUsersAsync()
        {
            var query = _db.Collection(CollectionName);
            var querySnapshot = await query.GetSnapshotAsync();

            return querySnapshot.Documents
                .Select(doc => doc.ConvertTo<UserProfile>())
                .ToList();
        }

        public async Task<bool> UpdateUserAsync(string uid, UpdateUserRequest request)
        {
            var docRef = _db.Collection(CollectionName).Document(uid);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists) return false;

            var updates = new Dictionary<string, object>();

            if (request.IsBanned.HasValue)
                updates["IsBanned"] = request.IsBanned.Value;

            if (!string.IsNullOrEmpty(request.Name))
                updates["Name"] = request.Name;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                updates["PhoneNumber"] = request.PhoneNumber;

            if (!string.IsNullOrEmpty(request.Role))
                updates["Role"] = request.Role;

            if (!string.IsNullOrEmpty(request.Surname))
                updates["Surname"] = request.Surname;

            if (updates.Count > 0)
            {
                await docRef.UpdateAsync(updates);
            }

            return true;
        }

        public async Task<bool> DeleteUserAsync(string uid)
        {
            var docRef = _db.Collection(CollectionName).Document(uid);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists) return false;

            await docRef.DeleteAsync();
            return true;
        }
    }
}