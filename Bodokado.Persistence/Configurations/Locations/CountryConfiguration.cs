using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Locations;

namespace Bodokado.Persistence.Configurations.Locations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Iso2).HasMaxLength(2);
        builder.Property(c => c.Iso3).HasMaxLength(3);
        builder.Property(c => c.PhoneCode).HasMaxLength(20);
        builder.Property(c => c.Capital).HasMaxLength(200);
        builder.Property(c => c.Currency).HasMaxLength(10);
        builder.Property(c => c.Region).HasMaxLength(100);
        builder.Property(c => c.Subregion).HasMaxLength(100);
        builder.Property(c => c.Latitude).HasPrecision(9, 6);
        builder.Property(c => c.Longitude).HasPrecision(9, 6);
    }
}
