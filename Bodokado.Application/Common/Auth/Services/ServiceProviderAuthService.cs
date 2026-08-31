using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Bodokado.Application.App.Auth.DTOs;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Application.App.ServiceProviderModule.Auth.Interfaces;
using Bodokado.Application.Common.Auth;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Auth.Services;
using Bodokado.Application.Common.Otp;
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Application.App.ServiceProviderModule.Auth.Services;

public class ServiceProviderAuthService : IServiceProviderAuthService
{
    private const string Role = "ServiceProvider";
    private readonly RoleAuthCore _core;

    public ServiceProviderAuthService(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, IOtpService otpService, IUserRepository userRepository, IJwtService jwtService, IRefreshTokenService refreshTokenService, IGoogleTokenValidator googleTokenValidator, Microsoft.Extensions.Options.IOptions<OtpSettings> otpSettings)
    {
        _core = new RoleAuthCore(userManager, roleManager, userRepository, otpService, jwtService, refreshTokenService, googleTokenValidator, otpSettings);
    }

    public Task<SendOtpForAuthResponseDto> SendOtpForLoginAsync(SendOtpForAuthRequestDto request)
        => _core.SendOtpForLoginAsync(request.PhoneNumber, Role);
    public Task<AuthResultDto> LoginOtpAsync(LoginOtpRequestDto request)
        => _core.LoginOtpAsync(request.PhoneNumber, request.Code, Role);
    public Task<AuthResultDto> LoginByPasswordAsync(LoginByPasswordRequestDto request)
        => _core.LoginByPasswordAsync(request.Username, request.Password, Role);
    public Task<AuthResultDto> RegisterByMobileAsync(RegisterByMobileRequestDto request)
        => _core.RegisterByMobileAsync(request.PhoneNumber, request.Code, Role);
    public async Task<AuthResultDto> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken cancellationToken)
        => await _core.ChangePasswordAsync(userId, request, cancellationToken);
    public Task<GoogleAuthResultDto> GoogleAuthAsync(GoogleAuthRequestDto request)
        => _core.GoogleAuthAsync(request.IdToken, Role);
}
