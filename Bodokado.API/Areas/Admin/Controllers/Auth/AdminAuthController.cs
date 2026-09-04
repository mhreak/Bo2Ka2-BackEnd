using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.Administrator.Auth.DTOs;
using Bodokado.Application.Administrator.Auth.Interfaces;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Areas.Admin.Controllers;

[ApiController]
[Route(ApiRoutes.Admin.Auth)]
[Tags("Admin Auth")]
public class AdminAuthController : AdminBaseController
{
    private readonly IAdminRegisterService _registerService;
    private readonly IAdminLoginService _loginService;
    private readonly IRefreshAccessTokenService _refreshAccessTokenService;
    private readonly IResponseLocalizer _responseLocalizer;

    public AdminAuthController(
        IAdminRegisterService registerService,
        IAdminLoginService loginService,
        IRefreshAccessTokenService refreshAccessTokenService,
        IResponseLocalizer responseLocalizer)
    {
        _registerService = registerService;
        _loginService = loginService;
        _refreshAccessTokenService = refreshAccessTokenService;
        _responseLocalizer = responseLocalizer;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(AdminRegisterRequestDto request)
    {
        await _registerService.RegisterAsync(request);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.AdminRegistered);
        return Ok(ApiResult.Success(message));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(AdminLoginRequestDto request)
    {
        var result = await _loginService.LoginAsync(request);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.LoginSuccess);
        return Ok(ApiResult.Success(result, message));
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
    {
        var result = await _refreshAccessTokenService.RefreshAsync(request);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.LoginSuccess);
        return Ok(ApiResult.Success(result, message));
    }
}
