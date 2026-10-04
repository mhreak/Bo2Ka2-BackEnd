using Microsoft.Extensions.Logging;
using Bodokado.Application.Common.Otp;

namespace Bodokado.Infrastructure.Services;

public class FakeSmsSender : ISmsSender
{
    private readonly ILogger<FakeSmsSender> _logger;
    public FakeSmsSender(ILogger<FakeSmsSender> logger) => _logger = logger;
    public Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[DEV-SMS] To: {PhoneNumber} | Message: {Message}", phoneNumber, message);
        return Task.CompletedTask;
    }
	public Task SendOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("[DEV-SMS-OTP] To: {PhoneNumber} | Code: {Code}", phoneNumber, code);
		return Task.CompletedTask;
	}
}
