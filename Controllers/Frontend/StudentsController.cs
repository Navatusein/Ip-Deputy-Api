using IpDeputyApi.Authorization;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend:Students Controller")]
[Route("frontend/student")]
[RequirePermission(Permissions.ManageStudents)]
public class StudentsController : CrudControllerBase<StudentDto, StudentFilter>
{
    public StudentsController(IStudentService service) : base(service)
    {
    }
}
