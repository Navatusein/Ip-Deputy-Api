using IpDeputyApi.Authorization;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend:Couples Controller")]
[Route("frontend/couple")]
[RequirePermission(Permissions.ManageSchedule)]
public class CouplesController : CrudControllerBase<CoupleDto, CoupleFilter>
{
    public CouplesController(ICoupleService service) : base(service)
    {
    }
}
