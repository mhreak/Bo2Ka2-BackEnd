using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Shops;

namespace Bodokado.Persistence.Configurations.Shops;

public class ShopCategoryConfiguration : IEntityTypeConfiguration<ShopCategory>
{
    public void Configure(EntityTypeBuilder<ShopCategory> builder)
    {
        builder.ToTable("ShopCategory");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        builder.Property(c => c.IconKey).HasMaxLength(200);
        builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(c => !c.IsDeleted);
        builder.HasIndex(c => c.Name);
    }
}