// ProductProductCategoryConfiguration.cs
using Bodokado.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductProductCategoryConfiguration : IEntityTypeConfiguration<ProductProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductProductCategory> builder)
    {
        builder.ToTable("Product_ProductCategory");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Product)
            .WithMany(p => p.ProductCategories)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasIndex(x => new { x.ProductId, x.ProductCategoryId }).IsUnique();
        builder.HasIndex(x => x.ProductCategoryId);
    }
}