using Core.Domain.Entities;
using Core.Persistence.Configurations.Base;
using Core.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations
{
    public class LanguageConfiguration : BaseConfiguration<Language, Guid>
    {
        public override void Configure(EntityTypeBuilder<Language> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.Name).HasColumnName("Name").HasMaxLength(LengthContraints.NameMaxLength);
            builder.Property(x => x.Symbol).HasColumnName("Symbol").HasMaxLength(LengthContraints.SymbolMaxLength);
            builder.Property(x => x.Flag).HasColumnName("Flag").HasMaxLength(50);
            builder.HasMany(p => p.Dictionaries);
            builder.ToTable(TableNameConstants.LANGUAGE);
        }
    }
}
