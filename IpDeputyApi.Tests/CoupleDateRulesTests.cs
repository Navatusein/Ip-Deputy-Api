using IpDeputyApi.Services.Schedule;

namespace IpDeputyApi.Tests;

public class CoupleDateRulesTests
{
    private static readonly DateOnly Thursday = new(2026, 9, 3);
    private static readonly DateOnly LastThursday = new(2026, 12, 31);

    [Fact]
    public void Validate_DerivesDayOfWeekFromRange()
    {
        var errors = CoupleDateRules.Validate(Thursday, LastThursday, [new DateOnly(2027, 1, 7)], [new DateOnly(2026, 10, 1)], out var dayOfWeekId);

        Assert.False(errors.HasErrors);
        Assert.Equal(4, dayOfWeekId);
    }

    [Fact]
    public void Validate_DerivesDayOfWeekFromAdditionalDatesOnly()
    {
        var errors = CoupleDateRules.Validate(null, null, [new DateOnly(2026, 10, 25)], [], out var dayOfWeekId);

        Assert.False(errors.HasErrors);
        Assert.Equal(7, dayOfWeekId);
    }

    [Fact]
    public void Validate_RejectsMixedWeekdays()
    {
        var errors = CoupleDateRules.Validate(Thursday, LastThursday, [new DateOnly(2026, 9, 4)], [], out var dayOfWeekId);

        Assert.Null(dayOfWeekId);
        Assert.Contains(CoupleDateRules.SameWeekdayMessage, errors.ToDictionary()["dates"]);
    }

    [Fact]
    public void Validate_RejectsEndDateOnDifferentWeekday()
    {
        var errors = CoupleDateRules.Validate(Thursday, new DateOnly(2026, 12, 30), [], [], out _);

        Assert.Contains(CoupleDateRules.SameWeekdayMessage, errors.ToDictionary()["dates"]);
    }

    [Fact]
    public void Validate_RequiresAnyDate()
    {
        var errors = CoupleDateRules.Validate(null, null, [], [], out var dayOfWeekId);

        Assert.Null(dayOfWeekId);
        Assert.Contains(CoupleDateRules.NoDatesMessage, errors.ToDictionary()["dates"]);
    }

    [Fact]
    public void Validate_RequiresEndDateWithStartDate()
    {
        var errors = CoupleDateRules.Validate(Thursday, null, [], [], out _);

        Assert.Contains(CoupleDateRules.EndDateRequiredMessage, errors.ToDictionary()["endDate"]);
    }

    [Fact]
    public void Validate_RequiresStartDateWithEndDate()
    {
        var errors = CoupleDateRules.Validate(null, LastThursday, [Thursday], [], out _);

        Assert.Contains(CoupleDateRules.StartDateRequiredMessage, errors.ToDictionary()["startDate"]);
    }

    [Fact]
    public void Validate_RejectsEndBeforeStart()
    {
        var errors = CoupleDateRules.Validate(LastThursday, Thursday, [], [], out _);

        Assert.Contains(CoupleDateRules.EndBeforeStartMessage, errors.ToDictionary()["endDate"]);
    }

    [Fact]
    public void Validate_RejectsDateBothAddedAndRemoved()
    {
        var date = new DateOnly(2026, 10, 1);
        var errors = CoupleDateRules.Validate(Thursday, LastThursday, [date], [date], out _);

        Assert.Contains(CoupleDateRules.AddedAndRemovedMessage, errors.ToDictionary()["dates"]);
    }
}
