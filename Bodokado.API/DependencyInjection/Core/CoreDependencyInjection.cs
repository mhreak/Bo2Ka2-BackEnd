using Bodokado.Application.Common.Otp;
using Bodokado.Infrastructure;
using Bodokado.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Bodokado.API.DependencyInjection;

public static class CoreDependencyInjection
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseDependencies(configuration);
        services.AddOtpModule(configuration);
        services.AddOptions<SmsIrOptions>()
            .Bind(configuration.GetSection("SmsIr"))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ApiKey),
                "SmsIr:ApiKey is required. Configure it with user-secrets or the SmsIr__ApiKey environment variable.")
            .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri) && baseUri.Scheme == Uri.UriSchemeHttps,
                "SmsIr:BaseUrl must be an absolute HTTPS URL.")
            .Validate(options => options.LineNumber > 0, "SmsIr:LineNumber must be configured with a valid sms.ir line number.")
            .Validate(options => options.VerifyTemplateId > 0, "SmsIr:VerifyTemplateId must be configured with an approved template ID.")
            .ValidateOnStart();

        services.AddHttpClient<ISmsSender, SmsIrSender>((sp, client) =>
        {
            var opt = sp.GetRequiredService<IOptions<SmsIrOptions>>().Value;
            client.BaseAddress = new Uri(opt.BaseUrl);
            client.DefaultRequestHeaders.Add("x-api-key", opt.ApiKey);
            client.Timeout = TimeSpan.FromSeconds(15);
        });
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
