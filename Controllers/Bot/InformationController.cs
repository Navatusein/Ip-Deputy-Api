using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Bot;

[Tags("Bot:Information Controller")]
[Route("bot/information")]
public class InformationController : BotControllerBase
{
    private readonly IBotInformationService _informationService;

    public InformationController(IBotInformationService informationService)
    {
        _informationService = informationService;
    }

    [HttpGet("students")]
    public async Task<ActionResult<IReadOnlyList<StudentInformationDto>>> GetStudentsAsync(CancellationToken cancellationToken)
    {
        return Ok(await _informationService.GetStudentsAsync(cancellationToken));
    }

    [HttpGet("teachers")]
    public async Task<ActionResult<IReadOnlyList<TeacherInformationDto>>> GetTeachersAsync(CancellationToken cancellationToken)
    {
        return Ok(await _informationService.GetTeachersAsync(cancellationToken));
    }

    [HttpGet("subjects")]
    public async Task<ActionResult<IReadOnlyList<SubjectInformationDto>>> GetSubjectsAsync(CancellationToken cancellationToken)
    {
        return Ok(await _informationService.GetSubjectsAsync(cancellationToken));
    }

    [HttpGet("links")]
    public async Task<ActionResult<IReadOnlyList<LinkInformationDto>>> GetLinksAsync(CancellationToken cancellationToken)
    {
        return Ok(await _informationService.GetLinksAsync(cancellationToken));
    }
}
