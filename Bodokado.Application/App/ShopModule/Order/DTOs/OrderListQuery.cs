using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.ShopModule.Orders.DTOs;

public class OrderListQuery : PaginationQuery
{
    public string? Search { get; set; }
    public OrderListFilter Filter { get; set; } = OrderListFilter.All;
}
