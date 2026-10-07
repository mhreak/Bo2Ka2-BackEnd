// Application/Administrator/Banners/Interfaces/IBannerRepository.cs
using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Banners;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.Administrator.Banners.Interfaces;

public interface IBannerRepository : IGenericRepository<Banner>
{
    Task<Banner?> GetByIdWithImageAsync(Guid id, CancellationToken ct = default);

    Task<List<Banner>> GetActiveOrderedAsync(BannerShowPlace? showPlace = null, CancellationToken ct = default);

    Task<List<Banner>> GetAllOrderedAsync(BannerShowPlace? showPlace = null, CancellationToken ct = default);
}