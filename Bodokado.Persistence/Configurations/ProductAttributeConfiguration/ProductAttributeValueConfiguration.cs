using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Persistence.Configurations.Products;

public class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.ToTable("ProductAttributeValue");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Title).IsRequired().HasMaxLength(150);
        builder.Property(v => v.Value).IsRequired().HasMaxLength(200);
        builder.Property(v => v.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(v => !v.IsDeleted);

        builder.HasOne(v => v.ProductAttribute)
            .WithMany(p => p.Values)
            .HasForeignKey(v => v.ProductAttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => v.ProductAttributeId);
        builder.HasIndex(v => v.SortOrder);
        builder.HasIndex(v => v.IsActive);
    }
}