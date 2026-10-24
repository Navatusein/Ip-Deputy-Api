using IpDeputyApi.Authorization;
using IpDeputyApi.Dto;
using IpDeputyApi.Paging;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

public abstract class CrudControllerBase<TDto, TFilter> : FrontendControllerBase
    where TDto : IHasId
    where TFilter : PageRequest
{
    protected readonly ICrudService<TDto, TFilter> Service;

    protected CrudControllerBase(ICrudService<TDto, TFilter> service)
    {
        Service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<TDto>>> GetPageAsync([FromQuery] TFilter filter, CancellationToken cancellationToken)
    {
        return Ok(await Service.GetPageAsync(filter, cancellationToken));
    }

    [HttpGet("all")]
    [SkipPermission]
    public async Task<ActionResult<IReadOnlyList<TDto>>> GetAllAsync([FromQuery] TFilter filter, CancellationToken cancellationToken)
    {
        return Ok(await Service.GetAllAsync(filter, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TDto>> GetAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        return Ok(await Service.GetAsync(id, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<TDto>> CreateAsync([FromBody] TDto dto, CancellationToken cancellationToken)
    {
        return StatusCode(StatusCodes.Status201Created, await Service.CreateAsync(dto, cancellationToken));
    }

    [HttpPut]
    public async Task<ActionResult<TDto>> UpdateAsync([FromBody] TDto dto, CancellationToken cancellationToken)
    {
        return Ok(await Service.UpdateAsync(dto, cancellationToken));
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAsync([FromQuery] int[] ids, CancellationToken cancellationToken)
    {
        await Service.DeleteAsync(ids, cancellationToken);
        return NoContent();
    }
}
