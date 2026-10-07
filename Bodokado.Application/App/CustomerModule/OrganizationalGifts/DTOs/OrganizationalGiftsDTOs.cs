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

    public List<AllowedShopDto> AllowedShops { get; set; } = new();
    public List<AllowedProductDto> AllowedProducts { get; set; } = new();
    public List<AllowedCategoryDto> AllowedCategories { get; set; } = new();
}

public class OrgGiftMessageDto
{
    public bool Enabled { get; set; }
    public OrganizationalMessageType? Type { get; set; }
    public string? Text { get; set; }
    public Guid? FileId { get; set; }
    public string? FilePath { get; set; }
}

public class AllowedShopDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoPath { get; set; }
    public string? Address { get; set; }
}

public class AllowedProductDto
{
    public Guid Id { get; set; }
    public Guid ShopId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? DiscountPrice { get; set; }
    public decimal EffectivePrice { get; set; }
    public string? MainImagePath { get; set; }
    public bool IsInStock { get; set; }
}

public class AllowedCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public Guid? ParentCategoryId { get; set; }
}