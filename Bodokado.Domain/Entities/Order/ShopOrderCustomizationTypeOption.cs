using Bodokado.Domain.Entities.Shops;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bodokado.Domain.Entities.Order;

public class ShopOrderCustomizationTypeOption
{
    public Guid ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    public Guid OrderCustomizationTypeOptionId { get; set; }
    public OrderCustomizationTypeOption OrderCustomizationTypeOption { get; set; } = null!;
}
