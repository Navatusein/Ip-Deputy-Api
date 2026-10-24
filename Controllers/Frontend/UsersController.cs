using IpDeputyApi.Authorization;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Paging;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend:Users Controller")]
[Route("frontend/user")]
[RequirePermission(Permissions.ManageUsers)]
public class UsersController : FrontendControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("permissions")]
    [SkipPermission]
    public ActionResult<IReadOnlyList<PermissionDto>> GetPermissions()
    {
        return Ok(_userService.GetPermissions());
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<UserDto>>> GetPageAsync([FromQuery] PageRequest filter, CancellationToken cancellationToken)
    {
        return Ok(await _userService.GetPageAsync(filter, cancellationToken));
    }

    [HttpGet("all")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAllAsync([FromQuery] PageRequest filter, CancellationToken cancellationToken)
    {
        return Ok(await _userService.GetAllAsync(filter, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        return Ok(await _userService.GetAsync(id, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateAsync([FromBody] UserDto dto, CancellationToken cancellationToken)
    {
        return StatusCode(StatusCodes.Status201Created, await _userService.CreateAsync(dto, cancellationToken));
    }

    [HttpPut]
    public async Task<ActionResult<UserDto>> UpdateAsync([FromBody] UserDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _userService.UpdateAsync(dto, cancellationToken));
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAsync([FromQuery] int[] ids, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(ids, cancellationToken);
        return NoContent();
    }
}
