# Upgrading production to API v2 (EF Core migrations)

Before v2 the schema was created with `Database.EnsureCreated()`. Now EF Core migrations manage it. On startup the API runs `DatabaseInitializer`:

1. If `__EFMigrationsHistory` is missing but `Students` exists, it creates the history table and marks `InitialCreate` as applied (baseline). `InitialCreate` matches the schema that `EnsureCreated` produced.
2. It runs `Database.MigrateAsync()` and applies everything after the baseline.
3. If there are no users and `InitialAdmin:Login` / `InitialAdmin:Password` are set, it creates an administrator with all permissions.

Migrations applied on top of the baseline:

| Migration | What it does |
|---|---|
| `AddUsers` | Creates `Users`, copies every `WebAuthentications` row into it (login lowercased, all permissions, legacy hash kept and upgraded to PBKDF2 on the next successful login), drops `WebAuthentications` |
| `SubjectCountsDefault` | DB default `0` for `Subjects.LaboratoryCount` / `PracticalCount` |
| `NormalizeNames` | Trims teacher and student fields and collapses repeated whitespace |
| `AlignCoupleDates` | Moves `StartDate` forward and `EndDate` back to the couple's weekday (visible schedule is unchanged), removes duplicate dates, converts additional dates that fall on another weekday into `AdditionalCouples`, drops removed dates on another weekday, recalculates `AdditionalCouples.DayOfWeekId` from `Date` |
| `AddTelegramIdUniqueIndex` | Deletes duplicate `Telegrams.TelegramId` rows (keeps the oldest) and adds a unique index |
| `RestrictScheduleDeletes` | Couples and additional couples no longer cascade-delete when a subject, subject type, teacher, couple time or day is deleted (the API returns 409 instead) |
| `FixSeedSequences` | Moves identity sequences of seeded tables past the seeded ids |

## 1. Back up

```bash
pg_dump -Fc -h <host> -U <user> -d <database> -f ip_deputy_before_v2.dump
pg_restore -l ip_deputy_before_v2.dump > /dev/null
```

Keep the old image tag so you can roll back.

## 2. Check the schema and data (read-only)

Compare the production schema with the baseline:

```bash
dotnet tool restore
dotnet ef migrations script 0 InitialCreate -o initial.sql
pg_dump -s -h <host> -U <user> -d <database> -f prod_schema.sql
```

Tables, columns, types and nullability must match. Pay special attention to `SubmissionWorks`, which may have been created by hand.

Then look at the rows that the migrations will change:

```sql
SELECT lower(btrim("Login")) AS login, count(*) FROM "WebAuthentications" GROUP BY 1 HAVING count(*) > 1;
SELECT "TelegramId", count(*) FROM "Telegrams" GROUP BY 1 HAVING count(*) > 1;
SELECT c."Id", c."DayOfWeekId", c."StartDate", c."EndDate" FROM "Couples" c
WHERE c."StartDate" IS NOT NULL AND EXTRACT(ISODOW FROM c."StartDate")::int <> c."DayOfWeekId";
SELECT d.* FROM "CoupleDates" d JOIN "Couples" c ON c."Id" = d."CoupleId"
WHERE EXTRACT(ISODOW FROM d."Date")::int <> c."DayOfWeekId";
```

Duplicate logins collapse into one user (the oldest row wins). Duplicate Telegram ids keep the oldest link.

## 3. Configuration

Every setting can be set with a flat environment variable. The standard ASP.NET form (`Database__ConnectionString`) and `appsettings.json` also work; the flat variables win.

| Env var | Config key | Required | Notes |
|---|---|---|---|
| `DATABASE_CONNECTION_STRING` | `Database:ConnectionString` | yes* | Npgsql format: `Host=...;Port=5432;Database=...;Username=...;Password=...` |
| `DATABASE_HOST`, `DATABASE_PORT`, `DATABASE_NAME`, `DATABASE_USER`, `DATABASE_PASSWORD` | `Database:ConnectionString` | yes* | used to build the connection string when `DATABASE_CONNECTION_STRING` is not set |
| `DATABASE_APPLY_MIGRATIONS` | `Database:ApplyMigrationsOnStartup` | no | default `true`; set `false` to apply migrations manually |
| `JWT_ACCESS_KEY` | `AuthorizeJWT:Key` | yes | at least 32 characters |
| `JWT_REFRESH_KEY` | `RefreshJWT:Key` | yes | at least 32 characters, different from the access key |
| `JWT_ISSUER`, `JWT_AUDIENCE` | `AuthorizeJWT:Issuer/Audience`, `RefreshJWT:Issuer/Audience` | yes | the same values are used for both tokens |
| `JWT_ACCESS_LIFETIME_MINUTES`, `JWT_REFRESH_LIFETIME_DAYS` | `AuthorizeJWT:LifetimeMinutes`, `RefreshJWT:LifetimeDays` | no | defaults `60` and `30` |
| `BOT_API_TOKEN` | `BotAuthorizeToken` | yes | value of `X-BOT-TOKEN`; the API refuses to start when it is empty |
| `BOT_TOKEN` | `BotToken` | yes | Telegram bot token, used to validate WebApp `initData` |
| `INIT_DATA_LIFETIME_HOURS` | `InitDataLifetimeHours` | no | default `24` |
| `CORS_ORIGINS` | `Origins` | yes | comma-separated frontend origins |
| `TIME_ZONE` | `Schedule:TimeZone` | no | default `Europe/Kyiv` |
| `INITIAL_ADMIN_LOGIN`, `INITIAL_ADMIN_PASSWORD` | `InitialAdmin:Login/Password` | no | only used when the `Users` table is empty |
| `TRUST_FORWARDED_HEADERS` | `ReverseProxy:TrustForwardedHeaders` | no | default `true`; uses `X-Forwarded-For` for login rate limiting |
| `LOG_LEVEL` | `Serilog:MinimumLevel:Default` | no | default `Information` |

\* either the full connection string or its parts.

`Database:Provider` and `Database:Version` are no longer used.

## 4. Deploy

Automatic (default): deploy the new image with the configuration above. The first start logs `Existing database without migrations history detected, marking ..._InitialCreate as applied` and applies the remaining migrations.

Manual: set `DATABASE_APPLY_MIGRATIONS=false`, then generate and review an idempotent script:

```bash
dotnet ef migrations script --idempotent -o upgrade.sql
```

On a database without history, first create the history table and insert the baseline row:

```sql
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" ("MigrationId" varchar(150) PRIMARY KEY, "ProductVersion" varchar(32) NOT NULL);
INSERT INTO "__EFMigrationsHistory" VALUES ('20261024100355_InitialCreate', '10.0.13');
```

Then run `psql -v ON_ERROR_STOP=1 -1 -f upgrade.sql`.

## 5. Verify

```sql
SELECT "MigrationId" FROM "__EFMigrationsHistory" ORDER BY 1;
SELECT "Id", "Login", "DisplayName", "Permissions" FROM "Users";
```

Log in to the admin panel with an existing login and password. Admin users have to log in again once, because the refresh cookie path and format changed.

## 6. Rollback

`Down` migrations exist but they lose data (for example new users without a legacy hash). To roll back, restore the dump and redeploy the old image:

```bash
pg_restore --clean --if-exists -h <host> -U <user> -d <database> ip_deputy_before_v2.dump
```

## Adding migrations later

```bash
dotnet tool restore
dotnet ef migrations add <Name> -o Database/Migrations
```

`DesignTimeDbContextFactory` uses `DATABASE_CONNECTION_STRING` (or `Database__ConnectionString`) or a local default, so no database is needed to add a migration.
