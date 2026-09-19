// ProductProperty.cs
using Bodokado.Domain.Common;
using Bodokado.Domain.Enums;

namespace Bodokado.Domain.Entities.Products;

public class ProductProperty : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ProductPropertyType Type { get; set; } = ProductPropertyType.Text;

    /// <summary>ترتیب نمایش (OrderItemsBy)</summary>
    public int SortOrder { get; set; }

    public Guid? ProductCategoryId { get; set; }
    public ProductCategory? ProductCategory { get; set; }
}