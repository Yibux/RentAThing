using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalSystem.Shared.DTOs
{
    public class CreateUserRequest
    {
        public string Uid { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
