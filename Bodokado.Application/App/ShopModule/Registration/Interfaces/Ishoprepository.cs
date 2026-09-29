using Bodokado.Application.App.CustomerModule.Shops.DTOs;
using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Entities.Shops;

namespace Bodokado.Application.App.ShopModule.Registration.Interfaces;

public interface IShopRepository : IGenericRepository<Shop>
{
    Task<Shop?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<Shop?> GetByUserIdWithDetailsAsync(Guid userId, CancellationToken ct = default);
    Task<bool> NationalCodeExistsAsync(string nationalCode, Guid excludeShopId, CancellationToken ct = default);

    /// <summary>لیست فروشگاه‌های تأییدشده برای اپ مشتری با فیلترهای مختلف</summary>
    Task<PagedResult<Shop>> GetPagedForCustomerAsync(ShopListQuery query, CancellationToken ct = default);

    Task<Shop?> GetByApiKeyAsync(string apiKey, CancellationToken ct = default);
    /// <summary>جزئیات یک فروشگاه تأییدشده برای اپ مشتری</summary>
    Task<Shop?> GetApprovedByIdWithDetailsAsync(Guid shopId, CancellationToken ct = default);
}