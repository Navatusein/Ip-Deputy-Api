using System.Security.Claims;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Paging;

namespace IpDeputyApi.Services.Interfaces;

public record AuthResult(AuthUserDto User, string? RefreshToken);

public record RefreshTokenData(int UserId, int TokenVersion);

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginDto dto, CancellationToken cancellationToken);
    Task<AuthResult> RefreshAsync(string? refreshToken, CancellationToken cancellationToken);
    Task<AuthUserDto> LoginBotAsync(LoginBotDto dto, CancellationToken cancellationToken);
    Task<string> ChangePasswordAsync(ClaimsPrincipal principal, ChangePasswordDto dto, CancellationToken cancellationToken);
    Task<AuthUserDto> GetMeAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
}

public interface ITokenService
{
    string CreateAdminAccessToken(User user);
    string CreateStudentAccessToken(Student student);
    string CreateRefreshToken(User user);
    Task<RefreshTokenData?> ReadRefreshTokenAsync(string token);
    TimeSpan RefreshTokenLifetime { get; }
}

public interface IPasswordService
{
    string Hash(User user, string password);
    PasswordCheckResult Verify(User user, string password);
}

public enum PasswordCheckResult
{
    Failed,
    Success,
    SuccessRehashNeeded
}

public interface IUserSessionValidator
{
    Task<bool> ValidateAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
    void Invalidate(int userId);
}

public interface ICurrentUser
{
    ClaimsPrincipal Principal { get; }
    int? UserId { get; }
}

public interface IUserService : ICrudService<UserDto, PageRequest>
{
    IReadOnlyList<PermissionDto> GetPermissions();
}
