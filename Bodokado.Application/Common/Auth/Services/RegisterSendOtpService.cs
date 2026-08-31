using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Otp;
using Bodokado.Domain.Entities.Users;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.Common.Auth.Services;

public class RegisterSendOtpService : IRegisterSendOtpService
{
    private readonly IOtpService _otpService;
    private readonly IUserRepository _userRepository;
    private readonly OtpSettings _otpSettings;
    private readonly UserManager<User> _userManager;
    private const string DefaultRole = "User";

    public RegisterSendOtpService(IOtpService otpService, IUserRepository userRepository, Microsoft.Extensions.Options.IOptions<OtpSettings> otpSettings, UserManager<User> userManager)
    {
        _otpService = otpService;
        _userRepository = userRepository;
        _otpSettings = otpSettings.Value;
        _userManager = userManager;
    }

    public async Task<SendOtpForAuthResponseDto> ExecuteAsync(RegisterSendOtpRequestDto request)
    {
        var existingUser = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
        if (existingUser is not null && await _userManager.IsInRoleAsync(existingUser, DefaultRole))
            throw new BadRequestException(MessageKeys.PhoneNumberAlreadyExists, "phone_number_already_exists");
        var result = await _otpService.GenerateAndSendAsync(request.PhoneNumber, OtpChannel.Sms);
        if (result.Status == OtpGenerationStatus.CooldownActive)
            throw new BadRequestException(MessageKeys.OtpCooldownActive, "otp_cooldown_active", result.RetryAfterSeconds);
        return new SendOtpForAuthResponseDto { Code = result.Code, ExpiresIn = _otpSettings.ExpirationMinutes * 60 };
    }
}
