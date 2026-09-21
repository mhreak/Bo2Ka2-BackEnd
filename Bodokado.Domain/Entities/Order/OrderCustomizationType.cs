using Bodokado.Domain.Common;
using Bodokado.Domain.Entities.Shops;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bodokado.Domain.Entities.Order;

public class OrderCustomizationType : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public short ShowOrder { get; set; }

    public Guid? ShopCategoryId { get; set; }
    public ShopCategory? ShopCategory { get; set; }

    public List<OrderCustomizationTypeOption> Options { get; set; } = new();
    public List<ShopOrderCustomizationType> ShopLinks { get; set; } = new();
}