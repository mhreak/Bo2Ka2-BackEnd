using Bodokado.Domain.Entities.Users;

namespace Bodokado.Application.Common.Profile.Interfaces;

public interface IUserProfileRepository
{
    Task<User?> GetUserWithProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateAvatarAsync(Guid userId, Guid fileAssetId, CancellationToken cancellationToken = default);
    Task UpdateCoverAsync(Guid userId, Guid fileAssetId, CancellationToken cancellationToken = default);
}
