using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Bot;

[Tags("Bot:Schedule Controller")]
[Route("bot/schedule")]
[Route("api/bot/schedule")]
public class ScheduleController : BotControllerBase
{
    private readonly IBotScheduleService _scheduleService;

    public ScheduleController(IBotScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet("day")]
    public async Task<ActionResult<ScheduleDayDto>> GetDayAsync([FromQuery] ulong telegramId, [FromQuery] string dateString, CancellationToken cancellationToken)
    {
        return Ok(await _scheduleService.GetDayAsync(telegramId, ParseDate(dateString), cancellationToken));
    }

    [HttpGet("week")]
    public async Task<ActionResult<ScheduleWeekDto>> GetWeekAsync([FromQuery] ulong telegramId, [FromQuery] string dateString, CancellationToken cancellationToken)
    {
        return Ok(await _scheduleService.GetWeekAsync(telegramId, ParseDate(dateString), cancellationToken));
    }
}
