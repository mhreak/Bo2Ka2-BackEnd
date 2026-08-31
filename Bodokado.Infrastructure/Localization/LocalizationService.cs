using System.Collections.Concurrent;
using System.Text.Json;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Enums;

namespace Bodokado.Infrastructure.Localization;

public class LocalizationService : ILocalizationService
{
    private readonly ConcurrentDictionary<Language, Dictionary<string, string>> _messages = new();

    public LocalizationService(string resourcesPath)
    {
        LoadLanguage(Language.English, Path.Combine(resourcesPath, "en.json"));
        LoadLanguage(Language.Persian, Path.Combine(resourcesPath, "fa.json"));
    }

    public string Get(string key, Language language, params object[] args)
    {
        if (!_messages.TryGetValue(language, out var dict) || !dict.TryGetValue(key, out var template))
        {
            if (language != Language.English && _messages.TryGetValue(Language.English, out var fallbackDict) && fallbackDict.TryGetValue(key, out var fallbackTemplate))
                template = fallbackTemplate;
            else
                return key;
        }
        return args.Length > 0 ? string.Format(template, args) : template;
    }

    private void LoadLanguage(Language language, string filePath)
    {
        if (!File.Exists(filePath))
        {
            _messages[language] = new Dictionary<string, string>();
            return;
        }
        var json = File.ReadAllText(filePath);
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        _messages[language] = dict;
    }
}
