namespace Bodokado.Application.Common.Otp;

public interface IEmailSender
{
    Task SendAsync(string email, string subject, string body, CancellationToken cancellationToken = default);
}
