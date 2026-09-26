// Application/.../Stories/Interfaces/IStoryRepository.cs
using Bodokado.Application.App.AdminModule.Stories.DTOs;
using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Domain.Entities.Stories;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.AdminModule.Stories.Interfaces;

public interface IStoryRepository : IGenericRepository<Story>
{
    Task<Story?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);

    Task<List<Story>> GetVisibleAsync(
        StoryShowPlace? showPlace,
        Guid? shopId,
        CancellationToken ct = default);

    Task<List<Story>> GetForAdminAsync(StoryListQuery query, CancellationToken ct = default);

    Task<List<Story>> GetForShopAsync(Guid shopId, CancellationToken ct = default);
}