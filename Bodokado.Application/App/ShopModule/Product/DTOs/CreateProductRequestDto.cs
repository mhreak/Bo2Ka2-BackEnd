namespace Bodokado.Application.App.ShopModule.Products.DTOs;

public class CreateProductRequestDto
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

    /// <summary>اگر true باشد محصول بلافاصله منتشر می‌شود</summary>
    public bool Publish { get; set; }

    /// <summary>شناسه فایل‌های آپلودشده (از قبل با File API آپلود شده‌اند)</summary>
    public List<Guid> ImageFileIds { get; set; } = new();

    public List<ProductColorDto> Colors { get; set; } = new();
}
