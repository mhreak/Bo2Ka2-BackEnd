// Persistence/Configurations/Organization/OrganizationConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Organizations;

namespace Bodokado.Persistence.Configurations.Organizations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organization");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrganizationName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.LogoFile)
            .WithMany()
            .HasForeignKey(x => x.LogoFileId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.OrganizationName);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.LogoFileId);
    }
}