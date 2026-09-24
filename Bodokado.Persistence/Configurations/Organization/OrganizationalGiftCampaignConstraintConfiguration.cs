// Persistence/Configurations/Organization/OrganizationalGiftCampaignConstraintConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Organizations;
using Bodokado.Domain.Enums;

namespace Bodokado.Persistence.Configurations.Organizations;

public class OrganizationalGiftCampaignConstraintConfiguration
    : IEntityTypeConfiguration<OrganizationalGiftCampaignConstraint>
{
    public void Configure(EntityTypeBuilder<OrganizationalGiftCampaignConstraint> builder)
    {
        builder.ToTable("OrganizationalGiftCampaignConstraint");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Constraint).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.ConstraintType)
            .IsRequired()
            .HasConversion<short>();

        builder.HasOne(x => x.OrganizationalGiftCampaign)
            .WithMany(c => c.Constraints)
            .HasForeignKey(x => x.OrganizationalGiftCampaignId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.OrganizationPersonnelCategory)
            .WithMany()
            .HasForeignKey(x => x.OrganizationPersonnelCategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasIndex(x => x.OrganizationalGiftCampaignId);
        builder.HasIndex(x => x.OrganizationPersonnelCategoryId);
        builder.HasIndex(x => x.ConstraintType);
    }
}