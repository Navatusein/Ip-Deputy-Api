using IpDeputyApi.Paging;

namespace IpDeputyApi.Services.Interfaces;

public interface ICrudService<TDto, in TFilter> where TFilter : PageRequest
{
    Task<PageResult<TDto>> GetPageAsync(TFilter filter, CancellationToken cancellationToken);
    Task<IReadOnlyList<TDto>> GetAllAsync(TFilter filter, CancellationToken cancellationToken);
    Task<TDto> GetAsync(int id, CancellationToken cancellationToken);
    Task<TDto> CreateAsync(TDto dto, CancellationToken cancellationToken);
    Task<TDto> UpdateAsync(TDto dto, CancellationToken cancellationToken);
    Task DeleteAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken);
}
