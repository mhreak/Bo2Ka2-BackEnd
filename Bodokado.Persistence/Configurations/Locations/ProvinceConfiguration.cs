using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Locations;

namespace Bodokado.Persistence.Configurations.Locations;

public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.ToTable("Provinces");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.StateCode).HasMaxLength(20);
        builder.Property(p => p.Latitude).HasPrecision(9, 6);
        builder.Property(p => p.Longitude).HasPrecision(9, 6);
        builder.HasOne(p => p.Country).WithMany(c => c.Provinces).HasForeignKey(p => p.CountryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(p => p.CountryId);
    }
}
