using System.ComponentModel.DataAnnotations;

namespace IpDeputyApi.Options;

public abstract class TokenOptions
{
    [Required, MinLength(32)]
    public string Key { get; set; } = "";

    [Required]
    public string Issuer { get; set; } = "";

    [Required]
    public string Audience { get; set; } = "";
}

public class AccessTokenOptions : TokenOptions
{
    public const string Section = "AuthorizeJWT";

    [Range(1, 1440)]
    public int LifetimeMinutes { get; set; } = 60;
}

public class RefreshTokenOptions : TokenOptions
{
    public const string Section = "RefreshJWT";

    [Range(1, 365)]
    public int LifetimeDays { get; set; } = 30;
}
