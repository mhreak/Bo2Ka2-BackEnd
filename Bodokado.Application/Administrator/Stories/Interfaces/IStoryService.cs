using Bodokado.Application.App.AdminModule.Stories.DTOs;
using Bodokado.Domain.Enums;

public interface IStoryService
{
    // App / Customer — فقط قابل‌نمایش
    Task<List<StoryDto>> GetVisibleAsync(StoryShowPlace? showPlace = null, Guid? shopId = null, CancellationToken ct = default);

    // Admin
    Task<List<StoryDto>> GetAllForAdminAsync(StoryListQuery query, CancellationToken ct = default);
    Task<StoryDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<StoryDto> CreateAsync(CreateStoryRequestDto request, bool isAdmin, CancellationToken ct = default);
    Task<StoryDto> UpdateAsync(Guid id, UpdateStoryRequestDto request, bool isAdmin, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}