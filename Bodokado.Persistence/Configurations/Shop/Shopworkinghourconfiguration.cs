using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Shops;

namespace Bodokado.Persistence.Configurations.Shops;

public class ShopWorkingHourConfiguration : IEntityTypeConfiguration<ShopWorkingHour>
{
    public void Configure(EntityTypeBuilder<ShopWorkingHour> builder)
    {
        builder.ToTable("ShopWorkingHour");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(w => !w.IsDeleted);

        builder.HasOne(w => w.Shop)
            .WithMany(s => s.WorkingHours)
            .HasForeignKey(w => w.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(w => new { w.ShopId, w.DayOfWeek }).IsUnique();
    }
}