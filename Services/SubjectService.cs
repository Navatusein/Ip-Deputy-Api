using System.Linq.Expressions;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Mapping;
using IpDeputyApi.Paging;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;

namespace IpDeputyApi.Services;

public class SubjectService : CrudServiceBase<Subject, SubjectDto, PageRequest>, ISubjectService
{
    private static readonly SortMap<Subject> SortMap = new SortMap<Subject>()
        .Add("name", x => x.Name)
        .Add("shortName", x => x.ShortName)
        .Add("laboratoryCount", x => x.LaboratoryCount)
        .Add("practicalCount", x => x.PracticalCount)
        .Default(query => query.OrderBy(x => x.Name));

    private static readonly IReadOnlyList<Expression<Func<Subject, string?>>> Search = [x => x.Name, x => x.ShortName];

    public SubjectService(IRepository<Subject> repository, IUnitOfWork unitOfWork) : base(repository, unitOfWork)
    {
    }

    protected override SortMap<Subject> Sorts => SortMap;
    protected override IReadOnlyList<Expression<Func<Subject, string?>>> SearchFields => Search;
    protected override string NotFoundMessage => "Предмет не знайдено";

    protected override IQueryable<SubjectDto> Project(IQueryable<Subject> query) => query.ProjectToDto();

    protected override void Apply(SubjectDto dto, Subject entity) => dto.ApplyTo(entity);

    protected override void Normalize(SubjectDto dto)
    {
        dto.Name = TextNormalizer.Clean(dto.Name);
        dto.ShortName = TextNormalizer.Clean(dto.ShortName);
    }
}
