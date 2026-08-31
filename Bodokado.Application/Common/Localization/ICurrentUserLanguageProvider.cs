using Bodokado.Domain.Enums;

namespace Bodokado.Application.Common.Localization;

public interface ICurrentUserLanguageProvider
{
    Task<Language> GetCurrentLanguageAsync();
    void InvalidateCache(Guid userId);
}
