// Areas/Plugin/Controllers/PluginAuthController.cs
using System.Security.Claims;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.ShopModule.Products.DTOs;
using Bodokado.Application.App.ShopModule.Products.Interfaces;
using Bodokado.Application.App.ShopModule.Registration.DTOs;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Enums;
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
    private readonly IProductService _productService;
    private readonly IResponseLocalizer _localizer;

    public PluginAuthController( IProductService productService, IPluginAuthService pluginAuthService, IResponseLocalizer localizer)
    {
        _pluginAuthService = pluginAuthService;
        _productService = productService;
        _localizer = localizer;
    }

    private Guid GetUserId()
        => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>افزونه وردپرس با ApiKey توکن می‌گیرد</summary>
    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] PluginTokenRequestDto request, CancellationToken ct)
    {
        var data = await _pluginAuthService.IssueTokenAsync(request, ct);
        var message = await _localizer.LocalizeAsync(MessageKeys.PluginTokenIssued);
        return Ok(ApiResult.Success(data, message));
    }
    [HttpPost]
    public async Task<IActionResult> CreateBatch(
        [FromBody] PluginCreateProductsBatchRequestDto request,
        CancellationToken ct)
    {
        var data = await _productService.CreateFromPluginBatchAsync(GetUserId(), request, ct);
        var message = await _localizer.LocalizeAsync(MessageKeys.ProductCreated);
        return Ok(ApiResult.Success(data, message));
    }
}