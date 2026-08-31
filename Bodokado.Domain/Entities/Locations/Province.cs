using Bodokado.Domain.Common;

namespace Bodokado.Domain.Entities.Locations;

public class Province : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? StateCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public Guid CountryId { get; set; }
    public Country Country { get; set; } = null!;
    public List<City> Cities { get; set; } = new();
}
