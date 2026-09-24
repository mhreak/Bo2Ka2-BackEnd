// Domain/Entities/Organization/Organization.cs
using Bodokado.Domain.Common;
using Bodokado.Domain.Entities;

namespace Bodokado.Domain.Entities.Organizations;

public class Organization : BaseEntity
{
    public string OrganizationName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Guid? LogoFileId { get; set; }
    public FileAsset? LogoFile { get; set; }

    public List<UserOrganization> UserOrganizations { get; set; } = new();

    public List<OrganizationalGiftCampaign> GiftCampaigns { get; set; } = new();

    public List<OrganizationPersonnelCategory> PersonnelCategories { get; set; } = new();
}