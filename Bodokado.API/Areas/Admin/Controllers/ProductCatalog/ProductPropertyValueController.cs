using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;
using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Areas.Admin.Controllers;

[ApiController]
[Route(ApiRoutes.Admin.ProductPropertyValues)]
[Authorize(Roles = "Admin")]
[Tags("Admin Product Property Values")]
public class ProductPropertyValueController : ControllerBase
{
    private readonly IProductCatalogService _catalogService;
    private readonly IResponseLocalizer _responseLocalizer;

    public ProductPropertyValueController(IProductCatalogService catalogService, IResponseLocalizer responseLocalizer)
    {
        _catalogService = catalogService;
        _responseLocalizer = responseLocalizer;
    }

    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] Guid? productPropertyId,
        [FromQuery] bool onlyActive = false,
        CancellationToken ct = default)
    {
        var data = await _catalogService.GetPropertyValuesAsync(productPropertyId, onlyActive, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductPropertyValuesRetrieved);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var data = await _catalogService.GetPropertyValueByIdAsync(id, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductPropertyValueRetrieved);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductPropertyValueRequestDto request, CancellationToken ct)
    {
        var data = await _catalogService.CreatePropertyValueAsync(request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductPropertyValueCreated);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductPropertyValueRequestDto request, CancellationToken ct)
    {
        var data = await _catalogService.UpdatePropertyValueAsync(id, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductPropertyValueUpdated);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _catalogService.DeletePropertyValueAsync(id, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductPropertyValueDeleted);
        return Ok(ApiResult.Success(message));
    }
}