using Bodokado.Application.Common.Localization;
using Bodokado.Infrastructure.Localization;

namespace Bodokado.API.DependencyInjection;

public static class LocalizationDependencyInjection
{
    public static IServiceCollection AddLocalizationDependencies(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        var resourcesPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Messages");
        services.AddSingleton<ILocalizationService>(_ => new LocalizationService(resourcesPath));
        services.AddScoped<ICurrentUserLanguageProvider, CurrentUserLanguageProvider>();
        services.AddScoped<IResponseLocalizer, ResponseLocalizer>();
        return services;
    }
}
