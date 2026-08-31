using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Locations;

namespace Bodokado.Persistence.Configurations.Locations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Latitude).HasPrecision(9, 6);
        builder.Property(c => c.Longitude).HasPrecision(9, 6);
        builder.HasOne(c => c.Province).WithMany(p => p.Cities).HasForeignKey(c => c.ProvinceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.Country).WithMany().HasForeignKey(c => c.CountryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(c => c.ProvinceId);
        builder.HasIndex(c => c.CountryId);
    }
}
