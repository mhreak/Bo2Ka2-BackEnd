// Application/Common/Helpers/ApiKeyGenerator.cs
using System.Security.Cryptography;

namespace Bodokado.Application.Common.Helpers;

public static class ApiKeyGenerator
{
    public static string Create(int bytes = 32)
    {
        var data = RandomNumberGenerator.GetBytes(bytes);
        return Convert.ToHexString(data).ToLowerInvariant(); // 64 کاراکتر hex
    }
}