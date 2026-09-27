// Application/Administrator/Banners/Interfaces/IBannerRepository.cs
using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Banners;

namespace Bodokado.Application.Administrator.Banners.Interfaces;

public interface IBannerRepository : IGenericRepository<Banner>
{
    Task<Banner?> GetByIdWithImageAsync(Guid id, CancellationToken ct = default);
    Task<List<Banner>> GetActiveOrderedAsync(CancellationToken ct = default);
    Task<List<Banner>> GetAllOrderedAsync(CancellationToken ct = default);
}