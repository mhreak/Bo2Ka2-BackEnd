namespace Bodokado.Application.Common.Exceptions;

public class InvalidDeviceTokenException(string token)
    : Exception($"Device token is invalid or unregistered: {token[..8]}...");
