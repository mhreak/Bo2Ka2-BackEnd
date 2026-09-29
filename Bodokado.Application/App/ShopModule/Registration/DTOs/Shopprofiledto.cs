using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.ShopModule.Registration.DTOs;

public class ShopProfileDto
{
    public Guid Id { get; set; }

    // Step 1
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? NationalCode { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? ShopName { get; set; }
    public ShopCategoryDto? ShopCategory { get; set; }

    // Step 2
    public Guid? AvatarFileId { get; set; }
    public string? AvatarPath { get; set; }
    public Guid? CoverFileId { get; set; }
    public string? CoverPath { get; set; }
    public string? TextAddress { get; set; }

    public Guid? CityId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    // Step 3
    public string? ShabaNumber { get; set; }
    public string? ReturnPolicy { get; set; }
    public List<ShopWorkingHourDto> WorkingHours { get; set; } = new();

    // Status
    public ShopRegistrationStep CurrentStep { get; set; }
    public ShopVerificationStatus VerificationStatus { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? SubmittedAt { get; set; }
}

public class PluginTokenRequestDto
{
    public string ApiKey { get; set; } = string.Empty;
}

public class PluginTokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public Guid ShopId { get; set; }
    public string? ShopName { get; set; }
}

public class PluginUpsertProductRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsDiscountEnabled { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public string? Brand { get; set; }
    public bool Publish { get; set; } = true;

    /// <summary>شناسه محصول در وردپرس — برای به‌روزرسانی بعدی</summary>
    public string? ExternalId { get; set; }

    public Guid? MainImageFileId { get; set; }
    public List<Guid>? ImageFileIds { get; set; }
}