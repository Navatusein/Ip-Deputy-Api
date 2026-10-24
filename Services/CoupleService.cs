using System.Linq.Expressions;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Paging;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;
using IpDeputyApi.Services.Schedule;

namespace IpDeputyApi.Services;

public class CoupleService : CrudServiceBase<Couple, CoupleDto, CoupleFilter>, ICoupleService
{
    private static readonly SortMap<Couple> SortMap = new SortMap<Couple>()
        .Add("dayOfWeekId", x => x.DayOfWeekId)
        .Add("coupleTimeId", x => x.CoupleTime.Index)
        .Add("subjectId", x => x.Subject.Name)
        .Add("subjectTypeId", x => x.SubjectTypeId)
        .Add("teacherId", x => x.Teacher.Surname)
        .Add("subgroupId", x => x.SubgroupId)
        .Add("startDate", x => x.StartDate)
        .Add("endDate", x => x.EndDate)
        .Add("isRolling", x => x.IsRolling)
        .Add("cabinet", x => x.Cabinet)
        .Add("additionalInformation", x => x.AdditionalInformation)
        .Add("link", x => x.Link)
        .Add("weeksCount", x => x.EndDate!.Value.DayNumber - x.StartDate!.Value.DayNumber)
        .Default(query => query.OrderBy(x => x.DayOfWeekId).ThenBy(x => x.CoupleTime.Index).ThenBy(x => x.StartDate));

    private static readonly IReadOnlyList<Expression<Func<Couple, string?>>> Search =
    [
        x => x.Subject.Name,
        x => x.Subject.ShortName,
        x => x.Teacher.Surname,
        x => x.Teacher.Name,
        x => x.Cabinet
    ];

    private readonly ICoupleRepository _couples;
    private readonly IRepository<CoupleDate> _coupleDates;
    private readonly IRepository<Subject> _subjects;
    private readonly IRepository<SubjectType> _subjectTypes;
    private readonly IRepository<CoupleTime> _coupleTimes;
    private readonly IRepository<Teacher> _teachers;
    private readonly IRepository<Subgroup> _subgroups;

    public CoupleService(
        ICoupleRepository couples,
        IRepository<CoupleDate> coupleDates,
        IRepository<Subject> subjects,
        IRepository<SubjectType> subjectTypes,
        IRepository<CoupleTime> coupleTimes,
        IRepository<Teacher> teachers,
        IRepository<Subgroup> subgroups,
        IUnitOfWork unitOfWork) : base(couples, unitOfWork)
    {
        _couples = couples;
        _coupleDates = coupleDates;
        _subjects = subjects;
        _subjectTypes = subjectTypes;
        _coupleTimes = coupleTimes;
        _teachers = teachers;
        _subgroups = subgroups;
    }

    protected override SortMap<Couple> Sorts => SortMap;
    protected override IReadOnlyList<Expression<Func<Couple, string?>>> SearchFields => Search;
    protected override string NotFoundMessage => "Пару не знайдено";

    protected override IQueryable<Couple> ApplyFilter(IQueryable<Couple> query, CoupleFilter filter)
    {
        if (filter.DayOfWeekId != null)
            query = query.Where(x => x.DayOfWeekId == filter.DayOfWeekId);

        if (filter.SubjectId != null)
            query = query.Where(x => x.SubjectId == filter.SubjectId);

        if (filter.TeacherId != null)
            query = query.Where(x => x.TeacherId == filter.TeacherId);

        if (filter.SubgroupId != null)
            query = query.Where(x => x.SubgroupId == filter.SubgroupId);

        return query;
    }

    protected override IQueryable<CoupleDto> Project(IQueryable<Couple> query)
    {
        return query.Select(x => new CoupleDto
        {
            Id = x.Id,
            SubjectId = x.SubjectId,
            SubjectTypeId = x.SubjectTypeId,
            DayOfWeekId = x.DayOfWeekId,
            CoupleTimeId = x.CoupleTimeId,
            SubgroupId = x.SubgroupId,
            TeacherId = x.TeacherId,
            StartDate = x.StartDate,
            EndDate = x.EndDate,
            IsRolling = x.IsRolling,
            Cabinet = x.Cabinet,
            AdditionalInformation = x.AdditionalInformation,
            Link = x.Link,
            AdditionalDates = x.CoupleDates
                .Where(d => !d.IsRemovedDate)
                .OrderBy(d => d.Date)
                .Select(d => new CoupleDateDto { Id = d.Id, Date = d.Date })
                .ToList(),
            RemovedDates = x.CoupleDates
                .Where(d => d.IsRemovedDate)
                .OrderBy(d => d.Date)
                .Select(d => new CoupleDateDto { Id = d.Id, Date = d.Date })
                .ToList()
        });
    }

    protected override void AfterLoad(IReadOnlyList<CoupleDto> items)
    {
        foreach (var item in items)
            item.WeeksCount = ScheduleCalculator.GetWeeksCount(item.StartDate, item.EndDate);
    }

    protected override void Apply(CoupleDto dto, Couple entity)
    {
        entity.SubjectId = dto.SubjectId;
        entity.SubjectTypeId = dto.SubjectTypeId;
        entity.DayOfWeekId = dto.DayOfWeekId;
        entity.CoupleTimeId = dto.CoupleTimeId;
        entity.SubgroupId = dto.SubgroupId;
        entity.TeacherId = dto.TeacherId;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.IsRolling = dto.IsRolling;
        entity.Cabinet = dto.Cabinet;
        entity.AdditionalInformation = dto.AdditionalInformation;
        entity.Link = dto.Link;
    }

    protected override void Normalize(CoupleDto dto)
    {
        dto.Cabinet = TextNormalizer.TrimOptional(dto.Cabinet);
        dto.AdditionalInformation = TextNormalizer.TrimOptional(dto.AdditionalInformation);
        dto.Link = TextNormalizer.TrimOptional(dto.Link);
        dto.AdditionalDates = Distinct(dto.AdditionalDates);
        dto.RemovedDates = Distinct(dto.RemovedDates);
    }

    protected override async Task ValidateAsync(CoupleDto dto, Couple? existing, CancellationToken cancellationToken)
    {
        var errors = CoupleDateRules.Validate(
            dto.StartDate,
            dto.EndDate,
            dto.AdditionalDates.Select(x => x.Date).ToList(),
            dto.RemovedDates.Select(x => x.Date).ToList(),
            out var dayOfWeekId);

        await EnsureExistsAsync(_subjects, dto.SubjectId, "subjectId", errors, cancellationToken);
        await EnsureExistsAsync(_subjectTypes, dto.SubjectTypeId, "subjectTypeId", errors, cancellationToken);
        await EnsureExistsAsync(_coupleTimes, dto.CoupleTimeId, "coupleTimeId", errors, cancellationToken);
        await EnsureExistsAsync(_teachers, dto.TeacherId, "teacherId", errors, cancellationToken);
        await EnsureExistsAsync(_subgroups, dto.SubgroupId, "subgroupId", errors, cancellationToken);

        errors.ThrowIfAny();

        dto.DayOfWeekId = dayOfWeekId!.Value;
    }

    public override async Task<CoupleDto> CreateAsync(CoupleDto dto, CancellationToken cancellationToken)
    {
        Normalize(dto);
        await ValidateAsync(dto, null, cancellationToken);

        var entity = new Couple();
        Apply(dto, entity);

        foreach (var date in dto.AdditionalDates)
            entity.CoupleDates.Add(new CoupleDate { Date = date.Date, IsRemovedDate = false });

        foreach (var date in dto.RemovedDates)
            entity.CoupleDates.Add(new CoupleDate { Date = date.Date, IsRemovedDate = true });

        await UnitOfWork.ExecuteInTransactionAsync(async token =>
        {
            await _couples.AddAsync(entity, token);
            await UnitOfWork.SaveChangesAsync(token);
        }, cancellationToken);

        return await GetAsync(entity.Id, cancellationToken);
    }

    public override async Task<CoupleDto> UpdateAsync(CoupleDto dto, CancellationToken cancellationToken)
    {
        Normalize(dto);

        var entity = await _couples.GetWithDatesAsync(dto.Id, cancellationToken)
            ?? throw new NotFoundException(NotFoundMessage);

        await ValidateAsync(dto, entity, cancellationToken);
        Apply(dto, entity);

        SyncDates(entity, dto.AdditionalDates, false);
        SyncDates(entity, dto.RemovedDates, true);

        await UnitOfWork.ExecuteInTransactionAsync(token => UnitOfWork.SaveChangesAsync(token), cancellationToken);

        return await GetAsync(entity.Id, cancellationToken);
    }

    private void SyncDates(Couple entity, IReadOnlyList<CoupleDateDto> incoming, bool isRemoved)
    {
        var current = entity.CoupleDates.Where(x => x.IsRemovedDate == isRemoved).ToList();
        var kept = new HashSet<CoupleDate>();
        var claimedDates = entity.CoupleDates.Where(x => x.IsRemovedDate != isRemoved).ToList();

        foreach (var item in incoming)
        {
            var match = current.FirstOrDefault(x => item.Id != 0 && x.Id == item.Id && !kept.Contains(x))
                ?? current.FirstOrDefault(x => x.Date == item.Date && !kept.Contains(x));

            if (match != null)
            {
                match.Date = item.Date;
                kept.Add(match);
                continue;
            }

            var reused = claimedDates.FirstOrDefault(x => item.Id != 0 && x.Id == item.Id);

            if (reused != null)
            {
                reused.IsRemovedDate = isRemoved;
                reused.Date = item.Date;
                kept.Add(reused);
                claimedDates.Remove(reused);
                continue;
            }

            entity.CoupleDates.Add(new CoupleDate { Date = item.Date, IsRemovedDate = isRemoved });
        }

        foreach (var stale in current.Where(x => !kept.Contains(x)))
        {
            entity.CoupleDates.Remove(stale);
            _coupleDates.Remove(stale);
        }
    }

    private static List<CoupleDateDto> Distinct(IEnumerable<CoupleDateDto> dates)
    {
        return dates
            .GroupBy(x => x.Date)
            .Select(x => x.OrderByDescending(d => d.Id).First())
            .OrderBy(x => x.Date)
            .ToList();
    }
}
