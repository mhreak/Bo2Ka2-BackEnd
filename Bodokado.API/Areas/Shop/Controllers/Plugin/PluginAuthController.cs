// Areas/Plugin/Controllers/PluginAuthController.cs
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.ShopModule.Registration.DTOs;
using Bodokado.Application.Common.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Bodokado.Application.App.ShopModule.Registration.Services.ShopRegistrationService;


namespace Bodokado.API.Areas.Shop.Controllers;

[ApiController]
[Route(ApiRoutes.Shop.Plugin)]
[AllowAnonymous]
[Tags("Plugin Auth")]
public class PluginAuthController : ControllerBase
{
    private readonly IPluginAuthService _pluginAuthService;
    private readonly IResponseLocalizer _localizer;

    public PluginAuthController(IPluginAuthService pluginAuthService, IResponseLocalizer localizer)
    {
        _pluginAuthService = pluginAuthService;
        _localizer = localizer;
    }

    /// <summary>افزونه وردپرس با ApiKey توکن می‌گیرد</summary>
    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] PluginTokenRequestDto request, CancellationToken ct)
    {
        var data = await _pluginAuthService.IssueTokenAsync(request, ct);
        var message = await _localizer.LocalizeAsync(MessageKeys.PluginTokenIssued);
        return Ok(ApiResult.Success(data, message));
    }
}