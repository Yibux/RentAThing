using System;

namespace RentalSystem.Shared.DTOs
{
    public class CreateRentalDto
    {
        public string ItemId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}