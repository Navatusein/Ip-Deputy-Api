using System.Linq.Expressions;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Mapping;
using IpDeputyApi.Paging;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Services;

public class SubmissionStudentService : CrudServiceBase<SubmissionStudent, SubmissionStudentDto, SubmissionStudentFilter>, ISubmissionStudentService
{
    private const string AlreadyRegisteredMessage = "Студент уже записаний на цю роботу";

    private static readonly SortMap<SubmissionStudent> SortMap = new SortMap<SubmissionStudent>()
        .Add("submittedAt", x => x.SubmittedAt)
        .Add("studentId", x => x.Student.Surname)
        .Add("submissionWorkId", x => x.SubmissionWork.Index)
        .Add("submissionsConfigId", x => x.SubmissionsConfigId)
        .Add("preferredPosition", x => x.PreferredPosition)
        .Default(query => query.OrderBy(x => x.SubmissionsConfigId).ThenBy(x => x.SubmissionWork.Index));

    private static readonly IReadOnlyList<Expression<Func<SubmissionStudent, string?>>> Search =
    [
        x => x.Student.Surname,
        x => x.Student.Name,
        x => x.SubmissionWork.Name
    ];

    private readonly IRepository<SubmissionWork> _works;
    private readonly IRepository<Student> _students;
    private readonly ISubmissionsConfigRepository _configs;
    private readonly IAppClock _clock;

    public SubmissionStudentService(
        IRepository<SubmissionStudent> repository,
        IRepository<SubmissionWork> works,
        IRepository<Student> students,
        ISubmissionsConfigRepository configs,
        IAppClock clock,
        IUnitOfWork unitOfWork) : base(repository, unitOfWork)
    {
        _works = works;
        _students = students;
        _configs = configs;
        _clock = clock;
    }

    protected override SortMap<SubmissionStudent> Sorts => SortMap;
    protected override IReadOnlyList<Expression<Func<SubmissionStudent, string?>>> SearchFields => Search;
    protected override string NotFoundMessage => "Запис не знайдено";

    protected override IQueryable<SubmissionStudent> ApplyFilter(IQueryable<SubmissionStudent> query, SubmissionStudentFilter filter)
    {
        if (filter.SubmissionsConfigId != null)
            query = query.Where(x => x.SubmissionsConfigId == filter.SubmissionsConfigId);

        if (filter.StudentId != null)
            query = query.Where(x => x.StudentId == filter.StudentId);

        return query;
    }

    protected override IQueryable<SubmissionStudentDto> Project(IQueryable<SubmissionStudent> query) => query.ProjectToDto();

    protected override void Apply(SubmissionStudentDto dto, SubmissionStudent entity)
    {
        entity.SubmissionWorkId = dto.SubmissionWorkId;
        entity.StudentId = dto.StudentId;
        entity.SubmissionsConfigId = dto.SubmissionsConfigId;
        entity.PreferredPosition = dto.PreferredPosition;
        entity.SubmittedAt = dto.SubmittedAt == default ? _clock.Today : dto.SubmittedAt;
    }

    protected override async Task ValidateAsync(SubmissionStudentDto dto, SubmissionStudent? existing, CancellationToken cancellationToken)
    {
        var errors = new ValidationErrors();

        if (!Enum.IsDefined(dto.PreferredPosition))
            errors.Add("preferredPosition", ValidationMessages.InvalidValue);

        var work = await _works.Query().FirstOrDefaultAsync(x => x.Id == dto.SubmissionWorkId, cancellationToken);

        if (work == null)
            errors.Add("submissionWorkId", ValidationMessages.NotFound);
        else
            dto.SubmissionsConfigId = work.SubmissionsConfigId;

        await EnsureExistsAsync(_students, dto.StudentId, "studentId", errors, cancellationToken);

        errors.ThrowIfAny();

        var existingId = existing?.Id ?? 0;

        if (await Repository.Query().AnyAsync(x => x.Id != existingId && x.StudentId == dto.StudentId && x.SubmissionWorkId == dto.SubmissionWorkId, cancellationToken))
            throw new ConflictException(AlreadyRegisteredMessage);
    }

    public override async Task<SubmissionStudentDto> CreateAsync(SubmissionStudentDto dto, CancellationToken cancellationToken)
    {
        var result = await UnitOfWork.ExecuteInTransactionAsync(async token =>
        {
            var created = await base.CreateAsync(dto, token);
            await SyncPreferredPositionAsync(created.StudentId, created.SubmissionsConfigId, created.PreferredPosition, token);
            return created;
        }, cancellationToken);

        return result;
    }

    public override async Task<SubmissionStudentDto> UpdateAsync(SubmissionStudentDto dto, CancellationToken cancellationToken)
    {
        var result = await UnitOfWork.ExecuteInTransactionAsync(async token =>
        {
            var updated = await base.UpdateAsync(dto, token);
            await SyncPreferredPositionAsync(updated.StudentId, updated.SubmissionsConfigId, updated.PreferredPosition, token);
            return updated;
        }, cancellationToken);

        return result;
    }

    public async Task<IReadOnlyList<SubmissionStudentDto>> GetForStudentAsync(int studentId, CancellationToken cancellationToken)
    {
        return await Repository.Query()
            .Where(x => x.StudentId == studentId)
            .OrderBy(x => x.SubmissionsConfigId)
            .ThenBy(x => x.SubmissionWork.Index)
            .ThenBy(x => x.Id)
            .ProjectToDto()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SubmissionStudentDto>> CreateForStudentAsync(int studentId, MySubmissionCreateDto dto, bool enforceSubgroup, CancellationToken cancellationToken)
    {
        var errors = new ValidationErrors();

        if (!Enum.IsDefined(dto.PreferredPosition))
            errors.Add("preferredPosition", ValidationMessages.InvalidValue);

        var workIds = dto.SubmissionWorkIds.Distinct().ToList();

        if (workIds.Count == 0)
            errors.Add("submissionWorkIds", ValidationMessages.Required);

        errors.ThrowIfAny();

        var student = await _students.Query().FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken)
            ?? throw new NotFoundException("Студента не знайдено");

        var config = await _configs.Query().FirstOrDefaultAsync(x => x.Id == dto.SubmissionsConfigId, cancellationToken)
            ?? throw new NotFoundException("Список захисту не знайдено");

        if (enforceSubgroup && config.SubgroupId != null && config.SubgroupId != student.SubgroupId)
            throw new ForbiddenException("Цей список доступний лише для іншої підгрупи");

        var validWorkIds = await _works.Query()
            .Where(x => x.SubmissionsConfigId == config.Id && workIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (validWorkIds.Count != workIds.Count)
            throw new ValidationException("submissionWorkIds", "Роботу не знайдено у цьому списку");

        var registered = await Repository.Query()
            .Where(x => x.StudentId == studentId && workIds.Contains(x.SubmissionWorkId))
            .Select(x => x.SubmissionWorkId)
            .ToListAsync(cancellationToken);

        await UnitOfWork.ExecuteInTransactionAsync(async token =>
        {
            foreach (var workId in workIds.Except(registered))
            {
                await Repository.AddAsync(new SubmissionStudent
                {
                    StudentId = studentId,
                    SubmissionWorkId = workId,
                    SubmissionsConfigId = config.Id,
                    PreferredPosition = dto.PreferredPosition,
                    SubmittedAt = _clock.Today
                }, token);
            }

            await UnitOfWork.SaveChangesAsync(token);
            await SyncPreferredPositionAsync(studentId, config.Id, dto.PreferredPosition, token);
        }, cancellationToken);

        return await Repository.Query()
            .Where(x => x.StudentId == studentId && x.SubmissionsConfigId == config.Id)
            .OrderBy(x => x.SubmissionWork.Index)
            .ThenBy(x => x.Id)
            .ProjectToDto()
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteForStudentAsync(int studentId, IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
            throw new ValidationException("ids", ValidationMessages.Required);

        await Repository.QueryTracked()
            .Where(x => x.StudentId == studentId && ids.Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task UpdatePreferredPositionAsync(int studentId, PreferredPositionDto dto, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(dto.PreferredPosition))
            throw new ValidationException("preferredPosition", ValidationMessages.InvalidValue);

        await SyncPreferredPositionAsync(studentId, dto.SubmissionsConfigId, dto.PreferredPosition, cancellationToken);
    }

    private Task SyncPreferredPositionAsync(int studentId, int submissionsConfigId, PreferredPosition position, CancellationToken cancellationToken)
    {
        return Repository.QueryTracked()
            .Where(x => x.StudentId == studentId && x.SubmissionsConfigId == submissionsConfigId)
            .ExecuteUpdateAsync(x => x.SetProperty(s => s.PreferredPosition, position), cancellationToken);
    }
}
