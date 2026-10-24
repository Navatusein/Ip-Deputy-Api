using System.Linq.Expressions;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Mapping;
using IpDeputyApi.Paging;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;
using IpDeputyApi.Services.Schedule;

namespace IpDeputyApi.Services;

public class AdditionalCoupleService : CrudServiceBase<AdditionalCouple, AdditionalCoupleDto, AdditionalCoupleFilter>, IAdditionalCoupleService
{
    private const string TimeChoiceMessage = "Вкажіть або номер пари, або час";

    private static readonly SortMap<AdditionalCouple> SortMap = new SortMap<AdditionalCouple>()
        .Add("date", x => x.Date)
        .Add("subjectId", x => x.Subject.Name)
        .Add("subjectTypeId", x => x.SubjectTypeId)
        .Add("teacherId", x => x.Teacher.Surname)
        .Add("subgroupId", x => x.SubgroupId)
        .Add("coupleTimeId", x => x.CoupleTimeId)
        .Add("time", x => x.Time)
        .Add("cabinet", x => x.Cabinet)
        .Add("dayOfWeekId", x => x.DayOfWeekId)
        .Add("additionalInformation", x => x.AdditionalInformation)
        .Add("link", x => x.Link)
        .Default(query => query.OrderByDescending(x => x.Date).ThenBy(x => x.CoupleTimeId).ThenBy(x => x.Time));

    private static readonly IReadOnlyList<Expression<Func<AdditionalCouple, string?>>> Search =
    [
        x => x.Subject.Name,
        x => x.Subject.ShortName,
        x => x.Teacher.Surname,
        x => x.Teacher.Name,
        x => x.Cabinet
    ];

    private readonly IRepository<Subject> _subjects;
    private readonly IRepository<SubjectType> _subjectTypes;
    private readonly IRepository<CoupleTime> _coupleTimes;
    private readonly IRepository<Teacher> _teachers;
    private readonly IRepository<Subgroup> _subgroups;

    public AdditionalCoupleService(
        IAdditionalCoupleRepository repository,
        IRepository<Subject> subjects,
        IRepository<SubjectType> subjectTypes,
        IRepository<CoupleTime> coupleTimes,
        IRepository<Teacher> teachers,
        IRepository<Subgroup> subgroups,
        IUnitOfWork unitOfWork) : base(repository, unitOfWork)
    {
        _subjects = subjects;
        _subjectTypes = subjectTypes;
        _coupleTimes = coupleTimes;
        _teachers = teachers;
        _subgroups = subgroups;
    }

    protected override SortMap<AdditionalCouple> Sorts => SortMap;
    protected override IReadOnlyList<Expression<Func<AdditionalCouple, string?>>> SearchFields => Search;
    protected override string NotFoundMessage => "Додаткову пару не знайдено";

    protected override IQueryable<AdditionalCouple> ApplyFilter(IQueryable<AdditionalCouple> query, AdditionalCoupleFilter filter)
    {
        if (filter.DateFrom != null)
            query = query.Where(x => x.Date >= filter.DateFrom);

        if (filter.DateTo != null)
            query = query.Where(x => x.Date <= filter.DateTo);

        if (filter.SubjectId != null)
            query = query.Where(x => x.SubjectId == filter.SubjectId);

        if (filter.TeacherId != null)
            query = query.Where(x => x.TeacherId == filter.TeacherId);

        return query;
    }

    protected override IQueryable<AdditionalCoupleDto> Project(IQueryable<AdditionalCouple> query) => query.ProjectToDto();

    protected override void Apply(AdditionalCoupleDto dto, AdditionalCouple entity)
    {
        dto.ApplyTo(entity);
        entity.DayOfWeekId = ScheduleCalculator.ToDayOfWeekId(dto.Date);
    }

    protected override void Normalize(AdditionalCoupleDto dto)
    {
        dto.Cabinet = TextNormalizer.TrimOptional(dto.Cabinet);
        dto.AdditionalInformation = TextNormalizer.TrimOptional(dto.AdditionalInformation);
        dto.Link = TextNormalizer.TrimOptional(dto.Link);
        dto.DayOfWeekId = ScheduleCalculator.ToDayOfWeekId(dto.Date);
    }

    protected override async Task ValidateAsync(AdditionalCoupleDto dto, AdditionalCouple? existing, CancellationToken cancellationToken)
    {
        var errors = new ValidationErrors();

        if ((dto.CoupleTimeId == null) == (dto.Time == null))
        {
            errors.Add("coupleTimeId", TimeChoiceMessage);
            errors.Add("time", TimeChoiceMessage);
        }

        if (dto.Date == default)
            errors.Add("date", ValidationMessages.Required);

        await EnsureExistsAsync(_subjects, dto.SubjectId, "subjectId", errors, cancellationToken);
        await EnsureExistsAsync(_subjectTypes, dto.SubjectTypeId, "subjectTypeId", errors, cancellationToken);
        await EnsureExistsAsync(_coupleTimes, dto.CoupleTimeId, "coupleTimeId", errors, cancellationToken);
        await EnsureExistsAsync(_teachers, dto.TeacherId, "teacherId", errors, cancellationToken);
        await EnsureExistsAsync(_subgroups, dto.SubgroupId, "subgroupId", errors, cancellationToken);

        errors.ThrowIfAny();
    }
}
