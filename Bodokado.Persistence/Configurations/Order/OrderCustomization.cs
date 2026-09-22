using Bodokado.Domain.Entities.Order;
using Bodokado.Domain.Entities.Shops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bodokado.Persistence.Configurations.OrderCustomizations;


public class OrderCustomizationTypeConfiguration : IEntityTypeConfiguration<OrderCustomizationType>
{
    public void Configure(EntityTypeBuilder<OrderCustomizationType> builder)
    {
        builder.ToTable("OrderCustomizationType");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(100);
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.ShowOrder).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.ShopCategory)
            .WithMany()
            .HasForeignKey(x => x.ShopCategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasIndex(x => x.ShopCategoryId);
        builder.HasIndex(x => x.ShowOrder);
        builder.HasIndex(x => x.IsActive);
    }
}


public class ShopOrderCustomizationTypeConfiguration : IEntityTypeConfiguration<ShopOrderCustomizationType>
{
    public void Configure(EntityTypeBuilder<ShopOrderCustomizationType> builder)
    {
        builder.ToTable("Shop_OrderCustomizationType");
        builder.HasKey(x => new { x.ShopId, x.OrderCustomizationTypeId });

        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

        builder.HasOne(x => x.Shop)
            .WithMany()
            .HasForeignKey(x => x.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.OrderCustomizationType)
            .WithMany(t => t.ShopLinks)
            .HasForeignKey(x => x.OrderCustomizationTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.IsActive);
    }
}

public class OrderCustomizationTypeOptionConfiguration : IEntityTypeConfiguration<OrderCustomizationTypeOption>
{
    public void Configure(EntityTypeBuilder<OrderCustomizationTypeOption> builder)
    {
        builder.ToTable("OrderCustomizationTypeOption");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OptionName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ShowOrder).IsRequired();
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.OrderCustomizationType)
            .WithMany(t => t.Options)
            .HasForeignKey(x => x.OrderCustomizationTypeId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(x => x.ImageFile)
            .WithMany()
            .HasForeignKey(x => x.ImageFileId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ThumbnailImageFile)
            .WithMany()
            .HasForeignKey(x => x.ThumbnailImageFileId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.OrderCustomizationTypeId);
        builder.HasIndex(x => x.ShowOrder);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.ImageFileId);
        builder.HasIndex(x => x.ThumbnailImageFileId);
    }
}


public class ShopOrderCustomizationTypeOptionConfiguration : IEntityTypeConfiguration<ShopOrderCustomizationTypeOption>
{
    public void Configure(EntityTypeBuilder<ShopOrderCustomizationTypeOption> builder)
    {
        builder.ToTable("Shop_OrderCustomizationTypeOption");
        builder.HasKey(x => new { x.ShopId, x.OrderCustomizationTypeOptionId });

        builder.HasOne(x => x.Shop)
            .WithMany()
            .HasForeignKey(x => x.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.OrderCustomizationTypeOption)
            .WithMany(o => o.ShopLinks)
            .HasForeignKey(x => x.OrderCustomizationTypeOptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}