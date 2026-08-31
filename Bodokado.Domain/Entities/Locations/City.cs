using Bodokado.Domain.Common;

namespace Bodokado.Domain.Entities.Locations;

public class City : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public Guid ProvinceId { get; set; }
    public Province Province { get; set; } = null!;
    public Guid CountryId { get; set; }
    public Country Country { get; set; } = null!;
}
