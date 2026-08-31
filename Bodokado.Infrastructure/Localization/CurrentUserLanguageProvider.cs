using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Enums;
using Bodokado.Persistence.Context;

namespace Bodokado.Infrastructure.Localization;

public class CurrentUserLanguageProvider : ICurrentUserLanguageProvider
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;

    public CurrentUserLanguageProvider(IHttpContextAccessor httpContextAccessor, AppDbContext context, IMemoryCache cache)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _cache = cache;
    }

    public async Task<Language> GetCurrentLanguageAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userIdClaim = httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return ResolveFromAcceptLanguageHeader(httpContext);
        var cacheKey = $"user-lang:{userId}";
        if (_cache.TryGetValue(cacheKey, out Language cachedLang))
            return cachedLang;
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        var lang = Language.English;
        _cache.Set(cacheKey, lang, CacheDuration);
        return lang;
    }

    public void InvalidateCache(Guid userId)
    {
        _cache.Remove($"user-lang:{userId}");
    }

    private static Language ResolveFromAcceptLanguageHeader(HttpContext? httpContext)
    {
        var header = httpContext?.Request.Headers["Accept-Language"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(header) && header.StartsWith("fa", StringComparison.OrdinalIgnoreCase))
            return Language.Persian;
        return Language.English;
    }
}
