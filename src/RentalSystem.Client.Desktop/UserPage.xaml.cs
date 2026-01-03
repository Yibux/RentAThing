using Grpc.Core;
using Grpc.Net.Client;
using RentalSystem.Shared.Protos;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RentalSystem.Client.Desktop
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Role { get; set; }
        public bool IsBanned { get; set; }
        public Brush IsBannedColor => IsBanned ? Brushes.Red : Brushes.Green;
    }

    public partial class UsersPage : Page
    {
        private readonly string _token;
        private const string BackendUrl = "http://localhost:5003";

        public ObservableCollection<UserViewModel> Users { get; set; } = new ObservableCollection<UserViewModel>();

        public UsersPage(string token)
        {
            InitializeComponent();
            _token = token;
            dgUsers.ItemsSource = Users;
            LoadUsers();
        }

        private async void LoadUsers()
        {
            try
            {
                var channel = GrpcChannel.ForAddress(BackendUrl, new GrpcChannelOptions { Credentials = ChannelCredentials.Insecure });
                var client = new AuthService.AuthServiceClient(channel);
                var headers = new Metadata { { "Authorization", $"Bearer {_token}" } };

                var response = await client.GetAllUsersAsync(new EmptyRequest(), headers);

                Users.Clear();
                foreach (var u in response.Users)
                {
                    Users.Add(new UserViewModel
                    {
                        Id = u.Id,
                        Email = u.Email,
                        Name = u.Name,
                        Surname = u.Surname,
                        Role = u.Role,
                        IsBanned = u.IsBanned
                    });
                }
            }
            catch (Exception ex) { MessageBox.Show($"Błąd: {ex.Message}"); }
        }

        private async void UpdateBanStatus(bool isBanned)
        {
            if (dgUsers.SelectedItem is UserViewModel selectedUser)
            {
                try
                {
                    var channel = GrpcChannel.ForAddress(BackendUrl, new GrpcChannelOptions { Credentials = ChannelCredentials.Insecure });
                    var client = new AuthService.AuthServiceClient(channel);
                    var headers = new Metadata { { "Authorization", $"Bearer {_token}" } };

                    var response = await client.SetUserBanStatusAsync(new BanUserRequest { UserId = selectedUser.Id, IsBanned = isBanned }, headers);
                    if (response.Success) LoadUsers();
                }
                catch (Exception ex) { MessageBox.Show($"Błąd: {ex.Message}"); }
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadUsers();
        private void BtnBan_Click(object sender, RoutedEventArgs e) => UpdateBanStatus(true);
        private void BtnUnban_Click(object sender, RoutedEventArgs e) => UpdateBanStatus(false);
    }
}