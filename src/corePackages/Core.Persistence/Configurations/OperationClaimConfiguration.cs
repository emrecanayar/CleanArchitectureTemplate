using Core.Domain.Entities;
using Core.Persistence.Configurations.Base;
using Core.Persistence.Constants;
using Core.Persistence.Seeds;
using Core.Security.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations
{
    public class OperationClaimConfiguration : BaseConfiguration<OperationClaim, Guid>
    {
        public override void Configure(EntityTypeBuilder<OperationClaim> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.Name).HasColumnName("Name").IsRequired(true).HasMaxLength(LengthContraints.NameMaxLength);
            builder.HasIndex(x => x.Name, "UK_OperationClaims_Name").IsUnique();
            builder.ToTable(TableNameConstants.OPERATION_CLAIM);
            builder.HasData(getSeeds());
        }

        private HashSet<OperationClaim> getSeeds()
        {
            HashSet<OperationClaim> seeds =
                new()
                {
                new OperationClaim { Id =SeedData.AdminOperationClaimId, Name = GeneralOperationClaims.Admin }
                };

            return seeds;
        }
    }
}
