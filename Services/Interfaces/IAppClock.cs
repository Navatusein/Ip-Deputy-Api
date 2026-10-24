namespace IpDeputyApi.Services.Interfaces;

public interface IAppClock
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}
