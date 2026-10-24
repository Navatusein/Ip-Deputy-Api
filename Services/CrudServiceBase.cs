using System.Linq.Expressions;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Paging;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Services;

public abstract class CrudServiceBase<TEntity, TDto, TFilter> : ICrudService<TDto, TFilter>
    where TEntity : class, IEntity, new()
    where TDto : IHasId
    where TFilter : PageRequest
{
    protected readonly IRepository<TEntity> Repository;
    protected readonly IUnitOfWork UnitOfWork;

    protected CrudServiceBase(IRepository<TEntity> repository, IUnitOfWork unitOfWork)
    {
        Repository = repository;
        UnitOfWork = unitOfWork;
    }

    protected abstract SortMap<TEntity> Sorts { get; }
    protected abstract IReadOnlyList<Expression<Func<TEntity, string?>>> SearchFields { get; }
    protected virtual string NotFoundMessage => ValidationMessages.NotFound;

    protected abstract IQueryable<TDto> Project(IQueryable<TEntity> query);
    protected abstract void Apply(TDto dto, TEntity entity);

    protected virtual IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, TFilter filter)
    {
        return query;
    }

    protected virtual void Normalize(TDto dto)
    {
    }

    protected virtual Task ValidateAsync(TDto dto, TEntity? existing, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected virtual Task ValidateDeleteAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected virtual void AfterLoad(IReadOnlyList<TDto> items)
    {
    }

    protected IQueryable<TEntity> BuildQuery(TFilter filter)
    {
        return ApplyFilter(Repository.Query(), filter).ApplySearch(filter.Q, SearchFields);
    }

    public virtual async Task<PageResult<TDto>> GetPageAsync(TFilter filter, CancellationToken cancellationToken)
    {
        var query = Sorts.Apply(BuildQuery(filter), filter.Sort, filter.IsDescending);
        var result = await query.ToPageResultAsync(filter, Project, cancellationToken);

        AfterLoad(result.Items);

        return result;
    }

    public virtual async Task<IReadOnlyList<TDto>> GetAllAsync(TFilter filter, CancellationToken cancellationToken)
    {
        var items = await Project(Sorts.ApplyDefault(BuildQuery(filter))).ToListAsync(cancellationToken);

        AfterLoad(items);

        return items;
    }

    public virtual async Task<TDto> GetAsync(int id, CancellationToken cancellationToken)
    {
        var dto = await Project(Repository.Query().Where(x => x.Id == id)).FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(NotFoundMessage);

        AfterLoad([dto]);

        return dto;
    }

    public virtual async Task<TDto> CreateAsync(TDto dto, CancellationToken cancellationToken)
    {
        Normalize(dto);
        await ValidateAsync(dto, null, cancellationToken);

        var entity = new TEntity();
        Apply(dto, entity);

        await Repository.AddAsync(entity, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return await GetAsync(entity.Id, cancellationToken);
    }

    public virtual async Task<TDto> UpdateAsync(TDto dto, CancellationToken cancellationToken)
    {
        Normalize(dto);

        var entity = await Repository.FindAsync(dto.Id, cancellationToken)
            ?? throw new NotFoundException(NotFoundMessage);

        await ValidateAsync(dto, entity, cancellationToken);
        Apply(dto, entity);

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return await GetAsync(entity.Id, cancellationToken);
    }

    public virtual async Task DeleteAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
    {
        var distinctIds = ids.Distinct().ToList();

        if (distinctIds.Count == 0)
            throw new ValidationException("ids", ValidationMessages.Required);

        await ValidateDeleteAsync(distinctIds, cancellationToken);
        await Repository.DeleteByIdsAsync(distinctIds, cancellationToken);
    }

    protected static async Task EnsureExistsAsync<T>(IRepository<T> repository, int? id, string field, ValidationErrors errors, CancellationToken cancellationToken)
        where T : class, IEntity
    {
        if (id == null)
            return;

        if (!await repository.ExistsAsync(id.Value, cancellationToken))
            errors.Add(field, ValidationMessages.NotFound);
    }
}
