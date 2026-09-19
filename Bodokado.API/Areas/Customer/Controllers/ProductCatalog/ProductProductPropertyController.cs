using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Areas.Customer.Controllers;

[ApiController]
[Route(ApiRoutes.Customer.ProductProductProperties)]
[AllowAnonymous]
[Tags("Product Properties On Product")]
public class ProductProductPropertyController : ControllerBase
{
    private readonly IProductCatalogService _catalogService;
    private readonly IResponseLocalizer _responseLocalizer;

    public ProductProductPropertyController(IProductCatalogService catalogService, IResponseLocalizer responseLocalizer)
    {
        _catalogService = catalogService;
        _responseLocalizer = responseLocalizer;
    }

    [HttpGet("by-product/{productId:guid}")]
    public async Task<IActionResult> GetByProduct(Guid productId, CancellationToken ct)
    {
        var data = await _catalogService.GetProductPropertiesAsync(productId, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductProductPropertiesRetrieved);
        return Ok(ApiResult.Success(data, message));
    }
}