using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using Bodokado.Application.Common.Auth;
using Bodokado.Application.Common.Otp;
using Bodokado.Application.Common.Settings;
using Bodokado.Infrastructure.Services;

namespace Bodokado.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOtpModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OtpSettings>(configuration.GetSection(OtpSettings.SectionName));
        AddRedisConnection(services, configuration);
        services.AddScoped<IOtpService, RedisOtpService>();
        services.AddScoped<ISmsSender, FakeSmsSender>();
        services.AddScoped<IEmailSender, FakeEmailSender>();
        return services;
    }

    public static IServiceCollection AddTokenModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TokenSettings>(configuration.GetSection(TokenSettings.SectionName));
        AddRedisConnection(services, configuration);
        services.AddScoped<IRefreshTokenService, RedisRefreshTokenService>();
        return services;
    }

    private static void AddRedisConnection(IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Redis connection string ('ConnectionStrings:Redis') is missing.");
        services.TryAddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnectionString));
    }
}
