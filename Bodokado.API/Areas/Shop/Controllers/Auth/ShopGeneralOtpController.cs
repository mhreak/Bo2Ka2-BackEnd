using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Constants;

namespace Bodokado.API.Areas.Shop.Controllers.Auth;

[ApiController]
[Route(ApiRoutes.Shop.Auth)]
[Tags("Shop Auth")]
public class ShopGeneralOtpController : ControllerBase
{
    private readonly IGeneralOtpAuthService _generalOtpAuthService;
    private readonly IResponseLocalizer _responseLocalizer;

    public ShopGeneralOtpController(IGeneralOtpAuthService generalOtpAuthService, IResponseLocalizer responseLocalizer)
    {
        _generalOtpAuthService = generalOtpAuthService;
        _responseLocalizer = responseLocalizer;
    }

    [AllowAnonymous]
    [HttpPost("send-otp-general")]
    public async Task<IActionResult> SendOtp(GeneralOtpSendRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _generalOtpAuthService.SendOtpAsync(request, cancellationToken);
        return Ok(ApiResult.Success(result, await _responseLocalizer.LocalizeAsync(MessageKeys.OtpSent)));
    }

    [AllowAnonymous]
    [HttpPost("submit-otp-general")]
    public async Task<IActionResult> SubmitOtp(GeneralOtpSubmitRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _generalOtpAuthService.SubmitOtpAsync(request, RoleNames.Shop, cancellationToken);
        return Ok(ApiResult.Success(result, await _responseLocalizer.LocalizeAsync(MessageKeys.LoginSuccess)));
    }
}