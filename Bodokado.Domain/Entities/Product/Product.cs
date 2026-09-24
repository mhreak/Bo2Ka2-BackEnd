using Bodokado.Domain.Common;
using Bodokado.Domain.Entities;
using Bodokado.Domain.Entities.Shops;
using Bodokado.Domain.Enums;

namespace Bodokado.Domain.Entities.Products;

public class Product : BaseEntity
{
    public Guid ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

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
    public int SoldCount { get; set; }

    public ProductStatus Status { get; set; } = ProductStatus.Draft;

    /// <summary>تصویر اصلی محصول (از API آپلود فایل)</summary>
    public Guid? MainImageFileId { get; set; }
    public FileAsset? MainImageFile { get; set; }


    public ProductType ProductType { get; set; } = ProductType.Simple;

    /// <summary>سایر تصاویر محصول</summary>
    public List<ShopProductImage> Images { get; set; } = new();
    public List<ShopProductVariation> Variations { get; set; } = new();


    public List<ProductProductAttribute> ProductAttributes { get; set; } = new();

    public bool IsActiveByAdmin { get; set; } = true;
}