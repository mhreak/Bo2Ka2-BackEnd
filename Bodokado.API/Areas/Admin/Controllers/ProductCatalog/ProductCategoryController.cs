//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Bodokado.API.Constants;
//using Bodokado.API.Helpers;
//using Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;
//using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
//using Bodokado.Application.Common.Localization;

//namespace Bodokado.API.Areas.Admin.Controllers;

//[ApiController]
//[Route(ApiRoutes.Admin.ProductCategories)]
//[Authorize(Roles = "Admin")]
//[Tags("Admin Product Categories")]
//public class ProductCategoryController : ControllerBase
//{
//    private readonly IProductCatalogService _catalogService;
//    private readonly IResponseLocalizer _responseLocalizer;

//    public ProductCategoryController(IProductCatalogService catalogService, IResponseLocalizer responseLocalizer)
//    {
//        _catalogService = catalogService;
//        _responseLocalizer = responseLocalizer;
//    }

//    /// <summary>لیست دسته‌بندی محصولات (درختی یا تخت)</summary>
//    [HttpGet]
//    public async Task<IActionResult> GetList(
//        [FromQuery] bool onlyActive = false,
//        [FromQuery] bool asTree = true,
//        CancellationToken ct = default)
//    {
//        var data = await _catalogService.GetCategoriesAsync(onlyActive, asTree, ct);
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

//    [HttpPost]
//    public async Task<IActionResult> Create([FromBody] CreateProductCategoryRequestDto request, CancellationToken ct)
//    {
//        var data = await _catalogService.CreateCategoryAsync(request, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductCategoryCreated);
//        return Ok(ApiResult.Success(data, message));
//    }

//    [HttpPut("{id:guid}")]
//    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCategoryRequestDto request, CancellationToken ct)
//    {
//        var data = await _catalogService.UpdateCategoryAsync(id, request, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductCategoryUpdated);
//        return Ok(ApiResult.Success(data, message));
//    }

//    [HttpDelete("{id:guid}")]
//    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
//    {
//        await _catalogService.DeleteCategoryAsync(id, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductCategoryDeleted);
//        return Ok(ApiResult.Success(message));
//    }
//}