using IpDeputyApi.Database.Models;
using IpDeputyApi.Services.Schedule;

namespace IpDeputyApi.Tests;

public class ScheduleCalculatorTests
{
    private static readonly DateOnly Start = new(2026, 9, 3);
    private static readonly DateOnly End = new(2026, 12, 31);

    private static Couple CreateCouple(bool isRolling = false, params CoupleDate[] dates)
    {
        var couple = new Couple { StartDate = Start, EndDate = End, IsRolling = isRolling };

        foreach (var date in dates)
            couple.CoupleDates.Add(date);

        return couple;
    }

    [Theory]
    [InlineData(2026, 9, 3, true)]
    [InlineData(2026, 9, 10, true)]
    [InlineData(2026, 9, 4, false)]
    [InlineData(2026, 8, 27, false)]
    [InlineData(2027, 1, 7, false)]
    [InlineData(2026, 12, 31, true)]
    public void OccursRegularly_Weekly(int year, int month, int day, bool expected)
    {
        Assert.Equal(expected, ScheduleCalculator.OccursRegularly(Start, End, false, new DateOnly(year, month, day)));
    }

    [Theory]
    [InlineData(2026, 9, 3, true)]
    [InlineData(2026, 9, 10, false)]
    [InlineData(2026, 9, 17, true)]
    [InlineData(2026, 9, 24, false)]
    public void OccursRegularly_Rolling(int year, int month, int day, bool expected)
    {
        Assert.Equal(expected, ScheduleCalculator.OccursRegularly(Start, End, true, new DateOnly(year, month, day)));
    }

    [Fact]
    public void OccursRegularly_WithoutStartDate_ReturnsFalse()
    {
        Assert.False(ScheduleCalculator.OccursRegularly(null, End, false, Start));
    }

    [Fact]
    public void OccursRegularly_WithoutEndDate_IsOpenEnded()
    {
        Assert.True(ScheduleCalculator.OccursRegularly(Start, null, false, new DateOnly(2027, 3, 4)));
    }

    [Fact]
    public void OccursOn_RemovedDate_IsExcluded()
    {
        var couple = CreateCouple(false, new CoupleDate { Date = new DateOnly(2026, 9, 10), IsRemovedDate = true });

        Assert.False(ScheduleCalculator.OccursOn(couple, new DateOnly(2026, 9, 10)));
        Assert.True(ScheduleCalculator.OccursOn(couple, new DateOnly(2026, 9, 17)));
    }

    [Fact]
    public void OccursOn_AdditionalDate_WinsOverRemoved()
    {
        var date = new DateOnly(2026, 9, 10);
        var couple = CreateCouple(false,
            new CoupleDate { Date = date, IsRemovedDate = true },
            new CoupleDate { Date = date, IsRemovedDate = false });

        Assert.True(ScheduleCalculator.OccursOn(couple, date));
    }

    [Fact]
    public void OccursOn_AdditionalDateOutsideRange()
    {
        var date = new DateOnly(2027, 2, 4);
        var couple = CreateCouple(false, new CoupleDate { Date = date, IsRemovedDate = false });

        Assert.True(ScheduleCalculator.OccursOn(couple, date));
    }

    [Fact]
    public void GetDates_CombinesRegularAdditionalAndRemoved()
    {
        var couple = CreateCouple(true,
            new CoupleDate { Date = new DateOnly(2026, 9, 17), IsRemovedDate = true },
            new CoupleDate { Date = new DateOnly(2026, 9, 24), IsRemovedDate = false });

        var dates = ScheduleCalculator.GetDates(couple, new DateOnly(2026, 9, 1), new DateOnly(2026, 10, 1));

        Assert.Equal([new DateOnly(2026, 9, 3), new DateOnly(2026, 9, 24), new DateOnly(2026, 10, 1)], dates);
    }

    [Fact]
    public void GetDates_RespectsFromDate()
    {
        var couple = CreateCouple();

        var dates = ScheduleCalculator.GetDates(couple, new DateOnly(2026, 12, 20), new DateOnly(2027, 6, 1));

        Assert.Equal([new DateOnly(2026, 12, 24), new DateOnly(2026, 12, 31)], dates);
    }

    [Theory]
    [InlineData("2026-09-03", "2026-12-31", 18)]
    [InlineData("2026-09-03", "2026-09-03", 1)]
    [InlineData("2026-09-03", "2026-09-09", 1)]
    [InlineData("2026-09-03", "2026-09-10", 2)]
    public void GetWeeksCount(string start, string end, int expected)
    {
        Assert.Equal(expected, ScheduleCalculator.GetWeeksCount(DateOnly.Parse(start), DateOnly.Parse(end)));
    }

    [Fact]
    public void GetWeeksCount_WithoutRange_ReturnsNull()
    {
        Assert.Null(ScheduleCalculator.GetWeeksCount(Start, null));
        Assert.Null(ScheduleCalculator.GetWeeksCount(End, Start));
    }

    [Theory]
    [InlineData("2026-10-19", 1)]
    [InlineData("2026-10-22", 4)]
    [InlineData("2026-10-25", 7)]
    public void ToDayOfWeekId_UsesIsoNumbering(string date, int expected)
    {
        Assert.Equal(expected, ScheduleCalculator.ToDayOfWeekId(DateOnly.Parse(date)));
    }

    [Theory]
    [InlineData("2026-10-19")]
    [InlineData("2026-10-22")]
    [InlineData("2026-10-25")]
    public void GetWeekStart_ReturnsMonday(string date)
    {
        Assert.Equal(new DateOnly(2026, 10, 19), ScheduleCalculator.GetWeekStart(DateOnly.Parse(date)));
    }
}
