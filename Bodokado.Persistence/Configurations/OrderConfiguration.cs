using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Orders;

namespace Bodokado.Persistence.Configurations.Orders;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(32);
        builder.Property(o => o.BuyerName).IsRequired().HasMaxLength(150);
        builder.Property(o => o.BuyerPhone).IsRequired().HasMaxLength(20);
        builder.Property(o => o.DeliveryAddress).HasMaxLength(1000);
        builder.Property(o => o.DeliveryTimeSlot).HasMaxLength(50);
        builder.Property(o => o.PackagingType).HasMaxLength(100);
        builder.Property(o => o.PackagingNote).HasMaxLength(500);
        builder.Property(o => o.GiftCardType).HasMaxLength(100);
        builder.Property(o => o.GiftCardColor).HasMaxLength(50);
        builder.Property(o => o.RibbonStyle).HasMaxLength(100);
        builder.Property(o => o.GiftCardDesignKey).HasMaxLength(100);
        builder.Property(o => o.GiftMessage).HasMaxLength(1000);
        builder.Property(o => o.RecipientName).HasMaxLength(150);
        builder.Property(o => o.DiscountCode).HasMaxLength(50);
        builder.Property(o => o.RejectionReason).HasMaxLength(1000);
        builder.Property(o => o.ShopNote).HasMaxLength(1000);

        builder.Property(o => o.GoodsAmount).HasPrecision(18, 0);
        builder.Property(o => o.ShippingCost).HasPrecision(18, 0);
        builder.Property(o => o.PackagingCost).HasPrecision(18, 0);
        builder.Property(o => o.DiscountAmount).HasPrecision(18, 0);
        builder.Property(o => o.FinalAmount).HasPrecision(18, 0);
        builder.Property(o => o.Latitude).HasPrecision(9, 6);
        builder.Property(o => o.Longitude).HasPrecision(9, 6);

        builder.Property(o => o.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(o => !o.IsDeleted);

        builder.HasOne(o => o.Shop)
            .WithMany()
            .HasForeignKey(o => o.ShopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.CustomerUser)
            .WithMany()
            .HasForeignKey(o => o.CustomerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Province)
            .WithMany()
            .HasForeignKey(o => o.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.City)
            .WithMany()
            .HasForeignKey(o => o.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.ShopId);
        builder.HasIndex(o => o.CustomerUserId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);
    }
}
