namespace IpDeputyApi.Exceptions;

public class ValidationException : HttpException
{
    public const string DefaultMessage = "Помилка валідації";

    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IReadOnlyDictionary<string, string[]> errors, string message = DefaultMessage)
        : base(message, StatusCodes.Status400BadRequest, errors)
    {
        Errors = errors;
    }

    public ValidationException(string field, string error)
        : this(new Dictionary<string, string[]> { [field] = [error] })
    {
    }
}
