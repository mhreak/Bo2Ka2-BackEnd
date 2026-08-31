using Bodokado.Infrastructure;

namespace Bodokado.API.DependencyInjection;

public static class CoreDependencyInjection
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseDependencies(configuration);
        services.AddOtpModule(configuration);
        services.AddTokenModule(configuration);
        services.AddCoreRepositoryDependencies();
        services.AddCoreApplicationServiceDependencies();
        services.AddIdentityDependencies();
        services.AddValidationDependencies();
        services.AddJwtDependencies(configuration);
        services.AddApiDependencies();
        services.AddSwaggerDependencies();
        services.AddLocalizationDependencies();
        return services;
    }
}
