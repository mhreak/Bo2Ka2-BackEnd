using Bodokado.Domain.Common;
using Bodokado.Domain.Entities;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Domain.Entities.Shops;

public class ShopProductImage : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid FileId { get; set; }
    public FileAsset File { get; set; } = null!;

    public int SortOrder { get; set; }
}