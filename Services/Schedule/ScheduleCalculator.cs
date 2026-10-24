using IpDeputyApi.Database.Models;

namespace IpDeputyApi.Services.Schedule;

public static class ScheduleCalculator
{
    public static int ToDayOfWeekId(DateOnly date)
    {
        return date.DayOfWeek == System.DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;
    }

    public static DateOnly GetWeekStart(DateOnly date)
    {
        return date.AddDays(1 - ToDayOfWeekId(date));
    }

    public static int? GetWeeksCount(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate == null || endDate == null || endDate < startDate)
            return null;

        return (endDate.Value.DayNumber - startDate.Value.DayNumber) / 7 + 1;
    }

    public static bool OccursRegularly(DateOnly? startDate, DateOnly? endDate, bool isRolling, DateOnly date)
    {
        if (startDate == null)
            return false;

        if (date < startDate || (endDate != null && date > endDate))
            return false;

        var days = date.DayNumber - startDate.Value.DayNumber;

        if (days % 7 != 0)
            return false;

        return !isRolling || days / 7 % 2 == 0;
    }

    public static bool OccursOn(Couple couple, DateOnly date)
    {
        if (couple.CoupleDates.Any(x => !x.IsRemovedDate && x.Date == date))
            return true;

        if (couple.CoupleDates.Any(x => x.IsRemovedDate && x.Date == date))
            return false;

        return OccursRegularly(couple.StartDate, couple.EndDate, couple.IsRolling, date);
    }

    public static IReadOnlyList<DateOnly> GetDates(Couple couple, DateOnly from, DateOnly to)
    {
        var candidates = new SortedSet<DateOnly>();

        if (couple.StartDate != null)
        {
            var step = couple.IsRolling ? 14 : 7;
            var last = couple.EndDate is { } endDate && endDate < to ? endDate : to;

            for (var date = couple.StartDate.Value; date <= last; date = date.AddDays(step))
            {
                if (date >= from)
                    candidates.Add(date);
            }
        }

        foreach (var coupleDate in couple.CoupleDates.Where(x => !x.IsRemovedDate && x.Date >= from && x.Date <= to))
            candidates.Add(coupleDate.Date);

        return candidates.Where(x => OccursOn(couple, x)).ToList();
    }
}
