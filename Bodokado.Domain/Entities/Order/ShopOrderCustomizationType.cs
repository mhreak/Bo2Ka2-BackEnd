using Bodokado.Domain.Entities.Shops;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bodokado.Domain.Entities.Order;

public class ShopOrderCustomizationType
{
    public Guid ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    public Guid OrderCustomizationTypeId { get; set; }
    public OrderCustomizationType OrderCustomizationType { get; set; } = null!;

    public bool IsActive { get; set; } = true;
}
