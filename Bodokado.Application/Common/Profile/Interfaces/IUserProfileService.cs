using Bodokado.Application.Common.Profile.DTOs;

namespace Bodokado.Application.Common.Profile.Interfaces;

public interface IUserProfileService
{
    Task<UserProfileDto> GetMeAsync(Guid userId, CancellationToken ct = default);
    Task<UserProfileDto> UpdateMeAsync(Guid userId, UpdateProfileRequestDto request, CancellationToken ct = default);
    Task SetPasswordAsync(Guid userId, SetPasswordRequestDto request, CancellationToken ct = default);
}
