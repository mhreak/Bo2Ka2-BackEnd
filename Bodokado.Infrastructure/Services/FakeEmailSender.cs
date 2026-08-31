using Microsoft.Extensions.Logging;
using Bodokado.Application.Common.Otp;

namespace Bodokado.Infrastructure.Services;

public class FakeEmailSender : IEmailSender
{
    private readonly ILogger<FakeEmailSender> _logger;
    public FakeEmailSender(ILogger<FakeEmailSender> logger) => _logger = logger;
    public Task SendAsync(string email, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[DEV-EMAIL] To: {Email} | Subject: {Subject} | Body: {Body}", email, subject, body);
        return Task.CompletedTask;
    }
}
