using Microsoft.AspNetCore.Identity;
using Bodokado.Domain.Entities.Users;
using System.Security.Claims;

namespace Bodokado.Application.Common.Auth.Interfaces;

public interface IJwtService
{
    Task<string> GenerateAccessToken(User user, UserManager<User> userManager, Guid sessionId, string? activeRole = null, IEnumerable<Claim>? extraClaims = null);
    DateTime GetAccessTokenExpiry();
    DateTime GetRefreshTokenExpiry();
}
