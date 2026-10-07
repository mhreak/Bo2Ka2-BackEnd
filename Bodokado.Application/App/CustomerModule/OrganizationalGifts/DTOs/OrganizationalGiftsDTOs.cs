using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.CustomerModule.OrganizationalGifts.DTOs;

public class VerifyGiftCodeRequestDto
{
    public string Code { get; set; } = string.Empty;
}

public class VerifyGiftCodeResponseDto
{
    public Guid CampaignId { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public string? OrganizationLogoPath { get; set; }
    public OccasionType OccasionType { get; set; }

    public string GiftCode { get; set; } = string.Empty;
    public long MaxBudget { get; set; }
    public long RemainingBudget { get; set; }

    public OrgGiftMessageDto? Message { get; set; }

    public List<Guid> AllowedShopIds { get; set; } = new();
    public List<Guid> AllowedProductIds { get; set; } = new();
    public List<Guid> AllowedCategoryIds { get; set; } = new();
}

public class OrgGiftMessageDto
{
    public bool Enabled { get; set; }
    public OrganizationalMessageType? Type { get; set; }
    public string? Text { get; set; }
    public Guid? FileId { get; set; }
    public string? FilePath { get; set; }
}