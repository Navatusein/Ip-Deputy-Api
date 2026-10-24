using IpDeputyApi.Options;
using IpDeputyApi.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace IpDeputyApi.Services;

public class AppClock : IAppClock
{
    private readonly TimeProvider _timeProvider;
    private readonly TimeZoneInfo _timeZone;

    public AppClock(TimeProvider timeProvider, IOptions<ScheduleOptions> options)
    {
        _timeProvider = timeProvider;
        _timeZone = ResolveTimeZone(options.Value.TimeZone);
    }

    public DateTime UtcNow => _timeProvider.GetUtcNow().UtcDateTime;

    public DateOnly Today => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(UtcNow, _timeZone));

    private static TimeZoneInfo ResolveTimeZone(string id)
    {
        foreach (var candidate in new[] { id, "Europe/Kyiv", "Europe/Kiev", "FLE Standard Time" })
        {
            if (TimeZoneInfo.TryFindSystemTimeZoneById(candidate, out var timeZone))
                return timeZone;
        }

        return TimeZoneInfo.Utc;
    }
}
