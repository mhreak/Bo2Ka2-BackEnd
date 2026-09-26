// API/Areas/Admin/Controllers/Stories/StoryController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.AdminModule.Stories.DTOs;
using Bodokado.Application.App.AdminModule.Stories.Interfaces;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Areas.Admin.Controllers;

[ApiController]
[Route(ApiRoutes.Admin.Stories)]
[Authorize(Roles = "Admin")]
[Tags("Admin Stories")]
public class StoryController : ControllerBase
{
    private readonly IStoryService _storyService;
    private readonly IResponseLocalizer _responseLocalizer;

    public StoryController(IStoryService storyService, IResponseLocalizer responseLocalizer)
    {
        _storyService = storyService;
        _responseLocalizer = responseLocalizer;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] StoryListQuery query, CancellationToken ct)
    {
        var data = await _storyService.GetAllForAdminAsync(query, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.StoriesRetrieved);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var data = await _storyService.GetByIdAsync(id, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.StoryRetrieved);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStoryRequestDto request, CancellationToken ct)
    {
        var data = await _storyService.CreateAsync(request, isAdmin: true, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.StoryCreated);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStoryRequestDto request, CancellationToken ct)
    {
        var data = await _storyService.UpdateAsync(id, request, isAdmin: true, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.StoryUpdated);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _storyService.DeleteAsync(id, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.StoryDeleted);
        return Ok(ApiResult.Success(message));
    }
}