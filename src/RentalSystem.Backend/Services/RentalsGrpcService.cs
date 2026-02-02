using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using RentalSystem.Shared.DTOs;
using RentalSystem.Shared.Models;
using RentalSystem.Shared.Protos;
using System.Security.Claims;

namespace RentalSystem.Backend.Services
{
    [Authorize]
    public class RentalsGrpcService : RentalsGrpc.RentalsGrpcBase
    {
        private readonly IRentalsService _rentalsService;

        public RentalsGrpcService(IRentalsService rentalsService)
        {
            _rentalsService = rentalsService;
        }

        public override async Task<RentalListResponse> GetAllRentals(Empty request, ServerCallContext context)
        {
            var rentals = await _rentalsService.GetAllRentalsAsync();
            return MapToRentalListResponse(rentals);
        }

        public override async Task<RentalListResponse> GetMyRentals(Empty request, ServerCallContext context)
        {
            var userId = GetUserId(context);
            if (userId == null) throw new RpcException(new Status(StatusCode.Unauthenticated, "No user ID found"));

            var rentals = await _rentalsService.GetUserRentalsAsync(userId);
            return MapToRentalListResponse(rentals);
        }

        public override async Task<CreateRentalResponse> CreateRental(CreateRentalRequest request, ServerCallContext context)
        {
            var userId = GetUserId(context);
            if (userId == null) throw new RpcException(new Status(StatusCode.Unauthenticated, "No user ID found"));

            var dto = new CreateRentalDto
            {
                ItemId = request.ItemId,
                StartDate = request.StartDate.ToDateTime(),
                EndDate = request.EndDate.ToDateTime()
            };

            try
            {
                var id = await _rentalsService.CreateRentalAsync(userId, dto);
                return new CreateRentalResponse { Id = id };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
            }
        }

        public override async Task<Empty> UpdateRental(UpdateRentalRequest request, ServerCallContext context)
        {
            var userId = GetUserId(context);
            if (userId == null) throw new RpcException(new Status(StatusCode.Unauthenticated, "No user ID found"));

            var dto = new UpdateRentalDto();

            if (request.StartDate != null) dto.StartDate = request.StartDate.ToDateTime();
            if (request.EndDate != null) dto.EndDate = request.EndDate.ToDateTime();
            if (!string.IsNullOrEmpty(request.Status)) dto.Status = request.Status;

            var success = await _rentalsService.UpdateRentalAsync(request.Id, userId, dto);

            if (!success)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Cannot update rental (wrong user or status)."));
            }

            return new Empty();
        }

        public override async Task<Empty> RateOwner(RateOwnerRequest request, ServerCallContext context)
        {
            var userId = GetUserId(context);
            if (userId == null) throw new RpcException(new Status(StatusCode.Unauthenticated, "No user ID found"));

            try
            {
                var success = await _rentalsService.RateOwnerAsync(request.RentalId, userId, request.Rating);
                if (!success)
                {
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, "Could not rate owner (already rated or not yours)."));
                }
            }
            catch (ArgumentException ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
            }

            return new Empty();
        }

        private string? GetUserId(ServerCallContext context)
        {
            return context.GetHttpContext().User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private RentalListResponse MapToRentalListResponse(List<Rental> rentals)
        {
            var response = new RentalListResponse();
            response.Rentals.AddRange(rentals.Select(r => new RentalMessage
            {
                Id = r.Id,
                BorrowerId = r.BorrowerId,
                OwnerId = r.OwnerId,
                ItemId = r.ItemId,
                StartDate = Timestamp.FromDateTime(DateTime.SpecifyKind(r.StartDate, DateTimeKind.Utc)),
                EndDate = Timestamp.FromDateTime(DateTime.SpecifyKind(r.EndDate, DateTimeKind.Utc)),
                CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(r.CreatedAt, DateTimeKind.Utc)),
                Price = (double)r.Price,
                Status = r.Status,
                IsRated = r.IsRated
            }));

            foreach (var r in rentals)
            {
                var msg = response.Rentals.FirstOrDefault(x => x.Id == r.Id);
                if (msg != null && r.Photos != null)
                {
                    msg.Photos.AddRange(r.Photos);
                }
            }

            return response;
        }
    }
}