using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RentalSystem.Shared.Models;

namespace RentalSystem.Client.Desktop
{
    public class ListingViewModel
    {
        public Item Model { get; set; }

        public ListingViewModel(Item item)
        {
            Model = item;
            LoadPhotos();
        }

        public string Id => Model.Id;
        public string Title => Model.Title;
        public string Description => Model.Description;
        public string Category => Model.Category;
        public decimal PricePerDay => Model.PricePerDay;
        public string Currency => Model.Currency;
        public string Status => Model.Status;
        public ItemLocation Location => Model.Location;

        public Brush StatusColor
        {
            get
            {
                switch (Model.Status)
                {
                    case "APPROVED": return Brushes.Green;
                    case "REJECTED": return Brushes.Red;
                    case "PENDING":
                    default: return Brushes.Orange;
                }
            }
        }

        public ObservableCollection<ImageSource> Photos { get; set; } = new ObservableCollection<ImageSource>();

        private void LoadPhotos()
        {
            if (Model.Photos == null) return;

            foreach (var photoStr in Model.Photos)
            {
                try
                {
                    if (string.IsNullOrEmpty(photoStr)) continue;

                    if (photoStr.StartsWith("data:image"))
                    {
                        var base64 = photoStr.Split(',')[1];
                        var bytes = Convert.FromBase64String(base64);

                        using (var stream = new MemoryStream(bytes))
                        {
                            var image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = stream;
                            image.EndInit();
                            image.Freeze();
                            Photos.Add(image);
                        }
                    }
                    else
                    {
                        var image = new BitmapImage(new Uri(photoStr));
                        Photos.Add(image);
                    }
                }
                catch
                {
                }
            }
        }
    }
}