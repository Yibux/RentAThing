using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Grpc.Core;
using Grpc.Net.Client;
using RentalSystem.Shared.AppConstants;
using RentalSystem.Shared.Models;
using RentalSystem.Shared.Protos;

namespace RentalSystem.Client.Desktop
{
    public class RentalsViewModel : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly RentalsGrpc.RentalsGrpcClient _client;
        private readonly string _token;

        public ObservableCollection<Rental> Rentals { get; set; } = new ObservableCollection<Rental>();

        public ICommand CancelRentalCommand { get; }

        public RentalsViewModel(string token)
        {
            _token = token;

            _channel = GrpcChannel.ForAddress(AppConstants.BACKEND_GRPC_URL, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure
            });

            _client = new RentalsGrpc.RentalsGrpcClient(_channel);

            CancelRentalCommand = new RelayCommand<Rental>(async (r) => await CancelRentalAsync(r));
        }

        private Metadata AuthHeaders => new Metadata
        {
            { "Authorization", $"Bearer {_token}" }
        };

        public async Task LoadRentalsAsync()
        {
            try
            {
                // Przekazujemy AuthHeaders jako drugi parametr
                var response = await _client.GetAllRentalsAsync(new Google.Protobuf.WellKnownTypes.Empty(), AuthHeaders);

                Rentals.Clear();
                foreach (var msg in response.Rentals)
                {
                    Rentals.Add(MapToModel(msg));
                }
            }
            catch (RpcException ex)
            {
                MessageBox.Show($"Błąd gRPC: {ex.Status.Detail} ({ex.StatusCode})");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}");
            }
        }

        private async Task CancelRentalAsync(Rental rental)
        {
            if (rental == null || rental.Status == "CANCELLED") return;

            var confirm = MessageBox.Show("Are you sure to cancel the rental?", "Approval",
                                          MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var request = new UpdateRentalRequest
                {
                    Id = rental.Id,
                    Status = "CANCELLED"
                };

                await _client.UpdateRentalAsync(request, AuthHeaders);

                rental.Status = "CANCELLED";
                MessageBox.Show("Rental has been canceled.");
            }
            catch (RpcException ex)
            {
                MessageBox.Show($"Nie udało się anulować: {ex.Status.Detail}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}");
            }
        }

        private Rental MapToModel(RentalMessage msg)
        {
            return new Rental
            {
                Id = msg.Id,
                BorrowerId = msg.BorrowerId,
                OwnerId = msg.OwnerId,
                ItemId = msg.ItemId,
                Status = msg.Status,
                IsRated = msg.IsRated,
                StartDate = msg.StartDate.ToDateTime(),
                EndDate = msg.EndDate.ToDateTime(),
                CreatedAt = msg.CreatedAt.ToDateTime(),
                Price = (decimal)msg.Price
            };
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}