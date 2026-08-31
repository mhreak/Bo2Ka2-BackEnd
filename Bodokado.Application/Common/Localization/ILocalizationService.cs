namespace Bodokado.Application.Common.Localization;

public interface ILocalizationService
{
    string Get(string key, Bodokado.Domain.Enums.Language language, params object[] args);
}
