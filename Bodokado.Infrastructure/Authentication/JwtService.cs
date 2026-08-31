using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Infrastructure.Authentication;

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(Microsoft.Extensions.Options.IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public async Task<string> GenerateAccessToken(User user, UserManager<User> userManager, Guid sessionId, string? activeRole = null)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("sid", sessionId.ToString())
        };
        if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            claims.Add(new(ClaimTypes.MobilePhone, user.PhoneNumber));
        if (!string.IsNullOrWhiteSpace(user.UserName))
            claims.Add(new(ClaimTypes.Name, user.UserName));
        var roles = activeRole is not null ? new[] { activeRole } : await userManager.GetRolesAsync(user);
        foreach (var role in roles)
            claims.Add(new(ClaimTypes.Role, role));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: GetAccessTokenExpiry(),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public DateTime GetAccessTokenExpiry() => DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);
    public DateTime GetRefreshTokenExpiry() => DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);
}
