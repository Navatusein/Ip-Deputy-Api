using IpDeputyApi.Authorization;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Paging;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend:Links Controller")]
[Route("frontend/link")]
[RequirePermission(Permissions.ManageLinks)]
public class LinksController : CrudControllerBase<LinkDto, PageRequest>
{
    public LinksController(ILinkService service) : base(service)
    {
    }
}
