using RentalSystem.Shared.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RentalSystem.Client.Desktop
{
    public class ListingViewModel : INotifyPropertyChanged
    {
        public Item Model { get; private set; }

        public ListingViewModel(Item item)
        {
            Model = item;
            LoadPhotos();
        }

        public string Id => Model.Id;
        public string Title => Model.Title;
        public string Description => Model.Description;
        public string Category => Model.Category;

        public string PriceDisplay => $"{Model.PricePerDay} {Model.Currency} / dzień";

        public string Status => Model.Status;
        public ItemLocation Location => Model.Location;

        public Brush StatusColor
        {
            get
            {
                if (Model.Status == "APPROVED") return Brushes.Green;
                if (Model.Status == "REJECTED") return Brushes.Red;
                return Brushes.Orange;
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

                    BitmapImage image = new BitmapImage();

                    if (photoStr.StartsWith("data:image"))
                    {
                        var base64 = photoStr.Contains(",") ? photoStr.Split(',')[1] : photoStr;
                        var bytes = Convert.FromBase64String(base64);

                        using (var stream = new MemoryStream(bytes))
                        {
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = stream;
                            image.EndInit();
                        }
                    }
                    else
                    {
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.UriSource = new Uri(photoStr);
                        image.EndInit();
                    }

                    image.Freeze();
                    Photos.Add(image);
                }
                catch (Exception)
                {
                    
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}