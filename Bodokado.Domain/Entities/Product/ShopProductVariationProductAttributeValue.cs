// Domain/Entities/Product/ShopProductVariationProductAttributeValue.cs
namespace Bodokado.Domain.Entities.Products;

public class ShopProductVariationProductAttributeValue
{
    public Guid ShopProductVariationId { get; set; }
    public ShopProductVariation ShopProductVariation { get; set; } = null!;

    public Guid ProductAttributeValueId { get; set; }
    public ProductAttributeValue ProductAttributeValue { get; set; } = null!;
}