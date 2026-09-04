using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.Common.Auth;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Application.Common.Auth.Services;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Areas.Customer.Controllers;

[ApiController]
[Route(ApiRoutes.Customer.Auth)]
[Tags("Customer Auth")]
public class CustomerAuthController : ControllerBase
{
    private readonly RoleAuthCore _roleAuthCore;
    private readonly IRegisterSendOtpService _registerSendOtpService;
    private readonly IRefreshAccessTokenService _refreshAccessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IResponseLocalizer _responseLocalizer;
    private const string UserRole = "User";

    public CustomerAuthController(
        RoleAuthCore roleAuthCore,
        IRegisterSendOtpService registerSendOtpService,
        IRefreshAccessTokenService refreshAccessTokenService,
        IRefreshTokenService refreshTokenService,
        IResponseLocalizer responseLocalizer)
    {
        _roleAuthCore = roleAuthCore;
        _registerSendOtpService = registerSendOtpService;
        _refreshAccessTokenService = refreshAccessTokenService;
        _refreshTokenService = refreshTokenService;
        _responseLocalizer = responseLocalizer;
    }

    /// <summary>ارسال OTP برای ثبت‌نام</summary>
    [AllowAnonymous]
    [HttpPost("register-send-otp")]
    public async Task<IActionResult> RegisterSendOtp(RegisterSendOtpRequestDto request)
    {
        var result = await _registerSendOtpService.ExecuteAsync(request);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OtpSent);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>ارسال OTP برای ورود</summary>
    [AllowAnonymous]
    [HttpPost("send-otp-for-login")]
    public async Task<IActionResult> SendOtpForLogin(SendOtpForAuthRequestDto request)
    {
        var result = await _roleAuthCore.SendOtpForLoginAsync(request.PhoneNumber, UserRole);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OtpSent);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>ورود با OTP</summary>
    [AllowAnonymous]
    [HttpPost("login-otp")]
    public async Task<IActionResult> LoginOtp(LoginOtpRequestDto request)
    {
        var result = await _roleAuthCore.LoginOtpAsync(request.PhoneNumber, request.Code, UserRole);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.LoginSuccess);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>ورود با نام کاربری و رمز</summary>
    [AllowAnonymous]
    [HttpPost("login-by-password")]
    public async Task<IActionResult> LoginByPassword(LoginByPasswordRequestDto request)
    {
        var result = await _roleAuthCore.LoginByPasswordAsync(request.Username, request.Password, UserRole);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.LoginSuccess);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>تکمیل ثبت‌نام با موبایل + OTP (ساخت کاربر + توکن)</summary>
    [AllowAnonymous]
    [HttpPost("register-by-mobile")]
    public async Task<IActionResult> RegisterByMobile(RegisterByMobileRequestDto request)
    {
        var result = await _roleAuthCore.RegisterByMobileAsync(request.PhoneNumber, request.Code, UserRole);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.RegisterSuccess);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>ورود / ثبت‌نام با Google</summary>
    [AllowAnonymous]
    [HttpPost("google")]
    public async Task<IActionResult> Google(GoogleAuthRequestDto request)
    {
        var result = await _roleAuthCore.GoogleAuthAsync(request.IdToken, UserRole);
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

    /// <summary>تغییر رمز عبور (نیاز به JWT)</summary>
    [Authorize(Roles = "User")]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto request, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _roleAuthCore.ChangePasswordAsync(userId, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.PasswordChanged);
        return Ok(ApiResult.Success(result, message));
    }

    /// <summary>خروج و ابطال Refresh Token</summary>
    [Authorize(Roles = "User")]
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
