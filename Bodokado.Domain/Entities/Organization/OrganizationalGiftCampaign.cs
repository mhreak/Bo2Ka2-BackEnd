// Domain/Entities/Organization/OrganizationalGiftCampaign.cs
using Bodokado.Domain.Common;
using Bodokado.Domain.Enums;

namespace Bodokado.Domain.Entities.Organizations;

public class OrganizationalGiftCampaign : BaseEntity
{
    public string CampaignName { get; set; } = string.Empty;

    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public DateTime StartDateTime { get; set; }
    public DateTime FinishDateTime { get; set; }

    public OccasionType OccasionType { get; set; }

    public List<OrganizationalGiftCampaignConstraint> Constraints { get; set; } = new();

    public bool OrganizationalMessageEnabled { get; set; }

    public OrganizationalMessageType? MessageType { get; set; }

    public string? OrganizationalMessage { get; set; }

    public Guid? OrganizationalMessageFileId { get; set; }
    public FileAsset? OrganizationalMessageFile { get; set; }

    public bool IsActiveByAdmin { get; set; } = true;
}