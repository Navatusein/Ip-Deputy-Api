using System.Reflection;
using IpDeputyApi.Database.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

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
    }

    private static async Task BaselineExistingDatabaseAsync(IpDeputyDbContext context, CancellationToken cancellationToken)
    {
        if (!await context.Database.CanConnectAsync(cancellationToken))
            return;

        var historyRepository = context.GetService<IHistoryRepository>();

        if (await historyRepository.ExistsAsync(cancellationToken))
            return;

        var tableExists = await context.Database
            .SqlQueryRaw<bool>("SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = current_schema() AND table_name = 'Students') AS \"Value\"")
            .SingleAsync(cancellationToken);

        if (!tableExists)
            return;

        var baselineId = typeof(InitialCreate).GetCustomAttribute<MigrationAttribute>()!.Id;

        Logger.Warning("Existing database without migrations history detected, marking {MigrationId} as applied", baselineId);

        await context.Database.ExecuteSqlRawAsync(historyRepository.GetCreateIfNotExistsScript(), cancellationToken);
        await context.Database.ExecuteSqlRawAsync(historyRepository.GetInsertScript(new HistoryRow(baselineId, ProductInfo.GetVersion())), cancellationToken);
    }
}
