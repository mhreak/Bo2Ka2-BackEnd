// API/Areas/Customer/Controllers/ProductCatalog/ProductCategoryController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Areas.Customer.Controllers;

[ApiController]
[Route(ApiRoutes.Customer.ProductCategories)]
[AllowAnonymous]
[Tags("Product Categories")]
public class ProductCategoryController : ControllerBase
{
    private readonly IProductCatalogService _catalogService;
    private readonly IResponseLocalizer _responseLocalizer;

    public ProductCategoryController(
        IProductCatalogService catalogService,
        IResponseLocalizer responseLocalizer)
    {
        _catalogService = catalogService;
        _responseLocalizer = responseLocalizer;
    }

    /// <summary>
    /// لیست دسته‌بندی‌های فعال برای اپ
    /// asTree=true → درختی | asTree=false → تخت
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool asTree = true,
        CancellationToken ct = default)
    {
        // فقط فعال‌ها برای مشتری
        var data = await _catalogService.GetCategoriesAsync(
            onlyActive: true,
            asTree: asTree,
            ct: ct);

        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductCategoriesRetrieved);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var data = await _catalogService.GetCategoryByIdAsync(id, ct);

        if (!data.IsActive)
            return NotFound(ApiResult.Error(await _responseLocalizer.LocalizeAsync(MessageKeys.ProductCategoryNotFound)));

        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductCategoryRetrieved);
        return Ok(ApiResult.Success(data, message));
    }
}