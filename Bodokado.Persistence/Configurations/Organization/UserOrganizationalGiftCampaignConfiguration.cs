// Persistence/Configurations/Organization/UserOrganizationalGiftCampaignConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Organizations;

namespace Bodokado.Persistence.Configurations.Organizations;

public class UserOrganizationalGiftCampaignConfiguration
    : IEntityTypeConfiguration<UserOrganizationalGiftCampaign>
{
    public void Configure(EntityTypeBuilder<UserOrganizationalGiftCampaign> builder)
    {
        builder.ToTable("User_OrganizationalGiftCampaign");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.GiftCode)
            .IsRequired()
            .HasMaxLength(10)
            .IsFixedLength(); // اختیاری؛ اگر دقیقاً همیشه ۱۰ کاراکتر است

        builder.Property(x => x.UserId)
            .IsRequired(false);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.OrganizationalGiftCampaign)
            .WithMany()
            .HasForeignKey(x => x.OrganizationalGiftCampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.GiftCode).IsUnique();
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.OrganizationalGiftCampaignId);
    }
}