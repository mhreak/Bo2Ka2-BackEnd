using Microsoft.AspNetCore.Identity;
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Application.Common.Auth.Interfaces;

public interface IJwtService
{
    Task<string> GenerateAccessToken(User user, UserManager<User> userManager, Guid sessionId, string? activeRole = null);
    DateTime GetAccessTokenExpiry();
    DateTime GetRefreshTokenExpiry();
}
