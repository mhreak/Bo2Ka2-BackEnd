namespace Bodokado.Application.Common.Otp;

public interface ISmsSender
{
    Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
    Task SendOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken = default);
}