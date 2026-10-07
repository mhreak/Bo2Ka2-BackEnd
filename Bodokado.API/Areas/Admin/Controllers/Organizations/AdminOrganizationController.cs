// Areas/Admin/Controllers/Organizations/AdminOrganizationController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.Administrator.Organizations.DTOs;
using Bodokado.Application.Administrator.Organizations.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Constants;

namespace Bodokado.API.Areas.Admin.Controllers;

[ApiController]
[Route(ApiRoutes.Admin.Organizations)]
[Authorize(Roles = RoleNames.Admin)]
[Tags("Admin Organizations")]
public class AdminOrganizationController : ControllerBase
{
    private readonly IOrganizationService _organizationService;
    private readonly IResponseLocalizer _responseLocalizer;

    public AdminOrganizationController(
        IOrganizationService organizationService,
        IResponseLocalizer responseLocalizer)
    {
        _organizationService = organizationService;
        _responseLocalizer = responseLocalizer;
    }

    /// <summary>لیست سازمان‌ها</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? onlyActive, CancellationToken ct)
    {
        var data = await _organizationService.GetAllAsync(onlyActive, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OrganizationsRetrieved);
        return Ok(ApiResult.Success(data, message));
    }

    /// <summary>جزئیات یک سازمان</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var data = await _organizationService.GetByIdAsync(id, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OrganizationRetrieved);
        return Ok(ApiResult.Success(data, message));
    }

    /// <summary>ایجاد سازمان</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationRequestDto request, CancellationToken ct)
    {
        var data = await _organizationService.CreateAsync(request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OrganizationCreated);
        return Ok(ApiResult.Success(data, message));
    }

    /// <summary>ویرایش سازمان</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrganizationRequestDto request, CancellationToken ct)
    {
        var data = await _organizationService.UpdateAsync(id, request, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OrganizationUpdated);
        return Ok(ApiResult.Success(data, message));
    }

    /// <summary>حذف نرم سازمان</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _organizationService.DeleteAsync(id, ct);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.OrganizationDeleted);
        return Ok(ApiResult.Success(message));
    }
}