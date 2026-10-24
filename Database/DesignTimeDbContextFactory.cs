using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IpDeputyApi.Database;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<IpDeputyDbContext>
{
    public IpDeputyDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("Database__ConnectionString")
            ?? "Host=localhost;Port=5432;Database=ip_deputy;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<IpDeputyDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new IpDeputyDbContext(options);
    }
}
