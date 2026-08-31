using Bodokado.Domain.Enums;

namespace Bodokado.Application.Common.Otp;

public interface IOtpService
{
    Task<OtpGenerationResult> GenerateAndSendAsync(string destination, OtpChannel channel, CancellationToken cancellationToken = default);
    Task<OtpVerificationResult> VerifyAsync(string destination, OtpChannel channel, string code, CancellationToken cancellationToken = default);
}
