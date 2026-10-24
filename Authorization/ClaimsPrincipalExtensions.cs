using System.Security.Claims;

namespace IpDeputyApi.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static string? GetKind(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(AppClaims.Kind);
    }

    public static bool IsAdmin(this ClaimsPrincipal principal)
    {
        return principal.GetKind() == UserKinds.Admin;
    }

    public static int? GetUserId(this ClaimsPrincipal principal)
    {
        if (!principal.IsAdmin())
            return null;

        return int.TryParse(principal.FindFirstValue(AppClaims.Subject), out var id) ? id : null;
    }

    public static int? GetStudentId(this ClaimsPrincipal principal)
    {
        return int.TryParse(principal.FindFirstValue(AppClaims.StudentId), out var id) ? id : null;
    }

    public static bool HasPermission(this ClaimsPrincipal principal, string permission)
    {
        return principal.IsAdmin() && principal.FindAll(AppClaims.Permission).Any(x => x.Value == permission);
    }

    public static IReadOnlyList<string> GetPermissions(this ClaimsPrincipal principal)
    {
        return principal.FindAll(AppClaims.Permission).Select(x => x.Value).Distinct().ToList();
    }
}
