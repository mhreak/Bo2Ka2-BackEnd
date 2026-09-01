using Bodokado.Application.App.ShopModule.Products.DTOs;
using Bodokado.Application.Common.Pagination;

namespace Bodokado.Application.App.ShopModule.Products.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductListItemDto>> GetMyProductsAsync(Guid userId, ProductListQuery query, CancellationToken ct = default);
    Task<ProductDetailDto> GetByIdAsync(Guid userId, Guid productId, CancellationToken ct = default);
    Task<ProductDetailDto> CreateAsync(Guid userId, CreateProductRequestDto request, CancellationToken ct = default);
    Task<ProductDetailDto> UpdateAsync(Guid userId, Guid productId, UpdateProductRequestDto request, CancellationToken ct = default);
    Task DeleteAsync(Guid userId, Guid productId, CancellationToken ct = default);
}
