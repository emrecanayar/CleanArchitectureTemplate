namespace Core.Security.Dtos
{
    public class UserForLoginDto
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string? AuthenticatorCode { get; set; }
    }
}
