using Microsoft.AspNetCore.Identity;

namespace Bodokado.Domain.Entities.Users;

public class User : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public Guid? AdminId { get; set; }
    public Admin? Admin { get; set; }


}
