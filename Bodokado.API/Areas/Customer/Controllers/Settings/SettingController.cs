// Areas/Customer/Controllers/Settings/SettingController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.AdminModule.Settings;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Administrator.Settings.Interfaces;

namespace Bodokado.API.Areas.Customer.Controllers;

[ApiController]
[Route(ApiRoutes.Customer.Settings)]
[AllowAnonymous]
[Tags("Settings")]
public class SettingController : ControllerBase
{
    private readonly ISettingService _settingService;
    private readonly IResponseLocalizer _responseLocalizer;

    public SettingController(ISettingService settingService, IResponseLocalizer responseLocalizer)
    {
        _settingService = settingService;
        _responseLocalizer = responseLocalizer;
    }

    /// <summary>تنظیمات هر صفحه — key مثلاً homepage، product-list، shop-detail</summary>
    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKey(string key, CancellationToken ct)
    {
        var data = await _settingService.GetByKeyAsync(key, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.SettingRetrieved);
        return Ok(ApiResult.Success(data, message));
    }
}