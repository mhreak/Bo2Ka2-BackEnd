using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Filters;
using Bodokado.Application.Common.Auth.Validators;

namespace Bodokado.API.DependencyInjection;

public static class ValidationDependencyInjection
{
    public static IServiceCollection AddValidationDependencies(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<LoginOtpRequestValidator>();
        services.AddScoped<ValidationFilter>();
        services.Configure<MvcOptions>(options => { options.Filters.Add<ValidationFilter>(); });
        return services;
    }
}
