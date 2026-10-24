using IpDeputyApi.Authorization;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend:AdditionalCouples Controller")]
[Route("frontend/additional-couple")]
[RequirePermission(Permissions.ManageSchedule)]
public class AdditionalCouplesController : CrudControllerBase<AdditionalCoupleDto, AdditionalCoupleFilter>
{
    public AdditionalCouplesController(IAdditionalCoupleService service) : base(service)
    {
    }
}
