using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Profile.DTOs;
using Bodokado.Application.Common.Profile.Interfaces;

namespace Bodokado.API.Controllers;

[ApiController]
[Tags("User")]
[Route(ApiRoutes.Generic.Users)]
[Authorize(Roles = "User")]
public class UserController : ControllerBase
{
    private readonly IUserProfileService _profileService;
    private readonly IResponseLocalizer _responseLocalizer;

    public UserController(IUserProfileService profileService, IResponseLocalizer responseLocalizer)
    {
        _profileService = profileService;
        _responseLocalizer = responseLocalizer;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>پروفایل کاربر جاری</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var profile = await _profileService.GetMeAsync(CurrentUserId, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProfileRetrieved);
        return Ok(ApiResult.Success(profile, message));
    }

    /// <summary>به‌روزرسانی پروفایل</summary>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateProfileRequestDto request, CancellationToken ct)
    {
        var profile = await _profileService.UpdateMeAsync(CurrentUserId, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ProfileUpdated);
        return Ok(ApiResult.Success(profile, message));
    }

    /// <summary>تنظیم اولین رمز عبور (برای کاربرانی که فقط با OTP ثبت‌نام کرده‌اند)</summary>
    [HttpPost("me/set-password")]
    public async Task<IActionResult> SetPassword(SetPasswordRequestDto request, CancellationToken ct)
    {
        await _profileService.SetPasswordAsync(CurrentUserId, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.PasswordSet);
        return Ok(ApiResult.Success(message));
    }
}
