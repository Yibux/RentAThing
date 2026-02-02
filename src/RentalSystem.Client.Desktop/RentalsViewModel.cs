using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;
using RentalSystem.Shared.DTOs;
using RentalSystem.Shared.Models;

namespace RentalSystem.Client.Desktop
{
    public class RentalsViewModel
    {
        private readonly HttpClient _httpClient;
        public ObservableCollection<Rental> Rentals { get; set; } = new ObservableCollection<Rental>();

        public ICommand CancelRentalCommand { get; }

        public RentalsViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            CancelRentalCommand = new RelayCommand<Rental>(async (r) => await CancelRentalAsync(r));
        }

        public async Task LoadRentalsAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<Rental>>("api/rentals");
                if (result != null)
                {
                    Rentals.Clear();
                    foreach (var rental in result) Rentals.Add(rental);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async Task CancelRentalAsync(Rental rental)
        {
            if (rental == null || rental.Status == "CANCELLED") return;

            var confirm = MessageBox.Show("Do you want to cancel rental?", "Approve",
                                          MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var updateDto = new UpdateRentalDto { Status = "CANCELLED" };

                var response = await _httpClient.PatchAsJsonAsync($"api/rentals/{rental.Id}", updateDto);

                if (response.IsSuccessStatusCode)
                {
                    rental.Status = "CANCELLED";
                    MessageBox.Show("Rental has been canceled.");
                }
                else
                {
                    MessageBox.Show("Unable to decline rental.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"API Error: {ex.Message}");
            }
        }
    }
}