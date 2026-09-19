using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Areas.Corporate.Controllers;

[ApiController]
[Route(ApiRoutes.Corporate.ProductPropertyValues)]
[AllowAnonymous]
[Tags("Product Property Values")]
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
    public async Task<IActionResult> GetList([FromQuery] Guid? productPropertyId, CancellationToken ct = default)
    {
        var data = await _catalogService.GetPropertyValuesAsync(productPropertyId, onlyActive: true, ct);
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
}