using System.Collections;
using Npgsql;

namespace IpDeputyApi.Extensions;

public static class EnvironmentConfigurationExtensions
{
    private static readonly Dictionary<string, string[]> KeyMap = new()
    {
        ["DATABASE_CONNECTION_STRING"] = ["Database:ConnectionString"],
        ["DATABASE_APPLY_MIGRATIONS"] = ["Database:ApplyMigrationsOnStartup"],
        ["JWT_ACCESS_KEY"] = ["AuthorizeJWT:Key"],
        ["JWT_REFRESH_KEY"] = ["RefreshJWT:Key"],
        ["JWT_ISSUER"] = ["AuthorizeJWT:Issuer", "RefreshJWT:Issuer"],
        ["JWT_AUDIENCE"] = ["AuthorizeJWT:Audience", "RefreshJWT:Audience"],
        ["JWT_ACCESS_LIFETIME_MINUTES"] = ["AuthorizeJWT:LifetimeMinutes"],
        ["JWT_REFRESH_LIFETIME_DAYS"] = ["RefreshJWT:LifetimeDays"],
        ["BOT_API_TOKEN"] = ["BotAuthorizeToken"],
        ["BOT_TOKEN"] = ["BotToken"],
        ["INIT_DATA_LIFETIME_HOURS"] = ["InitDataLifetimeHours"],
        ["CORS_ORIGINS"] = ["CorsOrigins"],
        ["TIME_ZONE"] = ["Schedule:TimeZone"],
        ["INITIAL_ADMIN_LOGIN"] = ["InitialAdmin:Login"],
        ["INITIAL_ADMIN_PASSWORD"] = ["InitialAdmin:Password"],
        ["TRUST_FORWARDED_HEADERS"] = ["ReverseProxy:TrustForwardedHeaders"],
        ["LOG_LEVEL"] = ["Serilog:MinimumLevel:Default"]
    };

    public static IConfigurationBuilder AddAppEnvironmentVariables(this IConfigurationBuilder builder)
    {
        return builder.AddInMemoryCollection(MapEnvironment(Environment.GetEnvironmentVariables()));
    }

    public static Dictionary<string, string?> MapEnvironment(IDictionary environment)
    {
        var values = environment.Keys
            .Cast<object>()
            .Select(key => key.ToString()!)
            .Where(key => !string.IsNullOrWhiteSpace(environment[key]?.ToString()))
            .ToDictionary(key => key, key => environment[key]!.ToString()!, StringComparer.OrdinalIgnoreCase);

        var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var (name, keys) in KeyMap)
        {
            if (!values.TryGetValue(name, out var value))
                continue;

            foreach (var key in keys)
                result[key] = value;
        }

        if (!values.ContainsKey("DATABASE_CONNECTION_STRING") && values.TryGetValue("DATABASE_HOST", out var host))
            result["Database:ConnectionString"] = BuildConnectionString(host, values);

        return result;
    }

    private static string BuildConnectionString(string host, IReadOnlyDictionary<string, string> values)
    {
        var builder = new NpgsqlConnectionStringBuilder { Host = host };

        if (values.TryGetValue("DATABASE_PORT", out var port) && int.TryParse(port, out var portNumber))
            builder.Port = portNumber;

        if (values.TryGetValue("DATABASE_NAME", out var database))
            builder.Database = database;

        if (values.TryGetValue("DATABASE_USER", out var user))
            builder.Username = user;

        if (values.TryGetValue("DATABASE_PASSWORD", out var password))
            builder.Password = password;

        return builder.ConnectionString;
    }
}
