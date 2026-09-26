// API/Areas/Customer/Controllers/Stories/StoryController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.AdminModule.Stories.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Enums;

namespace Bodokado.API.Areas.Customer.Controllers;

[ApiController]
[Route(ApiRoutes.Customer.Stories)]
[AllowAnonymous]
[Tags("Stories")]
public class StoryController : ControllerBase
{
    private readonly IStoryService _storyService;
    private readonly IResponseLocalizer _responseLocalizer;

    public StoryController(IStoryService storyService, IResponseLocalizer responseLocalizer)
    {
        _storyService = storyService;
        _responseLocalizer = responseLocalizer;
    }

    /// <summary>
    /// استوری‌های قابل‌نمایش برای اپ
    /// showPlace: 1=HomeTop | 2=HomeMiddle | 3=ShopPage
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetVisible(
        [FromQuery] StoryShowPlace? showPlace = null,
        [FromQuery] Guid? shopId = null,
        CancellationToken ct = default)
    {
        showPlace ??= StoryShowPlace.ApplicationHomePageTopStorySection;

        var data = await _storyService.GetVisibleAsync(showPlace, shopId, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.StoriesRetrieved);
        return Ok(ApiResult.Success(data, message));
    }
}