using System.Text.Json;
using Bodokado.Application.Administrator.Settings.Interfaces;
using Bodokado.Application.App.AdminModule.Settings.DTOs;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Entities.Settings;

namespace Bodokado.Application.App.AdminModule.Settings.Services;

public class SettingService : ISettingService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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

        // Value می‌تواند string یا object از فرانت باشد
        var json = NormalizeValueToJson(request.Value);
        if (string.IsNullOrWhiteSpace(json))
            throw new BadRequestException(MessageKeys.SettingValueRequired, "setting_value_required");

        try
        {
            using var _ = JsonDocument.Parse(json);
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
                Value = json,
                CreatedAt = DateTime.UtcNow
            };
            await _settingRepository.AddAsync(setting);
        }
        else
        {
            setting.Value = json;
            setting.UpdatedAt = DateTime.UtcNow;
            _settingRepository.Update(setting);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return Map(setting);
    }

    private static string NormalizeValueToJson(object? value)
    {
        if (value is null)
            return string.Empty;

        if (value is string s)
            return s.Trim();

        if (value is JsonElement el)
        {
            return el.ValueKind == JsonValueKind.String
                ? (el.GetString() ?? string.Empty).Trim()
                : JsonSerializer.Serialize(el, JsonOptions);
        }

        return JsonSerializer.Serialize(value, JsonOptions);
    }

    private static SettingDto Map(Setting s)
    {
        object? value = null;
        if (!string.IsNullOrWhiteSpace(s.Value))
        {
            try
            {
                value = JsonSerializer.Deserialize<JsonElement>(s.Value);
            }
            catch
            {
                value = s.Value;
            }
        }

        return new SettingDto
        {
            Id = s.Id,
            Key = s.Key,
            Value = value,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        };
    }
}