using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Persistence.Configurations.Products;

public class ShopProductVariationConfiguration : IEntityTypeConfiguration<ShopProductVariation>
{
    public void Configure(EntityTypeBuilder<ShopProductVariation> builder)
    {
        builder.ToTable("ShopProductVariation");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        // ⬇️ همین‌جا
        builder.HasOne(x => x.Product)
            .WithMany(p => p.Variations)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProductId);
    }
}