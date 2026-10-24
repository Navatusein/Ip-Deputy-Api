using IpDeputyApi.Database.Models;

namespace IpDeputyApi.Repositories.Interfaces;

public interface ICoupleRepository : IRepository<Couple>
{
    Task<Couple?> GetWithDatesAsync(int id, CancellationToken cancellationToken);
    Task<List<Couple>> GetForPeriodAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken);
    Task<List<Couple>> GetBySubjectTypesAsync(IReadOnlyCollection<int> subjectTypeIds, CancellationToken cancellationToken);
}

public interface IAdditionalCoupleRepository : IRepository<AdditionalCouple>
{
    Task<List<AdditionalCouple>> GetForPeriodAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken);
}

public interface ITelegramRepository : IRepository<Telegram>
{
    Task<Telegram?> GetByTelegramIdAsync(ulong telegramId, CancellationToken cancellationToken);
    Task<Telegram?> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken);
}

public interface ISubmissionsConfigRepository : IRepository<SubmissionsConfig>
{
    Task<SubmissionsConfig?> GetWithWorksAsync(int id, CancellationToken cancellationToken);
    Task<List<SubmissionsConfig>> GetWithSubmissionsAsync(CancellationToken cancellationToken);
}
