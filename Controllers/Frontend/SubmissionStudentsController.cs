using IpDeputyApi.Authorization;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend:SubmissionStudents Controller")]
[Route("frontend/submission-student")]
[RequirePermission(Permissions.ManageSubmissions)]
public class SubmissionStudentsController : CrudControllerBase<SubmissionStudentDto, SubmissionStudentFilter>
{
    private readonly ISubmissionStudentService _submissionService;

    public SubmissionStudentsController(ISubmissionStudentService service) : base(service)
    {
        _submissionService = service;
    }

    [HttpGet("my")]
    [SkipPermission]
    public async Task<ActionResult<IReadOnlyList<SubmissionStudentDto>>> GetMyAsync([FromQuery] int? studentId, CancellationToken cancellationToken)
    {
        var resolvedStudentId = ResolveStudentId(studentId, Permissions.ManageSubmissions);
        return Ok(await _submissionService.GetForStudentAsync(resolvedStudentId, cancellationToken));
    }

    [HttpPost("my")]
    [SkipPermission]
    public async Task<ActionResult<IReadOnlyList<SubmissionStudentDto>>> CreateMyAsync(
        [FromBody] MySubmissionCreateDto dto,
        [FromQuery] int? studentId,
        CancellationToken cancellationToken)
    {
        var resolvedStudentId = ResolveStudentId(studentId, Permissions.ManageSubmissions);
        var enforceSubgroup = !User.HasPermission(Permissions.ManageSubmissions);

        return Ok(await _submissionService.CreateForStudentAsync(resolvedStudentId, dto, enforceSubgroup, cancellationToken));
    }

    [HttpDelete("my")]
    [SkipPermission]
    public async Task<ActionResult> DeleteMyAsync([FromQuery] int[] ids, [FromQuery] int? studentId, CancellationToken cancellationToken)
    {
        var resolvedStudentId = ResolveStudentId(studentId, Permissions.ManageSubmissions);
        await _submissionService.DeleteForStudentAsync(resolvedStudentId, ids, cancellationToken);
        return NoContent();
    }

    [HttpPut("my/preferred-position")]
    [SkipPermission]
    public async Task<ActionResult> UpdatePreferredPositionAsync(
        [FromBody] PreferredPositionDto dto,
        [FromQuery] int? studentId,
        CancellationToken cancellationToken)
    {
        var resolvedStudentId = ResolveStudentId(studentId, Permissions.ManageSubmissions);
        await _submissionService.UpdatePreferredPositionAsync(resolvedStudentId, dto, cancellationToken);
        return NoContent();
    }
}
