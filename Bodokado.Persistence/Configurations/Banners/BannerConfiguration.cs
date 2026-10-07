// Persistence/Configurations/Banners/BannerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Banners;

namespace Bodokado.Persistence.Configurations.Banners;

public class BannerConfiguration : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.ToTable("Banner");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired(false).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Link).HasMaxLength(500);
        builder.Property(x => x.ShowOrder).IsRequired();
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Image)
            .WithMany()
            .HasForeignKey(x => x.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.ShowOrder);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.ImageId);

        builder.Property(x => x.ShowPlace).IsRequired();
        builder.HasIndex(x => x.ShowPlace);
        builder.HasIndex(x => new { x.ShowPlace, x.IsActive, x.ShowOrder });
    }
}