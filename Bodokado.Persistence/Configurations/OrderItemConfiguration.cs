using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Orders;

namespace Bodokado.Persistence.Configurations.Orders;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(i => i.SelectedColor).HasMaxLength(50);
        builder.Property(i => i.UnitPrice).HasPrecision(18, 0);
        builder.Property(i => i.LineTotal).HasPrecision(18, 0);

        builder.Property(i => i.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(i => !i.IsDeleted);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.OrderId);
        builder.HasIndex(i => i.ProductId);
    }
}
