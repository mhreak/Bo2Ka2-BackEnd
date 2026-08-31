using System.Text.Json.Serialization;

namespace Bodokado.Persistence.Seeders.Models;

public class StateJson
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("country_id")]
    public int CountryId { get; set; }

    [JsonPropertyName("state_code")]
    public string? StateCode { get; set; }

    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
}