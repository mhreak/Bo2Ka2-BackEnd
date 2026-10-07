// API/Areas/Customer/Controllers/Banners/BannerController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.Administrator.Banners.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Enums;

namespace Bodokado.API.Areas.Customer.Controllers;

[ApiController]
[Route(ApiRoutes.Customer.Banners)]
[AllowAnonymous]
[Tags("Banners")]
public class BannerController : ControllerBase
{
    private readonly IBannerService _bannerService;
    private readonly IResponseLocalizer _responseLocalizer;

    public BannerController(IBannerService bannerService, IResponseLocalizer responseLocalizer)
    {
        _bannerService = bannerService;
        _responseLocalizer = responseLocalizer;
    }

    /// <summary>بنرهای فعال برای homepage / اپ</summary>
    [HttpGet]
    public async Task<IActionResult> GetActive([FromQuery] BannerShowPlace? showPlace,CancellationToken ct)
    {
        var data = await _bannerService.GetActiveAsync(showPlace,ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.BannersRetrieved);
        return Ok(ApiResult.Success(data, message));
    }
}