namespace Bodokado.Infrastructure.Services;

public class SmsIrOptions
{
    public string BaseUrl { get; set; } = "https://api.sms.ir/v1/";
    public string ApiKey { get; set; } = "";
    public long LineNumber { get; set; }
    public int VerifyTemplateId { get; set; }
}