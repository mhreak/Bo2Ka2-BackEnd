using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.ShopModule.Products.DTOs;

public class ProductListQuery : PaginationQuery
{
    public string? Search { get; set; }
    public ProductListFilter Filter { get; set; } = ProductListFilter.All;
}
