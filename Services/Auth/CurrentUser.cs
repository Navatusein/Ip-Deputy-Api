using System.Security.Claims;
using IpDeputyApi.Authorization;
using IpDeputyApi.Services.Interfaces;

namespace IpDeputyApi.Services.Auth;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal Principal => _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    public int? UserId => Principal.GetUserId();
}
