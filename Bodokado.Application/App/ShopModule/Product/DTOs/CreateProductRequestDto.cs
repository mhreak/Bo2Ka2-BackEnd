using Bodokado.Domain.Enums;

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

    public Guid? MainImageFileId { get; set; }
    public List<Guid>? ImageFileIds { get; set; }  // فقط تصاویر فرعی
    public ProductType ProductType { get; set; } = ProductType.Simple;


}
