using IpDeputyApi.Authorization;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Paging;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend:SubjectTypes Controller")]
[Route("frontend/subject-type")]
[RequirePermission(Permissions.ManageSubjects)]
public class SubjectTypesController : CrudControllerBase<SubjectTypeDto, PageRequest>
{
    public SubjectTypesController(ISubjectTypeService service) : base(service)
    {
    }
}
