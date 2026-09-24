// Persistence/Configurations/Organization/OrganizationalGiftCampaignConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Organizations;
using Bodokado.Domain.Enums;

namespace Bodokado.Persistence.Configurations.Organizations;

public class OrganizationalGiftCampaignConfiguration
    : IEntityTypeConfiguration<OrganizationalGiftCampaign>
{
    public void Configure(EntityTypeBuilder<OrganizationalGiftCampaign> builder)
    {
        builder.ToTable("OrganizationalGiftCampaign");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CampaignName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.StartDateTime).IsRequired();
        builder.Property(x => x.FinishDateTime).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.OccasionType)
            .IsRequired()
            .HasConversion<short>();

        builder.HasOne(x => x.Organization)
            .WithMany(o => o.GiftCampaigns)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(x => x.OrganizationalMessageEnabled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.MessageType)
            .HasConversion<short?>();

        builder.Property(x => x.OrganizationalMessage)
            .HasMaxLength(2000); // اگر در سند طول دقیق دیگری بود عوض کن

        builder.HasOne(x => x.OrganizationalMessageFile)
            .WithMany()
            .HasForeignKey(x => x.OrganizationalMessageFileId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.OrganizationId);
        builder.HasIndex(x => x.StartDateTime);
        builder.HasIndex(x => x.FinishDateTime);
        builder.HasIndex(x => x.OccasionType);

        builder.Property(x => x.IsActiveByAdmin)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(x => x.IsActiveByAdmin);
    }
}