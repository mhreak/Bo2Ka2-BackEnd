using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Filters;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Areas.Admin.Controllers;

[Authorize(Roles = Bodokado.Domain.Constants.RoleNames.Admin)]
[ApiController]
[TypeFilter(typeof(ControllerExceptionFilterAttribute))]
public abstract class AdminBaseController : ControllerBase
{
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId)
            && !Guid.TryParse(User.FindFirstValue("sub"), out userId))
            throw new UnauthorizedAccessException(MessageKeys.InvalidCredentials);
        return userId;
    }
}
