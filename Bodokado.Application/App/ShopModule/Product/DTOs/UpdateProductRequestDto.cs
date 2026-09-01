namespace Bodokado.Application.App.ShopModule.Products.DTOs;

public class UpdateProductRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal? WeightGrams { get; set; }
    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }

    public string? Brand { get; set; }

    public decimal BasePrice { get; set; }
    public bool IsDiscountEnabled { get; set; }
    public decimal? DiscountPrice { get; set; }

    public int StockQuantity { get; set; }
    public bool HasSpecialPackaging { get; set; }
    public bool IsSpecial { get; set; }

    public bool Publish { get; set; }

    /// <summary>لیست کامل شناسه فایل‌های تصویر (ترتیب = SortOrder). تصاویر قبلی که در لیست نباشند حذف می‌شوند.</summary>
    public List<Guid> ImageFileIds { get; set; } = new();

    public List<ProductColorDto> Colors { get; set; } = new();
}
