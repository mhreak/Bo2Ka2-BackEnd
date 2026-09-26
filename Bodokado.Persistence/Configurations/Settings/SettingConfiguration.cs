// Persistence/Configurations/Settings/SettingConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Settings;

namespace Bodokado.Persistence.Configurations.Settings;

public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("Setting");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Value)
            .IsRequired()
            .HasColumnType("nvarchar(max)"); // JSON می‌تواند بلند باشد

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => x.Key).IsUnique();
    }
}