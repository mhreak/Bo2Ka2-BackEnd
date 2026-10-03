// Domain/Entities/Products/ProductProductCategory.cs
using Bodokado.Domain.Common;

namespace Bodokado.Domain.Entities.Products;

public class ProductProductCategory : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid ProductCategoryId { get; set; }
    public ProductCategory ProductCategory { get; set; } = null!;
}