using System.Text.Json.Serialization;

namespace Bodokado.Persistence.Seeders.Models;

public class CountryJson
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Iso2 { get; set; }
    public string? Iso3 { get; set; }

    [JsonPropertyName("phonecode")]
    public string? PhoneCode { get; set; }

    public string? Capital { get; set; }
    public string? Currency { get; set; }
    public string? Region { get; set; }
    public string? Subregion { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
}