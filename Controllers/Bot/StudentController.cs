using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Bot;

[Tags("Bot:Student Controller")]
[Route("bot/student")]
public class StudentController : BotControllerBase
{
    private readonly IBotStudentService _studentService;

    public StudentController(IBotStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet("settings/{telegramId}")]
    public async Task<ActionResult<StudentSettingsDto>> GetSettingsAsync([FromRoute] ulong telegramId, CancellationToken cancellationToken)
    {
        return Ok(await _studentService.GetSettingsAsync(telegramId, cancellationToken));
    }

    [HttpPut("settings")]
    public async Task<ActionResult<StudentSettingsDto>> UpdateSettingsAsync([FromBody] StudentSettingsDto settingsDto, CancellationToken cancellationToken)
    {
        return Ok(await _studentService.UpdateSettingsAsync(settingsDto, cancellationToken));
    }

    [HttpPut("last-activity/{telegramId}")]
    public async Task<ActionResult> UpdateLastActivityAsync([FromRoute] ulong telegramId, CancellationToken cancellationToken)
    {
        await _studentService.UpdateLastActivityAsync(telegramId, cancellationToken);
        return Ok();
    }
}
