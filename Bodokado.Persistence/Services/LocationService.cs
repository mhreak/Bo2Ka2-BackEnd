using Microsoft.EntityFrameworkCore;
using Bodokado.Application.Common.Location.DTOs;
using Bodokado.Application.Common.Location.Interfaces;
using Bodokado.Persistence.Context;

namespace Bodokado.Persistence.Services;

public class LocationService : ILocationService
{
    private readonly AppDbContext _db;

    public LocationService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<CountryDto>> GetCountriesAsync(CancellationToken ct = default)
    {
        return await _db.Countries.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Name,
                Iso2 = c.Iso2,
                Iso3 = c.Iso3,
                PhoneCode = c.PhoneCode
            })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ProvinceDto>> GetProvincesAsync(Guid countryId, CancellationToken ct = default)
    {
        return await _db.Provinces.AsNoTracking()
            .Where(p => p.CountryId == countryId)
            .OrderBy(p => p.Name)
            .Select(p => new ProvinceDto
            {
                Id = p.Id,
                Name = p.Name,
                StateCode = p.StateCode,
                CountryId = p.CountryId
            })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CityDto>> GetCitiesByProvinceAsync(Guid provinceId, CancellationToken ct = default)
    {
        return await _db.Cities.AsNoTracking()
            .Where(c => c.ProvinceId == provinceId)
            .OrderBy(c => c.Name)
            .Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name,
                ProvinceId = c.ProvinceId,
                CountryId = c.CountryId
            })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CityDto>> GetCitiesByCountryAsync(Guid countryId, CancellationToken ct = default)
    {
        return await _db.Cities.AsNoTracking()
            .Where(c => c.CountryId == countryId)
            .OrderBy(c => c.Name)
            .Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name,
                ProvinceId = c.ProvinceId,
                CountryId = c.CountryId
            })
            .ToListAsync(ct);
    }
}
