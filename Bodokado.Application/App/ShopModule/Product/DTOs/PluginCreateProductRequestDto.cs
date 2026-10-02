// Application/.../Plugin/DTOs/PluginCreateProductRequestDto.cs
namespace Bodokado.Application.App.Plugin.DTOs;

public class PluginCreateProductRequestDto
{
    /// <summary>شناسه محصول در وردپرس (برای جلوگیری از تکرار بعدی)</summary>
    public string? ExternalId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Brand { get; set; }

    public decimal BasePrice { get; set; }
    public bool IsDiscountEnabled { get; set; }
    public decimal? DiscountPrice { get; set; }

    public int StockQuantity { get; set; }
    public bool HasSpecialPackaging { get; set; }
    public bool IsSpecial { get; set; }

    /// <summary>true = Published ، false = Draft</summary>
    public bool Publish { get; set; } = true;

    public decimal? WeightGrams { get; set; }
    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }

    public string? MainImageUrl { get; set; }

    /// <summary>آدرس سایر تصاویر</summary>
    public List<string>? ImageUrls { get; set; }
}