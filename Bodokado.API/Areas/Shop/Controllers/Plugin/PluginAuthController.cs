using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.ShopModule.Registration.DTOs; // یا هر جایی که PluginTokenRequestDto هست
using Bodokado.Application.Common.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Bodokado.Application.App.ShopModule.Registration.Services.ShopRegistrationService;

namespace Bodokado.API.Areas.Shop.Controllers;

[ApiController]
[Route(ApiRoutes.Shop.PluginAuth)] // api/v1/shop/plugin/auth
[Tags("Plugin Auth")]
public class PluginAuthController : ControllerBase
{
    private readonly IPluginAuthService _pluginAuthService;
    private readonly IResponseLocalizer _localizer;

    public PluginAuthController(
        IPluginAuthService pluginAuthService,
        IResponseLocalizer localizer)
    {
        _pluginAuthService = pluginAuthService;
        _localizer = localizer;
    }

    /// <summary>افزونه با ApiKey توکن می‌گیرد</summary>
    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<IActionResult> Token(
        [FromBody] PluginTokenRequestDto request,
        CancellationToken ct)
    {
        var data = await _pluginAuthService.IssueTokenAsync(request, ct);
        var message = await _localizer.LocalizeAsync(MessageKeys.PluginTokenIssued);
        return Ok(ApiResult.Success(data, message));
    }
}