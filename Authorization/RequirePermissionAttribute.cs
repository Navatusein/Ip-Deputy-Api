using IpDeputyApi.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IpDeputyApi.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
public class RequirePermissionAttribute : Attribute, IAuthorizationFilter
{
    public string Permission { get; }

    public RequirePermissionAttribute(string permission)
    {
        Permission = permission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var metadata = context.ActionDescriptor.EndpointMetadata;

        if (metadata.OfType<SkipPermissionAttribute>().Any() || metadata.OfType<IAllowAnonymous>().Any())
            return;

        var effective = metadata.OfType<RequirePermissionAttribute>().LastOrDefault();

        if (!ReferenceEquals(effective, this))
            return;

        var user = context.HttpContext.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            context.Result = new ObjectResult(new ErrorDto(StatusCodes.Status401Unauthorized, "Не авторизовано"))
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        if (!user.HasPermission(Permission))
        {
            context.Result = new ObjectResult(new ErrorDto(StatusCodes.Status403Forbidden, "Недостатньо прав"))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
