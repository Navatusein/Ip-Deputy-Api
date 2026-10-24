using System.Security.Cryptography;
using System.Text;
using IpDeputyApi.Services.Auth;

namespace IpDeputyApi.Tests;

public class TelegramInitDataValidatorTests
{
    private const string BotToken = "123456:test-token";
    private static readonly DateTime Now = new(2026, 10, 24, 12, 0, 0, DateTimeKind.Utc);

    private static string Sign(Dictionary<string, string> fields)
    {
        var checkString = string.Join("\n", fields.OrderBy(x => x.Key, StringComparer.Ordinal).Select(x => $"{x.Key}={x.Value}"));
        var secret = HMACSHA256.HashData(Encoding.UTF8.GetBytes("WebAppData"), Encoding.UTF8.GetBytes(BotToken));
        var hash = Convert.ToHexStringLower(HMACSHA256.HashData(secret, Encoding.UTF8.GetBytes(checkString)));

        return string.Join("&", fields.Append(new KeyValuePair<string, string>("hash", hash))
            .Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));
    }

    private static Dictionary<string, string> Fields(DateTime authDate)
    {
        return new Dictionary<string, string>
        {
            ["auth_date"] = new DateTimeOffset(authDate).ToUnixTimeSeconds().ToString(),
            ["query_id"] = "AAE",
            ["user"] = "{\"id\":987654321,\"first_name\":\"Іван\"}"
        };
    }

    [Fact]
    public void Validate_ReturnsTelegramId()
    {
        var initData = Sign(Fields(Now.AddMinutes(-10)));

        Assert.Equal(987654321UL, TelegramInitDataValidator.Validate(initData, BotToken, Now, TimeSpan.FromHours(24)));
    }

    [Fact]
    public void Validate_RejectsExpiredData()
    {
        var initData = Sign(Fields(Now.AddHours(-25)));

        Assert.Null(TelegramInitDataValidator.Validate(initData, BotToken, Now, TimeSpan.FromHours(24)));
    }

    [Fact]
    public void Validate_RejectsTamperedData()
    {
        var initData = Sign(Fields(Now)).Replace("987654321", "111111111");

        Assert.Null(TelegramInitDataValidator.Validate(initData, BotToken, Now, TimeSpan.FromHours(24)));
    }

    [Fact]
    public void Validate_RejectsWrongBotToken()
    {
        var initData = Sign(Fields(Now));

        Assert.Null(TelegramInitDataValidator.Validate(initData, "other", Now, TimeSpan.FromHours(24)));
    }

    [Fact]
    public void Validate_RejectsMissingHash()
    {
        Assert.Null(TelegramInitDataValidator.Validate("auth_date=1&user=%7B%7D", BotToken, Now, TimeSpan.FromHours(24)));
    }
}
