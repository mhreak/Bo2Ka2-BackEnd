using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Persistence.Configurations.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasOne(u => u.Admin)
            .WithOne()
            .HasForeignKey<User>(u => u.AdminId)
            .OnDelete(DeleteBehavior.SetNull);

    }
}

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.ToTable("Admins");
        builder.HasKey(a => a.Id);
    }
}
