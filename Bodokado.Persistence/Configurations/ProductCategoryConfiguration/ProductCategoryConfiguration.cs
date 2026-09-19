// ProductCategoryConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Persistence.Configurations.Products;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategory");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Image)
            .WithMany()
            .HasForeignKey(c => c.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.ParentCategoryId);
        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.IsActive);
    }
}