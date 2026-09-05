using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Persistence.Configurations.Products;

public class ProductColorConfiguration : IEntityTypeConfiguration<ProductColor>
{
    public void Configure(EntityTypeBuilder<ProductColor> builder)
    {
        builder.ToTable("ProductColor");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
        builder.Property(c => c.HexCode).HasMaxLength(20);

        builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasIndex(c => c.ProductId);
    }
}
