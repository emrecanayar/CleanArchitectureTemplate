using Core.Domain.Entities;
using Core.Persistence.Configurations.Base;
using Core.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations
{
    public class EmailAuthenticatorConfiguration : BaseConfiguration<EmailAuthenticator, Guid>
    {
        public override void Configure(EntityTypeBuilder<EmailAuthenticator> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.UserId).HasColumnName("UserId");
            builder.Property(x => x.ActivationKey).HasColumnName("ActivationKey").IsRequired(false).HasMaxLength(LengthContraints.ActivationKey);
            builder.Property(x => x.IsVerified).HasColumnName("IsVerified").IsRequired(true);
            builder.ToTable(TableNameConstants.EMAIL_AUTHENTICATOR);

        }
    }
}
