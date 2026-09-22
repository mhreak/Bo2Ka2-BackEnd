//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Bodokado.API.Constants;
//using Bodokado.API.Helpers;
//using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
//using Bodokado.Application.Common.Localization;

//namespace Bodokado.API.Areas.Corporate.Controllers;

//[ApiController]
//[Route(ApiRoutes.Corporate.ProductCategories)]
//[AllowAnonymous]
//[Tags("Product Categories")]
//public class ProductCategoryController : ControllerBase
//{
//    private readonly IProductCatalogService _catalogService;
//    private readonly IResponseLocalizer _responseLocalizer;

//    public ProductCategoryController(IProductCatalogService catalogService, IResponseLocalizer responseLocalizer)
//    {
//        _catalogService = catalogService;
//        _responseLocalizer = responseLocalizer;
//    }

//    [HttpGet]
//    public async Task<IActionResult> GetList([FromQuery] bool asTree = true, CancellationToken ct = default)
//    {
//        var data = await _catalogService.GetCategoriesAsync(onlyActive: true, asTree: asTree, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductCategoriesRetrieved);
//        return Ok(ApiResult.Success(data, message));
//    }

//    [HttpGet("{id:guid}")]
//    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
//    {
//        var data = await _catalogService.GetCategoryByIdAsync(id, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductCategoryRetrieved);
//        return Ok(ApiResult.Success(data, message));
//    }
//}