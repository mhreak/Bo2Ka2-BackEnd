namespace Bodokado.Application.Common.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message, string v)
        : base(message)
    {
    }
}
