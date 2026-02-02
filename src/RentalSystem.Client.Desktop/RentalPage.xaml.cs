using System.Windows;
using System.Windows.Controls;

namespace RentalSystem.Client.Desktop
{
    public partial class RentalsPage : Page
    {
        private readonly RentalsViewModel _viewModel;

        public RentalsPage(string token)
        {
            InitializeComponent();
            _viewModel = new RentalsViewModel(token);
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadRentalsAsync();
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadRentalsAsync();
        }
    }
}