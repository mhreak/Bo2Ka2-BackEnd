// Bodokado.API/Areas/OrganizationAdmin/Controllers/GiftCampaigns/GiftCampaignController.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.App.OrganizationAdminModule.GiftCampaigns.DTOs;
using Bodokado.Application.App.OrganizationAdminModule.GiftCampaigns.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Constants;

namespace Bodokado.API.Areas.OrganizationAdmin.Controllers;

[ApiController]
[Route(ApiRoutes.OrganizationAdmin.GiftCampaigns)]
[Authorize(Roles = RoleNames.AdminOrganization)]
[Tags("Organization Admin - Gift Campaigns")]
public class GiftCampaignController : ControllerBase
{
    private readonly IGiftCampaignService _service;
    private readonly IResponseLocalizer _responseLocalizer;

    public GiftCampaignController(
        IGiftCampaignService service,
        IResponseLocalizer responseLocalizer)
    {
        _service = service;
        _responseLocalizer = responseLocalizer;
    }

    private Guid CurrentOrganizationId
    {
        get
        {
            var value = User.FindFirstValue("organization_id");
            if (string.IsNullOrWhiteSpace(value) || !Guid.TryParse(value, out var orgId))
                throw new UnauthorizedAccessException(MessageKeys.NoAccess);
            return orgId;
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query, CancellationToken ct)
    {
        var result = await _service.GetAllAsync(CurrentOrganizationId, query, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.GiftCampaignsRetrieved);
        return Ok(ApiResult.Success(result, message));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(CurrentOrganizationId, id, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.GiftCampaignRetrieved);
        return Ok(ApiResult.Success(result, message));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateGiftCampaignRequestDto request,
        CancellationToken ct)
    {
        var result = await _service.CreateAsync(CurrentOrganizationId, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.GiftCampaignCreated);
        return Ok(ApiResult.Success(result, message));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateGiftCampaignRequestDto request,
        CancellationToken ct)
    {
        var result = await _service.UpdateAsync(CurrentOrganizationId, id, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.GiftCampaignUpdated);
        return Ok(ApiResult.Success(result, message));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(CurrentOrganizationId, id, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.GiftCampaignDeleted);
        return Ok(ApiResult.Success(message));
    }

    [HttpPost("{id:guid}/codes/generate")]
    public async Task<IActionResult> GenerateCodes(
        Guid id,
        [FromBody] GenerateGiftCodesRequestDto request,
        CancellationToken ct)
    {
        var data = await _service.GenerateCodesAsync(CurrentOrganizationId, id, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.GiftCodesGenerated);
        return Ok(ApiResult.Success(data, message));
    }

    /// <summary>لیست کدهای یک کمپین</summary>
    [HttpGet("{id:guid}/codes")]
    public async Task<IActionResult> GetCodes(
        Guid id,
        [FromQuery] bool? onlyUnused,
        CancellationToken ct)
    {
        var data = await _service.GetCodesAsync(CurrentOrganizationId, id, onlyUnused, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.GiftCodesRetrieved);
        return Ok(ApiResult.Success(data, message));
    }
}