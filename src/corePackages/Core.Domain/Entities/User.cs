using Core.Domain.ComplexTypes.Enums;
using Core.Domain.Entities.Base;

namespace Core.Domain.Entities
{
    public class User : Entity<Guid>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public byte[] PasswordSalt { get; set; }

        public byte[] PasswordHash { get; set; }

        public AuthenticatorType AuthenticatorType { get; set; }

        public CultureType CultureType { get; set; }

        public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; } = null!;

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = null!;

        public virtual ICollection<EmailAuthenticator> EmailAuthenticators { get; set; } = null!;

        public virtual ICollection<OtpAuthenticator> OtpAuthenticators { get; set; } = null!;

        public User()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            PasswordHash = Array.Empty<byte>();
            PasswordSalt = Array.Empty<byte>();
        }

        public User(
            string firstName,
            string lastName,
            string email,
            byte[] passwordSalt,
            byte[] passwordHash,
            RecordStatu status,
            AuthenticatorType authenticatorType)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordSalt = passwordSalt;
            PasswordHash = passwordHash;
            Status = status;
            AuthenticatorType = authenticatorType;
        }

        public User(
            int id,
            string firstName,
            string lastName,
            string email,
            byte[] passwordSalt,
            byte[] passwordHash,
            RecordStatu status,
            AuthenticatorType authenticatorType)
            : base()
        {
            Id = Guid.Empty;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordSalt = passwordSalt;
            PasswordHash = passwordHash;
            Status = status;
            AuthenticatorType = authenticatorType;
        }
    }
}