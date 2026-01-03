using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using RentalSystem.Backend.Services;
using RentalSystem.Shared.DTOs;
using RentalSystem.Shared.Protos;
using System.Security.Claims;

namespace RentalSystem.Backend.Services
{
    [Authorize]
    public class AuthGrpcService : AuthService.AuthServiceBase
    {
        private readonly IUsersService _usersService;

        public AuthGrpcService(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public override async Task<UserProfileResponse> GetMyProfile(EmptyRequest request, ServerCallContext context)
        {
            var uid = context.GetHttpContext().User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(uid))
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Brak UID"));

            var user = await _usersService.GetUserByIdAsync(uid);
            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

            return new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                Name = user.Name ?? "",
                Surname = user.Surname ?? "",
                IsBanned = user.IsBanned
            };
        }

        public override async Task<UsersListResponse> GetAllUsers(EmptyRequest request, ServerCallContext context)
        {
            var users = await _usersService.GetAllUsersAsync();
            var response = new UsersListResponse();
            response.Users.AddRange(users.Select(u => new UserProfileResponse
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name ?? "",
                Surname = u.Surname ?? "",
                Role = u.Role,
                IsBanned = u.IsBanned
            }));
            return response;
        }

        public override async Task<ActionResponse> SetUserBanStatus(BanUserRequest request, ServerCallContext context)
        {
            var success = await _usersService.UpdateUserAsync(request.UserId, new UpdateUserRequest { IsBanned = request.IsBanned });
            return new ActionResponse { Success = success };
        }
    }
}