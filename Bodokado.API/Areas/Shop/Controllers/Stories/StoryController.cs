// API/Areas/Shop/Controllers/Stories/StoryController.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.AdminModule.Stories.DTOs;
using Bodokado.Application.App.AdminModule.Stories.Interfaces;
using Bodokado.Application.App.ShopModule.Registration.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Enums;

namespace Bodokado.API.Areas.Shop.Controllers;

[ApiController]
[Route(ApiRoutes.Shop.Stories)]
[Authorize(Roles = "Shop")]
[Tags("Shop Stories")]
public class StoryController : ControllerBase
{
    private readonly IStoryService _storyService;
    private readonly IShopRepository _shopRepository;
    private readonly IResponseLocalizer _responseLocalizer;

    public StoryController(
        IStoryService storyService,
        IShopRepository shopRepository,
        IResponseLocalizer responseLocalizer)
    {
        _storyService = storyService;
        _shopRepository = shopRepository;
        _responseLocalizer = responseLocalizer;
    }

    private Guid GetUserId()
        => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private async Task<Guid> GetShopIdAsync(CancellationToken ct)
    {
        var shop = await _shopRepository.GetByUserIdAsync(GetUserId(), ct);
        if (shop is null)
            throw new BadRequestException(MessageKeys.ShopNotFound, "shop_not_found");
        return shop.Id;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyStories(CancellationToken ct)
    {
        var shopId = await GetShopIdAsync(ct);
        // از طریق سرویس ادمین با فیلتر shop — یا متد جدا در سرویس
        var data = await _storyService.GetAllForAdminAsync(
            new StoryListQuery { ShopId = shopId, Page = 1, PageSize = 100 }, ct);

        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.StoriesRetrieved);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var shopId = await GetShopIdAsync(ct);
        var data = await _storyService.GetByIdAsync(id, ct);

        if (data.ShopId != shopId)
            throw new NotFoundException(MessageKeys.StoryNotFound, "story_not_found");

        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.StoryRetrieved);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStoryRequestDto request, CancellationToken ct)
    {
        var shopId = await GetShopIdAsync(ct);
        request.ShopId = shopId;
        // فروشگاه فقط برای صفحه فروشگاه استوری می‌سازد (اختیاری — اگر قانون دیگری داری عوض کن)
        if (request.ShowPlace == 0)
            request.ShowPlace = StoryShowPlace.ShopPage;

        var data = await _storyService.CreateAsync(request, isAdmin: false, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.StoryCreated);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStoryRequestDto request, CancellationToken ct)
    {
        var shopId = await GetShopIdAsync(ct);
        var existing = await _storyService.GetByIdAsync(id, ct);

        if (existing.ShopId != shopId)
            throw new NotFoundException(MessageKeys.StoryNotFound, "story_not_found");

        request.ShopId = shopId;
        // فروشگاه نباید این دو را عوض کند
        request.DisabledByAdmin = null;
        request.IsActiveByAdmin = null;

        var data = await _storyService.UpdateAsync(id, request, isAdmin: false, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.StoryUpdated);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var shopId = await GetShopIdAsync(ct);
        var existing = await _storyService.GetByIdAsync(id, ct);

        if (existing.ShopId != shopId)
            throw new NotFoundException(MessageKeys.StoryNotFound, "story_not_found");

        await _storyService.DeleteAsync(id, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.StoryDeleted);
        return Ok(ApiResult.Success(message));
    }
}