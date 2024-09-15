using AutoMapper;
using IpDeputyApi.Authentication;
using IpDeputyApi.Database;
using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace IpDeputyApi.Controllers.Bot;

[Tags("Bot:Student Controller")]
[Route("bot/student")]
[ApiController]
public class StudentController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<StudentController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public StudentController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("settings/{telegramId}")]
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    public async Task<ActionResult<StudentSettingsDto>> GetSettingsAsync([FromRoute] ulong telegramId, CancellationToken cancellationToken)
    {
        var telegram = await _context.Telegrams.FirstOrDefaultAsync(x => x.TelegramId == telegramId, cancellationToken);

        if (telegram == null)
            throw new HttpException("Invalid student id", StatusCodes.Status404NotFound);

        var settings = _mapper.Map<StudentSettingsDto>(telegram);
        
        return StatusCode(StatusCodes.Status200OK, settings);
    }

    [HttpPut]
    [Route("settings")]
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    public async Task<ActionResult<StudentSettingsDto>> UpdateSettingsAsync([FromBody] StudentSettingsDto settingsDto, CancellationToken cancellationToken)
    {
        var telegram = await _context.Telegrams.FirstOrDefaultAsync(x => x.TelegramId == settingsDto.TelegramId, cancellationToken);

        if (telegram == null)
            throw new HttpException("Invalid student id", StatusCodes.Status404NotFound);

        telegram.Language = settingsDto.Language;
        telegram.ScheduleCompact = settingsDto.ScheduleCompact;

        await _context.SaveChangesAsync(cancellationToken);

        return StatusCode(StatusCodes.Status200OK, settingsDto);
    }


    [HttpPut]
    [Route("last-activity/{telegramId}")]
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    public async Task<ActionResult> UpdateLastActivityAsync([FromRoute] ulong telegramId, CancellationToken cancellationToken)
    {
        var telegram = await _context.Telegrams.FirstOrDefaultAsync(x => x.TelegramId == telegramId, cancellationToken);

        if (telegram == null)
            throw new HttpException("Invalid student id", StatusCodes.Status404NotFound);

        telegram.LastActivity = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        
        return StatusCode(StatusCodes.Status200OK);
    }
}