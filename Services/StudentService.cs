using System.Linq.Expressions;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Mapping;
using IpDeputyApi.Paging;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;

namespace IpDeputyApi.Services;

public class StudentService : CrudServiceBase<Student, StudentDto, StudentFilter>, IStudentService
{
    private static readonly SortMap<Student> SortMap = new SortMap<Student>()
        .Add("index", x => x.Index)
        .Add("name", x => x.Name)
        .Add("surname", x => x.Surname)
        .Add("patronymic", x => x.Patronymic)
        .Add("subgroupId", x => x.SubgroupId)
        .Add("birthday", x => x.Birthday)
        .Add("email", x => x.Email)
        .Add("telegramNickname", x => x.TelegramNickname)
        .Default(query => query.OrderBy(x => x.Index));

    private static readonly IReadOnlyList<Expression<Func<Student, string?>>> Search =
    [
        x => x.Surname,
        x => x.Name,
        x => x.Patronymic,
        x => x.Email,
        x => x.TelegramNickname,
        x => x.TelegramPhone
    ];

    private readonly IRepository<Subgroup> _subgroups;

    public StudentService(IRepository<Student> repository, IRepository<Subgroup> subgroups, IUnitOfWork unitOfWork) : base(repository, unitOfWork)
    {
        _subgroups = subgroups;
    }

    protected override SortMap<Student> Sorts => SortMap;
    protected override IReadOnlyList<Expression<Func<Student, string?>>> SearchFields => Search;
    protected override string NotFoundMessage => "Студента не знайдено";

    protected override IQueryable<Student> ApplyFilter(IQueryable<Student> query, StudentFilter filter)
    {
        if (filter.SubgroupId != null)
            query = query.Where(x => x.SubgroupId == filter.SubgroupId);

        return query;
    }

    protected override IQueryable<StudentDto> Project(IQueryable<Student> query) => query.ProjectToDto();

    protected override void Apply(StudentDto dto, Student entity) => dto.ApplyTo(entity);

    protected override void Normalize(StudentDto dto)
    {
        dto.Name = TextNormalizer.Clean(dto.Name);
        dto.Surname = TextNormalizer.Clean(dto.Surname);
        dto.Patronymic = TextNormalizer.Clean(dto.Patronymic);
        dto.TelegramPhone = TextNormalizer.Trim(dto.TelegramPhone);
        dto.ContactPhone = TextNormalizer.TrimOptional(dto.ContactPhone);
        dto.Email = TextNormalizer.Trim(dto.Email);
        dto.FitEmail = TextNormalizer.Trim(dto.FitEmail);
        dto.TelegramNickname = TextNormalizer.TrimOptional(dto.TelegramNickname);
    }

    protected override async Task ValidateAsync(StudentDto dto, Student? existing, CancellationToken cancellationToken)
    {
        var errors = new ValidationErrors();

        if (dto.Name.Length == 0)
            errors.Add("name", ValidationMessages.Required);

        if (dto.Surname.Length == 0)
            errors.Add("surname", ValidationMessages.Required);

        if (dto.TelegramPhone.Length == 0)
            errors.Add("telegramPhone", ValidationMessages.Required);

        await EnsureExistsAsync(_subgroups, dto.SubgroupId, "subgroupId", errors, cancellationToken);

        errors.ThrowIfAny();
    }
}
