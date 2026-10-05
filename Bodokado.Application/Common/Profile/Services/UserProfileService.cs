using Microsoft.AspNetCore.Identity;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Profile.DTOs;
using Bodokado.Application.Common.Profile.Interfaces;
using Bodokado.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Bodokado.Application.Common.Profile.Services;

public class UserProfileService : IUserProfileService
{
    private readonly UserManager<User> _userManager;

    public UserProfileService(UserManager<User> userManager) => _userManager = userManager;

    private async Task<User> LoadUserWithProfileAsync(Guid userId, CancellationToken ct)
    {
        var user = await _userManager.Users
            .Include(u => u.City)
            .Include(u => u.AvatarFile)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null || user.IsDeleted)
            throw new BadRequestException(MessageKeys.UserNotFound, "user_not_found");

        if (!user.IsActive)
            throw new BadRequestException(MessageKeys.AccountInactive, "account_inactive");

        return user;
    }
    public async Task<UserProfileDto> GetMeAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await LoadUserWithProfileAsync(userId, ct);
        return await MapAsync(user);
    }

    public async Task<UserProfileDto> UpdateMeAsync(Guid userId, UpdateProfileRequestDto request, CancellationToken ct = default)
    {
       var user = await LoadUserWithProfileAsync(userId, ct);

        if (!string.IsNullOrWhiteSpace(request.FirstName))
            user.FirstName = request.FirstName.Trim();

        if (!string.IsNullOrWhiteSpace(request.LastName))
            user.LastName = request.LastName.Trim();

        if (!string.IsNullOrWhiteSpace(request.NationalCode))
        {
            var nc = request.NationalCode.Trim();
            if (nc.Length != 10 || !nc.All(char.IsDigit))
                throw new BadRequestException(MessageKeys.InvalidNationalCode, "invalid_national_code");
            user.NationalCode = nc;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim();
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing is not null && existing.Id != userId)
                throw new BadRequestException(MessageKeys.UsernameAlreadyExists, "email_already_exists");
            user.Email = email;
            user.NormalizedEmail = _userManager.NormalizeEmail(email);
        }

        if (request.BirthDate.HasValue)
            user.BirthDate = request.BirthDate.Value.Date;

        if (request.Address is not null)
            user.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();

        if (request.CityId.HasValue)
            user.CityId = request.CityId;

        if (request.Latitude.HasValue)
            user.Latitude = request.Latitude;

        if (request.Longitude.HasValue)
            user.Longitude = request.Longitude;

        if (request.AvatarFileId.HasValue)
        {
            // در صورت داشتن IFileAssetRepository مالکیت را چک کن
            user.AvatarFileId = request.AvatarFileId;
        }
        if (!string.IsNullOrWhiteSpace(request.ShamsiBirthDate))
            user.ShamsiBirthDate = request.ShamsiBirthDate.Trim();

        if (request.Gender.HasValue)
            user.Gender = request.Gender.Value;

        user.UpdatedAt = DateTime.UtcNow;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException(
                string.Join(", ", result.Errors.Select(e => e.Description)),
                "profile_update_failed");

        user = await LoadUserWithProfileAsync(userId, ct);
        return await MapAsync(user);
    }

    public async Task SetPasswordAsync(Guid userId, SetPasswordRequestDto request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new BadRequestException(MessageKeys.UserNotFound, "user_not_found");

        if (await _userManager.HasPasswordAsync(user))
            throw new BadRequestException(MessageKeys.PasswordAlreadySet, "password_already_set_use_change");

        var result = await _userManager.AddPasswordAsync(user, request.NewPassword);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)), "set_password_failed");
    }

    private async Task<UserProfileDto> MapAsync(User user)
{
    var roles = await _userManager.GetRolesAsync(user);
    return new UserProfileDto
    {
        Id = user.Id,
        PhoneNumber = user.PhoneNumber,
        FirstName = user.FirstName,
        LastName = user.LastName,
        NationalCode = user.NationalCode,
        Email = user.Email,
        BirthDate = user.BirthDate,
        ShamsiBirthDate = user.ShamsiBirthDate,
        Gender = user.Gender,
        Address = user.Address,
        CityId = user.CityId,
        CityName = user.City?.Name,
        Latitude = user.Latitude,
        Longitude = user.Longitude,
        AvatarFileId = user.AvatarFileId,
        AvatarPath = user.AvatarFile?.Path,
        WalletCredit = user.WalletCredit,
        IsActive = user.IsActive,
        Roles = roles.ToList(),
        HasPassword = await _userManager.HasPasswordAsync(user)
    };
}
}
