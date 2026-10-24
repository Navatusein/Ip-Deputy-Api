using System.Linq.Expressions;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Paging;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Services;

public class SubmissionsConfigService : CrudServiceBase<SubmissionsConfig, SubmissionsConfigDto, SubmissionsConfigFilter>, ISubmissionsConfigService
{
    private static readonly SortMap<SubmissionsConfig> SortMap = new SortMap<SubmissionsConfig>()
        .Add("subjectId", x => x.Subject!.Name)
        .Add("subjectTypeId", x => x.SubjectTypeId)
        .Add("subgroupId", x => x.SubgroupId)
        .Add("customName", x => x.CustomName)
        .Add("customType", x => x.CustomType)
        .Default(query => query.OrderBy(x => x.Id));

    private static readonly IReadOnlyList<Expression<Func<SubmissionsConfig, string?>>> Search =
    [
        x => x.Subject!.Name,
        x => x.Subject!.ShortName,
        x => x.CustomName,
        x => x.CustomType
    ];

    private readonly ISubmissionsConfigRepository _configs;
    private readonly IRepository<SubmissionWork> _works;
    private readonly IRepository<Student> _students;
    private readonly IRepository<Subject> _subjects;
    private readonly IRepository<SubjectType> _subjectTypes;
    private readonly IRepository<Subgroup> _subgroups;

    public SubmissionsConfigService(
        ISubmissionsConfigRepository configs,
        IRepository<SubmissionWork> works,
        IRepository<Student> students,
        IRepository<Subject> subjects,
        IRepository<SubjectType> subjectTypes,
        IRepository<Subgroup> subgroups,
        IUnitOfWork unitOfWork) : base(configs, unitOfWork)
    {
        _configs = configs;
        _works = works;
        _students = students;
        _subjects = subjects;
        _subjectTypes = subjectTypes;
        _subgroups = subgroups;
    }

    protected override SortMap<SubmissionsConfig> Sorts => SortMap;
    protected override IReadOnlyList<Expression<Func<SubmissionsConfig, string?>>> SearchFields => Search;
    protected override string NotFoundMessage => "Список захисту не знайдено";

    protected override IQueryable<SubmissionsConfig> ApplyFilter(IQueryable<SubmissionsConfig> query, SubmissionsConfigFilter filter)
    {
        if (filter.SubgroupId != null)
            query = query.Where(x => x.SubgroupId == filter.SubgroupId);

        if (filter.SubjectId != null)
            query = query.Where(x => x.SubjectId == filter.SubjectId);

        return query;
    }

    protected override IQueryable<SubmissionsConfigDto> Project(IQueryable<SubmissionsConfig> query)
    {
        return query.Select(x => new SubmissionsConfigDto
        {
            Id = x.Id,
            SubgroupId = x.SubgroupId,
            SubjectId = x.SubjectId,
            SubjectTypeId = x.SubjectTypeId,
            CustomType = x.CustomType,
            CustomName = x.CustomName,
            SubmissionWorks = x.SubmissionWorks
                .OrderBy(w => w.Index)
                .ThenBy(w => w.Id)
                .Select(w => new SubmissionWorkDto { Id = w.Id, Name = w.Name, Index = w.Index })
                .ToList()
        });
    }

    protected override void Apply(SubmissionsConfigDto dto, SubmissionsConfig entity)
    {
        entity.SubgroupId = dto.SubgroupId;
        entity.SubjectId = dto.SubjectId;
        entity.SubjectTypeId = dto.SubjectTypeId;
        entity.CustomType = dto.CustomType;
        entity.CustomName = dto.CustomName;
    }

    protected override void Normalize(SubmissionsConfigDto dto)
    {
        dto.CustomName = TextNormalizer.CleanOptional(dto.CustomName);
        dto.CustomType = TextNormalizer.CleanOptional(dto.CustomType);

        foreach (var work in dto.SubmissionWorks)
            work.Name = TextNormalizer.Clean(work.Name);
    }

    protected override async Task ValidateAsync(SubmissionsConfigDto dto, SubmissionsConfig? existing, CancellationToken cancellationToken)
    {
        var errors = new ValidationErrors();

        if (dto.SubjectId == null && dto.CustomName == null)
            errors.Add("customName", "Вкажіть предмет або власну назву");

        if (dto.SubjectTypeId == null && dto.CustomType == null)
            errors.Add("customType", "Вкажіть тип заняття або власний тип");

        if (dto.SubmissionWorks.Any(x => x.Name.Length == 0))
            errors.Add("submissionWorks", "Назва роботи є обов'язковою");

        await EnsureExistsAsync(_subjects, dto.SubjectId, "subjectId", errors, cancellationToken);
        await EnsureExistsAsync(_subjectTypes, dto.SubjectTypeId, "subjectTypeId", errors, cancellationToken);
        await EnsureExistsAsync(_subgroups, dto.SubgroupId, "subgroupId", errors, cancellationToken);

        errors.ThrowIfAny();
    }

    public override async Task<SubmissionsConfigDto> CreateAsync(SubmissionsConfigDto dto, CancellationToken cancellationToken)
    {
        Normalize(dto);
        await ValidateAsync(dto, null, cancellationToken);

        var entity = new SubmissionsConfig();
        Apply(dto, entity);

        foreach (var work in dto.SubmissionWorks)
            entity.SubmissionWorks.Add(new SubmissionWork { Name = work.Name, Index = work.Index });

        await UnitOfWork.ExecuteInTransactionAsync(async token =>
        {
            await _configs.AddAsync(entity, token);
            await UnitOfWork.SaveChangesAsync(token);
        }, cancellationToken);

        return await GetAsync(entity.Id, cancellationToken);
    }

    public override async Task<SubmissionsConfigDto> UpdateAsync(SubmissionsConfigDto dto, CancellationToken cancellationToken)
    {
        Normalize(dto);

        var entity = await _configs.GetWithWorksAsync(dto.Id, cancellationToken)
            ?? throw new NotFoundException(NotFoundMessage);

        await ValidateAsync(dto, entity, cancellationToken);
        Apply(dto, entity);

        var kept = new HashSet<int>();

        foreach (var work in dto.SubmissionWorks)
        {
            var existing = work.Id == 0 ? null : entity.SubmissionWorks.FirstOrDefault(x => x.Id == work.Id);

            if (existing != null)
            {
                existing.Name = work.Name;
                existing.Index = work.Index;
                kept.Add(existing.Id);
            }
            else
            {
                entity.SubmissionWorks.Add(new SubmissionWork { Name = work.Name, Index = work.Index });
            }
        }

        foreach (var stale in entity.SubmissionWorks.Where(x => x.Id != 0 && !kept.Contains(x.Id)).ToList())
        {
            entity.SubmissionWorks.Remove(stale);
            _works.Remove(stale);
        }

        await UnitOfWork.ExecuteInTransactionAsync(token => UnitOfWork.SaveChangesAsync(token), cancellationToken);

        return await GetAsync(entity.Id, cancellationToken);
    }

    public async Task<IReadOnlyList<SubmissionsConfigDto>> GetForStudentAsync(int studentId, bool otherSubgroups, CancellationToken cancellationToken)
    {
        var student = await _students.Query().FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken)
            ?? throw new NotFoundException("Студента не знайдено");

        var query = _configs.Query()
            .Where(x => otherSubgroups || x.SubgroupId == null || x.SubgroupId == student.SubgroupId)
            .OrderBy(x => x.Id);

        return await Project(query).ToListAsync(cancellationToken);
    }
}
