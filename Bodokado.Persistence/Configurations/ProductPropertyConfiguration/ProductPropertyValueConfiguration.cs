using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Persistence.Configurations.Products;

public class ProductPropertyValueConfiguration : IEntityTypeConfiguration<ProductPropertyValue>
{
    public void Configure(EntityTypeBuilder<ProductPropertyValue> builder)
    {
        builder.ToTable("ProductPropertyValue");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Title).IsRequired().HasMaxLength(150);
        builder.Property(v => v.Value).IsRequired().HasMaxLength(200);
        builder.Property(v => v.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(v => !v.IsDeleted);

        builder.HasOne(v => v.ProductProperty)
            .WithMany(p => p.Values)
            .HasForeignKey(v => v.ProductPropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => v.ProductPropertyId);
        builder.HasIndex(v => v.SortOrder);
        builder.HasIndex(v => v.IsActive);
    }
}