# Ip-Deputy-Api

The REST API for [Ip-Deputy](https://github.com/Navatusein/Ip-Deputy). It is the single source of truth for the Telegram bot and the admin panel.

Built with .NET 10, ASP.NET Core, EF Core 10 and PostgreSQL. Mapping is done with Mapperly and logging with Serilog.

## Structure

| Folder | Purpose |
|---|---|
| `Controllers/Bot` | Endpoints for the Telegram bot under `/api/bot`, protected by the `X-BOT-TOKEN` header |
| `Controllers/Frontend` | Endpoints for the admin panel and Web App under `/api/frontend`, protected by JWT and permissions |
| `Services` | Business logic; returns DTOs |
| `Repositories` | Data access and `IUnitOfWork` transactions |
| `Services/Schedule` | `ScheduleCalculator` and date rules shared by every schedule calculation |
| `Database` | `DbContext`, entity configurations, migrations, startup initializer |
| `Paging` | `page / pageSize / sort / order / q` query contract and `PageResult<T>` |
| `IpDeputyApi.Tests` | xUnit tests |

## Development

```bash
docker compose -f docker-compose.dev.yml up -d
dotnet user-secrets set "Database:ConnectionString" "Host=localhost;Port=5432;Database=ip_deputy;Username=postgres;Password=postgres"
dotnet run
dotnet test IpDeputyApi.sln
```

Running `dotnet run` starts the API on `http://localhost:8000`. In Development the OpenAPI UI is served at `/api/scalar`.

To add a migration:

```bash
dotnet tool restore
dotnet ef migrations add <Name> -o Database/Migrations
```

Migrations are applied on startup. Upgrading a database created by the pre-migrations version is described in [MIGRATION.md](MIGRATION.md).

## Configuration

| Key (env var) | Description |
|---|---|
| `Database__ConnectionString` | PostgreSQL connection string |
| `AuthorizeJWT__Key`, `__Issuer`, `__Audience` | Access token settings; the key must be at least 32 characters |
| `RefreshJWT__Key`, `__Issuer`, `__Audience` | Refresh token settings; use a different key |
| `BotAuthorizeToken` | Shared secret for the `X-BOT-TOKEN` header |
| `BotToken` | Telegram bot token used to validate Web App `initData` |
| `InitialAdmin__Login`, `InitialAdmin__Password` | First administrator, created only when there are no users |
| `Origins__0`, `Origins__1`, ... | Allowed CORS origins |
| `Schedule__TimeZone` | Defaults to `Europe/Kyiv` |

## Permissions

Admin users have a flat list of permissions: `manage-students`, `manage-teachers`, `manage-subjects`, `manage-schedule`, `manage-submissions`, `manage-links`, `manage-users`. Students who open the Telegram Web App get a student token that can only access their own submission queue.

## License

[MIT](LICENSE)
