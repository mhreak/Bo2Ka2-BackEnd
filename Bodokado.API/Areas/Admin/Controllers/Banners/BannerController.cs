// API/Areas/Admin/Controllers/Banners/BannerController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.Administrator.Banners.DTOs;
using Bodokado.Application.Administrator.Banners.Interfaces;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Areas.Admin.Controllers;

[ApiController]
[Route(ApiRoutes.Admin.Banners)]
[Authorize(Roles = Bodokado.Domain.Constants.RoleNames.Admin)]
[Tags("Admin Banners")]
public class BannerController : ControllerBase
{
    private readonly IBannerService _bannerService;
    private readonly IResponseLocalizer _responseLocalizer;

    public BannerController(IBannerService bannerService, IResponseLocalizer responseLocalizer)
    {
        _bannerService = bannerService;
        _responseLocalizer = responseLocalizer;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var data = await _bannerService.GetAllAsync(ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.BannersRetrieved);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var data = await _bannerService.GetByIdAsync(id, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.BannerRetrieved);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBannerRequestDto request, CancellationToken ct)
    {
        var data = await _bannerService.CreateAsync(request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.BannerCreated);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBannerRequestDto request, CancellationToken ct)
    {
        var data = await _bannerService.UpdateAsync(id, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.BannerUpdated);
        return Ok(ApiResult.Success(data, message));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _bannerService.DeleteAsync(id, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.BannerDeleted);
        return Ok(ApiResult.Success(message));
    }
}