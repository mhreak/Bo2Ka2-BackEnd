using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Bodokado.Application.Common.Auth;
using Bodokado.Application.Common.Models;
using Bodokado.Application.Common.Settings;

namespace Bodokado.Infrastructure.Services;

public class RedisRefreshTokenService : IRefreshTokenService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly TokenSettings _settings;
    private const string UserIdField = "userId";
    private const string TokenHashField = "tokenHash";
    private const string RoleField = "role";
    private const string CreatedAtField = "createdAt";

    public RedisRefreshTokenService(IConnectionMultiplexer redis, Microsoft.Extensions.Options.IOptions<TokenSettings> settings)
    {
        _redis = redis;
        _settings = settings.Value;
    }

    public async Task<RefreshTokenRotationResult> RotateAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var tokenHash = Hash(refreshToken);
        var tokenKey = BuildRefreshTokenKey(tokenHash);
        var sessionIdValue = await db.StringGetAsync(tokenKey);
        if (sessionIdValue.IsNullOrEmpty || !Guid.TryParse((string)sessionIdValue!, out var sessionId))
            return RefreshTokenRotationResult.Invalid();
        var sessionKey = BuildSessionKey(sessionId);
        var sessionEntries = await db.HashGetAllAsync(sessionKey);
        if (sessionEntries.Length == 0)
        {
            await db.KeyDeleteAsync(tokenKey);
            return RefreshTokenRotationResult.Invalid();
        }
        var sessionDict = sessionEntries.ToDictionary(e => e.Name.ToString(), e => e.Value.ToString());
        var userId = Guid.Parse(sessionDict[UserIdField]);
        var role = sessionDict[RoleField];
        if (!sessionDict.TryGetValue(CreatedAtField, out var createdAtStr) || !DateTime.TryParse(createdAtStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var createdAt))
        {
            await db.KeyDeleteAsync(tokenKey);
            await db.KeyDeleteAsync(sessionKey);
            return RefreshTokenRotationResult.Invalid();
        }
        var absoluteLifetime = TimeSpan.FromDays(_settings.RefreshTokenAbsoluteExpirationDays);
        if (DateTime.UtcNow > createdAt.Add(absoluteLifetime))
        {
            await db.KeyDeleteAsync(tokenKey);
            await db.KeyDeleteAsync(sessionKey);
            await db.SetRemoveAsync(BuildUserSessionsKey(userId, role), sessionId.ToString());
            return RefreshTokenRotationResult.Invalid();
        }
        await db.KeyDeleteAsync(tokenKey);
        var slidingTtl = TimeSpan.FromDays(_settings.RefreshTokenExpirationDays);
        var newRawToken = GenerateRawToken();
        var newTokenHash = Hash(newRawToken);
        await StoreSessionAsync(db, sessionId, userId, role, newTokenHash, slidingTtl, createdAt);
        return RefreshTokenRotationResult.Success(userId, sessionId, newRawToken, DateTime.UtcNow.Add(slidingTtl), role);
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var tokenHash = Hash(refreshToken);
        var tokenKey = BuildRefreshTokenKey(tokenHash);
        var sessionIdValue = await db.StringGetAsync(tokenKey);
        await db.KeyDeleteAsync(tokenKey);
        if (sessionIdValue.IsNullOrEmpty || !Guid.TryParse((string)sessionIdValue!, out var sessionId))
            return;
        var sessionKey = BuildSessionKey(sessionId);
        var sessionEntries = await db.HashGetAllAsync(sessionKey);
        await db.KeyDeleteAsync(sessionKey);
        if (sessionEntries.Length > 0)
        {
            var dict = sessionEntries.ToDictionary(e => e.Name.ToString(), e => e.Value.ToString());
            if (dict.TryGetValue(UserIdField, out var userIdStr) && dict.TryGetValue(RoleField, out var role))
                await db.SetRemoveAsync(BuildUserSessionsKey(Guid.Parse(userIdStr), role), sessionId.ToString());
        }
    }

    public async Task<RefreshTokenIssueResult> IssueAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var sessionId = Guid.NewGuid();
        var slidingTtl = TimeSpan.FromDays(_settings.RefreshTokenExpirationDays);
        var createdAt = DateTime.UtcNow;
        var rawToken = GenerateRawToken();
        var tokenHash = Hash(rawToken);
        await StoreSessionAsync(db, sessionId, userId, role, tokenHash, slidingTtl, createdAt);
        await db.SetAddAsync(BuildUserSessionsKey(userId, role), sessionId.ToString());
        return new RefreshTokenIssueResult { RefreshToken = rawToken, SessionId = sessionId, ExpiresAtUtc = DateTime.UtcNow.Add(slidingTtl) };
    }

    public async Task RevokeAllByRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var userSessionsKey = BuildUserSessionsKey(userId, role);
        var sessionIds = await db.SetMembersAsync(userSessionsKey);
        var tasks = sessionIds.Where(v => Guid.TryParse((string)v!, out _)).Select(v => db.KeyDeleteAsync(BuildSessionKey(Guid.Parse((string)v!))));
        await Task.WhenAll(tasks);
        await db.KeyDeleteAsync(userSessionsKey);
        await db.KeyDeleteAsync(userSessionsKey);
    }

    private static async Task StoreSessionAsync(IDatabase db, Guid sessionId, Guid userId, string role, string tokenHash, TimeSpan ttl, DateTime createdAt)
    {
        var sessionKey = BuildSessionKey(sessionId);
        var entries = new HashEntry[]
        {
            new(UserIdField, userId.ToString()),
            new(RoleField, role),
            new(TokenHashField, tokenHash),
            new(CreatedAtField, createdAt.ToString("O"))
        };
        await db.HashSetAsync(sessionKey, entries);
        await db.KeyExpireAsync(sessionKey, ttl);
        await db.StringSetAsync(BuildRefreshTokenKey(tokenHash), sessionId.ToString(), ttl);
    }

    private static string GenerateRawToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private static string Hash(string rawToken)
    {
        var bytes = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes);
    }

    private static string BuildSessionKey(Guid sessionId) => $"session:{sessionId}";
    private static string BuildRefreshTokenKey(string tokenHash) => $"refresh_token:{tokenHash}";
    private static string BuildUserSessionsKey(Guid userId, string role) => $"user_sessions:{userId}:{role}";
}
