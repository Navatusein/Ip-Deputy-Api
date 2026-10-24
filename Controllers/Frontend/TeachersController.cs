using IpDeputyApi.Authorization;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Paging;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend:Teachers Controller")]
[Route("frontend/teacher")]
[RequirePermission(Permissions.ManageTeachers)]
public class TeachersController : CrudControllerBase<TeacherDto, PageRequest>
{
    public TeachersController(ITeacherService service) : base(service)
    {
    }
}
