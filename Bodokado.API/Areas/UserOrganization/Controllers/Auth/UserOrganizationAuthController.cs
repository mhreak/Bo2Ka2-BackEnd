using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.UserOrganizationModule.Auth.Interfaces;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Auth;

namespace Bodokado.API.Areas.UserOrganization.Controllers;

[ApiController]
[Route(ApiRoutes.UserOrganization.Auth)]
[Tags("User Organization Auth")]
public class UserOrganizationAuthController : ControllerBase
{
    private readonly IUserOrganizationAuthService _userOrganizationAuthService;
    private readonly IRefreshAccessTokenService _refreshAccessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IResponseLocalizer _responseLocalizer;

    public UserOrganizationAuthController(
        IUserOrganizationAuthService userOrganizationAuthService,
        IRefreshAccessTokenService refreshAccessTokenService,
        IRefreshTokenService refreshTokenService,
        IResponseLocalizer responseLocalizer)
    {
        _userOrganizationAuthService = userOrganizationAuthService;
        _refreshAccessTokenService = refreshAccessTokenService;
        _refreshTokenService = refreshTokenService;
        _responseLocalizer = responseLocalizer;
    }

    /// <summary>ارسال OTP برای ثبت‌نام سازمانی</summary>
    [AllowAnonymous]
    [HttpPost("register-send-otp")]
    public async Task<IActionResult> RegisterSendOtp(RegisterSendOtpRequestDto request)
    {
        var result = await _userOrganizationAuthService.SendOtpForRegisterAsync(request);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OtpSent);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>ارسال OTP برای ورود سازمانی</summary>
    [AllowAnonymous]
    [HttpPost("send-otp-for-login")]
    public async Task<IActionResult> SendOtpForLogin(SendOtpForAuthRequestDto request)
    {
        var result = await _userOrganizationAuthService.SendOtpForLoginAsync(request);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OtpSent);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>ورود سازمانی با OTP</summary>
    [AllowAnonymous]
    [HttpPost("login-otp")]
    public async Task<IActionResult> LoginOtp(LoginOtpRequestDto request)
    {
        var result = await _userOrganizationAuthService.LoginOtpAsync(request);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.LoginSuccess);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>ورود سازمانی با نام کاربری و رمز</summary>
    [AllowAnonymous]
    [HttpPost("login-by-password")]
    public async Task<IActionResult> LoginByPassword(LoginByPasswordRequestDto request)
    {
        var result = await _userOrganizationAuthService.LoginByPasswordAsync(request);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.LoginSuccess);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>تکمیل ثبت‌نام سازمانی با موبایل + OTP (ساخت کاربر + توکن، شروع ویزارد ثبت‌نام)</summary>
    [AllowAnonymous]
    [HttpPost("register-by-mobile")]
    public async Task<IActionResult> RegisterByMobile(RegisterByMobileRequestDto request)
    {
        var result = await _userOrganizationAuthService.RegisterByMobileAsync(request);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.RegisterSuccess);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>ورود / ثبت‌نام سازمانی با Google</summary>
    [AllowAnonymous]
    [HttpPost("google")]
    public async Task<IActionResult> Google(GoogleAuthRequestDto request)
    {
        var result = await _userOrganizationAuthService.GoogleAuthAsync(request);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.LoginSuccess);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>تمدید Access Token</summary>
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
    {
        var refreshToken = !string.IsNullOrWhiteSpace(request.RefreshToken)
            ? request.RefreshToken
            : Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            var missingTokenMessage = await _responseLocalizer.LocalizeAsync(MessageKeys.InvalidCredentials);
            return BadRequest(ApiResult.BadRequest(missingTokenMessage));
        }

        var result = await _refreshAccessTokenService.RefreshAsync(new RefreshTokenRequestDto { RefreshToken = refreshToken });
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.TokenRefreshed);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>تغییر رمز عبور سازمانی (نیاز به JWT)</summary>
    [Authorize(Roles = Bodokado.Domain.Constants.RoleNames.UserOrganization)]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto request, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _userOrganizationAuthService.ChangePasswordAsync(userId, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.PasswordChanged);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>خروج و ابطال Refresh Token</summary>
    [Authorize(Roles = Bodokado.Domain.Constants.RoleNames.UserOrganization)]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequestDto request)
    {
        var refreshToken = !string.IsNullOrWhiteSpace(request.RefreshToken)
            ? request.RefreshToken
            : Request.Cookies["refreshToken"];

        if (!string.IsNullOrWhiteSpace(refreshToken))
            await _refreshTokenService.RevokeAsync(refreshToken);

        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.LogoutSuccess);
        return Ok(ApiResult.Success(message));
    }
}