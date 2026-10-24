namespace IpDeputyApi.Exceptions;

public class ValidationErrors
{
    private readonly Dictionary<string, List<string>> _errors = new();

    public bool HasErrors => _errors.Count > 0;

    public ValidationErrors Add(string field, string error)
    {
        if (!_errors.TryGetValue(field, out var list))
        {
            list = new List<string>();
            _errors[field] = list;
        }

        if (!list.Contains(error))
            list.Add(error);

        return this;
    }

    public IReadOnlyDictionary<string, string[]> ToDictionary()
    {
        return _errors.ToDictionary(x => x.Key, x => x.Value.ToArray());
    }

    public void ThrowIfAny()
    {
        if (HasErrors)
            throw new ValidationException(ToDictionary());
    }
}
