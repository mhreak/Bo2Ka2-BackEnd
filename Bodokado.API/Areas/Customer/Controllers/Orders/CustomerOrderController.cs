using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.CustomerModule.Orders.DTOs;
using Bodokado.Application.App.CustomerModule.Orders.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Pagination;

namespace Bodokado.API.Areas.Customer.Controllers;

[ApiController]
[Route(ApiRoutes.Customer.Orders)]
[Authorize(Roles = "User")]
[Tags("Customer Orders")]
public class CustomerOrderController : ControllerBase
{
    private readonly ICustomerOrderService _orderService;
    private readonly IResponseLocalizer _responseLocalizer;

    public CustomerOrderController(ICustomerOrderService orderService, IResponseLocalizer responseLocalizer)
    {
        _orderService = orderService;
        _responseLocalizer = responseLocalizer;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>ثبت سفارش جدید (چک‌اوت)</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequestDto request, CancellationToken ct)
    {
        var result = await _orderService.CreateAsync(CurrentUserId, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OrderCreated);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>لیست سفارش‌های من</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyOrders([FromQuery] PaginationQuery query, CancellationToken ct)
    {
        var result = await _orderService.GetMyOrdersAsync(CurrentUserId, query, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OrdersRetrieved);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>جزئیات سفارش من</summary>
    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetById(Guid orderId, CancellationToken ct)
    {
        var result = await _orderService.GetByIdAsync(CurrentUserId, orderId, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OrderRetrieved);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>لغو سفارش (فقط در وضعیت در انتظار یا تایید شده)</summary>
    [HttpPost("{orderId:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid orderId, CancellationToken ct)
    {
        await _orderService.CancelAsync(CurrentUserId, orderId, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OrderCancelled);
        return Ok(ApiResult.Success(message));
    }
}
