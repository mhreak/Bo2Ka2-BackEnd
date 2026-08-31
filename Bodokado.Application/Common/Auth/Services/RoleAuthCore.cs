using Microsoft.AspNetCore.Identity;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Otp;
using Bodokado.Domain.Entities.Users;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.Common.Auth.Services;

public class RoleAuthCore
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IUserRepository _userRepository;
    private readonly IOtpService _otpService;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IGoogleTokenValidator _googleTokenValidator;
    private readonly OtpSettings _otpSettings;
    private bool isNewUser;

    public RoleAuthCore(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, IUserRepository userRepository, IOtpService otpService, IJwtService jwtService, IRefreshTokenService refreshTokenService, IGoogleTokenValidator googleTokenValidator, Microsoft.Extensions.Options.IOptions<OtpSettings> otpSettings)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userRepository = userRepository;
        _otpService = otpService;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _googleTokenValidator = googleTokenValidator;
        _otpSettings = otpSettings.Value;
    }

    public async Task<SendOtpForAuthResponseDto> SendOtpForLoginAsync(string phoneNumber, string requiredRole)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
        if (user is null || !await _userManager.IsInRoleAsync(user, requiredRole))
            throw new BadRequestException(MessageKeys.AccountNotFound, "account_not_found");
        if (!user.IsActive)
            throw new BadRequestException(MessageKeys.AccountInactive, "account_inactive");
        var result = await _otpService.GenerateAndSendAsync(phoneNumber, OtpChannel.Sms);
        if (result.Status == OtpGenerationStatus.CooldownActive)
            throw new BadRequestException(MessageKeys.OtpCooldownActive, "otp_cooldown_active");
        return new SendOtpForAuthResponseDto { Code = result.Code, ExpiresIn = _otpSettings.ExpirationMinutes * 60 };
    }

    public async Task<AuthResultDto> LoginOtpAsync(string phoneNumber, string code, string requiredRole)
    {
        var verify = await _otpService.VerifyAsync(phoneNumber, OtpChannel.Sms, code);
        EnsureOtpValid(verify);
        var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
        if (user is null || !await _userManager.IsInRoleAsync(user, requiredRole))
            throw new BadRequestException(MessageKeys.AccountNotFound, "account_not_found");
        if (!user.IsActive)
            throw new BadRequestException(MessageKeys.AccountInactive, "account_inactive");
        return await IssueTokensAsync(user, requiredRole);
    }

    public async Task<AuthResultDto> LoginByPasswordAsync(string username, string password, string requiredRole)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
            throw new UnauthorizedAccessException(MessageKeys.InvalidCredentials);
        if (!user.IsActive)
            throw new UnauthorizedAccessException(MessageKeys.AccountInactive);
        var validPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!validPassword)
            throw new UnauthorizedAccessException(MessageKeys.InvalidCredentials);
        if (!await _userManager.IsInRoleAsync(user, requiredRole))
            throw new UnauthorizedAccessException(MessageKeys.NoAccess);
        return await IssueTokensAsync(user, requiredRole);
    }

    public async Task<AuthResultDto> RegisterByMobileAsync(string phoneNumber, string code, string role)
    {
        var verify = await _otpService.VerifyAsync(phoneNumber, OtpChannel.Sms, code);
        EnsureOtpValid(verify);
        var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
        if (user is null)
        {
            user = new User { PhoneNumber = phoneNumber, UserName = phoneNumber, IsActive = true };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new BadRequestException(errors, "user_creation_failed");
            }
        }
        else if (!user.IsActive)
        {
            throw new BadRequestException(MessageKeys.AccountInactive, "account_inactive");
        }
        if (!await _userManager.IsInRoleAsync(user, role))
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
            await _userManager.AddToRoleAsync(user, role);
        }
        return await IssueTokensAsync(user, role);
    }

    public async Task<AuthResultDto> IssueTokensAsync(User user, string role)
    {
        var session = await _refreshTokenService.IssueAsync(user.Id, role);
        var accessToken = await _jwtService.GenerateAccessToken(user, _userManager, session.SessionId, role);
        return new AuthResultDto { AccessToken = accessToken, RefreshToken = session.RefreshToken, ExpiresAt = _jwtService.GetAccessTokenExpiry() };
    }

    private static void EnsureOtpValid(OtpVerificationResult verify)
    {
        switch (verify.Status)
        {
            case OtpVerificationStatus.NotFoundOrExpired:
                throw new BadRequestException(MessageKeys.OtpNotFoundOrExpired, "otp_not_found_or_expired");
            case OtpVerificationStatus.MaxAttemptsExceeded:
                throw new BadRequestException(MessageKeys.OtpMaxAttemptsExceeded, "otp_max_attempts_exceeded");
            case OtpVerificationStatus.InvalidCode:
                throw new BadRequestException(MessageKeys.OtpInvalidCode, "otp_invalid_code");
        }
    }

    public async Task<AuthResultDto> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new BadRequestException(MessageKeys.UserNotFound, "user_not_found");
        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)), "change_password_failed");
        var roles = await _userManager.GetRolesAsync(user);
        var revokeTasks = roles.Select(role => _refreshTokenService.RevokeAllByRoleAsync(userId, role, cancellationToken));
        await Task.WhenAll(revokeTasks);
        var primaryRole = roles.FirstOrDefault() ?? "User";
        return await IssueTokensAsync(user, primaryRole);
    }

    public async Task<GoogleAuthResultDto> GoogleAuthAsync(string idToken, string requiredRole)
    {
        var googleUser = await _googleTokenValidator.ValidateAsync(idToken);
        if (googleUser is null)
            throw new BadRequestException(MessageKeys.InvalidGoogleToken, "invalid_google_token");
        if (!googleUser.EmailVerified)
            throw new BadRequestException(MessageKeys.GoogleEmailNotVerified, "google_email_not_verified");
        var user = await _userRepository.GetByEmailAsync(googleUser.Email);
        if (user is null)
        {
            isNewUser = true;
            var userName = await GenerateUniqueUsernameAsync(googleUser.Email);
            user = new User
            {
                UserName = userName,
                Email = googleUser.Email,
                EmailConfirmed = true,
                FirstName = googleUser.FirstName,
                LastName = googleUser.LastName,
                IsActive = true
            };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new BadRequestException(MessageKeys.UnexpectedError, "google_user_creation_failed", errors);
            }
        }
        if (!user.IsActive)
            throw new BadRequestException(MessageKeys.AccountInactive, "account_inactive");
        if (!await _userManager.IsInRoleAsync(user, requiredRole))
        {
            if (!await _roleManager.RoleExistsAsync(requiredRole))
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = requiredRole });
            await _userManager.AddToRoleAsync(user, requiredRole);
        }
        var tokens = await IssueTokensAsync(user, requiredRole);
        return new GoogleAuthResultDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            ExpiresAt = tokens.ExpiresAt,
            IsNewUser = isNewUser,
            IsProfileCompleted = true
        };
    }

    private async Task<string> GenerateUniqueUsernameAsync(string email)
    {
        var baseUsername = email.Split('@')[0];
        var candidate = baseUsername;
        var suffix = 1;
        while (await _userManager.FindByNameAsync(candidate) is not null)
        {
            candidate = $"{baseUsername}{suffix}";
            suffix++;
        }
        return candidate;
    }
}
