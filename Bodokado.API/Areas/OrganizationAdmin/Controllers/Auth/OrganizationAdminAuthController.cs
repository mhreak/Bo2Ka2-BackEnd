using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.OrganizationAdminModule.Auth.Interfaces;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Areas.OrganizationAdmin.Controllers;

[ApiController]
[Route(ApiRoutes.OrganizationAdmin.Auth)]
[Tags("Organization Admin Auth")]
public class OrganizationAdminAuthController : ControllerBase
{
    private readonly IOrganizationAdminAuthService _authService;
    private readonly IResponseLocalizer _responseLocalizer;

    public OrganizationAdminAuthController(
        IOrganizationAdminAuthService authService,
        IResponseLocalizer responseLocalizer)
    {
        _authService = authService;
        _responseLocalizer = responseLocalizer;
    }

    /// <summary>ورود ادمین سازمان با نام کاربری و رمز</summary>
    [AllowAnonymous]
    [HttpPost("login-by-password")]
    public async Task<IActionResult> LoginByPassword(
        [FromBody] LoginByPasswordRequestDto request,
        CancellationToken ct)
    {
        var data = await _authService.LoginByPasswordAsync(request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.LoginSuccess);
        return Ok(ApiResult.Success(data, message));
    }
}