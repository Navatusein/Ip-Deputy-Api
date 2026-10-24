using System.Security.Claims;
using IpDeputyApi.Authorization;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace IpDeputyApi.Services.Auth;

public class UserSessionValidator : IUserSessionValidator
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

    private readonly IRepository<User> _users;
    private readonly IMemoryCache _cache;

    public UserSessionValidator(IRepository<User> users, IMemoryCache cache)
    {
        _users = users;
        _cache = cache;
    }

    public async Task<bool> ValidateAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        if (!principal.IsAdmin())
            return principal.GetKind() == UserKinds.Student;

        var userId = principal.GetUserId();

        if (userId == null || !int.TryParse(principal.FindFirstValue(AppClaims.TokenVersion), out var tokenVersion))
            return false;

        var state = await _cache.GetOrCreateAsync(CacheKey(userId.Value), entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;

            return _users.Query()
                .Where(x => x.Id == userId)
                .Select(x => new UserSessionState(x.IsActive, x.TokenVersion, x.Permissions))
                .FirstOrDefaultAsync(cancellationToken);
        });

        if (state == null || !state.IsActive || state.TokenVersion != tokenVersion)
            return false;

        if (principal.Identity is ClaimsIdentity identity)
        {
            foreach (var claim in identity.FindAll(AppClaims.Permission).ToList())
                identity.RemoveClaim(claim);

            identity.AddClaims(state.Permissions.Distinct().Select(x => new Claim(AppClaims.Permission, x)));
        }

        return true;
    }

    public void Invalidate(int userId)
    {
        _cache.Remove(CacheKey(userId));
    }

    private static string CacheKey(int userId)
    {
        return $"user-session:{userId}";
    }

    private sealed record UserSessionState(bool IsActive, int TokenVersion, List<string> Permissions);
}
