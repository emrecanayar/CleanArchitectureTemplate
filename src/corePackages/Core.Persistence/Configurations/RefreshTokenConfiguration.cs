using Core.Domain.Entities;
using Core.Persistence.Configurations.Base;
using Core.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations
{
    public class RefreshTokenConfiguration : BaseConfiguration<RefreshToken, Guid>
    {
        public override void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.UserId).HasColumnName("UserId").IsRequired(true);
            builder.Property(x => x.Token).HasColumnName("Token").IsRequired(true).HasMaxLength(LengthContraints.TokenMaxLength);
            builder.Property(x => x.Expires).HasColumnName("Expires").IsRequired(true);
            builder.Property(x => x.CreatedByIp).HasColumnName("CreatedByIp").IsRequired(true).HasMaxLength(LengthContraints.CreatedByMaxLength).ValueGeneratedOnAdd();
            builder.Property(x => x.Revoked).HasColumnName("Revoked").IsRequired(false).ValueGeneratedOnAdd();
            builder.Property(x => x.RevokedByIp).HasColumnName("RevokedByIp").IsRequired(false).HasMaxLength(LengthContraints.CreatedByMaxLength).ValueGeneratedOnAdd();
            builder.Property(x => x.ReplacedByToken).HasColumnName("ReplacedByToken").IsRequired(false).HasMaxLength(LengthContraints.CreatedByMaxLength).ValueGeneratedOnAdd();
            builder.Property(x => x.ReasonRevoked).HasColumnName("ReasonRevoked").IsRequired(false).HasMaxLength(LengthContraints.CreatedByMaxLength).ValueGeneratedOnAdd();
            builder.ToTable(TableNameConstants.REFRESHTOKEN);
        }
    }
}
