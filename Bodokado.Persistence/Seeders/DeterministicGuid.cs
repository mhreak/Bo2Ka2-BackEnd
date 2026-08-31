// tymo.Persistence/Seeders/DeterministicGuid.cs
using System.Security.Cryptography;
using System.Text;

namespace Bodokado.Persistence.Seeders;

/// <summary>
/// از یه رشته (مثلاً "Country_1")، همیشه یه Guid ثابت و تکرارپذیر می‌سازه.
/// این‌جوری هر بار Seed اجرا بشه، همون Id عددی منبع JSON، همیشه دقیقاً همون Guid رو می‌گیره.
/// </summary>
public static class DeterministicGuid
{
    public static Guid Create(string input)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return new Guid(hash);
    }
}