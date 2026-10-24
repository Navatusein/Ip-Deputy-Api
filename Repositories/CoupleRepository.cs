using IpDeputyApi.Database;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Repositories;

public class CoupleRepository : RepositoryBase<Couple>, ICoupleRepository
{
    public CoupleRepository(IpDeputyDbContext context) : base(context)
    {
    }

    public Task<Couple?> GetWithDatesAsync(int id, CancellationToken cancellationToken)
    {
        return Set.Include(x => x.CoupleDates).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<Couple>> GetForPeriodAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken)
    {
        return Set.AsNoTracking()
            .Include(x => x.Subject)
            .Include(x => x.SubjectType)
            .Include(x => x.CoupleTime)
            .Include(x => x.CoupleDates)
            .Where(x => (x.StartDate != null && x.StartDate <= to && (x.EndDate == null || x.EndDate >= from))
                || x.CoupleDates.Any(d => !d.IsRemovedDate && d.Date >= from && d.Date <= to))
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public Task<List<Couple>> GetBySubjectTypesAsync(IReadOnlyCollection<int> subjectTypeIds, CancellationToken cancellationToken)
    {
        return Set.AsNoTracking()
            .Include(x => x.CoupleDates)
            .Where(x => subjectTypeIds.Contains(x.SubjectTypeId))
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }
}
