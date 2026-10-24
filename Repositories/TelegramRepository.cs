using IpDeputyApi.Database;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Repositories;

public class TelegramRepository : RepositoryBase<Telegram>, ITelegramRepository
{
    public TelegramRepository(IpDeputyDbContext context) : base(context)
    {
    }

    public Task<Telegram?> GetByTelegramIdAsync(ulong telegramId, CancellationToken cancellationToken)
    {
        return Set.Include(x => x.Student).FirstOrDefaultAsync(x => x.TelegramId == telegramId, cancellationToken);
    }

    public Task<Telegram?> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken)
    {
        return Set.FirstOrDefaultAsync(x => x.StudentId == studentId, cancellationToken);
    }
}
