using Bodokado.Application.App.ShopModule.Products.DTOs;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.CustomerModule.Products.DTOs;

public class CustomerProductDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Brand { get; set; }

    public decimal? WeightGrams { get; set; }
    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }

    public decimal BasePrice { get; set; }
    public bool IsDiscountEnabled { get; set; }
    public decimal? DiscountPrice { get; set; }
    public decimal EffectivePrice { get; set; }
    public int? DiscountPercent { get; set; }

    public bool IsInStock { get; set; }
    public bool HasSpecialPackaging { get; set; }
    public bool IsSpecial { get; set; }
    public int SoldCount { get; set; }

    public ProductType ProductType { get; set; }
    public Guid? MainImageFileId { get; set; }
    public List<ProductImageDto> Images { get; set; } = new();

    public Guid ShopId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public string? ShopAvatarPath { get; set; }
    public bool ShopIsOpenNow { get; set; }

    public DateTime CreatedAt { get; set; }
}
