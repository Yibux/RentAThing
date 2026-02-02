using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RentalSystem.Client.Desktop.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                switch (status.ToUpper())
                {
                    case "APPROVED":
                        return Brushes.LightGreen;
                    case "COMPLETED":
                        return Brushes.LightBlue;
                    case "CANCELLED":
                    case "REJECTED":
                        return Brushes.LightPink;
                    case "PENDING":
                    case "REQUESTED":
                        return Brushes.LightYellow;
                    default:
                        return Brushes.White;
                }
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}