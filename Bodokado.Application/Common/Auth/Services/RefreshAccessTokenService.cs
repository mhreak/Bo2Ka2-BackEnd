using Microsoft.AspNetCore.Identity;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Models;
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Application.Common.Auth.Services;

public class RefreshAccessTokenService : IRefreshAccessTokenService
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;

    public RefreshAccessTokenService(IRefreshTokenService refreshTokenService, UserManager<User> userManager, IJwtService jwtService)
    {
        _refreshTokenService = refreshTokenService;
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<AuthResultDto> RefreshAsync(RefreshTokenRequestDto request)
    {
        var rotation = await _refreshTokenService.RotateAsync(request.RefreshToken);
        if (rotation.Status == RefreshTokenRotationStatus.Invalid)
            throw new BadRequestException(MessageKeys.InvalidCredentials, "invalid_refresh_token");
        var user = await _userManager.FindByIdAsync(rotation.UserId.ToString());
        if (user is null)
            throw new BadRequestException(MessageKeys.UserNotFound, "user_not_found");
        var accessToken = await _jwtService.GenerateAccessToken(user, _userManager, rotation.SessionId, rotation.Role);
        return new AuthResultDto { AccessToken = accessToken, RefreshToken = rotation.NewRefreshToken!, ExpiresAt = _jwtService.GetAccessTokenExpiry() };
    }
}
