using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Bodokado.Application.Common.Otp;
using Bodokado.Domain.Enums;

namespace Bodokado.Infrastructure.Services;

public class RedisOtpService : IOtpService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ISmsSender _smsSender;
    private readonly IEmailSender _emailSender;
    private readonly OtpSettings _settings;
    private const string CodeField = "code";
    private const string AttemptsField = "attempts";

    public RedisOtpService(IConnectionMultiplexer redis, ISmsSender smsSender, IEmailSender emailSender, Microsoft.Extensions.Options.IOptions<OtpSettings> settings)
    {
        _redis = redis;
        _smsSender = smsSender;
        _emailSender = emailSender;
        _settings = settings.Value;
    }

    public async Task<OtpGenerationResult> GenerateAndSendAsync(string destination, OtpChannel channel, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var normalizedDestination = Normalize(destination);
        var cooldownKey = BuildCooldownKey(channel, normalizedDestination);
        var cooldownTtl = await db.KeyTimeToLiveAsync(cooldownKey);
        if (cooldownTtl.HasValue)
            return OtpGenerationResult.Cooldown((int)Math.Ceiling(cooldownTtl.Value.TotalSeconds));
        var code = GenerateNumericCode(_settings.CodeLength);
        var otpKey = BuildOtpKey(channel, normalizedDestination);
        var entries = new HashEntry[] { new(CodeField, code), new(AttemptsField, 0) };
        await db.HashSetAsync(otpKey, entries);
        await db.KeyExpireAsync(otpKey, TimeSpan.FromMinutes(_settings.ExpirationMinutes));
        await db.StringSetAsync(cooldownKey, "1", TimeSpan.FromSeconds(_settings.ResendCooldownSeconds));
        await SendAsync(normalizedDestination, channel, code, cancellationToken);
        return OtpGenerationResult.Sent(_settings.ExposeCodeInResponse ? code : null);
    }

    public async Task<OtpVerificationResult> VerifyAsync(string destination, OtpChannel channel, string code, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var normalizedDestination = Normalize(destination);
        var otpKey = BuildOtpKey(channel, normalizedDestination);
        var entries = await db.HashGetAllAsync(otpKey);
        if (entries.Length == 0)
            return OtpVerificationResult.NotFoundOrExpired();
        var dict = entries.ToDictionary(e => e.Name.ToString(), e => e.Value);
        var attempts = (int)dict[AttemptsField];
        if (attempts >= _settings.MaxAttempts)
        {
            await db.KeyDeleteAsync(otpKey);
            return OtpVerificationResult.MaxAttemptsExceeded();
        }
        var storedCode = dict[CodeField].ToString();
        if (!string.Equals(storedCode, code, StringComparison.Ordinal))
        {
            var newAttempts = await db.HashIncrementAsync(otpKey, AttemptsField);
            var remaining = Math.Max(0, _settings.MaxAttempts - (int)newAttempts);
            return OtpVerificationResult.InvalidCode(remaining);
        }
        await db.KeyDeleteAsync(otpKey);
        return OtpVerificationResult.Success();
    }

    private Task SendAsync(string destination, OtpChannel channel, string code, CancellationToken cancellationToken)
    {
        var message = $"کد تایید شما: {code}";
        return channel switch
        {
            OtpChannel.Sms => _smsSender.SendAsync(destination, message, cancellationToken),
            OtpChannel.Email => _emailSender.SendAsync(destination, "کد تایید", message, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
        };
    }

    private static string GenerateNumericCode(int length)
    {
        var max = (int)Math.Pow(10, length);
        var value = RandomNumberGenerator.GetInt32(0, max);
        return value.ToString(new string('0', length));
    }

    private static string Normalize(string destination) => destination.Trim().ToLowerInvariant();
    private static string BuildOtpKey(OtpChannel channel, string destination) => $"otp:{channel}:{destination}";
    private static string BuildCooldownKey(OtpChannel channel, string destination) => $"otp:cooldown:{channel}:{destination}";
}
