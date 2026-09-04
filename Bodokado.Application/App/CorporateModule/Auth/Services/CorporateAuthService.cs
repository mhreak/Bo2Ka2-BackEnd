using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Bodokado.Application.App.CorporateModule.Auth.Interfaces;
using Bodokado.Application.Common.Auth;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Application.Common.Auth.Services;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Otp;
using Bodokado.Domain.Entities.Users;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.CorporateModule.Auth.Services;

public class CorporateAuthService : ICorporateAuthService
{
    private const string Role = "Corporate";

    private readonly RoleAuthCore _core;
    private readonly IOtpService _otpService;
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly OtpSettings _otpSettings;

    public CorporateAuthService(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IOtpService otpService,
        IUserRepository userRepository,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        IGoogleTokenValidator googleTokenValidator,
        IOptions<OtpSettings> otpSettings)
    {
        _core = new RoleAuthCore(userManager, roleManager, userRepository, otpService, jwtService, refreshTokenService, googleTokenValidator, otpSettings);
        _otpService = otpService;
        _userRepository = userRepository;
        _userManager = userManager;
        _otpSettings = otpSettings.Value;
    }

    public async Task<SendOtpForAuthResponseDto> SendOtpForRegisterAsync(RegisterSendOtpRequestDto request)
    {
        var existingUser = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
        if (existingUser is not null && await _userManager.IsInRoleAsync(existingUser, Role))
            throw new BadRequestException(MessageKeys.PhoneNumberAlreadyExists, "phone_number_already_exists");

        var result = await _otpService.GenerateAndSendAsync(request.PhoneNumber, OtpChannel.Sms);
        if (result.Status == OtpGenerationStatus.CooldownActive)
            throw new BadRequestException(MessageKeys.OtpCooldownActive, "otp_cooldown_active", result.RetryAfterSeconds);

        return new SendOtpForAuthResponseDto { Code = result.Code, ExpiresIn = _otpSettings.ExpirationMinutes * 60 };
    }

    public Task<SendOtpForAuthResponseDto> SendOtpForLoginAsync(SendOtpForAuthRequestDto request)
        => _core.SendOtpForLoginAsync(request.PhoneNumber, Role);

    public Task<AuthResultDto> LoginOtpAsync(LoginOtpRequestDto request)
        => _core.LoginOtpAsync(request.PhoneNumber, request.Code, Role);

    public Task<AuthResultDto> LoginByPasswordAsync(LoginByPasswordRequestDto request)
        => _core.LoginByPasswordAsync(request.Username, request.Password, Role);

    public Task<AuthResultDto> RegisterByMobileAsync(RegisterByMobileRequestDto request)
        => _core.RegisterByMobileAsync(request.PhoneNumber, request.Code, Role);

    public Task<AuthResultDto> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken cancellationToken)
        => _core.ChangePasswordAsync(userId, request, cancellationToken);

    public Task<GoogleAuthResultDto> GoogleAuthAsync(GoogleAuthRequestDto request)
        => _core.GoogleAuthAsync(request.IdToken, Role);
}