using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Bodokado.Persistence.Configurations.Products;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("ShopProduct");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(4000);
        builder.Property(p => p.Brand).HasMaxLength(100);

        builder.Property(p => p.WeightGrams).HasPrecision(18, 2);
        builder.Property(p => p.LengthCm).HasPrecision(18, 2);
        builder.Property(p => p.WidthCm).HasPrecision(18, 2);
        builder.Property(p => p.HeightCm).HasPrecision(18, 2);
        builder.Property(p => p.BasePrice).HasPrecision(18, 0);
        builder.Property(p => p.DiscountPrice).HasPrecision(18, 0);

        // لیست شناسه فایل‌ها به‌صورت JSON در ستون خود Product ذخیره می‌شود (بدون جدول جدا)
       

        builder.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasOne(p => p.Shop)
            .WithMany()
            .HasForeignKey(p => p.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.ProductType)
            .IsRequired()
            .HasDefaultValue(ProductType.Simple)
            .HasConversion<short>();

        builder.Property(x => x.IsActiveByAdmin)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(x => x.IsActiveByAdmin);


        builder.HasIndex(p => p.ShopId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.IsSpecial);
        builder.HasIndex(p => p.SoldCount);
        builder.HasIndex(p => new { p.ShopId, p.Name });
    }
}