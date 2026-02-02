using System.Windows;

namespace RentalSystem.Client.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly string _token;

        public MainWindow(string token)
        {
            InitializeComponent();
            _token = token;

            MainFrame.Navigate(new UsersPage(_token));
        }

        public MainWindow() : this("") { }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UsersPage(_token));
        }

        private void BtnListings_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ListingsPage(_token));
        }

        private void BtnRentals_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RentalsPage(_token));
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}