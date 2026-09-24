using Bodokado.Domain.Entities.Organizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserOrganizationConfiguration : IEntityTypeConfiguration<UserOrganization>
{
    public void Configure(EntityTypeBuilder<UserOrganization> builder)
    {
        builder.ToTable("User_Organization");
        builder.HasKey(x => new { x.UserId, x.OrganizationId });

        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);   // این معمولاً OK است

        // مهم: Cascade نگذار
        builder.HasOne(x => x.Organization)
            .WithMany(o => o.UserOrganizations)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OrganizationPersonnelCategory)
            .WithMany()
            .HasForeignKey(x => x.OrganizationPersonnelCategoryId)
            .OnDelete(DeleteBehavior.Restrict)  // یا SetNull — نه مسیر دوم cascade از Organization
            .IsRequired(false);

        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.OrganizationPersonnelCategoryId);
    }
}