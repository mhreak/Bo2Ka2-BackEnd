namespace Bodokado.Application.App.CustomerModule.Shops.DTOs;

public class ShopListItemDto
{
    public Guid Id { get; set; }
    public string ShopName { get; set; } = string.Empty;

    public Guid? AvatarFileId { get; set; }
    public string? AvatarPath { get; set; }
    public Guid? CoverFileId { get; set; }
    public string? CoverPath { get; set; }

    public Guid ShopCategoryId { get; set; }
    public string ShopCategoryName { get; set; } = string.Empty;

    public Guid? CityId { get; set; }
    public string? CityName { get; set; }
    public string? TextAddress { get; set; }

    public bool IsOpenNow { get; set; }
    public bool EnableStories { get; set; }
    public int ProductCount { get; set; }

    public DateTime CreatedAt { get; set; }
    public bool IsNew { get; set; }
}
