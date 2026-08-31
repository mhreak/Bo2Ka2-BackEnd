using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Bodokado.API.DependencyInjection;

public static class ApiDependencyInjection
{
    public static IServiceCollection AddApiDependencies(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddControllers();
        services.AddOpenApi();
        return services;
    }
}

static class ControllerDependencyInjection
{
    public static IMvcBuilder AddControllers(this IServiceCollection services)
    {
        return services.AddControllers(options => { options.Filters.Add(new ProducesAttribute("application/json")); })
            .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
    }
}
