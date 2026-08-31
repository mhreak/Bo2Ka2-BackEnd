using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bodokado.API.DependencyInjection;

public static class SwaggerDependencyInjection
{
    public static IServiceCollection AddSwaggerDependencies(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // یک سند واحد تا همه APIها (Auth, User, Location, Admin, File) دیده شوند
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Bodokado API",
                Version = "v1",
                Description = "Generic starter: Auth, User, Location, Admin, Files"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT token — مثال: Bearer eyJhbGciOi..."
            });

            options.OperationFilter<AuthorizeSecurityOperationFilter>();
            options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
        });
        return services;
    }

    private sealed class AuthorizeSecurityOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorize = context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>()
                .Concat(context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>() ?? Enumerable.Empty<AuthorizeAttribute>())
                .Any();
            if (!hasAuthorize) return;

            var allowAnonymous = context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any()
                || (context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any() ?? false);
            if (allowAnonymous) return;

            operation.Security ??= [];
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
            });
        }
    }
}
