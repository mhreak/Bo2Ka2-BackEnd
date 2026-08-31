using Bodokado.Application.Common.Auth.Services;
using Bodokado.Application.Common.Interfaces;

namespace Bodokado.API.DependencyInjection;

public static class CoreApplicationServiceDependencyInjection
{
    public static IServiceCollection AddCoreApplicationServiceDependencies(this IServiceCollection services)
    {
        services.AddScoped<IRoleContext>(_ => new GenericRoleContext("User"));
        return services;
    }
}
