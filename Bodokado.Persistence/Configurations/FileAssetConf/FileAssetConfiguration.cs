using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities;

namespace Bodokado.Persistence.Configurations.FileAssetConf;

public class FileAssetConfiguration : IEntityTypeConfiguration<FileAsset>
{
    public void Configure(EntityTypeBuilder<FileAsset> builder)
    {
        builder.ToTable("Files");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.FileName).IsRequired().HasMaxLength(300);
        builder.Property(f => f.Extension).IsRequired().HasMaxLength(20);
        builder.Property(f => f.UploadFileType).IsRequired().HasMaxLength(100);
        builder.Property(f => f.Path).IsRequired().HasMaxLength(1000);
        builder.Property(f => f.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(f => !f.IsDeleted);
        builder.HasIndex(f => f.UploaderId);
        builder.HasIndex(f => f.IsDeleted);
    }
}
