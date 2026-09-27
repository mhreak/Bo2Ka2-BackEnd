using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.CustomerModule.Shops.DTOs;
using Bodokado.Application.App.CustomerModule.Shops.Interfaces;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Areas.Customer.Controllers;

[ApiController]
[Route(ApiRoutes.Customer.Shops)]
[AllowAnonymous]
[Tags("Customer Shops")]
public class ShopController : ControllerBase
{
    private readonly ICustomerShopService _shopService;
    private readonly IResponseLocalizer _responseLocalizer;

    public ShopController(ICustomerShopService shopService, IResponseLocalizer responseLocalizer)
    {
        _shopService = shopService;
        _responseLocalizer = responseLocalizer;
    }

    /// <summary>
    /// لیست فروشگاه‌ها برای اپ مشتری (صفحه اصلی / جستجو) با فیلترهای مختلف:
    /// search, shopCategoryId, cityId, provinceId, onlyOpenNow, hasStories,
    /// hasSpecialProducts, onlyNew, sortBy, page, pageSize
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ShopListQuery query, CancellationToken ct)
    {
        var result = await _shopService.GetAllAsync(query, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ShopsRetrieved);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>جزئیات یک فروشگاه (فقط فروشگاه‌های تأییدشده)</summary>
    [HttpGet("{shopId:guid}")]
    public async Task<IActionResult> GetById(Guid shopId, CancellationToken ct)
    {
        var result = await _shopService.GetByIdAsync(shopId, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ShopRetrieved);
        return Ok(ApiResult.Success(result, message));
    }
}
