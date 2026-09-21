using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Persistence.Configurations.Products;

public class ProductProductAttributeConfiguration : IEntityTypeConfiguration<ProductProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductProductAttribute> builder)
    {
        builder.ToTable("Product_ProductAttribute");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value).HasMaxLength(100);
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Product)
            .WithMany(p => p.ProductAttributes)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ProductAttribute)
            .WithMany(p => p.ProductAttributeLinks)
            .HasForeignKey(x => x.ProductAttributeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.ProductAttributeId);
        builder.HasIndex(x => new { x.ProductId, x.ProductAttributeId });
    }
}