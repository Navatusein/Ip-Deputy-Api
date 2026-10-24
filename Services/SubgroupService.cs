using System.Linq.Expressions;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Mapping;
using IpDeputyApi.Paging;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;

namespace IpDeputyApi.Services;

public class SubgroupService : CrudServiceBase<Subgroup, SubgroupDto, PageRequest>, ISubgroupService
{
    private static readonly SortMap<Subgroup> SortMap = new SortMap<Subgroup>()
        .Add("index", x => x.Index)
        .Add("name", x => x.Name)
        .Default(query => query.OrderBy(x => x.Index));

    private static readonly IReadOnlyList<Expression<Func<Subgroup, string?>>> Search = [x => x.Name];

    public SubgroupService(IRepository<Subgroup> repository, IUnitOfWork unitOfWork) : base(repository, unitOfWork)
    {
    }

    protected override SortMap<Subgroup> Sorts => SortMap;
    protected override IReadOnlyList<Expression<Func<Subgroup, string?>>> SearchFields => Search;
    protected override string NotFoundMessage => "Підгрупу не знайдено";

    protected override IQueryable<SubgroupDto> Project(IQueryable<Subgroup> query) => query.ProjectToDto();

    protected override void Apply(SubgroupDto dto, Subgroup entity) => dto.ApplyTo(entity);

    protected override void Normalize(SubgroupDto dto)
    {
        dto.Name = TextNormalizer.Clean(dto.Name);
    }
}
