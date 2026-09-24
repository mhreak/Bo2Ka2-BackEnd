// Domain/Entities/Delivery/ShopOrderDeliveryRule.cs
using Bodokado.Domain.Common;
using Bodokado.Domain.Entities.Shops;

namespace Bodokado.Domain.Entities.Delivery;

public class ShopOrderDeliveryRule : BaseEntity
{
    public Guid ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    public Guid DeliveryServiceProviderId { get; set; }
    public DeliveryServiceProvider DeliveryServiceProvider { get; set; } = null!;

    public string? IncludedCityIds { get; set; }
    public string? IncludedProvinceIds { get; set; }
    public string? ExcludedCityIds { get; set; }
    public string? ExcludedProvinceIds { get; set; }

    public bool IsActive { get; set; } = true;
}