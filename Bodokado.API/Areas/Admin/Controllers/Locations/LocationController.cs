using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Location.Interfaces;

namespace Bodokado.API.Areas.Admin.Controllers;

/// <summary>کشور / استان / شهر</summary>
[ApiController]
[Route(ApiRoutes.Admin.Locations)]
[AllowAnonymous]
[Tags("Location")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly IResponseLocalizer _responseLocalizer;

    public LocationController(ILocationService locationService, IResponseLocalizer responseLocalizer)
    {
        _locationService = locationService;
        _responseLocalizer = responseLocalizer;
    }

    /// <summary>لیست کشورها</summary>
    [HttpGet("countries")]
    public async Task<IActionResult> GetCountries(CancellationToken ct)
    {
        var data = await _locationService.GetCountriesAsync(ct);
        return Ok(ApiResult.Success(data, await _responseLocalizer.LocalizeAsync(MessageKeys.Success)));
    }

    /// <summary>استان‌های یک کشور</summary>
    [HttpGet("countries/{countryId:guid}/provinces")]
    public async Task<IActionResult> GetProvinces(Guid countryId, CancellationToken ct)
    {
        var data = await _locationService.GetProvincesAsync(countryId, ct);
        return Ok(ApiResult.Success(data, await _responseLocalizer.LocalizeAsync(MessageKeys.Success)));
    }

    /// <summary>شهرهای یک استان</summary>
    [HttpGet("provinces/{provinceId:guid}/cities")]
    public async Task<IActionResult> GetCitiesByProvince(Guid provinceId, CancellationToken ct)
    {
        var data = await _locationService.GetCitiesByProvinceAsync(provinceId, ct);
        return Ok(ApiResult.Success(data, await _responseLocalizer.LocalizeAsync(MessageKeys.Success)));
    }

    /// <summary>همه شهرهای یک کشور</summary>
    [HttpGet("countries/{countryId:guid}/cities")]
    public async Task<IActionResult> GetCitiesByCountry(Guid countryId, CancellationToken ct)
    {
        var data = await _locationService.GetCitiesByCountryAsync(countryId, ct);
        return Ok(ApiResult.Success(data, await _responseLocalizer.LocalizeAsync(MessageKeys.Success)));
    }
}
