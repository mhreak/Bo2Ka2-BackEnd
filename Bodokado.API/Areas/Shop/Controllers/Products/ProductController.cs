using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.ShopModule.Products.DTOs;
using Bodokado.Application.App.ShopModule.Products.Interfaces;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Areas.Shop.Controllers;

[ApiController]
[Route(ApiRoutes.Shop.Products)]
[Authorize(Roles = "Shop")]
[Tags("Shop Products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IResponseLocalizer _responseLocalizer;

    public ProductController(IProductService productService, IResponseLocalizer responseLocalizer)
    {
        _productService = productService;
        _responseLocalizer = responseLocalizer;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>لیست محصولات فروشگاه من (جستجو + فیلتر همه/خاص/پرفروش)</summary>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] ProductListQuery query, CancellationToken ct)
    {
        var result = await _productService.GetMyProductsAsync(CurrentUserId, query, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductsRetrieved);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>جزئیات یک محصول</summary>
    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetById(Guid productId, CancellationToken ct)
    {
        var result = await _productService.GetByIdAsync(CurrentUserId, productId, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductRetrieved);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>افزودن محصول جدید</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequestDto request, CancellationToken ct)
    {
        var result = await _productService.CreateAsync(CurrentUserId, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductCreated);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>ویرایش محصول</summary>
    [HttpPut("{productId:guid}")]
    public async Task<IActionResult> Update(Guid productId, [FromBody] UpdateProductRequestDto request, CancellationToken ct)
    {
        var result = await _productService.UpdateAsync(CurrentUserId, productId, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductUpdated);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>حذف محصول (soft delete)</summary>
    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> Delete(Guid productId, CancellationToken ct)
    {
        await _productService.DeleteAsync(CurrentUserId, productId, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductDeleted);
        return Ok(ApiResult.Success(message));
    }
}
