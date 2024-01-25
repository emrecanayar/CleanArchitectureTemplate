using Core.Domain.Entities;
using Core.Persistence.Configurations.Base;
using Core.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations
{
    public class UploadedFileConfiguration : BaseConfiguration<UploadedFile, Guid>
    {
        public override void Configure(EntityTypeBuilder<UploadedFile> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.Token).HasColumnName("Token").IsRequired(true).HasMaxLength(LengthContraints.TokenMaxLength);
            builder.Property(x => x.Directory).HasColumnName("Directory").IsRequired(false).HasMaxLength(LengthContraints.TokenMaxLength);
            builder.Property(x => x.Path).HasColumnName("Path").IsRequired(false).HasMaxLength(LengthContraints.TokenMaxLength);
            builder.Property(x => x.Extension).HasColumnName("Extension").IsRequired(false).HasMaxLength(LengthContraints.TokenMaxLength);
            builder.Property(x => x.FileType).HasColumnName("FileType").IsRequired(false).HasConversion<int>();
            builder.ToTable(TableNameConstants.UPLOADED_FILE);
        }
    }
}
