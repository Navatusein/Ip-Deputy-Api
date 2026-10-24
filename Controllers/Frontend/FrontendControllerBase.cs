using IpDeputyApi.Authorization;
using IpDeputyApi.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

[ApiController]
[Authorize]
public abstract class FrontendControllerBase : ControllerBase
{
    protected int ResolveStudentId(int? requestedStudentId, string adminPermission)
    {
        if (requestedStudentId != null)
        {
            if (User.GetStudentId() == requestedStudentId || User.HasPermission(adminPermission))
                return requestedStudentId.Value;

            throw new ForbiddenException();
        }

        return User.GetStudentId() ?? throw new ValidationException("studentId", "Не вказано студента");
    }
}
