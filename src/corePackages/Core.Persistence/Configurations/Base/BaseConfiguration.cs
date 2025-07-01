using Core.Domain.ComplexTypes.Enums;
using Core.Domain.Entities.Base;
using Core.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations.Base
{
    public abstract class BaseConfiguration<T, TKey> : IEntityTypeConfiguration<T>
        where T : Entity<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("Id").HasDefaultValueSql("(newid())");
            builder.Property(x => x.Status).HasColumnName("Status").IsRequired(false).HasDefaultValue(RecordStatu.Active);
            builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy").IsRequired(true).HasMaxLength(LengthContraints.CreatedByMaxLength).ValueGeneratedOnAdd();
            builder.Property(x => x.ModifiedBy).HasColumnName("ModifiedBy").IsRequired(false).HasMaxLength(LengthContraints.CreatedByMaxLength);
            builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired(true).ValueGeneratedOnAdd().HasDefaultValueSql("(getdate())");
            builder.Property(x => x.ModifiedDate).HasColumnName("ModifiedDate").IsRequired(false);
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").IsRequired(true).HasDefaultValue(false);
            builder.Property(x => x.DeletedDate).HasColumnName("DeletedDate").IsRequired(false);
            builder.HasQueryFilter(x => !x.IsDeleted && x.DeletedDate == null);
        }
    }
}
