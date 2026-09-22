//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Bodokado.API.Constants;
//using Bodokado.API.Helpers;
//using Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;
//using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
//using Bodokado.Application.Common.Localization;

//namespace Bodokado.API.Areas.Admin.Controllers;

//[ApiController]
//[Route(ApiRoutes.Admin.ProductProductProperties)]
//[Authorize(Roles = "Admin")]
//[Tags("Admin Product Product-Properties")]
//public class ProductProductPropertyController : ControllerBase
//{
//    private readonly IProductCatalogService _catalogService;
//    private readonly IResponseLocalizer _responseLocalizer;

//    public ProductProductPropertyController(IProductCatalogService catalogService, IResponseLocalizer responseLocalizer)
//    {
//        _catalogService = catalogService;
//        _responseLocalizer = responseLocalizer;
//    }

//    [HttpGet("by-product/{productId:guid}")]
//    public async Task<IActionResult> GetByProduct(Guid productId, CancellationToken ct)
//    {
//        var data = await _catalogService.GetProductPropertiesAsync(productId, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductProductPropertiesRetrieved);
//        return Ok(ApiResult.Success(data, message));
//    }

//    [HttpGet("{id:guid}")]
//    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
//    {
//        var data = await _catalogService.GetProductPropertyByIdAsync(id, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductProductPropertyRetrieved);
//        return Ok(ApiResult.Success(data, message));
//    }

//    [HttpPost]
//    public async Task<IActionResult> Create([FromBody] CreateProductProductAttributeRequestDto request, CancellationToken ct)
//    {
//        var data = await _catalogService.CreateProductPropertyAsync(request, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductProductPropertyCreated);
//        return Ok(ApiResult.Success(data, message));
//    }

//    [HttpPut("{id:guid}")]
//    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductProductAttributeRequestDto request, CancellationToken ct)
//    {
//        var data = await _catalogService.UpdateProductPropertyAsync(id, request, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductProductPropertyUpdated);
//        return Ok(ApiResult.Success(data, message));
//    }

//    [HttpDelete("{id:guid}")]
//    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
//    {
//        await _catalogService.DeleteProductPropertyAsync(id, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductProductPropertyDeleted);
//        return Ok(ApiResult.Success(message));
//    }

//    [HttpPut("by-product/{productId:guid}/set")]
//    public async Task<IActionResult> SetForProduct(Guid productId, [FromBody] SetProductAttributesRequestDto request, CancellationToken ct)
//    {
//        var data = await _catalogService.SetProductPropertiesAsync(productId, request, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductProductPropertiesSet);
//        return Ok(ApiResult.Success(data, message));
//    }
//}