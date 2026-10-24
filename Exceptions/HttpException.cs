namespace IpDeputyApi.Exceptions;

public class HttpException : Exception
{
    public int StatusCode { get; }
    public object? ResponseData { get; }

    public HttpException(string message, int statusCode, object? responseData = null) : base(message)
    {
        StatusCode = statusCode;
        ResponseData = responseData;
    }
}
