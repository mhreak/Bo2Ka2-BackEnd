

using Bodokado.Domain.Common;
using Bodokado.Domain.Entities.Shops;

namespace Bodokado.Domain.Entities.ShopRatingOptions;

public class ShopRatingOption : BaseEntity
{
    public string OptionName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public short ShowOrder { get; set; }

    public Guid? ShopCategoryId { get; set; }
    public ShopCategory? ShopCategory { get; set; }
}
