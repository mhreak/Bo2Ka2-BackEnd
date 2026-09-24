// Domain/Entities/Wallet/WalletTransactionLog.cs
using Bodokado.Domain.Common;
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Domain.Entities.Wallet;

public class WalletTransactionLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    /// <summary>مبلغ؛ می‌تواند منفی باشد</summary>
    public long Amount { get; set; }

    public DateTime TransactionDateTime { get; set; }

    public string? Description { get; set; }
}