//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Bodokado.API.Constants;
//using Bodokado.API.Helpers;
//using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
//using Bodokado.Application.Common.Localization;

//namespace Bodokado.API.Areas.Corporate.Controllers;

//[ApiController]
//[Route(ApiRoutes.Corporate.ProductProperties)]
//[AllowAnonymous]
//[Tags("Product Properties")]
//public class ProductPropertyController : ControllerBase
//{
//    private readonly IProductCatalogService _catalogService;
//    private readonly IResponseLocalizer _responseLocalizer;

//    public ProductPropertyController(IProductCatalogService catalogService, IResponseLocalizer responseLocalizer)
//    {
//        _catalogService = catalogService;
//        _responseLocalizer = responseLocalizer;
//    }

//    [HttpGet]
//    public async Task<IActionResult> GetList([FromQuery] Guid? productCategoryId, CancellationToken ct)
//    {
//        var data = await _catalogService.GetPropertiesAsync(productCategoryId, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductPropertiesRetrieved);
//        return Ok(ApiResult.Success(data, message));
//    }

//    [HttpGet("{id:guid}")]
//    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
//    {
//        var data = await _catalogService.GetPropertyByIdAsync(id, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductPropertyRetrieved);
//        return Ok(ApiResult.Success(data, message));
//    }
//}