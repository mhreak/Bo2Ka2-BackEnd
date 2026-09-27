using Bodokado.Application.App.CustomerModule.Shops.DTOs;
using Bodokado.Application.Common.Pagination;

namespace Bodokado.Application.App.CustomerModule.Shops.Interfaces;

public interface ICustomerShopService
{
    Task<PagedResult<ShopListItemDto>> GetAllAsync(ShopListQuery query, CancellationToken ct = default);
    Task<ShopDetailDto> GetByIdAsync(Guid shopId, CancellationToken ct = default);
}
