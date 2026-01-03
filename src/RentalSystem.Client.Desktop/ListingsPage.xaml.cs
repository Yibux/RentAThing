using System.Windows.Controls;

namespace RentalSystem.Client.Desktop
{
    public partial class ListingsPage : Page
    {
        private readonly string _token;

        public ListingsPage(string token)
        {
            InitializeComponent();
            _token = token;
        }
    }
}