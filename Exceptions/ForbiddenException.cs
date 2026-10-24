namespace IpDeputyApi.Exceptions;

public class ForbiddenException : HttpException
{
    public ForbiddenException(string message = "Недостатньо прав") : base(message, StatusCodes.Status403Forbidden)
    {
    }
}
