using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend:Lookups Controller")]
[Route("frontend")]
public class LookupsController : FrontendControllerBase
{
    private readonly ILookupService _lookupService;

    public LookupsController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet("couple-time/all")]
    public async Task<ActionResult<IReadOnlyList<CoupleTimeDto>>> GetCoupleTimesAsync(CancellationToken cancellationToken)
    {
        return Ok(await _lookupService.GetCoupleTimesAsync(cancellationToken));
    }

    [HttpGet("day-of-week/all")]
    public async Task<ActionResult<IReadOnlyList<DayOfWeekDto>>> GetDaysOfWeekAsync(CancellationToken cancellationToken)
    {
        return Ok(await _lookupService.GetDaysOfWeekAsync(cancellationToken));
    }
}
