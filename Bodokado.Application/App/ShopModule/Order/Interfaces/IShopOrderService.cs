using Bodokado.Application.App.ShopModule.Orders.DTOs;
using Bodokado.Application.Common.Pagination;

namespace Bodokado.Application.App.ShopModule.Orders.Interfaces;

public interface IShopOrderService
{
    Task<PagedResult<OrderListItemDto>> GetOrdersAsync(Guid shopUserId, OrderListQuery query, CancellationToken ct = default);
    Task<OrderDetailDto> GetByIdAsync(Guid shopUserId, Guid orderId, CancellationToken ct = default);
    Task<OrderDetailDto> ConfirmAsync(Guid shopUserId, Guid orderId, ConfirmOrderRequestDto request, CancellationToken ct = default);
    Task<OrderDetailDto> RejectAsync(Guid shopUserId, Guid orderId, RejectOrderRequestDto request, CancellationToken ct = default);
    Task<OrderDetailDto> MarkShippedAsync(Guid shopUserId, Guid orderId, CancellationToken ct = default);
}
