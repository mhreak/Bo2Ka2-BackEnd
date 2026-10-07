using Bodokado.Application.Administrator.Banners.DTOs;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.Administrator.Banners.Interfaces;

public interface IBannerService
{
    Task<List<BannerDto>> GetActiveAsync(BannerShowPlace? showPlace = null, CancellationToken ct = default);
    Task<List<BannerDto>> GetAllAsync(BannerShowPlace? showPlace = null, CancellationToken ct = default);
    Task<BannerDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BannerDto> CreateAsync(CreateBannerRequestDto request, CancellationToken ct = default);
    Task<BannerDto> UpdateAsync(Guid id, UpdateBannerRequestDto request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}