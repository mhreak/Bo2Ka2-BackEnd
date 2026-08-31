using Microsoft.AspNetCore.Identity;
using Bodokado.Application.App.Auth.DTOs;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Application.Common.Auth.Services;

public class LoginService : ILoginService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;

    public LoginService(UserManager<User> userManager, IJwtService jwtService, IRefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<AuthResultDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user is null)
            throw new UnauthorizedAccessException("نام کاربری یا رمز عبور اشتباه است.");
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            throw new UnauthorizedAccessException("نام کاربری یا رمز عبور اشتباه است.");
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? throw new UnauthorizedAccessException("نام کاربری یا رمز عبور اشتباه است.");
        var session = await _refreshTokenService.IssueAsync(user.Id, role);
        var accessToken = await _jwtService.GenerateAccessToken(user, _userManager, session.SessionId);
        return new AuthResultDto { AccessToken = accessToken, RefreshToken = session.RefreshToken, ExpiresAt = _jwtService.GetAccessTokenExpiry() };
    }
}
