using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.ShopModule.Orders.DTOs;
using Bodokado.Application.App.ShopModule.Orders.Interfaces;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Entities.Orders;
using Bodokado.Domain.Enums;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Repositories;

namespace Bodokado.Persistence.Repositories.Orders;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }

    public async Task<Order?> GetByIdForShopAsync(Guid orderId, Guid shopId, CancellationToken ct = default)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId && o.ShopId == shopId && !o.IsDeleted, ct);
    }

    public async Task<Order?> GetByIdWithDetailsForShopAsync(Guid orderId, Guid shopId, CancellationToken ct = default)
    {
        return await _context.Orders
            .Include(o => o.Items.Where(i => !i.IsDeleted))
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.ShopId == shopId && !o.IsDeleted, ct);
    }

    public async Task<Order?> GetByIdForCustomerAsync(Guid orderId, Guid customerUserId, CancellationToken ct = default)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerUserId == customerUserId && !o.IsDeleted, ct);
    }

    public async Task<Order?> GetByIdWithDetailsForCustomerAsync(Guid orderId, Guid customerUserId, CancellationToken ct = default)
    {
        return await _context.Orders
            .Include(o => o.Items.Where(i => !i.IsDeleted))
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerUserId == customerUserId && !o.IsDeleted, ct);
    }

    public async Task<PagedResult<Order>> GetPagedForShopAsync(Guid shopId, OrderListQuery query, CancellationToken ct = default)
    {
        var normalized = query.Normalize();
        IQueryable<Order> source = _context.Orders
            .AsNoTracking()
            .Include(o => o.Items.Where(i => !i.IsDeleted))
            .Where(o => o.ShopId == shopId && !o.IsDeleted);

        source = query.Filter switch
        {
            OrderListFilter.Pending => source.Where(o => o.Status == OrderStatus.Pending),
            OrderListFilter.Confirmed => source.Where(o => o.Status == OrderStatus.Confirmed),
            OrderListFilter.Shipped => source.Where(o => o.Status == OrderStatus.Shipped),
            _ => source
        };

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            source = source.Where(o =>
                o.OrderNumber.Contains(term)
                || o.BuyerName.Contains(term)
                || o.Items.Any(i => !i.IsDeleted && i.ProductName.Contains(term)));
        }

        source = source.OrderByDescending(o => o.CreatedAt);

        var totalCount = await source.CountAsync(ct);
        var items = await source.Skip(normalized.Skip).Take(normalized.Take).ToListAsync(ct);
        return PagedResult<Order>.Create(items, normalized, totalCount);
    }

    public async Task<PagedResult<Order>> GetPagedForCustomerAsync(Guid customerUserId, PaginationQuery query, CancellationToken ct = default)
    {
        var normalized = query.Normalize();
        IQueryable<Order> source = _context.Orders
            .AsNoTracking()
            .Include(o => o.Items.Where(i => !i.IsDeleted))
            .Where(o => o.CustomerUserId == customerUserId && !o.IsDeleted)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await source.CountAsync(ct);
        var items = await source.Skip(normalized.Skip).Take(normalized.Take).ToListAsync(ct);
        return PagedResult<Order>.Create(items, normalized, totalCount);
    }

    public async Task<string> GenerateOrderNumberAsync(CancellationToken ct = default)
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"BK-{datePart}-";
        var countToday = await _context.Orders.CountAsync(o => o.OrderNumber.StartsWith(prefix), ct);
        return $"{prefix}{(countToday + 1):D4}";
    }
}