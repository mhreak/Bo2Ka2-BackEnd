// ProductPropertyConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Persistence.Configurations.Products;

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("ProductAttribute");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(150);

        builder.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasOne(p => p.ProductCategory)
            .WithMany(c => c.Attributes)
            .HasForeignKey(p => p.ProductCategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => p.ProductCategoryId);
        builder.HasIndex(p => p.SortOrder);
    }
}