using Bodokado.Domain.Entities.Organizations;
using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Entities.Shops;

namespace Bodokado.Application.App.CustomerModule.OrganizationalGifts.Interfaces;

public interface ICustomerOrganizationalGiftRepository
{
    Task<UserOrganizationalGiftCampaign?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<List<Shop>> GetShopsByIdsAsync(IEnumerable<Guid> shopIds, CancellationToken ct = default);
    Task<List<Product>> GetProductsByIdsAsync(IEnumerable<Guid> productIds, CancellationToken ct = default);
    Task<List<ProductCategory>> GetCategoriesByIdsAsync(IEnumerable<Guid> categoryIds, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
