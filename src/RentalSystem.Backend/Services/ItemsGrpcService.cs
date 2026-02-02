using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using RentalSystem.Shared.DTOs;
using RentalSystem.Shared.Protos;

namespace RentalSystem.Backend.Services
{
    [Authorize]
    public class ItemsGrpcService : ItemsGrpc.ItemsGrpcBase
    {
        private readonly IItemsService _itemsService;

        public ItemsGrpcService(IItemsService itemsService)
        {
            _itemsService = itemsService;
        }

        public override async Task<ItemsListResponse> GetAllItems(EmptyRequest request, ServerCallContext context)
        {
            var items = await _itemsService.GetAllItemsAsync();
            var response = new ItemsListResponse();

            foreach (var item in items)
            {
                var grpcModel = new ItemGrpcModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description ?? "",
                    Category = item.Category,
                    PricePerDay = (double)item.PricePerDay,
                    Currency = item.Currency,
                    Status = item.Status,
                    OwnerId = item.OwnerId,
                    RejectionReason = item.RejectionReason ?? "",

                    Location = new GrpcLocation
                    {
                        City = item.Location?.City ?? "",
                        AddressLabel = item.Location?.AddressLabel ?? "",
                        Country = item.Location?.Country ?? "",
                        Street = item.Location?.Street ?? "",
                        HouseNumber = item.Location?.HouseNumber ?? "",
                        PostalCode = item.Location?.PostalCode ?? "",

                        Latitude = item.Location?.Latitude ?? 0.0,
                        Longitude = item.Location?.Longitude ?? 0.0
                    }
                };

                if (item.Photos != null)
                {
                    grpcModel.Photos.AddRange(item.Photos);
                }

                response.Items.Add(grpcModel);
            }

            return response;
        }

        public override async Task<ActionResponse> ModerateItem(ModerateItemRequest request, ServerCallContext context)
        {
            var dto = new ModerateItemDto
            {
                Status = request.Status,
                RejectionReason = string.IsNullOrEmpty(request.RejectionReason) ? null : request.RejectionReason
            };

            var success = await _itemsService.ModerateItemAsync(request.ItemId, dto);

            return new ActionResponse
            {
                Success = success,
                Message = success ? "Zaktualizowano status." : "Nie znaleziono przedmiotu."
            };
        }
    }
}