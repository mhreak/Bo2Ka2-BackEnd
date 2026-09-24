// Persistence/Configurations/Delivery/ShopOrderDeliveryRuleConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Delivery;

namespace Bodokado.Persistence.Configurations.Delivery;

public class ShopOrderDeliveryRuleConfiguration : IEntityTypeConfiguration<ShopOrderDeliveryRule>
{
    public void Configure(EntityTypeBuilder<ShopOrderDeliveryRule> builder)
    {
        builder.ToTable("ShopOrderDeliveryRule");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.IncludedCityIds).HasMaxLength(2000);
        builder.Property(x => x.IncludedProvinceIds).HasMaxLength(2000);
        builder.Property(x => x.ExcludedCityIds).HasMaxLength(2000);
        builder.Property(x => x.ExcludedProvinceIds).HasMaxLength(2000);
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Shop)
            .WithMany()
            .HasForeignKey(x => x.ShopId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(x => x.DeliveryServiceProvider)
            .WithMany()
            .HasForeignKey(x => x.DeliveryServiceProviderId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasIndex(x => x.ShopId);
        builder.HasIndex(x => x.DeliveryServiceProviderId);
        builder.HasIndex(x => x.IsActive);
    }
}