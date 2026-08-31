using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Bodokado.Domain.Entities.Locations;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Seeders.Models;

namespace Bodokado.Persistence.Seeders;

public static class LocationSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Countries.AnyAsync()
            && await context.Provinces.AnyAsync()
            && await context.Cities.AnyAsync())
            return;

        var dataDir = Path.Combine(AppContext.BaseDirectory, "Seeders", "Data");

        var countriesFilePath = Path.Combine(dataDir, "countries.json");
        var statesFilePath = Path.Combine(dataDir, "states.json");
        var citiesFilePath = Path.Combine(dataDir, "cities.json");

        Console.WriteLine($"[LocationSeeder] خوندن فایل‌ها از: {dataDir}");

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var countriesJson = JsonSerializer.Deserialize<List<CountryJson>>(
            await File.ReadAllTextAsync(countriesFilePath), jsonOptions)!;

        var statesJson = JsonSerializer.Deserialize<List<StateJson>>(
            await File.ReadAllTextAsync(statesFilePath), jsonOptions)!;

        var citiesJson = JsonSerializer.Deserialize<List<CityJson>>(
            await File.ReadAllTextAsync(citiesFilePath), jsonOptions)!;

        Console.WriteLine($"[LocationSeeder] خونده شد -> Countries: {countriesJson.Count}, States: {statesJson.Count}, Cities: {citiesJson.Count}");

        // نگاشت int (منبع JSON) -> Guid قطعی
        var countryIdMap = countriesJson
            .Select(c => c.Id)
            .Distinct()
            .ToDictionary(id => id, id => DeterministicGuid.Create($"Country_{id}"));

        var provinceIdMap = statesJson
            .Select(s => s.Id)
            .Distinct()
            .ToDictionary(id => id, id => DeterministicGuid.Create($"Province_{id}"));

        var countries = countriesJson
            .GroupBy(c => c.Id)
            .Select(g =>
            {
                var c = g.First();
                return new Country
                {
                    Id = countryIdMap[c.Id],
                    Name = c.Name,
                    Iso2 = c.Iso2 ?? string.Empty,
                    Iso3 = c.Iso3 ?? string.Empty,
                    PhoneCode = c.PhoneCode ?? string.Empty,
                    Capital = c.Capital ?? string.Empty,
                    Currency = c.Currency ?? string.Empty,
                    Region = c.Region ?? string.Empty,
                    Subregion = c.Subregion ?? string.Empty,
                    Latitude = ParseDecimal(c.Latitude),
                    Longitude = ParseDecimal(c.Longitude)
                };
            })
            .ToList();

        var validCountryOriginalIds = countriesJson.Select(c => c.Id).ToHashSet();

        var allProvinces = statesJson
            .GroupBy(s => s.Id)
            .Select(g =>
            {
                var s = g.First();
                return new
                {
                    Entity = new Province
                    {
                        Id = provinceIdMap[s.Id],
                        Name = s.Name,
                        StateCode = s.StateCode,
                        Latitude = ParseDecimal(s.Latitude),
                        Longitude = ParseDecimal(s.Longitude)
                    },
                    OriginalCountryId = s.CountryId
                };
            })
            .ToList();

        // فقط استان‌هایی که CountryId معتبر دارن نگه داشته می‌شن
        var provinces = new List<Province>();
        var skippedProvinces = 0;
        foreach (var p in allProvinces)
        {
            if (validCountryOriginalIds.Contains(p.OriginalCountryId))
            {
                p.Entity.CountryId = countryIdMap[p.OriginalCountryId];
                provinces.Add(p.Entity);
            }
            else
            {
                skippedProvinces++;
            }
        }

        if (skippedProvinces > 0)
            Console.WriteLine($"[LocationSeeder] هشدار: {skippedProvinces} استان به‌خاطر CountryId نامعتبر رد شد.");

        var validProvinceOriginalIds = statesJson.Select(s => s.Id).ToHashSet();

        var allCities = citiesJson
            .GroupBy(c => c.Id)
            .Select(g =>
            {
                var c = g.First();
                return new
                {
                    Entity = new City
                    {
                        Id = DeterministicGuid.Create($"City_{c.Id}"),
                        Name = c.Name,
                        Latitude = ParseDecimal(c.Latitude),
                        Longitude = ParseDecimal(c.Longitude)
                    },
                    OriginalProvinceId = c.StateId,
                    OriginalCountryId = c.CountryId
                };
            })
            .ToList();

        var cities = new List<City>();
        var skippedCities = 0;
        foreach (var c in allCities)
        {
            if (validCountryOriginalIds.Contains(c.OriginalCountryId) &&
                validProvinceOriginalIds.Contains(c.OriginalProvinceId))
            {
                c.Entity.ProvinceId = provinceIdMap[c.OriginalProvinceId];
                c.Entity.CountryId = countryIdMap[c.OriginalCountryId];
                cities.Add(c.Entity);
            }
            else
            {
                skippedCities++;
            }
        }

        if (skippedCities > 0)
            Console.WriteLine($"[LocationSeeder] هشدار: {skippedCities} شهر به‌خاطر CountryId/ProvinceId نامعتبر رد شد.");

        Console.WriteLine($"[LocationSeeder] آماده‌ی Insert -> Countries: {countries.Count}, Provinces: {provinces.Count}, Cities: {cities.Count}");

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            if (await context.Cities.AnyAsync())
            {
                await context.Cities.ExecuteDeleteAsync();
                Console.WriteLine("[LocationSeeder] Cities قبلی پاک شد.");
            }

            if (await context.Provinces.AnyAsync())
            {
                await context.Provinces.ExecuteDeleteAsync();
                Console.WriteLine("[LocationSeeder] Provinces قبلی پاک شد.");
            }

            if (await context.Countries.AnyAsync())
            {
                await context.Countries.ExecuteDeleteAsync();
                Console.WriteLine("[LocationSeeder] Countries قبلی پاک شد.");
            }

            await BulkInsertAsync(context, countries);
            await BulkInsertAsync(context, provinces);
            await BulkInsertAsync(context, cities);

            await transaction.CommitAsync();
            Console.WriteLine("[LocationSeeder] Seed با موفقیت تمام شد.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"[LocationSeeder] خطا در Seed: {ex.Message}");
            throw;
        }
    }

    private static async Task BulkInsertAsync<T>(AppDbContext context, List<T> entities) where T : class
    {
        const int batchSize = 2000;
        context.ChangeTracker.AutoDetectChangesEnabled = false;
        try
        {
            for (var i = 0; i < entities.Count; i += batchSize)
            {
                var batch = entities.Skip(i).Take(batchSize);
                await context.Set<T>().AddRangeAsync(batch);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
            }
        }
        finally
        {
            context.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }

    private static decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }
}