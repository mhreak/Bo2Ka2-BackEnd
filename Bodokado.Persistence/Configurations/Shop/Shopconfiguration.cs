using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Shops;

namespace Bodokado.Persistence.Configurations.Shops;

public class ShopConfiguration : IEntityTypeConfiguration<Shop>
{
    public void Configure(EntityTypeBuilder<Shop> builder)
    {
        builder.ToTable("Shops");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.FirstName).HasMaxLength(100);
        builder.Property(s => s.LastName).HasMaxLength(100);
        builder.Property(s => s.NationalCode).HasMaxLength(10);
        builder.Property(s => s.ShopName).HasMaxLength(200);
        builder.Property(s => s.TextAddress).HasMaxLength(1000);
        builder.Property(s => s.ShabaNumber).HasMaxLength(26);
        builder.Property(s => s.ReturnPolicy).HasMaxLength(2000);
        builder.Property(s => s.RejectionReason).HasMaxLength(1000);
        builder.Property(s => s.Latitude).HasPrecision(9, 6);
        builder.Property(s => s.Longitude).HasPrecision(9, 6);
        builder.Property(s => s.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasOne(s => s.User)
            .WithOne(u => u.Shop)
            .HasForeignKey<Shop>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.ShopCategory)
            .WithMany(c => c.Shops)
            .HasForeignKey(s => s.ShopCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Province)
            .WithMany()
            .HasForeignKey(s => s.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.City)
            .WithMany()
            .HasForeignKey(s => s.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.AvatarFile)
            .WithMany()
            .HasForeignKey(s => s.AvatarFileId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.CoverFile)
            .WithMany()
            .HasForeignKey(s => s.CoverFileId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => s.UserId).IsUnique();
        builder.HasIndex(s => s.NationalCode);
        builder.HasIndex(s => s.VerificationStatus);
    }
}