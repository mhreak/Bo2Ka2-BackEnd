using Bodokado.Domain.Common;

namespace Bodokado.Domain.Entities.Locations;

public class Country : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Iso2 { get; set; } = string.Empty;
    public string Iso3 { get; set; } = string.Empty;
    public string PhoneCode { get; set; } = string.Empty;
    public string Capital { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string CurrencySymbol { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Subregion { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public List<Province> Provinces { get; set; } = new();
}
