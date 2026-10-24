using System.Collections;
using IpDeputyApi.Extensions;

namespace IpDeputyApi.Tests;

public class EnvironmentConfigurationTests
{
    [Fact]
    public void MapEnvironment_MapsSnakeCaseNamesToConfigurationKeys()
    {
        var result = EnvironmentConfigurationExtensions.MapEnvironment(new Hashtable
        {
            ["JWT_ACCESS_KEY"] = "access",
            ["JWT_ISSUER"] = "ip-deputy",
            ["BOT_API_TOKEN"] = "bot",
            ["TIME_ZONE"] = "Europe/Kyiv",
            ["UNRELATED"] = "value"
        });

        Assert.Equal("access", result["AuthorizeJWT:Key"]);
        Assert.Equal("ip-deputy", result["AuthorizeJWT:Issuer"]);
        Assert.Equal("ip-deputy", result["RefreshJWT:Issuer"]);
        Assert.Equal("bot", result["BotAuthorizeToken"]);
        Assert.Equal("Europe/Kyiv", result["Schedule:TimeZone"]);
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void MapEnvironment_BuildsConnectionStringFromParts()
    {
        var result = EnvironmentConfigurationExtensions.MapEnvironment(new Hashtable
        {
            ["DATABASE_HOST"] = "db",
            ["DATABASE_PORT"] = "5433",
            ["DATABASE_NAME"] = "ip_deputy",
            ["DATABASE_USER"] = "user",
            ["DATABASE_PASSWORD"] = "secret"
        });

        Assert.Equal("Host=db;Port=5433;Database=ip_deputy;Username=user;Password=secret", result["Database:ConnectionString"]);
    }

    [Fact]
    public void MapEnvironment_PrefersFullConnectionString()
    {
        var result = EnvironmentConfigurationExtensions.MapEnvironment(new Hashtable
        {
            ["DATABASE_CONNECTION_STRING"] = "Host=main",
            ["DATABASE_HOST"] = "other"
        });

        Assert.Equal("Host=main", result["Database:ConnectionString"]);
    }

    [Fact]
    public void MapEnvironment_SkipsEmptyValues()
    {
        var result = EnvironmentConfigurationExtensions.MapEnvironment(new Hashtable
        {
            ["INITIAL_ADMIN_LOGIN"] = "",
            ["DATABASE_HOST"] = " "
        });

        Assert.Empty(result);
    }
}
