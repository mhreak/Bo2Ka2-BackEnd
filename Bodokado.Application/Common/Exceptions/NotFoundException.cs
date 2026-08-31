namespace Bodokado.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message, string v)
        : base(message)
    {
    }
}
