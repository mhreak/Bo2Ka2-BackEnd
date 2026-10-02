using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.ShopModule.Products.DTOs;
using Bodokado.Application.App.ShopModule.Products.Interfaces;
using Bodokado.Application.Common.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bodokado.API.Areas.Shop.Controllers;

[ApiController]
[Route(ApiRoutes.Shop.PluginProducts)] // api/v1/shop/plugin/products
[Authorize(Roles = "Shop")]
[Tags("Plugin Products")]
public class PluginProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IResponseLocalizer _localizer;

    public PluginProductController(
        IProductService productService,
        IResponseLocalizer localizer)
    {
        _productService = productService;
        _localizer = localizer;
    }

    private Guid GetUserId()
    {
        var raw =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(raw) || !Guid.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("User is not authenticated.");

        return userId;
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