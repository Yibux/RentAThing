using Grpc.Core;
using Grpc.Net.Client;
using RentalSystem.Shared.AppConstants;
using RentalSystem.Shared.Models;
using RentalSystem.Shared.Protos;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace RentalSystem.Client.Desktop
{
    public partial class ListingsPage : Page, INotifyPropertyChanged
    {
        private readonly string _token;
        private readonly GrpcChannel _channel;
        private readonly ItemsGrpc.ItemsGrpcClient _client;

        private ListingViewModel _selectedListing;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ListingViewModel> Listings { get; set; } = new ObservableCollection<ListingViewModel>();

        public ListingViewModel SelectedListing
        {
            get => _selectedListing;
            set
            {
                _selectedListing = value;
                OnPropertyChanged();
                DetailsVisibility = _selectedListing != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _detailsVisibility = Visibility.Collapsed;
        public Visibility DetailsVisibility
        {
            get => _detailsVisibility;
            set { _detailsVisibility = value; OnPropertyChanged(); }
        }

        public ListingsPage(string token)
        {
            InitializeComponent();
            _token = token;

            _channel = GrpcChannel.ForAddress(AppConstants.BACKEND_GRPC_URL, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure
            });
            _client = new ItemsGrpc.ItemsGrpcClient(_channel);

            this.DataContext = this;

            LoadListings();
        }

        private Metadata GetHeaders()
        {
            return new Metadata { { "Authorization", $"Bearer {_token}" } };
        }

        private async Task LoadListings()
        {
            try
            {
                var response = await _client.GetAllItemsAsync(new EmptyRequest(), GetHeaders());

                Listings.Clear();
                foreach (var protoItem in response.Items)
                {
                    var domainItem = MapToDomain(protoItem);
                    Listings.Add(new ListingViewModel(domainItem));
                }
            }
            catch (RpcException ex)
            {
                MessageBox.Show($"Błąd gRPC: {ex.Status.Detail}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}");
            }
        }

        private async void SendModerationDecision(string status, string reason)
        {
            if (SelectedListing == null) return;

            try
            {
                var request = new ModerateItemRequest
                {
                    ItemId = SelectedListing.Id,
                    Status = status,
                    RejectionReason = reason ?? ""
                };

                var response = await _client.ModerateItemAsync(request, GetHeaders());

                if (response.Success)
                {
                    MessageBox.Show("Status zaktualizowany!");
                    await LoadListings();

                    SelectedListing = null;
                    txtReason.Text = "";
                }
                else
                {
                    MessageBox.Show($"Błąd backendu: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd połączenia: {ex.Message}");
            }
        }

        private void LbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbItems.SelectedItem is ListingViewModel selected)
            {
                SelectedListing = selected;
                txtReason.Text = "";
            }
            else
            {
                SelectedListing = null;
            }
        }

        private void BtnApprove_Click(object sender, RoutedEventArgs e) => SendModerationDecision("APPROVED", null);

        private void BtnReject_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReason.Text))
            {
                MessageBox.Show("Podaj powód!");
                return;
            }
            SendModerationDecision("REJECTED", txtReason.Text);
        }

        private Item MapToDomain(ItemGrpcModel protoItem)
        {
            return new Item
            {
                Id = protoItem.Id,
                Title = protoItem.Title,
                Description = protoItem.Description,
                Category = protoItem.Category,
                PricePerDay = (decimal)protoItem.PricePerDay,
                Currency = protoItem.Currency,
                Status = protoItem.Status,
                OwnerId = protoItem.OwnerId,
                Photos = protoItem.Photos.ToList(),
                Location = new ItemLocation
                {
                    City = protoItem.Location.City,
                    AddressLabel = protoItem.Location.AddressLabel,
                    Country = protoItem.Location.Country,
                    Street = protoItem.Location.Street,
                    HouseNumber = protoItem.Location.HouseNumber,
                    PostalCode = protoItem.Location.PostalCode,
                    Latitude = protoItem.Location.Latitude,
                    Longitude = protoItem.Location.Longitude
                }
            };
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}