using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Persistence.Configurations.Products;

public class ProductProductPropertyConfiguration : IEntityTypeConfiguration<ProductProductProperty>
{
    public void Configure(EntityTypeBuilder<ProductProductProperty> builder)
    {
        builder.ToTable("Product_ProductProperty");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value).HasMaxLength(100);
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Product)
            .WithMany(p => p.ProductProperties)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ProductProperty)
            .WithMany(p => p.ProductLinks)
            .HasForeignKey(x => x.ProductPropertyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.ProductPropertyId);
        builder.HasIndex(x => new { x.ProductId, x.ProductPropertyId });
    }
}