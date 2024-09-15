using AutoMapper;
using IpDeputyApi.Authentication;
using IpDeputyApi.Database;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace IpDeputyApi.Controllers.Bot;

[Tags("Bot:Schedule Controller")]
[Route("api/bot/schedule")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<ScheduleController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public ScheduleController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("day")]
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    public async Task<ActionResult<ScheduleDayDto>> GetDayScheduleAsync([FromQuery] ulong telegramId, [FromQuery] string dateString, CancellationToken cancellationToken)
    {
        var telegram = await _context.Telegrams.FirstOrDefaultAsync(x => x.TelegramId == telegramId, cancellationToken);

        if (telegram == null)
            throw new HttpException("Invalid student id", StatusCodes.Status404NotFound);

        var date = DateOnly.Parse(dateString);

        var scheduleDayDto = new ScheduleDayDto
        {
            Date = date.ToString("dd.MM"),
            Couples = await GetCouplesForDayAsync(telegram.Student, date, cancellationToken)
        };

        return StatusCode(StatusCodes.Status200OK, scheduleDayDto);
    }

    [HttpGet]
    [Route("week")]
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    public async Task<ActionResult<ScheduleWeekDto>> GetWeekScheduleAsync([FromQuery] ulong telegramId, [FromQuery] string dateString, CancellationToken cancellationToken)
    {
        var telegram = await _context.Telegrams.FirstOrDefaultAsync(x => x.TelegramId == telegramId, cancellationToken);

        if (telegram == null)
            throw new HttpException("Invalid student id", StatusCodes.Status404NotFound);

        var date = DateOnly.Parse(dateString);
        var startWeek = date.AddDays(-(date.DayOfWeek == 0 ? 6 : (int)date.DayOfWeek - 1));

        var scheduleDayDtos = new List<ScheduleDayDto>();
        
        for (var i = 0; i < 7; i++)
        {
            var day = startWeek.AddDays(i);
            
            scheduleDayDtos.Add(new ScheduleDayDto
            {
                Date = day.ToString("dd.MM"),
                Couples = await GetCouplesForDayAsync(telegram.Student, day, cancellationToken)
            });
        }
        
        var scheduleWeekDto = new ScheduleWeekDto()
        {
            CoupleTimes = await _context.CoupleTimes
                .OrderBy(x => x.Index)
                .Select(x => x.GetTimeFormatted())
                .ToListAsync(cancellationToken),
            ScheduleDays = scheduleDayDtos
        };

        return StatusCode(StatusCodes.Status200OK, scheduleWeekDto);
    }

    private static bool FilterCouples(Couple couple, DateOnly date)
    {
        if (couple.CoupleDates.Any(x => !x.IsRemovedDate && x.Date == date))
            return true;

        if (couple.CoupleDates.Any(x => x.IsRemovedDate && x.Date == date))
            return false;

        if (couple.StartDate == null)
            return false;
        
        if (date < couple.StartDate || date > couple.EndDate)
            return false;
        
        var startDate = couple.StartDate.Value.ToDateTime(new TimeOnly());
        var dateTime = date.ToDateTime(new TimeOnly());

        var weekSpan = (int)(dateTime - startDate).TotalDays / 7;

        if (couple.IsRolling && weekSpan % 2 == 1)
            return false;

        return true;
    }
    
    private async Task<List<CoupleDataDto>> GetCouplesForDayAsync(Student student, DateOnly date, CancellationToken cancellationToken)
    {
        var couples = await _context.Couples
            .Where(x => x.DayOfWeek.Index == (int)date.DayOfWeek)
            .OrderBy(x => x.CoupleTime.Index)
            .ToListAsync(cancellationToken);

        couples = couples.Where(x => FilterCouples(x, date)).ToList(); 
        
        var coupleDataDtos = couples.Select(couple => new CoupleDataDto
        {
            Subject = couple.Subject.ShortName,
            SubjectType = couple.SubjectType.ShortName,
            CoupleIndex = couple.CoupleTime.Index - 1,
            Time = couple.CoupleTime.GetTimeFormatted(),
            IsMySubgroup = couple.SubgroupId == null || couple.SubgroupId == student.SubgroupId,
            Cabinet = couple.Cabinet,
            Link = couple.Link,
            AdditionalInformation = couple.AdditionalInformation,
        }).ToList();
        
        var additionalCouples = await _context.AdditionalCouples
            .Where(x => x.Date == date)
            .Select(x => new CoupleDataDto
            {
                Subject = x.Subject.ShortName,
                SubjectType = x.SubjectType.ShortName,
                CoupleIndex = x.CoupleTimeId != null ? x.CoupleTimeId - 1 : null,
                Time = x.CoupleTimeId == null ? x.Time!.Value.ToString("HH:mm") : x.CoupleTime!.GetTimeFormatted(),
                IsMySubgroup = x.SubgroupId == null || x.SubgroupId == student.SubgroupId,
                Cabinet = x.Cabinet,
                Link = x.Link,
                AdditionalInformation = x.AdditionalInformation,
            }).ToListAsync(cancellationToken);
        
        coupleDataDtos.AddRange(additionalCouples);

        return coupleDataDtos
            .OrderByDescending(x => x.IsMySubgroup)
            .ThenBy(x => x.Time)
            .ToList();
    }
}