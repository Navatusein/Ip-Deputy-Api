using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Mapping;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using DayOfWeek = IpDeputyApi.Database.Models.DayOfWeek;

namespace IpDeputyApi.Services;

public class LookupService : ILookupService
{
    private readonly IRepository<CoupleTime> _coupleTimes;
    private readonly IRepository<DayOfWeek> _daysOfWeek;

    public LookupService(IRepository<CoupleTime> coupleTimes, IRepository<DayOfWeek> daysOfWeek)
    {
        _coupleTimes = coupleTimes;
        _daysOfWeek = daysOfWeek;
    }

    public async Task<IReadOnlyList<CoupleTimeDto>> GetCoupleTimesAsync(CancellationToken cancellationToken)
    {
        return await _coupleTimes.Query().OrderBy(x => x.Index).ProjectToDto().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DayOfWeekDto>> GetDaysOfWeekAsync(CancellationToken cancellationToken)
    {
        return await _daysOfWeek.Query().OrderBy(x => x.Id).ProjectToDto().ToListAsync(cancellationToken);
    }
}
