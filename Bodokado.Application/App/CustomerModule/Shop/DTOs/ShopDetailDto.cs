using Bodokado.Application.App.ShopModule.Registration.DTOs;

namespace Bodokado.Application.App.CustomerModule.Shops.DTOs;

/// <summary>پروفایل عمومی فروشگاه که مشتری می‌بیند (بدون اطلاعات محرمانه مالک)</summary>
public class ShopDetailDto
{
    public Guid Id { get; set; }
    public string ShopName { get; set; } = string.Empty;

    public Guid? AvatarFileId { get; set; }
    public string? AvatarPath { get; set; }
    public Guid? CoverFileId { get; set; }
    public string? CoverPath { get; set; }

    public Guid ShopCategoryId { get; set; }
    public string ShopCategoryName { get; set; } = string.Empty;

    public string? TextAddress { get; set; }
    public Guid? CityId { get; set; }
    public string? CityName { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public string? ReturnPolicy { get; set; }

    public bool IsOpenNow { get; set; }
    public bool EnableStories { get; set; }
    public int ProductCount { get; set; }

    public List<ShopWorkingHourDto> WorkingHours { get; set; } = new();

    public DateTime CreatedAt { get; set; }
}
