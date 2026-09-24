// Persistence/Configurations/Discount/DiscountCodeConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Discounts;
using Bodokado.Domain.Enums;

namespace Bodokado.Persistence.Configurations.Discounts;

public class DiscountCodeConfiguration : IEntityTypeConfiguration<DiscountCode>
{
    public void Configure(EntityTypeBuilder<DiscountCode> builder)
    {
        builder.ToTable("DiscountCode");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.DiscountCodeType).IsRequired().HasConversion<short>();
        builder.Property(x => x.DiscountType).IsRequired().HasConversion<short>();

        builder.Property(x => x.IncludedOrganizationIds).HasMaxLength(2000);
        builder.Property(x => x.ExcludedOrganizationIds).HasMaxLength(2000);
        builder.Property(x => x.IncludedShopIds).HasMaxLength(2000);
        builder.Property(x => x.ExcludedShopIds).HasMaxLength(2000);
        builder.Property(x => x.IncludedProductCategoryIds).HasMaxLength(2000);
        builder.Property(x => x.ExcludedProductCategoryIds).HasMaxLength(2000);
        builder.Property(x => x.IncludedProductIds).HasMaxLength(2000);
        builder.Property(x => x.ExcludedProductIds).HasMaxLength(2000);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne(x => x.OrderCustomizationType)
            .WithMany()
            .HasForeignKey(x => x.OrderCustomizationTypeId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.Property(x => x.IsActiveByAdmin)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(x => x.IsActiveByAdmin);

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.StartDateTime);
        builder.HasIndex(x => x.FinishDateTime);
    }
}