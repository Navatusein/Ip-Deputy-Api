using System.Linq.Expressions;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Mapping;
using IpDeputyApi.Paging;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;

namespace IpDeputyApi.Services;

public class LinkService : CrudServiceBase<Link, LinkDto, PageRequest>, ILinkService
{
    private static readonly SortMap<Link> SortMap = new SortMap<Link>()
        .Add("name", x => x.Name)
        .Add("description", x => x.Description)
        .Add("url", x => x.Url)
        .Default(query => query.OrderBy(x => x.Name));

    private static readonly IReadOnlyList<Expression<Func<Link, string?>>> Search = [x => x.Name, x => x.Description, x => x.Url];

    public LinkService(IRepository<Link> repository, IUnitOfWork unitOfWork) : base(repository, unitOfWork)
    {
    }

    protected override SortMap<Link> Sorts => SortMap;
    protected override IReadOnlyList<Expression<Func<Link, string?>>> SearchFields => Search;
    protected override string NotFoundMessage => "Посилання не знайдено";

    protected override IQueryable<LinkDto> Project(IQueryable<Link> query) => query.ProjectToDto();

    protected override void Apply(LinkDto dto, Link entity) => dto.ApplyTo(entity);

    protected override void Normalize(LinkDto dto)
    {
        dto.Name = TextNormalizer.Clean(dto.Name);
        dto.Description = TextNormalizer.Trim(dto.Description);
        dto.Url = TextNormalizer.Trim(dto.Url);
    }
}
