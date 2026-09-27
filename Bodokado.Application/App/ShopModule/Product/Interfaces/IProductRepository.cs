using Bodokado.Application.App.CustomerModule.Products.DTOs;
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

    /// <summary>تعداد محصولات منتشرشدهٔ هر فروشگاه (برای نمایش در لیست فروشگاه‌های مشتری)</summary>
    Task<Dictionary<Guid, int>> GetPublishedCountsByShopIdsAsync(IEnumerable<Guid> shopIds, CancellationToken ct = default);

    /// <summary>لیست محصولات قابل نمایش برای مشتری (فقط فروشگاه‌های تأییدشده و محصولات منتشرشده)</summary>
    Task<PagedResult<Product>> GetPagedForCustomerAsync(CustomerProductListQuery query, CancellationToken ct = default);

    /// <summary>جزئیات یک محصول قابل نمایش برای مشتری</summary>
    Task<Product?> GetByIdForCustomerAsync(Guid productId, CancellationToken ct = default);
}
