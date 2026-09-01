using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.ShopModule.Products.DTOs;

public class ProductDetailDto
{
    public Guid Id { get; set; }
    public Guid ShopId { get; set; }
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
    public decimal EffectivePrice { get; set; }

    public int StockQuantity { get; set; }
    public bool IsInStock { get; set; }
    public bool HasSpecialPackaging { get; set; }
    public bool IsSpecial { get; set; }
    public int SoldCount { get; set; }
    public ProductStatus Status { get; set; }

    public List<ProductImageDto> Images { get; set; } = new();
    public List<ProductColorDto> Colors { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
