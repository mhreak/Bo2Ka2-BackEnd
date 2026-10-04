using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Bodokado.Application.Common.Otp;

namespace Bodokado.Infrastructure.Services;

public class SmsIrSender : ISmsSender
{
    private readonly HttpClient _http;
    private readonly SmsIrOptions _opt;
    private readonly ILogger<SmsIrSender> _log;

    public SmsIrSender(HttpClient http, IOptions<SmsIrOptions> opt, ILogger<SmsIrSender> log)
    {
        _http = http;
        _opt = opt.Value;
        _log = log;
    }

    public Task SendOtpAsync(string phoneNumber, string code, CancellationToken ct = default)
        => PostAsync("send/verify", new
        {
            mobile = NormalizeMobile(phoneNumber),
            templateId = _opt.VerifyTemplateId,
            parameters = new[] { new { name = "otp", value = code } }
        }, ct);

    public Task SendAsync(string phoneNumber, string message, CancellationToken ct = default)
        => PostAsync("send/bulk", new
        {
            lineNumber = _opt.LineNumber,
            messageText = message,
            mobiles = new[] { NormalizeMobile(phoneNumber) },
            sendDateTime = (long?)null
        }, ct);

    private static string NormalizeMobile(string phone)
    {
        var normalized = new System.Text.StringBuilder(phone.Length);
        foreach (var character in phone.Trim())
        {
            if (character is ' ' or '-' or '(' or ')' or '\t')
                continue;

            if (character is >= '\u06F0' and <= '\u06F9')
                normalized.Append((char)('0' + character - '\u06F0'));
            else if (character is >= '\u0660' and <= '\u0669')
                normalized.Append((char)('0' + character - '\u0660'));
            else
                normalized.Append(character);
        }

        var value = normalized.ToString();
        if (value.StartsWith("+98", StringComparison.Ordinal)) value = "0" + value[3..];
        else if (value.StartsWith("0098", StringComparison.Ordinal)) value = "0" + value[4..];
        else if (value.StartsWith("98", StringComparison.Ordinal) && value.Length == 12) value = "0" + value[2..];
        else if (value.StartsWith("9", StringComparison.Ordinal) && value.Length == 10) value = "0" + value;

        if (value.Length != 11 || !value.StartsWith("09", StringComparison.Ordinal) || value.Any(character => character is < '0' or > '9'))
            throw new ArgumentException("sms.ir requires an Iranian mobile number in 09xxxxxxxxx format.", nameof(phone));

        return value;
    }

    private async Task PostAsync(string path, object body, CancellationToken ct)
    {
        using var res = await _http.PostAsJsonAsync(path, body, ct);
        var json = await res.Content.ReadAsStringAsync(ct);

        int status = 0;
        string? message = null;
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("status", out var statusElement))
                statusElement.TryGetInt32(out status);
            if (doc.RootElement.TryGetProperty("message", out var m)) message = m.GetString();
        }
        catch (JsonException) { }
        catch (InvalidOperationException) { }

        if (res.IsSuccessStatusCode && status == 1)
        {
            _log.LogInformation("SMS.ir response: {Body}", json);
            return;
        }

        if (!res.IsSuccessStatusCode || status != 1)
        {
            _log.LogError("SMS.ir error {Code}: {Body}", (int)res.StatusCode, json);
            throw new InvalidOperationException(message ?? "ارسال پیامک ناموفق بود");
        }
    }
}