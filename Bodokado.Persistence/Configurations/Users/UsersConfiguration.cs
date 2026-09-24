using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Users;
using Bodokado.Domain.Enums;

namespace Bodokado.Persistence.Configurations.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("AspNetUsers"); // اگر Identity پیش‌فرض را عوض نکرده‌ای

        // پروفایل
        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);
        builder.Property(u => u.ShamsiBirthDate).HasMaxLength(20);
        builder.Property(u => u.Mobile).HasMaxLength(20);

        builder.Property(u => u.Gender)
            .HasConversion<short?>();

        // کیف پول
        builder.Property(u => u.WalletCredit)
            .IsRequired()
            .HasDefaultValue(0L);

        // وضعیت
        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasQueryFilter(u => !u.IsDeleted);

        // ایندکس‌ها
        builder.HasIndex(u => u.Mobile);
        builder.HasIndex(u => u.IsActive);
        builder.HasIndex(u => u.IsDeleted);
    }
}