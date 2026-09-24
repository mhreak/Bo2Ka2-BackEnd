// Domain/Entities/Organization/UserOrganizationalGiftCampaign.cs
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Domain.Entities.Organizations;

public class UserOrganizationalGiftCampaign
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid OrganizationalGiftCampaignId { get; set; }
    public OrganizationalGiftCampaign OrganizationalGiftCampaign { get; set; } = null!;

    /// <summary>کد هدیه ۱۰ کاراکتری — فقط هنگام ایجاد ست می‌شود</summary>
    public string GiftCode { get; set; } = string.Empty;
}