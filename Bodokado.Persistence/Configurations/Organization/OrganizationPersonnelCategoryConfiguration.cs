// Persistence/Configurations/Organization/OrganizationPersonnelCategoryConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Organizations;

namespace Bodokado.Persistence.Configurations.Organizations;

public class OrganizationPersonnelCategoryConfiguration
    : IEntityTypeConfiguration<OrganizationPersonnelCategory>
{
    public void Configure(EntityTypeBuilder<OrganizationPersonnelCategory> builder)
    {
        builder.ToTable("OrganizationPersonnelCategory");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CategoryName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Organization)
            .WithMany(o => o.PersonnelCategories)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasIndex(x => x.OrganizationId);
        builder.HasIndex(x => x.IsActive);
    }
}