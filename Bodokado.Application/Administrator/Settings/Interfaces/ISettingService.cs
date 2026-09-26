
using Bodokado.Application.App.AdminModule.Settings.DTOs;

namespace Bodokado.Application.Administrator.Settings.Interfaces;

public interface ISettingService
{
    Task<SettingDto?> GetByKeyAsync(string key, CancellationToken ct = default);
    Task<SettingDto> UpsertAsync(UpsertSettingRequestDto request, CancellationToken ct = default);
}