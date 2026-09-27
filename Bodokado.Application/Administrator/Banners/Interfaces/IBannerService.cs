// Application/Administrator/Banners/Interfaces/IBannerService.cs
using Bodokado.Application.Administrator.Banners.DTOs;

namespace Bodokado.Application.Administrator.Banners.Interfaces;

public interface IBannerService
{
    Task<List<BannerDto>> GetActiveAsync(CancellationToken ct = default);
    Task<List<BannerDto>> GetAllAsync(CancellationToken ct = default);
    Task<BannerDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BannerDto> CreateAsync(CreateBannerRequestDto request, CancellationToken ct = default);
    Task<BannerDto> UpdateAsync(Guid id, UpdateBannerRequestDto request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}