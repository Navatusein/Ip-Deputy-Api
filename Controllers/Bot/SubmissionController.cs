using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Bot;

[Tags("Bot:Submission Controller")]
[Route("bot/submission")]
public class SubmissionController : BotControllerBase
{
    private readonly IBotSubmissionService _submissionService;

    public SubmissionController(IBotSubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpGet("submissions-configs")]
    public async Task<ActionResult<IReadOnlyList<SubmissionsConfigDataDto>>> GetSubmissionsConfigsAsync(CancellationToken cancellationToken)
    {
        return Ok(await _submissionService.GetSubmissionsConfigsAsync(cancellationToken));
    }
}
