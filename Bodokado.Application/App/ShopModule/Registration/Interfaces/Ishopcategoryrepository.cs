using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Shops;

namespace Bodokado.Application.App.ShopModule.Registration.Interfaces;

public interface IShopCategoryRepository : IGenericRepository<ShopCategory>
{
    Task<List<ShopCategory>> SearchAsync(string? search, CancellationToken ct = default);
}