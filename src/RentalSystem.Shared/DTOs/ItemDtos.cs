using System.Collections.Generic;

namespace RentalSystem.Shared.DTOs
{
    public class CreateItemDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal PricePerDay { get; set; }
        public string Currency { get; set; } = "PLN";
        public ItemLocationDto Location { get; set; }
        public List<string> Photos { get; set; } = new List<string>();
    }

    public class ItemLocationDto
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string AddressLabel { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class ModerateItemDto
    {
        public string Status { get; set; }
        public string? RejectionReason { get; set; }
    }
}