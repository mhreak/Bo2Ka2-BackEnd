using Microsoft.AspNetCore.Identity;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Profile.DTOs;
using Bodokado.Application.Common.Profile.Interfaces;
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Application.Common.Profile.Services;

public class UserProfileService : IUserProfileService
{
    private readonly UserManager<User> _userManager;

    public UserProfileService(UserManager<User> userManager) => _userManager = userManager;

    public async Task<UserProfileDto> GetMeAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new BadRequestException(MessageKeys.UserNotFound, "user_not_found");
        if (user.IsDeleted || !user.IsActive)
            throw new BadRequestException(MessageKeys.AccountInactive, "account_inactive");
        return await MapAsync(user);
    }

    public async Task<UserProfileDto> UpdateMeAsync(Guid userId, UpdateProfileRequestDto request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new BadRequestException(MessageKeys.UserNotFound, "user_not_found");

        if (!string.IsNullOrWhiteSpace(request.FirstName))
            user.FirstName = request.FirstName.Trim();
        if (!string.IsNullOrWhiteSpace(request.LastName))
            user.LastName = request.LastName.Trim();
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim();
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing is not null && existing.Id != userId)
                throw new BadRequestException(MessageKeys.UsernameAlreadyExists, "email_already_exists");
            user.Email = email;
        }

        user.UpdatedAt = DateTime.UtcNow;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)), "profile_update_failed");

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
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            Roles = roles.ToList(),
            HasPassword = await _userManager.HasPasswordAsync(user)
        };
    }
}
