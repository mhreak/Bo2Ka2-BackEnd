// Domain/Entities/Organization/OrganizationPersonnelCategory.cs
using Bodokado.Domain.Common;

namespace Bodokado.Domain.Entities.Organizations;

public class OrganizationPersonnelCategory : BaseEntity
{
    public string CategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
}