// Areas/Admin/Controllers/Settings/SettingController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.AdminModule.Settings.DTOs;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Administrator.Settings.Interfaces;

namespace Bodokado.API.Areas.Admin.Controllers;

[ApiController]
[Route(ApiRoutes.Admin.Settings)]
[Authorize(Roles = "Admin")]
[Tags("Admin Settings")]
public class SettingController : ControllerBase
{
    private readonly ISettingService _settingService;
    private readonly IResponseLocalizer _responseLocalizer;

    public SettingController(ISettingService settingService, IResponseLocalizer responseLocalizer)
    {
        _settingService = settingService;
        _responseLocalizer = responseLocalizer;
    }

    /// <summary>دریافت تنظیمات یک صفحه با key — مثلاً homepage، shop-list، product-detail</summary>
    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKey(string key, CancellationToken ct)
    {
        var data = await _settingService.GetByKeyAsync(key, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.SettingRetrieved);
        return Ok(ApiResult.Success(data, message));
    }

    /// <summary>ذخیره / به‌روزرسانی تنظیمات هر صفحه (Value = JSON)</summary>
    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] UpsertSettingRequestDto request, CancellationToken ct)
    {
        var data = await _settingService.UpsertAsync(request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.SettingSaved);
        return Ok(ApiResult.Success(data, message));
    }
}