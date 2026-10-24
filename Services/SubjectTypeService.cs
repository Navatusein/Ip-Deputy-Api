using System.Linq.Expressions;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Mapping;
using IpDeputyApi.Paging;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;

namespace IpDeputyApi.Services;

public class SubjectTypeService : CrudServiceBase<SubjectType, SubjectTypeDto, PageRequest>, ISubjectTypeService
{
    private static readonly int SystemTypesMaxId = Enum.GetValues<SubjectTypes>().Max(x => (int)x);

    private static readonly SortMap<SubjectType> SortMap = new SortMap<SubjectType>()
        .Add("id", x => x.Id)
        .Add("name", x => x.Name)
        .Add("shortName", x => x.ShortName)
        .Default(query => query.OrderBy(x => x.Id));

    private static readonly IReadOnlyList<Expression<Func<SubjectType, string?>>> Search = [x => x.Name, x => x.ShortName];

    public SubjectTypeService(IRepository<SubjectType> repository, IUnitOfWork unitOfWork) : base(repository, unitOfWork)
    {
    }

    protected override SortMap<SubjectType> Sorts => SortMap;
    protected override IReadOnlyList<Expression<Func<SubjectType, string?>>> SearchFields => Search;
    protected override string NotFoundMessage => "Тип заняття не знайдено";

    protected override IQueryable<SubjectTypeDto> Project(IQueryable<SubjectType> query) => query.ProjectToDto();

    protected override void Apply(SubjectTypeDto dto, SubjectType entity) => dto.ApplyTo(entity);

    protected override void Normalize(SubjectTypeDto dto)
    {
        dto.Name = TextNormalizer.Clean(dto.Name);
        dto.ShortName = TextNormalizer.Clean(dto.ShortName);
    }

    protected override Task ValidateDeleteAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
    {
        if (ids.Any(x => x <= SystemTypesMaxId))
            throw new ConflictException("Системні типи занять не можна видаляти");

        return Task.CompletedTask;
    }
}
