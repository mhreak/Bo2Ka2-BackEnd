// Persistence/Configurations/Delivery/DeliveryServiceProviderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Delivery;

namespace Bodokado.Persistence.Configurations.Delivery;

public class DeliveryServiceProviderConfiguration : IEntityTypeConfiguration<DeliveryServiceProvider>
{
    public void Configure(EntityTypeBuilder<DeliveryServiceProvider> builder)
    {
        builder.ToTable("DeliveryServiceProvider");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServiceProviderName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.IncludedCityIds).HasMaxLength(2000);
        builder.Property(x => x.IncludedProvinceIds).HasMaxLength(2000);
        builder.Property(x => x.ExcludedCityIds).HasMaxLength(2000);
        builder.Property(x => x.ExcludedProvinceIds).HasMaxLength(2000);
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.LogoFile)
            .WithMany()
            .HasForeignKey(x => x.LogoFileId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.LogoFileId);
    }
}