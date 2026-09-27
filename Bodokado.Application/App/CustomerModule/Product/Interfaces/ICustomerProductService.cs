using Bodokado.Application.App.CustomerModule.Products.DTOs;
using Bodokado.Application.Common.Pagination;

namespace Bodokado.Application.App.CustomerModule.Products.Interfaces;

public interface ICustomerProductService
{
    Task<PagedResult<CustomerProductListItemDto>> GetAllAsync(CustomerProductListQuery query, CancellationToken ct = default);
    Task<CustomerProductDetailDto> GetByIdAsync(Guid productId, CancellationToken ct = default);
}
