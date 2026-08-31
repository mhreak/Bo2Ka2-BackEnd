using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Bodokado.Application.App.Auth.DTOs;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Otp;
using Bodokado.Domain.Entities.Users;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.Common.Auth.Services;

public class SendOtpService
{
    private static readonly System.Text.RegularExpressions.Regex PhoneNumberRegex = new(@"^\+[1-9]\d{6,14}$", System.Text.RegularExpressions.RegexOptions.Compiled);
    private readonly IOtpService _otpService;
    private readonly IUserRepository _userRepository;
    private readonly OtpSettings _otpSettings;

    public SendOtpService(IOtpService otpService, IUserRepository userRepository, Microsoft.Extensions.Options.IOptions<OtpSettings> otpSettings)
    {
        _otpService = otpService;
        _userRepository = userRepository;
        _otpSettings = otpSettings.Value;
    }

    public async Task<SendOtpResponseDto> SendOtpAsync(SendOtpDto dto)
    {
        if (!PhoneNumberRegex.IsMatch(dto.PhoneNumber))
            throw new BadRequestException(MessageKeys.InvalidPhoneNumberFormat, "invalid_phone_number_format");
        var existingUser = await _userRepository.GetByPhoneNumberAsync(dto.PhoneNumber);
        if (existingUser is not null)
            throw new BadRequestException(MessageKeys.PhoneNumberAlreadyExists, "phone_number_already_exists");
        var result = await _otpService.GenerateAndSendAsync(dto.PhoneNumber, OtpChannel.Sms);
        if (result.Status == OtpGenerationStatus.CooldownActive)
            throw new BadRequestException(MessageKeys.OtpCooldownActive, "otp_cooldown_active", result.RetryAfterSeconds);
        return new SendOtpResponseDto { Code = result.Code, ExpiresIn = _otpSettings.ExpirationMinutes * 60 };
    }
}
