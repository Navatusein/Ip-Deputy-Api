using System.Security.Claims;
using IpDeputyApi.Authorization;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Options;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IpDeputyApi.Services.Auth;

public class AuthService : IAuthService
{
    public const int MinPasswordLength = 8;

    private const string InvalidCredentialsMessage = "Невірний логін або пароль";
    private const string InvalidSessionMessage = "Сесія завершилась, увійдіть знову";

    private readonly IRepository<User> _users;
    private readonly IRepository<Student> _students;
    private readonly ITelegramRepository _telegrams;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly IUserSessionValidator _sessionValidator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppClock _clock;
    private readonly BotOptions _botOptions;

    public AuthService(
        IRepository<User> users,
        IRepository<Student> students,
        ITelegramRepository telegrams,
        IPasswordService passwordService,
        ITokenService tokenService,
        IUserSessionValidator sessionValidator,
        IUnitOfWork unitOfWork,
        IAppClock clock,
        IOptions<BotOptions> botOptions)
    {
        _users = users;
        _students = students;
        _telegrams = telegrams;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _sessionValidator = sessionValidator;
        _unitOfWork = unitOfWork;
        _clock = clock;
        _botOptions = botOptions.Value;
    }

    public async Task<AuthResult> LoginAsync(LoginDto dto, CancellationToken cancellationToken)
    {
        var login = UserService.NormalizeLogin(dto.Login);
        var user = await _users.QueryTracked().FirstOrDefaultAsync(x => x.Login == login, cancellationToken);

        if (user == null)
        {
            _passwordService.Verify(new User { PasswordHash = DummyHash.Value }, dto.Password);
            throw new HttpException(InvalidCredentialsMessage, StatusCodes.Status400BadRequest);
        }

        var result = _passwordService.Verify(user, dto.Password);

        if (result == PasswordCheckResult.Failed || !user.IsActive)
            throw new HttpException(InvalidCredentialsMessage, StatusCodes.Status400BadRequest);

        if (result == PasswordCheckResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordService.Hash(user, dto.Password);
            user.LegacyPasswordSalt = null;
        }

        user.LastLoginAt = _clock.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResult(ToAuthUser(user, _tokenService.CreateAdminAccessToken(user)), _tokenService.CreateRefreshToken(user));
    }

    public async Task<AuthResult> RefreshAsync(string? refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedException(InvalidSessionMessage);

        var data = await _tokenService.ReadRefreshTokenAsync(refreshToken)
            ?? throw new UnauthorizedException(InvalidSessionMessage);

        var user = await _users.Query().FirstOrDefaultAsync(x => x.Id == data.UserId, cancellationToken);

        if (user == null || !user.IsActive || user.TokenVersion != data.TokenVersion)
            throw new UnauthorizedException(InvalidSessionMessage);

        return new AuthResult(ToAuthUser(user, _tokenService.CreateAdminAccessToken(user)), _tokenService.CreateRefreshToken(user));
    }

    public async Task<AuthUserDto> LoginBotAsync(LoginBotDto dto, CancellationToken cancellationToken)
    {
        var telegramId = TelegramInitDataValidator.Validate(
            dto.InitData,
            _botOptions.BotToken,
            _clock.UtcNow,
            TimeSpan.FromHours(_botOptions.InitDataLifetimeHours))
            ?? throw new UnauthorizedException("Недійсні дані Telegram");

        var telegram = await _telegrams.GetByTelegramIdAsync(telegramId, cancellationToken)
            ?? throw new NotFoundException("Студента не знайдено");

        return new AuthUserDto
        {
            UserId = null,
            StudentId = telegram.StudentId,
            UserName = telegram.Student.Name,
            Kind = UserKinds.Student,
            Permissions = [],
            JwtToken = _tokenService.CreateStudentAccessToken(telegram.Student)
        };
    }

    public async Task<string> ChangePasswordAsync(ClaimsPrincipal principal, ChangePasswordDto dto, CancellationToken cancellationToken)
    {
        var userId = principal.GetUserId() ?? throw new ForbiddenException();

        var user = await _users.FindAsync(userId, cancellationToken)
            ?? throw new UnauthorizedException(InvalidSessionMessage);

        if (_passwordService.Verify(user, dto.CurrentPassword) == PasswordCheckResult.Failed)
            throw new ValidationException("currentPassword", "Невірний поточний пароль");

        if (dto.NewPassword.Length < MinPasswordLength)
            throw new ValidationException("newPassword", $"Пароль має містити щонайменше {MinPasswordLength} символів");

        user.PasswordHash = _passwordService.Hash(user, dto.NewPassword);
        user.LegacyPasswordSalt = null;
        user.TokenVersion++;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _sessionValidator.Invalidate(user.Id);

        return _tokenService.CreateRefreshToken(user);
    }

    public async Task<AuthUserDto> GetMeAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        if (principal.IsAdmin())
        {
            var userId = principal.GetUserId() ?? throw new UnauthorizedException();

            var user = await _users.Query().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
                ?? throw new UnauthorizedException(InvalidSessionMessage);

            return ToAuthUser(user, null);
        }

        var studentId = principal.GetStudentId() ?? throw new UnauthorizedException();

        var student = await _students.Query().FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken)
            ?? throw new UnauthorizedException(InvalidSessionMessage);

        return new AuthUserDto
        {
            StudentId = student.Id,
            UserName = student.Name,
            Kind = UserKinds.Student,
            Permissions = []
        };
    }

    private static AuthUserDto ToAuthUser(User user, string? jwtToken)
    {
        return new AuthUserDto
        {
            UserId = user.Id,
            StudentId = user.StudentId,
            UserName = user.DisplayName,
            Kind = UserKinds.Admin,
            Permissions = user.Permissions.Distinct().ToList(),
            JwtToken = jwtToken
        };
    }

    private static class DummyHash
    {
        public static readonly string Value = new Microsoft.AspNetCore.Identity.PasswordHasher<User>().HashPassword(new User(), Guid.NewGuid().ToString());
    }
}
