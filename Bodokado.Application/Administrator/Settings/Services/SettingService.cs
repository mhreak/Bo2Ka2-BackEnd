using System.Text;
using System.Text.Json;
using Bodokado.Application.Administrator.Banners.Interfaces;
using Bodokado.Application.Administrator.Settings.Interfaces;
using Bodokado.Application.App.AdminModule.Settings.DTOs;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Entities.Settings;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.AdminModule.Settings.Services;

public class SettingService : ISettingService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly ISettingRepository _settingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBannerRepository _bannerRepository;

    public SettingService(
        ISettingRepository settingRepository,
        IUnitOfWork unitOfWork,
        IBannerRepository bannerRepository)
    {
        _settingRepository = settingRepository;
        _unitOfWork = unitOfWork;
        _bannerRepository = bannerRepository;
    }

    public async Task<SettingDto?> GetByKeyAsync(string key, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new BadRequestException(MessageKeys.SettingKeyRequired, "setting_key_required");

        var setting = await _settingRepository.GetByKeyAsync(key.Trim().ToLowerInvariant(), ct);
        if (setting is null)
            return null;

        return await MapAsync(setting, enrichBanners: true, ct);
    }

    public async Task<SettingDto> UpsertAsync(UpsertSettingRequestDto request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Key))
            throw new BadRequestException(MessageKeys.SettingKeyRequired, "setting_key_required");

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

        // بعد از ذخیره نیازی به enrichment نیست؛ همان JSON ذخیره‌شده
        return await MapAsync(setting, enrichBanners: false, ct);
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

    private async Task<SettingDto> MapAsync(Setting s, bool enrichBanners, CancellationToken ct)
    {
        object? value = s.Value;

        if (!string.IsNullOrWhiteSpace(s.Value))
        {
            try
            {
                using var doc = JsonDocument.Parse(s.Value);
                var root = doc.RootElement.Clone();

                if (enrichBanners
                    && root.ValueKind == JsonValueKind.Object
                    && root.TryGetProperty("sections", out var sections)
                    && sections.ValueKind == JsonValueKind.Array)
                {
                    value = await EnrichSectionsWithBannerIdsAsync(root, s.Key, ct);
                }
                else
                {
                    value = root;
                }
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

    private async Task<object> EnrichSectionsWithBannerIdsAsync(
        JsonElement root,
        string settingKey,
        CancellationToken ct)
    {
        var showPlace = ResolveShowPlace(settingKey);
        var banners = await _bannerRepository.GetActiveOrderedAsync(showPlace, ct);
        var bannerIds = banners.Select(b => b.Id).ToList();

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();

            foreach (var prop in root.EnumerateObject())
            {
                if (prop.NameEquals("sections"))
                {
                    writer.WritePropertyName("sections");
                    writer.WriteStartArray();

                    foreach (var section in prop.Value.EnumerateArray())
                    {
                        writer.WriteStartObject();

                        string? type = null;
                        foreach (var sp in section.EnumerateObject())
                        {
                            if (sp.NameEquals("type"))
                                type = sp.Value.GetString();

                            sp.WriteTo(writer);
                        }

                        if (string.Equals(type, "banner", StringComparison.OrdinalIgnoreCase)
                            && !section.TryGetProperty("bannerIds", out _))
                        {
                            writer.WritePropertyName("bannerIds");
                            writer.WriteStartArray();
                            foreach (var id in bannerIds)
                                writer.WriteStringValue(id.ToString());
                            writer.WriteEndArray();
                        }

                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();
                }
                else
                {
                    prop.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
        }

        var json = Encoding.UTF8.GetString(stream.ToArray());
        return JsonSerializer.Deserialize<JsonElement>(json)!;
    }

    private static BannerShowPlace ResolveShowPlace(string settingKey)
        => settingKey.Trim().ToLowerInvariant() switch
        {
            "homepage" => BannerShowPlace.HomePage,
            "shop-list" or "shoplist" => BannerShowPlace.ShopList,
            "product-detail" or "productdetail" => BannerShowPlace.ProductDetail,
            _ => BannerShowPlace.HomePage
        };
}