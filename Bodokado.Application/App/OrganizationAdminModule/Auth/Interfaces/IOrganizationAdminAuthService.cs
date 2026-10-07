using Bodokado.Application.Common.Auth.DTOs;

namespace Bodokado.Application.App.OrganizationAdminModule.Auth.Interfaces;

public interface IOrganizationAdminAuthService
{
    Task<AuthResultDto> LoginByPasswordAsync(LoginByPasswordRequestDto request, CancellationToken ct = default);
}