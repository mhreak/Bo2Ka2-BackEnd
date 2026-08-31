using Bodokado.Application.Common.Models;

namespace Bodokado.Application.Common.Auth;

public interface IRefreshTokenService
{
    Task<RefreshTokenIssueResult> IssueAsync(Guid userId, string role, CancellationToken cancellationToken = default);
    Task<RefreshTokenRotationResult> RotateAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAllByRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);
}
