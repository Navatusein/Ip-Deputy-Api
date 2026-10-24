namespace IpDeputyApi.Exceptions;

public class UnauthorizedException : HttpException
{
    public UnauthorizedException(string message = "Не авторизовано") : base(message, StatusCodes.Status401Unauthorized)
    {
    }
}
