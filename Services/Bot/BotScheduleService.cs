using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;
using IpDeputyApi.Services.Schedule;
using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Services.Bot;

public class BotScheduleService : IBotScheduleService
{
    private readonly ITelegramRepository _telegrams;
    private readonly ICoupleRepository _couples;
    private readonly IAdditionalCoupleRepository _additionalCouples;
    private readonly IRepository<CoupleTime> _coupleTimes;

    public BotScheduleService(
        ITelegramRepository telegrams,
        ICoupleRepository couples,
        IAdditionalCoupleRepository additionalCouples,
        IRepository<CoupleTime> coupleTimes)
    {
        _telegrams = telegrams;
        _couples = couples;
        _additionalCouples = additionalCouples;
        _coupleTimes = coupleTimes;
    }

    public async Task<ScheduleDayDto> GetDayAsync(ulong telegramId, DateOnly date, CancellationToken cancellationToken)
    {
        var student = await GetStudentAsync(telegramId, cancellationToken);
        var days = await BuildDaysAsync(student, date, date, cancellationToken);

        return days[0];
    }

    public async Task<ScheduleWeekDto> GetWeekAsync(ulong telegramId, DateOnly date, CancellationToken cancellationToken)
    {
        var student = await GetStudentAsync(telegramId, cancellationToken);
        var weekStart = ScheduleCalculator.GetWeekStart(date);
        var days = await BuildDaysAsync(student, weekStart, weekStart.AddDays(6), cancellationToken);

        var coupleTimes = await _coupleTimes.Query().OrderBy(x => x.Index).ToListAsync(cancellationToken);

        return new ScheduleWeekDto
        {
            CoupleTimes = coupleTimes.Select(x => x.GetTimeFormatted()).ToList(),
            ScheduleDays = days
        };
    }

    private async Task<Student> GetStudentAsync(ulong telegramId, CancellationToken cancellationToken)
    {
        var telegram = await _telegrams.GetByTelegramIdAsync(telegramId, cancellationToken)
            ?? throw new NotFoundException("Студента не знайдено");

        return telegram.Student;
    }

    private async Task<List<ScheduleDayDto>> BuildDaysAsync(Student student, DateOnly from, DateOnly to, CancellationToken cancellationToken)
    {
        var couples = await _couples.GetForPeriodAsync(from, to, cancellationToken);
        var additionalCouples = await _additionalCouples.GetForPeriodAsync(from, to, cancellationToken);

        var days = new List<ScheduleDayDto>();

        for (var day = from; day <= to; day = day.AddDays(1))
        {
            var current = day;

            var regular = couples
                .Where(x => ScheduleCalculator.OccursOn(x, current))
                .OrderBy(x => x.CoupleTime.Index)
                .Select(x => new CoupleDataDto
                {
                    Subject = x.Subject.ShortName,
                    SubjectType = x.SubjectType.ShortName,
                    CoupleIndex = x.CoupleTime.Index - 1,
                    Time = x.CoupleTime.GetTimeFormatted(),
                    IsMySubgroup = x.SubgroupId == null || x.SubgroupId == student.SubgroupId,
                    Cabinet = x.Cabinet,
                    Link = x.Link,
                    AdditionalInformation = x.AdditionalInformation
                });

            var additional = additionalCouples
                .Where(x => x.Date == current)
                .Select(x => new CoupleDataDto
                {
                    Subject = x.Subject.ShortName,
                    SubjectType = x.SubjectType.ShortName,
                    CoupleIndex = x.CoupleTime != null ? x.CoupleTime.Index - 1 : null,
                    Time = x.CoupleTime?.GetTimeFormatted() ?? x.Time?.ToString("HH:mm") ?? "",
                    IsMySubgroup = x.SubgroupId == null || x.SubgroupId == student.SubgroupId,
                    Cabinet = x.Cabinet,
                    Link = x.Link,
                    AdditionalInformation = x.AdditionalInformation
                });

            days.Add(new ScheduleDayDto
            {
                Date = current.ToString("dd.MM"),
                Couples = regular
                    .Concat(additional)
                    .OrderByDescending(x => x.IsMySubgroup)
                    .ThenBy(x => x.Time, StringComparer.Ordinal)
                    .ToList()
            });
        }

        return days;
    }
}
