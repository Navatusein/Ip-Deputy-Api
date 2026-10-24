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
export DATABASE_CONNECTION_STRING="Host=localhost;Port=5432;Database=ip_deputy;Username=postgres;Password=postgres"
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

All settings can come from flat environment variables. The full list is in [MIGRATION.md](MIGRATION.md#3-configuration).

| Env var | Description |
|---|---|
| `DATABASE_CONNECTION_STRING` or `DATABASE_HOST` / `DATABASE_PORT` / `DATABASE_NAME` / `DATABASE_USER` / `DATABASE_PASSWORD` | PostgreSQL connection |
| `JWT_ACCESS_KEY`, `JWT_REFRESH_KEY` | Token signing keys; at least 32 characters each, and they must differ |
| `JWT_ISSUER`, `JWT_AUDIENCE` | Token issuer and audience |
| `BOT_API_TOKEN` | Shared secret for the `X-BOT-TOKEN` header |
| `BOT_TOKEN` | Telegram bot token used to validate Web App `initData` |
| `CORS_ORIGINS` | Comma-separated allowed origins |
| `TIME_ZONE` | Defaults to `Europe/Kyiv` |
| `INITIAL_ADMIN_LOGIN`, `INITIAL_ADMIN_PASSWORD` | First administrator, created only when there are no users |

The `appsettings.json` keys and the standard `Section__Key` variables still work; flat variables take precedence.

## Permissions

Admin users have a flat list of permissions: `manage-students`, `manage-teachers`, `manage-subjects`, `manage-schedule`, `manage-submissions`, `manage-links`, `manage-users`. Students who open the Telegram Web App get a student token that can only access their own submission queue.

## License

[MIT](LICENSE)
