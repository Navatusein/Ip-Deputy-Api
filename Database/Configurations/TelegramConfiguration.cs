using IpDeputyApi.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IpDeputyApi.Database.Configurations;

public class TelegramConfiguration : IEntityTypeConfiguration<Telegram>
{
    public void Configure(EntityTypeBuilder<Telegram> builder)
    {
        builder.HasIndex(x => x.TelegramId).IsUnique();
    }
}
