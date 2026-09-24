// Domain/Entities/Delivery/DeliveryServiceProvider.cs
using Bodokado.Domain.Common;
using Bodokado.Domain.Entities;

namespace Bodokado.Domain.Entities.Delivery;

public class DeliveryServiceProvider : BaseEntity
{
    public string ServiceProviderName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public string? IncludedCityIds { get; set; }
    public string? IncludedProvinceIds { get; set; }
    public string? ExcludedCityIds { get; set; }
    public string? ExcludedProvinceIds { get; set; }

    public Guid? LogoFileId { get; set; }
    public FileAsset? LogoFile { get; set; }
}