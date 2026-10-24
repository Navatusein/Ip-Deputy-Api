namespace IpDeputyApi.Exceptions;

public class NotFoundException : HttpException
{
    public NotFoundException(string message = "Запис не знайдено") : base(message, StatusCodes.Status404NotFound)
    {
    }
}
