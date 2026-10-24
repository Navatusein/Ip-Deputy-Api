using IpDeputyApi.Database;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Repositories;

public class AdditionalCoupleRepository : RepositoryBase<AdditionalCouple>, IAdditionalCoupleRepository
{
    public AdditionalCoupleRepository(IpDeputyDbContext context) : base(context)
    {
    }

    public Task<List<AdditionalCouple>> GetForPeriodAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken)
    {
        return Set.AsNoTracking()
            .Include(x => x.Subject)
            .Include(x => x.SubjectType)
            .Include(x => x.CoupleTime)
            .Where(x => x.Date >= from && x.Date <= to)
            .ToListAsync(cancellationToken);
    }
}
