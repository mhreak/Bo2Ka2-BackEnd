using Bodokado.Domain.Common;

namespace Bodokado.Domain.Entities.Products;

public class ShopProductVariation : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>نام/کد واریانت (اختیاری، حداکثر ۱۰۰)</summary>
    public string? Name { get; set; }

    public List<ShopProductVariationProductAttributeValue> AttributeValues { get; set; } = new();
}