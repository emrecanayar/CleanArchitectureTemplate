namespace Core.Security.Dtos
{
    public class UserForRegisterDto
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? RegistrationNumber { get; set; }

        public string? UserName { get; set; }

        public Guid CountryId { get; set; }
    }
}
