using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bodokado.API.DependencyInjection;

public static class SwaggerDependencyInjection
{
    public static IServiceCollection AddSwaggerDependencies(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("shop", new OpenApiInfo
            {
                Title = "Bodokado Shop API",
                Version = "v1",
                Description = "APIهای پنل فروشگاه"
            });

            options.SwaggerDoc("customer", new OpenApiInfo
            {
                Title = "Bodokado Customer API",
                Version = "v1",
                Description = "APIهای مشتری عادی (حساب شخصی)"
            });

            options.SwaggerDoc("corporate", new OpenApiInfo
            {
                Title = "Bodokado Corporate API",
                Version = "v1",
                Description = "APIهای مشتری سازمانی"
            });

            options.SwaggerDoc("admin", new OpenApiInfo
            {
                Title = "Bodokado Admin API",
                Version = "v1",
                Description = "APIهای پنل ادمین"
            });

            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (!apiDesc.TryGetMethodInfo(out var methodInfo))
                    return false;

                var ns = methodInfo.DeclaringType?.Namespace ?? "";

                return docName switch
                {
                    "shop" => ns.Contains("Areas.Shop", StringComparison.OrdinalIgnoreCase),
                    "customer" => ns.Contains("Areas.Customer", StringComparison.OrdinalIgnoreCase),
                    "corporate" => ns.Contains("Areas.Corporate", StringComparison.OrdinalIgnoreCase),
                    "admin" => ns.Contains("Areas.Admin", StringComparison.OrdinalIgnoreCase),
                    _ => false
                };
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT. مثال: Bearer {token}"
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });

        return services;
    }

    private sealed class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorize = context.MethodInfo.DeclaringType?
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                .Any() == true
                || context.MethodInfo
                    .GetCustomAttributes(true)
                    .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                    .Any();

            var allowAnonymous = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>()
                .Any()
                || context.MethodInfo.DeclaringType?
                    .GetCustomAttributes(true)
                    .OfType<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>()
                    .Any() == true;

            if (!hasAuthorize || allowAnonymous)
                return;

            operation.Security ??= new List<OpenApiSecurityRequirement>();
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
            });
        }
    }
}
