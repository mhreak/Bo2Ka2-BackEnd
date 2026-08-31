using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.ShopModule.Registration.DTOs;
using Bodokado.Application.App.ShopModule.Registration.Interfaces;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Controllers.Shop;

[ApiController]
[Route(ApiRoutes.Shop.Registration)]
[Authorize(Roles = "Shop")]
[Tags("Shop Registration")]
public class ShopRegistrationController : ControllerBase
{
    private readonly IShopRegistrationService _registrationService;
    private readonly IResponseLocalizer _responseLocalizer;

    public ShopRegistrationController(IShopRegistrationService registrationService, IResponseLocalizer responseLocalizer)
    {
        _registrationService = registrationService;
        _responseLocalizer = responseLocalizer;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>جستجو و لیست انواع فروشگاه (دسته‌بندی‌ها)</summary>
    [AllowAnonymous]
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories([FromQuery] string? search, CancellationToken ct)
    {
        var data = await _registrationService.GetCategoriesAsync(search, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ShopCategoriesRetrieved);
        return Ok(ApiResult.Success(data, message));
    }

    /// <summary>وضعیت و اطلاعات فعلی ثبت‌نام فروشگاه من</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var profile = await _registrationService.GetMyShopAsync(CurrentUserId, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ShopProfileRetrieved);
        return Ok(ApiResult.Success(profile, message));
    }

    /// <summary>مرحله ۱ - اطلاعات پایه (هویتی و نام/نوع فروشگاه)</summary>
    [HttpPost("step1/basic-info")]
    public async Task<IActionResult> SubmitBasicInfo(ShopBasicInfoRequestDto request, CancellationToken ct)
    {
        var profile = await _registrationService.SubmitBasicInfoAsync(CurrentUserId, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ShopBasicInfoSaved);
        return Ok(ApiResult.Success(profile, message));
    }

    /// <summary>مرحله ۲ - جزئیات فروشگاه (تصاویر، آدرس، موقعیت روی نقشه)</summary>
    [HttpPost("step2/shop-details")]
    public async Task<IActionResult> SubmitShopDetails(ShopDetailsRequestDto request, CancellationToken ct)
    {
        var profile = await _registrationService.SubmitShopDetailsAsync(CurrentUserId, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ShopDetailsSaved);
        return Ok(ApiResult.Success(profile, message));
    }

    /// <summary>مرحله ۳ - تایید نهایی (شماره شبا، قوانین مرجوعی، ساعات کاری) و ارسال برای بررسی</summary>
    [HttpPost("step3/final-confirmation")]
    public async Task<IActionResult> SubmitFinalConfirmation(ShopFinalConfirmationRequestDto request, CancellationToken ct)
    {
        var profile = await _registrationService.SubmitFinalConfirmationAsync(CurrentUserId, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ShopSubmittedForReview);
        return Ok(ApiResult.Success(profile, message));
    }
}