using System.Text.Json;
using Bodokado.Application.App.AdminModule.Settings.DTOs;
using Bodokado.Application.App.AdminModule.Settings;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Entities.Settings;
using Bodokado.Application.Administrator.Settings.Interfaces;

namespace Bodokado.Application.App.AdminModule.Settings.Services;

public class SettingService : ISettingService
{
    private readonly ISettingRepository _settingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SettingService(ISettingRepository settingRepository, IUnitOfWork unitOfWork)
    {
        _settingRepository = settingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SettingDto?> GetByKeyAsync(string key, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new BadRequestException(MessageKeys.SettingKeyRequired, "setting_key_required");

        var setting = await _settingRepository.GetByKeyAsync(key.Trim().ToLowerInvariant(), ct);
        if (setting is null)
            return null;

        return Map(setting);
    }

    public async Task<SettingDto> UpsertAsync(UpsertSettingRequestDto request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Key))
            throw new BadRequestException(MessageKeys.SettingKeyRequired, "setting_key_required");

        if (string.IsNullOrWhiteSpace(request.Value))
            throw new BadRequestException(MessageKeys.SettingValueRequired, "setting_value_required");

        // اعتبارسنجی اینکه Value واقعاً JSON معتبر باشد
        try
        {
            using var _ = JsonDocument.Parse(request.Value);
        }
        catch
        {
            throw new BadRequestException(MessageKeys.SettingValueInvalidJson, "setting_value_invalid_json");
        }

        var key = request.Key.Trim().ToLowerInvariant();
        var setting = await _settingRepository.GetByKeyAsync(key, ct);

        if (setting is null)
        {
            setting = new Setting
            {
                Id = Guid.NewGuid(),
                Key = key,
                Value = request.Value.Trim(),
                CreatedAt = DateTime.UtcNow
            };
            await _settingRepository.AddAsync(setting);
        }
        else
        {
            setting.Value = request.Value.Trim();
            setting.UpdatedAt = DateTime.UtcNow;
            _settingRepository.Update(setting);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return Map(setting);
    }

    private static SettingDto Map(Setting s) => new()
    {
        Id = s.Id,
        Key = s.Key,
        Value = s.Value,
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt
    };
}