using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Shops;

namespace Bodokado.Application.App.ShopModule.Registration.Interfaces;

public interface IShopRepository : IGenericRepository<Shop>
{
    Task<Shop?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<Shop?> GetByUserIdWithDetailsAsync(Guid userId, CancellationToken ct = default);
    Task<bool> NationalCodeExistsAsync(string nationalCode, Guid excludeShopId, CancellationToken ct = default);
}