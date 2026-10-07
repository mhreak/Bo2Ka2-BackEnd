using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.OrganizationAdminModule.GiftCampaigns.DTOs;

public class CreateGiftCampaignRequestDto
{
    public string CampaignName { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime FinishDateTime { get; set; }
    public OccasionType OccasionType { get; set; }

    public bool OrganizationalMessageEnabled { get; set; }
    public OrganizationalMessageType? MessageType { get; set; }
    public string? OrganizationalMessage { get; set; }
    public Guid? OrganizationalMessageFileId { get; set; }

    public List<CreateGiftCampaignConstraintDto> Constraints { get; set; } = new();
}

public class CreateGiftCampaignConstraintDto
{
    public GiftCampaignConstraintType ConstraintType { get; set; }
    public Guid? OrganizationPersonnelCategoryId { get; set; }
    public string Constraint { get; set; } = string.Empty;
}

public class UpdateGiftCampaignRequestDto
{
    public string CampaignName { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime FinishDateTime { get; set; }
    public OccasionType OccasionType { get; set; }

    public bool OrganizationalMessageEnabled { get; set; }
    public OrganizationalMessageType? MessageType { get; set; }
    public string? OrganizationalMessage { get; set; }
    public Guid? OrganizationalMessageFileId { get; set; }

    public List<CreateGiftCampaignConstraintDto> Constraints { get; set; } = new();
}

public class GiftCampaignListItemDto
{
    public Guid Id { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime FinishDateTime { get; set; }
    public OccasionType OccasionType { get; set; }
    public bool IsActiveByAdmin { get; set; }
    public int ConstraintCount { get; set; }
}

public class GiftCampaignDetailDto
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime FinishDateTime { get; set; }
    public OccasionType OccasionType { get; set; }

    public bool OrganizationalMessageEnabled { get; set; }
    public OrganizationalMessageType? MessageType { get; set; }
    public string? OrganizationalMessage { get; set; }
    public Guid? OrganizationalMessageFileId { get; set; }

    public bool IsActiveByAdmin { get; set; }

    public List<GiftCampaignConstraintDto> Constraints { get; set; } = new();
}

public class GiftCampaignConstraintDto
{
    public Guid Id { get; set; }
    public GiftCampaignConstraintType ConstraintType { get; set; }
    public Guid? OrganizationPersonnelCategoryId { get; set; }
    public string? OrganizationPersonnelCategoryName { get; set; }
    public string Constraint { get; set; } = string.Empty;
}

public static class CreateGiftCampaignConstraintDtoExtensions
{
    public static bool FinishBeforeStart(this CreateGiftCampaignConstraintDto c) => false; // placeholder، فعلاً استفاده نمی‌شود
}