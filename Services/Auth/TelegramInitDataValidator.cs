using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace IpDeputyApi.Services.Auth;

public static class TelegramInitDataValidator
{
    public static ulong? Validate(string initData, string botToken, DateTime utcNow, TimeSpan maxAge)
    {
        if (string.IsNullOrWhiteSpace(initData))
            return null;

        var data = QueryHelpers.ParseQuery(initData.TrimStart('?'))
            .ToDictionary(x => x.Key, x => x.Value.ToString());

        if (!data.TryGetValue("hash", out var hash) || string.IsNullOrEmpty(hash))
            return null;

        var checkString = string.Join("\n", data
            .Where(x => x.Key != "hash")
            .OrderBy(x => x.Key, StringComparer.Ordinal)
            .Select(x => $"{x.Key}={x.Value}"));

        var secretKey = HMACSHA256.HashData(Encoding.UTF8.GetBytes("WebAppData"), Encoding.UTF8.GetBytes(botToken));
        var signature = HMACSHA256.HashData(secretKey, Encoding.UTF8.GetBytes(checkString));

        byte[] providedSignature;

        try
        {
            providedSignature = Convert.FromHexString(hash);
        }
        catch (FormatException)
        {
            return null;
        }

        if (!CryptographicOperations.FixedTimeEquals(signature, providedSignature))
            return null;

        if (!data.TryGetValue("auth_date", out var authDateValue) || !long.TryParse(authDateValue, out var authDateSeconds))
            return null;

        var authDate = DateTimeOffset.FromUnixTimeSeconds(authDateSeconds).UtcDateTime;

        if (authDate > utcNow.AddMinutes(5) || utcNow - authDate > maxAge)
            return null;

        if (!data.TryGetValue("user", out var userJson))
            return null;

        try
        {
            using var document = JsonDocument.Parse(userJson);
            return document.RootElement.TryGetProperty("id", out var id) && id.TryGetUInt64(out var telegramId) ? telegramId : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
