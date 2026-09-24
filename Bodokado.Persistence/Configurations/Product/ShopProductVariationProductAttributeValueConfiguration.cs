// Persistence/Configurations/Product/ShopProductVariationProductAttributeValueConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Persistence.Configurations.Products;

public class ShopProductVariationProductAttributeValueConfiguration
    : IEntityTypeConfiguration<ShopProductVariationProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ShopProductVariationProductAttributeValue> builder)
    {
        builder.ToTable("ShopProductVariation_ProductAttributeValue");
        builder.HasKey(x => new { x.ShopProductVariationId, x.ProductAttributeValueId });

        builder.HasOne(x => x.ShopProductVariation)
            .WithMany(v => v.AttributeValues)
            .HasForeignKey(x => x.ShopProductVariationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ProductAttributeValue)
            .WithMany()
            .HasForeignKey(x => x.ProductAttributeValueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}