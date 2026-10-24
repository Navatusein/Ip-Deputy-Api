namespace IpDeputyApi.Dto;

public class ErrorDto
{
    public string Code { get; set; } = null!;
    public string Message { get; set; } = null!;
    public object? Data { get; set; }

    public ErrorDto()
    {
    }

    public ErrorDto(int statusCode, string message, object? data = null)
    {
        Code = statusCode.ToString();
        Message = message;
        Data = data;
    }
}
