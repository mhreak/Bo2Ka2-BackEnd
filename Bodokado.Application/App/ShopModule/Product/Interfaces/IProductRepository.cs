using Bodokado.Application.App.ShopModule.Products.DTOs;
using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.ShopModule.Products.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetByIdForShopAsync(Guid productId, Guid shopId, CancellationToken ct = default);
    Task<Product?> GetByIdWithDetailsForShopAsync(Guid productId, Guid shopId, CancellationToken ct = default);
    Task<PagedResult<Product>> GetPagedForShopAsync(Guid shopId, ProductListQuery query, CancellationToken ct = default);
}
