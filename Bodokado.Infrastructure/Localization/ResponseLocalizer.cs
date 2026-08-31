using Bodokado.Application.Common.Localization;

namespace Bodokado.Infrastructure.Localization;

public class ResponseLocalizer : IResponseLocalizer
{
    private readonly ILocalizationService _localizationService;
    private readonly ICurrentUserLanguageProvider _languageProvider;

    public ResponseLocalizer(ILocalizationService localizationService, ICurrentUserLanguageProvider languageProvider)
    {
        _localizationService = localizationService;
        _languageProvider = languageProvider;
    }

    public async Task<string> LocalizeAsync(string key, params object[] args)
    {
        var lang = await _languageProvider.GetCurrentLanguageAsync();
        return _localizationService.Get(key, lang, args);
    }
}
