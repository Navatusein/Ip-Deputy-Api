using System.Reflection;
using IpDeputyApi.Authorization;
using IpDeputyApi.Database.Migrations;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Options;
using IpDeputyApi.Services;
using IpDeputyApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;

namespace IpDeputyApi.Database;

public static class DatabaseInitializer
{
    private static Serilog.ILogger Logger => Serilog.Log.ForContext(typeof(DatabaseInitializer));

    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<IpDeputyDbContext>();

        await BaselineExistingDatabaseAsync(context, cancellationToken);
        await context.Database.MigrateAsync(cancellationToken);
        await SeedInitialAdminAsync(scope.ServiceProvider, context, cancellationToken);
    }

    private static async Task SeedInitialAdminAsync(IServiceProvider services, IpDeputyDbContext context, CancellationToken cancellationToken)
    {
        var options = services.GetRequiredService<IOptions<InitialAdminOptions>>().Value;

        if (string.IsNullOrWhiteSpace(options.Login) || string.IsNullOrWhiteSpace(options.Password))
            return;

        if (await context.Users.AnyAsync(cancellationToken))
            return;

        var passwordService = services.GetRequiredService<IPasswordService>();
        var clock = services.GetRequiredService<IAppClock>();

        var user = new User
        {
            Login = UserService.NormalizeLogin(options.Login),
            DisplayName = options.Login.Trim(),
            Permissions = Permissions.AllKeys.ToList(),
            IsActive = true,
            CreatedAt = clock.UtcNow
        };

        user.PasswordHash = passwordService.Hash(user, options.Password);

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        Logger.Warning("Initial administrator {Login} created", user.Login);
    }

    private static Task<bool> TableExistsAsync(IpDeputyDbContext context, string tableName, CancellationToken cancellationToken)
    {
        return context.Database
            .SqlQuery<bool>($"SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = current_schema() AND table_name = {tableName}) AS \"Value\"")
            .SingleAsync(cancellationToken);
    }

    private static async Task BaselineExistingDatabaseAsync(IpDeputyDbContext context, CancellationToken cancellationToken)
    {
        if (!await context.Database.CanConnectAsync(cancellationToken))
            return;

        var historyRepository = context.GetService<IHistoryRepository>();

        if (await TableExistsAsync(context, HistoryRepository.DefaultTableName, cancellationToken))
            return;

        await context.Database.ExecuteSqlRawAsync(historyRepository.GetCreateIfNotExistsScript(), cancellationToken);

        if (!await TableExistsAsync(context, "Students", cancellationToken))
            return;

        var baselineId = typeof(InitialCreate).GetCustomAttribute<MigrationAttribute>()!.Id;

        Logger.Warning("Existing database without migrations history detected, marking {MigrationId} as applied", baselineId);

        await context.Database.ExecuteSqlRawAsync(historyRepository.GetInsertScript(new HistoryRow(baselineId, ProductInfo.GetVersion())), cancellationToken);
    }
}
