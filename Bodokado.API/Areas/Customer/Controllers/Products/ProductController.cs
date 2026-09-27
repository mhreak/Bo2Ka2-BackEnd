using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.CustomerModule.Products.DTOs;
using Bodokado.Application.App.CustomerModule.Products.Interfaces;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Areas.Customer.Controllers;

[ApiController]
[Route(ApiRoutes.Customer.Products)]
[AllowAnonymous]
[Tags("Customer Products")]
public class ProductController : ControllerBase
{
    private readonly ICustomerProductService _productService;
    private readonly IResponseLocalizer _responseLocalizer;

    public ProductController(ICustomerProductService productService, IResponseLocalizer responseLocalizer)
    {
        _productService = productService;
        _responseLocalizer = responseLocalizer;
    }

    /// <summary>
    /// لیست محصولات برای اپ مشتری با فیلترهای مختلف:
    /// search, shopId, shopCategoryId, brand, minPrice, maxPrice,
    /// isSpecial, hasDiscount, inStockOnly, productType, sortBy, page, pageSize
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CustomerProductListQuery query, CancellationToken ct)
    {
        var result = await _productService.GetAllAsync(query, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductsRetrieved);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>جزئیات یک محصول (فقط محصولات منتشرشدهٔ فروشگاه‌های تأییدشده)</summary>
    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetById(Guid productId, CancellationToken ct)
    {
        var result = await _productService.GetByIdAsync(productId, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProductRetrieved);
        return Ok(ApiResult.Success(result, message));
    }
}
