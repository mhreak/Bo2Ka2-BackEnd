namespace Bodokado.Application.Common.Location.DTOs;

public class CountryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Iso2 { get; set; } = string.Empty;
    public string Iso3 { get; set; } = string.Empty;
    public string PhoneCode { get; set; } = string.Empty;
}

public class ProvinceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? StateCode { get; set; }
    public Guid CountryId { get; set; }
}

public class CityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ProvinceId { get; set; }
    public Guid CountryId { get; set; }
}
