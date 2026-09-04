using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.ShopModule.Orders.DTOs;
using Bodokado.Application.App.ShopModule.Orders.Interfaces;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Controllers.Shop;

[ApiController]
[Route(ApiRoutes.Shop.Orders)]
[Authorize(Roles = "Shop")]
[Tags("Shop Orders")]
public class ShopOrderController : ControllerBase
{
    private readonly IShopOrderService _orderService;
    private readonly IResponseLocalizer _responseLocalizer;

    public ShopOrderController(IShopOrderService orderService, IResponseLocalizer responseLocalizer)
    {
        _orderService = orderService;
        _responseLocalizer = responseLocalizer;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>لیست سفارشات فروشگاه (همه / در انتظار / تایید شده / ارسال شده)</summary>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] OrderListQuery query, CancellationToken ct)
    {
        var result = await _orderService.GetOrdersAsync(CurrentUserId, query, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OrdersRetrieved);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>جزئیات سفارش</summary>
    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetById(Guid orderId, CancellationToken ct)
    {
        var result = await _orderService.GetByIdAsync(CurrentUserId, orderId, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OrderRetrieved);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>تایید سفارش (با امکان تنظیم بسته‌بندی و هزینه)</summary>
    [HttpPost("{orderId:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid orderId, [FromBody] ConfirmOrderRequestDto? request, CancellationToken ct)
    {
        var result = await _orderService.ConfirmAsync(CurrentUserId, orderId, request ?? new ConfirmOrderRequestDto(), ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OrderConfirmed);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>رد سفارش</summary>
    [HttpPost("{orderId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid orderId, [FromBody] RejectOrderRequestDto? request, CancellationToken ct)
    {
        var result = await _orderService.RejectAsync(CurrentUserId, orderId, request ?? new RejectOrderRequestDto(), ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OrderRejected);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>علامت‌گذاری به‌عنوان ارسال‌شده</summary>
    [HttpPost("{orderId:guid}/ship")]
    public async Task<IActionResult> Ship(Guid orderId, CancellationToken ct)
    {
        var result = await _orderService.MarkShippedAsync(CurrentUserId, orderId, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OrderShipped);
        return Ok(ApiResult.Success(result, message));
    }
}
