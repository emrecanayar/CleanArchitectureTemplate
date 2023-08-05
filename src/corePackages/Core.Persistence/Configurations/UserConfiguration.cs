using Core.Domain.Entities;
using Core.Persistence.Configurations.Base;
using Core.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations
{
    public class UserConfiguration : BaseConfiguration<User, Guid>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.FirstName).HasColumnName("FirstName").IsRequired(true).HasMaxLength(LengthContraints.NameMaxLength);
            builder.Property(x => x.LastName).HasColumnName("LastName").IsRequired(true).HasMaxLength(LengthContraints.NameMaxLength);
            builder.Property(x => x.Email).HasColumnName("Email").IsRequired(true).HasMaxLength(LengthContraints.EmailMaxLength);
            builder.HasIndex(x => x.Email, "UK_Users_Email").IsUnique();
            builder.Property(x => x.PasswordHash).HasColumnName("PasswordHash").IsRequired(true).HasMaxLength(LengthContraints.PasswordMaxLength);
            builder.Property(x => x.PasswordSalt).HasColumnName("PasswordSalt").HasMaxLength(LengthContraints.PasswordMaxLength);
            builder.Property(x => x.AuthenticatorType).HasColumnName("AuthenticatorType").HasConversion<int>();
            builder.Property(x => x.CultureType).HasColumnName("CultureType").HasConversion<int>();
            builder.HasMany(u => u.UserOperationClaims);
            builder.HasMany(u => u.RefreshTokens);
            builder.HasMany(u => u.EmailAuthenticators);
            builder.HasMany(u => u.OtpAuthenticators);
            builder.ToTable(TableNameConstants.USER);
        }
    }
}
