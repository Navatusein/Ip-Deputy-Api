using System.Linq.Expressions;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Mapping;
using IpDeputyApi.Paging;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;

namespace IpDeputyApi.Services;

public class TeacherService : CrudServiceBase<Teacher, TeacherDto, PageRequest>, ITeacherService
{
    private static readonly SortMap<Teacher> SortMap = new SortMap<Teacher>()
        .Add("name", x => x.Name)
        .Add("surname", x => x.Surname)
        .Add("patronymic", x => x.Patronymic)
        .Add("email", x => x.Email)
        .Add("telegramNickname", x => x.TelegramNickname)
        .Default(query => query.OrderBy(x => x.Surname).ThenBy(x => x.Name));

    private static readonly IReadOnlyList<Expression<Func<Teacher, string?>>> Search =
    [
        x => x.Surname,
        x => x.Name,
        x => x.Patronymic,
        x => x.Email,
        x => x.TelegramNickname
    ];

    public TeacherService(IRepository<Teacher> repository, IUnitOfWork unitOfWork) : base(repository, unitOfWork)
    {
    }

    protected override SortMap<Teacher> Sorts => SortMap;
    protected override IReadOnlyList<Expression<Func<Teacher, string?>>> SearchFields => Search;
    protected override string NotFoundMessage => "Викладача не знайдено";

    protected override IQueryable<TeacherDto> Project(IQueryable<Teacher> query) => query.ProjectToDto();

    protected override void Apply(TeacherDto dto, Teacher entity) => dto.ApplyTo(entity);

    protected override void Normalize(TeacherDto dto)
    {
        dto.Name = TextNormalizer.Clean(dto.Name);
        dto.Surname = TextNormalizer.Clean(dto.Surname);
        dto.Patronymic = TextNormalizer.Clean(dto.Patronymic);
        dto.ContactPhone = TextNormalizer.CleanOptional(dto.ContactPhone);
        dto.Email = TextNormalizer.CleanOptional(dto.Email);
        dto.FitEmail = TextNormalizer.CleanOptional(dto.FitEmail);
        dto.TelegramNickname = TextNormalizer.CleanOptional(dto.TelegramNickname);
    }

    protected override Task ValidateAsync(TeacherDto dto, Teacher? existing, CancellationToken cancellationToken)
    {
        var errors = new ValidationErrors();

        if (dto.Name.Length == 0)
            errors.Add("name", ValidationMessages.Required);

        if (dto.Surname.Length == 0)
            errors.Add("surname", ValidationMessages.Required);

        errors.ThrowIfAny();

        return Task.CompletedTask;
    }
}
