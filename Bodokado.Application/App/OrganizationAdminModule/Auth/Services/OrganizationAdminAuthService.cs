using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Bodokado.Application.App.OrganizationAdminModule.Auth.Interfaces;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Auth.Services;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Constants;
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Application.App.OrganizationAdminModule.Auth.Services;

public class OrganizationAdminAuthService : IOrganizationAdminAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleAuthCore _core;

    public OrganizationAdminAuthService(UserManager<User> userManager, RoleAuthCore core)
    {
        _userManager = userManager;
        _core = core;
    }

    public async Task<AuthResultDto> LoginByPasswordAsync(LoginByPasswordRequestDto request, CancellationToken ct = default)
    {
        var username = request.Username?.Trim();
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(request.Password))
            throw new UnauthorizedAccessException(MessageKeys.InvalidCredentials);

        var user = await _userManager.FindByNameAsync(username);
        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException(MessageKeys.InvalidCredentials);

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException(MessageKeys.InvalidCredentials);

        if (!await _userManager.IsInRoleAsync(user, RoleNames.AdminOrganization))
            throw new UnauthorizedAccessException(MessageKeys.NoAccess);

        if (!user.OrganizationId.HasValue)
            throw new UnauthorizedAccessException(MessageKeys.NoAccess);

        var extraClaims = new[]
        {
            new Claim("organization_id", user.OrganizationId.Value.ToString())
        };

        return await _core.IssueTokensAsync(user, RoleNames.AdminOrganization, extraClaims);
    }
}