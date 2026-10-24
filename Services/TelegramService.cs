using System.Linq.Expressions;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Paging;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Services;

public class TelegramService : CrudServiceBase<Telegram, TelegramDto, PageRequest>, ITelegramService
{
    public const string DefaultLanguage = "uk";
    public static readonly IReadOnlyList<string> Languages = ["uk", "ru", "en"];

    private static readonly SortMap<Telegram> SortMap = new SortMap<Telegram>()
        .Add("studentId", x => x.Student.Surname)
        .Add("telegramId", x => x.TelegramId)
        .Add("language", x => x.Language)
        .Add("scheduleCompact", x => x.ScheduleCompact)
        .Add("lastActivity", x => x.LastActivity)
        .Default(query => query.OrderBy(x => x.Student.Index));

    private static readonly IReadOnlyList<Expression<Func<Telegram, string?>>> Search = [x => x.Student.Name, x => x.Student.Surname];

    private readonly IRepository<Student> _students;

    public TelegramService(IRepository<Telegram> repository, IRepository<Student> students, IUnitOfWork unitOfWork) : base(repository, unitOfWork)
    {
        _students = students;
    }

    protected override SortMap<Telegram> Sorts => SortMap;
    protected override IReadOnlyList<Expression<Func<Telegram, string?>>> SearchFields => Search;
    protected override string NotFoundMessage => "Telegram акаунт не знайдено";

    protected override IQueryable<TelegramDto> Project(IQueryable<Telegram> query)
    {
        return query.Select(x => new TelegramDto
        {
            Id = x.Id,
            StudentId = x.StudentId,
            TelegramId = x.TelegramId,
            Language = x.Language,
            ScheduleCompact = x.ScheduleCompact,
            LastActivity = x.LastActivity
        });
    }

    protected override void Apply(TelegramDto dto, Telegram entity)
    {
        entity.StudentId = dto.StudentId;
        entity.TelegramId = dto.TelegramId;

        if (dto.Language != null)
            entity.Language = dto.Language;
        else if (string.IsNullOrEmpty(entity.Language))
            entity.Language = DefaultLanguage;

        if (dto.ScheduleCompact != null)
            entity.ScheduleCompact = dto.ScheduleCompact.Value;
    }

    protected override void Normalize(TelegramDto dto)
    {
        dto.Language = TextNormalizer.TrimOptional(dto.Language)?.ToLowerInvariant();
    }

    protected override async Task ValidateAsync(TelegramDto dto, Telegram? existing, CancellationToken cancellationToken)
    {
        var errors = new ValidationErrors();

        if (dto.Language != null && !Languages.Contains(dto.Language))
            errors.Add("language", ValidationMessages.InvalidValue);

        if (dto.TelegramId == 0)
            errors.Add("telegramId", ValidationMessages.Required);

        await EnsureExistsAsync(_students, dto.StudentId, "studentId", errors, cancellationToken);

        errors.ThrowIfAny();

        var existingId = existing?.Id ?? 0;

        if (await Repository.Query().AnyAsync(x => x.Id != existingId && x.TelegramId == dto.TelegramId, cancellationToken))
            throw new ConflictException("Цей Telegram акаунт уже прив'язаний до іншого студента");

        if (await Repository.Query().AnyAsync(x => x.Id != existingId && x.StudentId == dto.StudentId, cancellationToken))
            throw new ConflictException("Студент уже має прив'язаний Telegram акаунт");
    }
}
