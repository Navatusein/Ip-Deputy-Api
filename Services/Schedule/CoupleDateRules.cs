using IpDeputyApi.Exceptions;

namespace IpDeputyApi.Services.Schedule;

public static class CoupleDateRules
{
    public const string SameWeekdayMessage = "Усі дати мають припадати на один день тижня";
    public const string NoDatesMessage = "Вкажіть дату початку або хоча б одну додаткову дату";
    public const string EndDateRequiredMessage = "Вкажіть дату завершення";
    public const string StartDateRequiredMessage = "Вкажіть дату початку";
    public const string EndBeforeStartMessage = "Дата завершення не може бути раніше дати початку";
    public const string AddedAndRemovedMessage = "Дата не може бути одночасно додатковою і скасованою";

    public static ValidationErrors Validate(
        DateOnly? startDate,
        DateOnly? endDate,
        IReadOnlyCollection<DateOnly> additionalDates,
        IReadOnlyCollection<DateOnly> removedDates,
        out int? dayOfWeekId)
    {
        var errors = new ValidationErrors();
        dayOfWeekId = null;

        if (startDate == null && additionalDates.Count == 0)
            errors.Add("dates", NoDatesMessage);

        if (startDate != null && endDate == null)
            errors.Add("endDate", EndDateRequiredMessage);

        if (startDate == null && endDate != null)
            errors.Add("startDate", StartDateRequiredMessage);

        if (startDate != null && endDate != null && endDate < startDate)
            errors.Add("endDate", EndBeforeStartMessage);

        if (additionalDates.Intersect(removedDates).Any())
            errors.Add("dates", AddedAndRemovedMessage);

        var allDates = new List<DateOnly>();

        if (startDate != null)
            allDates.Add(startDate.Value);

        if (endDate != null)
            allDates.Add(endDate.Value);

        allDates.AddRange(additionalDates);
        allDates.AddRange(removedDates);

        var weekdays = allDates.Select(ScheduleCalculator.ToDayOfWeekId).Distinct().ToList();

        if (weekdays.Count > 1)
            errors.Add("dates", SameWeekdayMessage);
        else if (weekdays.Count == 1)
            dayOfWeekId = weekdays[0];

        if (errors.HasErrors)
            dayOfWeekId = null;

        return errors;
    }
}
