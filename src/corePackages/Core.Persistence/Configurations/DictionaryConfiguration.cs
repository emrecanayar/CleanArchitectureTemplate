using Core.Domain.Entities;
using Core.Persistence.Configurations.Base;
using Core.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations
{
    public class DictionaryConfiguration : BaseConfiguration<Dictionary, Guid>
    {
        public override void Configure(EntityTypeBuilder<Dictionary> builder)
        {
            builder.Property(x => x.LanguageId).HasColumnName("LanguageId").IsRequired(true);
            builder.Property(x => x.EntryKey).HasColumnName("EntryKey").HasMaxLength(50);
            builder.Property(x => x.EntryValue).HasColumnName("EntryValue").HasColumnType(LengthContraints.MAX);
            builder.Property(x => x.Entity).HasColumnName("Entity").HasMaxLength(50);
            builder.Property(x => x.Property).HasColumnName("Property").HasMaxLength(50);
            builder.Property(x => x.ValueType).HasColumnName("ValueType").HasMaxLength(50);
            builder.HasOne(x => x.Language).WithMany(x => x.Dictionaries).HasForeignKey(x => x.LanguageId).IsRequired(true);
            builder.ToTable(TableNameConstants.DICTIONARY);
        }

    }
}
