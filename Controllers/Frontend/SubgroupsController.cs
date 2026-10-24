using IpDeputyApi.Authorization;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Paging;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend:Subgroups Controller")]
[Route("frontend/subgroup")]
[RequirePermission(Permissions.ManageStudents)]
public class SubgroupsController : CrudControllerBase<SubgroupDto, PageRequest>
{
    public SubgroupsController(ISubgroupService service) : base(service)
    {
    }
}
