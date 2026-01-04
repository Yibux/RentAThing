namespace RentalSystem.Shared.DTOs
{
    public class CreateUserRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Surname { get; set; }
        public string Uid { get; set; }
    }

    public class UpdateUserRequest
    {
        public bool? IsBanned { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Surname { get; set; }
    }
}