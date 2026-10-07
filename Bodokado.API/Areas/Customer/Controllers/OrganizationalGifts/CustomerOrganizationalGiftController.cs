using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.CustomerModule.OrganizationalGifts.DTOs;
using Bodokado.Application.App.CustomerModule.OrganizationalGifts.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Constants;

namespace Bodokado.API.Areas.Customer.Controllers.OrganizationalGifts;

[ApiController]
[Tags("CustomerOrganizationalGifts")]
[Route(ApiRoutes.Customer.OrganizationalGifts)]
[Authorize(Roles = RoleNames.Customer)]
public class CustomerOrganizationalGiftController : ControllerBase
{
    private readonly ICustomerOrganizationalGiftService _giftService;
    private readonly IResponseLocalizer _responseLocalizer;

    public CustomerOrganizationalGiftController(
        ICustomerOrganizationalGiftService giftService,
        IResponseLocalizer responseLocalizer)
    {
        _giftService = giftService;
        _responseLocalizer = responseLocalizer;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>اعتبارسنجی و ثبت کد هدیه سازمانی برای مشتری</summary>
    [HttpPost("codes/verify")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyGiftCodeRequestDto request, CancellationToken ct)
    {
        var result = await _giftService.VerifyCodeAsync(CurrentUserId, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.GiftCodeVerified);
        return Ok(ApiResult.Success(result, message));
    }
}
