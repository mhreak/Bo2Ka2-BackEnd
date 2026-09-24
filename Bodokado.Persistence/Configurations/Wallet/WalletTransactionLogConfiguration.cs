// Persistence/Configurations/Wallet/WalletTransactionLogConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Wallet;

namespace Bodokado.Persistence.Configurations.Wallet;

public class WalletTransactionLogConfiguration : IEntityTypeConfiguration<WalletTransactionLog>
{
    public void Configure(EntityTypeBuilder<WalletTransactionLog> builder)
    {
        builder.ToTable("WalletTransactionLog");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount).IsRequired();
        builder.Property(x => x.TransactionDateTime).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(50);
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.TransactionDateTime);
    }
}