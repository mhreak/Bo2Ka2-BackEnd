using Microsoft.AspNetCore.Identity;
using Bodokado.Application.App.Auth.DTOs;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Otp;
using Bodokado.Domain.Entities.Users;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.Common.Auth.Services;

public class SubmitOtpService : ISubmitOtpService
{
    private readonly IOtpService _otpService;
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;

    public SubmitOtpService(IOtpService otpService, IUserRepository userRepository, UserManager<User> userManager, IJwtService jwtService, IRefreshTokenService refreshTokenService)
    {
        _otpService = otpService;
        _userRepository = userRepository;
        _userManager = userManager;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<AuthResultDto> SubmitOtpAsync(SubmitOtpRequestDto request)
    {
        var result = await _otpService.VerifyAsync(request.PhoneNumber, OtpChannel.Sms, request.Code);
        switch (result.Status)
        {
            case OtpVerificationStatus.NotFoundOrExpired:
                throw new BadRequestException(MessageKeys.OtpNotFoundOrExpired, "otp_not_found_or_expired");
            case OtpVerificationStatus.MaxAttemptsExceeded:
                throw new BadRequestException(MessageKeys.OtpMaxAttemptsExceeded, "otp_max_attempts_exceeded");
            case OtpVerificationStatus.InvalidCode:
                throw new BadRequestException(MessageKeys.OtpInvalidCode, "otp_invalid_code");
        }
        var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
        if (user is null)
        {
            user = new User { PhoneNumber = request.PhoneNumber, UserName = request.PhoneNumber };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                throw new BadRequestException(MessageKeys.UserCreationError, "user_creation_failed");
        }
        var session = await _refreshTokenService.IssueAsync(user.Id, "Customer");
        var accessToken = await _jwtService.GenerateAccessToken(user, _userManager, session.SessionId);
        return new AuthResultDto { AccessToken = accessToken, RefreshToken = session.RefreshToken, ExpiresAt = _jwtService.GetAccessTokenExpiry() };
    }
}
