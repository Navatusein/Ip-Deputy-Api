using System.Text.RegularExpressions;

namespace IpDeputyApi.Services;

public static partial class TextNormalizer
{
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    public static string Clean(string? value)
    {
        return value == null ? "" : WhitespaceRegex().Replace(value.Trim(), " ");
    }

    public static string? CleanOptional(string? value)
    {
        var cleaned = Clean(value);
        return cleaned.Length == 0 ? null : cleaned;
    }

    public static string Trim(string? value)
    {
        return value?.Trim() ?? "";
    }

    public static string? TrimOptional(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }
}
