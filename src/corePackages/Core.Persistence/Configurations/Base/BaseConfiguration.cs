using Core.Domain.Entities.Base;
using Core.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Core.Domain.ComplexTypes.Enums;

namespace Core.Persistence.Configurations.Base
{
    public abstract class BaseConfiguration<T, TKey> : IEntityTypeConfiguration<T>
        where T : Entity<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("Id");
            builder.Property(x => x.Status).HasColumnName("Status").IsRequired(true).HasConversion<int>().HasDefaultValue(RecordStatu.Active);
            builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy").IsRequired(true).HasMaxLength(LengthContraints.CreatedByMaxLength).ValueGeneratedOnAdd();
            builder.Property(x => x.ModifiedBy).HasColumnName("ModifiedBy").IsRequired(false).HasMaxLength(LengthContraints.CreatedByMaxLength);
            builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired(true).ValueGeneratedOnAdd();
            builder.Property(x => x.ModifiedDate).HasColumnName("ModifiedDate").IsRequired(false);
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").IsRequired(true).HasDefaultValue(false);
            builder.Property(x => x.DeletedDate).HasColumnName("DeletedDate").IsRequired(false);
            builder.HasQueryFilter(x => x.IsDeleted == false && x.DeletedDate == null);
        }
    }
}
