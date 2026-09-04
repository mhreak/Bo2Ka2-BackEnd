using Bodokado.Application.App.CustomerModule.Orders.DTOs;
using Bodokado.Application.App.ShopModule.Orders.DTOs;
using Bodokado.Application.Common.Pagination;

namespace Bodokado.Application.App.CustomerModule.Orders.Interfaces;

public interface ICustomerOrderService
{
    Task<OrderDetailDto> CreateAsync(Guid customerUserId, CreateOrderRequestDto request, CancellationToken ct = default);
    Task<PagedResult<OrderListItemDto>> GetMyOrdersAsync(Guid customerUserId, PaginationQuery query, CancellationToken ct = default);
    Task<OrderDetailDto> GetByIdAsync(Guid customerUserId, Guid orderId, CancellationToken ct = default);
    Task CancelAsync(Guid customerUserId, Guid orderId, CancellationToken ct = default);
}
