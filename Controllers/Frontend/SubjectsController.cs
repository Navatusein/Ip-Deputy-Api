using IpDeputyApi.Authorization;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Paging;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend:Subjects Controller")]
[Route("frontend/subject")]
[RequirePermission(Permissions.ManageSubjects)]
public class SubjectsController : CrudControllerBase<SubjectDto, PageRequest>
{
    public SubjectsController(ISubjectService service) : base(service)
    {
    }
}
