using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Otp;
using Bodokado.Application.Common.Settings;
using Bodokado.Domain.Entities.Users;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.Common.Auth.Services;

public class GeneralOtpAuthService : IGeneralOtpAuthService
{
    private readonly IOtpService _otpService;
    private readonly OtpSettings _otpSettings;
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;

    public GeneralOtpAuthService(
        IOtpService otpService,
        IOptions<OtpSettings> otpSettings,
        IUserRepository userRepository,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService)
    {
        _otpService = otpService;
        _otpSettings = otpSettings.Value;
        _userRepository = userRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<SendOtpForAuthResponseDto> SendOtpAsync(GeneralOtpSendRequestDto request, CancellationToken cancellationToken = default)
    {
        var result = await _otpService.GenerateAndSendAsync(request.Mobile, OtpChannel.Sms, cancellationToken);
        if (result.Status == OtpGenerationStatus.CooldownActive)
            throw new BadRequestException(MessageKeys.OtpCooldownActive, "otp_cooldown_active", result.RetryAfterSeconds ?? 0);

        return new SendOtpForAuthResponseDto
        {
            Code = result.Code,
            ExpiresIn = _otpSettings.ExpirationMinutes * 60
        };
    }

    public async Task<GeneralOtpAuthResultDto> SubmitOtpAsync(GeneralOtpSubmitRequestDto request, string role, CancellationToken cancellationToken = default)
    {
        var verification = await _otpService.VerifyAsync(request.Mobile, OtpChannel.Sms, request.Code, cancellationToken);
        EnsureOtpValid(verification);

        var matchingUsers = await _userRepository.GetByMobileAsync(request.Mobile, cancellationToken);
        User? user = null;
        foreach (var matchingUser in matchingUsers)
        {
            if (await _userManager.IsInRoleAsync(matchingUser, role))
            {
                user = matchingUser;
                break;
            }
        }

        user ??= matchingUsers.FirstOrDefault();
        var isNewUser = user is null;
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        Bodokado.Application.Common.Models.RefreshTokenIssueResult? issuedToken = null;

        try
        {
            if (user is null)
            {
                var internationalMobile = $"+98{request.Mobile[1..]}";
                user = new User
                {
                    UserName = internationalMobile,
                    PhoneNumber = internationalMobile,
                    PhoneNumberConfirmed = true,
                    Mobile = request.Mobile,
                    IsActive = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(error => error.Description));
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
                {
                    var createRoleResult = await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                    if (!createRoleResult.Succeeded)
                    {
                        var errors = string.Join(", ", createRoleResult.Errors.Select(error => error.Description));
                        throw new BadRequestException(errors, "role_creation_failed");
                    }
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, role);
                if (!addRoleResult.Succeeded)
                {
                    var errors = string.Join(", ", addRoleResult.Errors.Select(error => error.Description));
                    throw new BadRequestException(errors, "role_assignment_failed");
                }
            }

            issuedToken = await _refreshTokenService.IssueAsync(user.Id, role, cancellationToken);
            var accessToken = await _jwtService.GenerateAccessToken(user, _userManager, issuedToken.SessionId, role);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new GeneralOtpAuthResultDto
            {
                AccessToken = accessToken,
                RefreshToken = issuedToken.RefreshToken,
                ExpiresAt = _jwtService.GetAccessTokenExpiry(),
                IsNewUser = isNewUser
            };
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            if (issuedToken is not null)
                await _refreshTokenService.RevokeAsync(issuedToken.RefreshToken, cancellationToken);
            throw;
        }
    }

    private static void EnsureOtpValid(OtpVerificationResult verification)
    {
        switch (verification.Status)
        {
            case OtpVerificationStatus.NotFoundOrExpired:
                throw new BadRequestException(MessageKeys.OtpNotFoundOrExpired, "otp_not_found_or_expired");
            case OtpVerificationStatus.MaxAttemptsExceeded:
                throw new BadRequestException(MessageKeys.OtpMaxAttemptsExceeded, "otp_max_attempts_exceeded");
            case OtpVerificationStatus.InvalidCode:
                throw new BadRequestException(MessageKeys.OtpInvalidCode, "otp_invalid_code", verification.RemainingAttempts ?? 0);
        }
    }
}