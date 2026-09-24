// Domain/Entities/Organization/OrganizationalGiftCampaignConstraint.cs
using Bodokado.Domain.Common;
using Bodokado.Domain.Enums;

namespace Bodokado.Domain.Entities.Organizations;

public class OrganizationalGiftCampaignConstraint : BaseEntity
{
    public Guid OrganizationalGiftCampaignId { get; set; }
    public OrganizationalGiftCampaign OrganizationalGiftCampaign { get; set; } = null!;

    public Guid? OrganizationPersonnelCategoryId { get; set; }
    public OrganizationPersonnelCategory? OrganizationPersonnelCategory { get; set; }

    public GiftCampaignConstraintType ConstraintType { get; set; }

    /// <summary>
    /// مقدار محدودیت (مثلاً مبلغ، یا لیست Idها به‌صورت رشته)
    /// </summary>
    public string Constraint { get; set; } = string.Empty;
}