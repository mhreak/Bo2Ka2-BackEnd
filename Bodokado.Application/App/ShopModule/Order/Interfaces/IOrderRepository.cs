using Bodokado.Application.App.ShopModule.Orders.DTOs;
using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Entities.Orders;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.ShopModule.Orders.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> GetByIdForShopAsync(Guid orderId, Guid shopId, CancellationToken ct = default);
    Task<Order?> GetByIdWithDetailsForShopAsync(Guid orderId, Guid shopId, CancellationToken ct = default);
    Task<Order?> GetByIdForCustomerAsync(Guid orderId, Guid customerUserId, CancellationToken ct = default);
    Task<Order?> GetByIdWithDetailsForCustomerAsync(Guid orderId, Guid customerUserId, CancellationToken ct = default);
    Task<PagedResult<Order>> GetPagedForShopAsync(Guid shopId, OrderListQuery query, CancellationToken ct = default);
    Task<PagedResult<Order>> GetPagedForCustomerAsync(Guid customerUserId, PaginationQuery query, CancellationToken ct = default);
    Task<string> GenerateOrderNumberAsync(CancellationToken ct = default);
}
