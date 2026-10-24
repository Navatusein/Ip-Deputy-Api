using IpDeputyApi.Authorization;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend:SubmissionsConfigs Controller")]
[Route("frontend/submissions-config")]
[RequirePermission(Permissions.ManageSubmissions)]
public class SubmissionsConfigsController : CrudControllerBase<SubmissionsConfigDto, SubmissionsConfigFilter>
{
    private readonly ISubmissionsConfigService _configService;

    public SubmissionsConfigsController(ISubmissionsConfigService service) : base(service)
    {
        _configService = service;
    }

    [HttpGet("for-student")]
    [SkipPermission]
    public async Task<ActionResult<IReadOnlyList<SubmissionsConfigDto>>> GetForStudentAsync(
        [FromQuery] bool otherSubgroups,
        [FromQuery] int? studentId,
        CancellationToken cancellationToken)
    {
        var resolvedStudentId = ResolveStudentId(studentId, Permissions.ManageSubmissions);
        return Ok(await _configService.GetForStudentAsync(resolvedStudentId, otherSubgroups, cancellationToken));
    }
}
