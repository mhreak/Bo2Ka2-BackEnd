// Domain/Entities/Organization/UserOrganization.cs
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Domain.Entities.Organizations;

public class UserOrganization
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public Guid? OrganizationPersonnelCategoryId { get; set; }
    public OrganizationPersonnelCategory? OrganizationPersonnelCategory { get; set; }
}