using Bodokado.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace Bodokado.Domain.Entities.Users;

public class User : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime? BirthDate { get; set; }
    public string? ShamsiBirthDate { get; set; }
    public string? Mobile { get; set; }
    public Gender? Gender { get; set; }

    /// <summary>موجودی کیف پول</summary>
    public long WalletCredit { get; set; }



    public Bodokado.Domain.Entities.Shops.Shop? Shop { get; set; }
}
