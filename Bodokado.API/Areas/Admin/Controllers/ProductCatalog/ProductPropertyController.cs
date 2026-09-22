//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Bodokado.API.Constants;
//using Bodokado.API.Helpers;
//using Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;
//using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
//using Bodokado.Application.Common.Localization;

//namespace Bodokado.API.Areas.Admin.Controllers;

//[ApiController]
//[Route(ApiRoutes.Admin.ProductProperties)]
//[Authorize(Roles = "Admin")]
//[Tags("Admin Product Properties")]
//public class ProductPropertyController : ControllerBase
//{
//    private readonly IProductCatalogService _catalogService;
//    private readonly IResponseLocalizer _responseLocalizer;

//    public ProductPropertyController(IProductCatalogService catalogService, IResponseLocalizer responseLocalizer)
//    {
//        _catalogService = catalogService;
//        _responseLocalizer = responseLocalizer;
//    }

//    /// <summary>لیست ویژگی‌ها (اختیاری فیلتر بر اساس دسته‌بندی)</summary>
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

//    [HttpPost]
//    public async Task<IActionResult> Create([FromBody] CreateProductAttributeRequestDto request, CancellationToken ct)
//    {
//        var data = await _catalogService.CreatePropertyAsync(request, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductPropertyCreated);
//        return Ok(ApiResult.Success(data, message));
//    }

//    [HttpPut("{id:guid}")]
//    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductAttributeRequestDto request, CancellationToken ct)
//    {
//        var data = await _catalogService.UpdatePropertyAsync(id, request, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductPropertyUpdated);
//        return Ok(ApiResult.Success(data, message));
//    }

//    [HttpDelete("{id:guid}")]
//    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
//    {
//        await _catalogService.DeletePropertyAsync(id, ct);
//        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductPropertyDeleted);
//        return Ok(ApiResult.Success(message));
//    }
//}