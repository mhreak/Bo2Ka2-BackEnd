// Persistence/Configurations/Story/StoryConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Stories;
using Bodokado.Domain.Enums;

namespace Bodokado.Persistence.Configurations.Stories;

public class StoryConfiguration : IEntityTypeConfiguration<Story>
{
    public void Configure(EntityTypeBuilder<Story> builder)
    {
        builder.ToTable("Story");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsPublished).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.DisabledByAdmin).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.StoryButtonText).HasMaxLength(30);
        builder.Property(x => x.ShowOrder).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.StoryButtonClickActionType)
            .IsRequired()
            .HasConversion<short>()
            .HasDefaultValue(StoryButtonClickActionType.None);

        builder.Property(x => x.ShowPlace)
            .IsRequired()
            .HasConversion<short>();

        builder.HasOne(x => x.Shop)
            .WithMany()
            .HasForeignKey(x => x.ShopId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne(x => x.MediaFile)
            .WithMany()
            .HasForeignKey(x => x.MediaFileId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.ShopId);
        builder.HasIndex(x => x.IsPublished);
        builder.HasIndex(x => x.DisabledByAdmin);
        builder.HasIndex(x => x.ShowPlace);
        builder.HasIndex(x => x.ShowOrder);
        builder.HasIndex(x => x.PublishDateTime);
        builder.HasIndex(x => x.MediaFileId);

        builder.Property(x => x.IsActiveByAdmin)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(x => x.IsActiveByAdmin);
    }
}