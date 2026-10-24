using System.Linq.Expressions;
using System.Text.RegularExpressions;
using IpDeputyApi.Authorization;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Paging;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Auth;
using IpDeputyApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Services;

public partial class UserService : CrudServiceBase<User, UserDto, PageRequest>, IUserService
{
    private static readonly SortMap<User> SortMap = new SortMap<User>()
        .Add("login", x => x.Login)
        .Add("displayName", x => x.DisplayName)
        .Add("isActive", x => x.IsActive)
        .Add("lastLoginAt", x => x.LastLoginAt)
        .Add("studentId", x => x.StudentId)
        .Default(query => query.OrderBy(x => x.Login));

    private static readonly IReadOnlyList<Expression<Func<User, string?>>> Search = [x => x.Login, x => x.DisplayName];

    private readonly IRepository<Student> _students;
    private readonly IPasswordService _passwordService;
    private readonly IUserSessionValidator _sessionValidator;
    private readonly ICurrentUser _currentUser;
    private readonly IAppClock _clock;

    public UserService(
        IRepository<User> repository,
        IRepository<Student> students,
        IPasswordService passwordService,
        IUserSessionValidator sessionValidator,
        ICurrentUser currentUser,
        IAppClock clock,
        IUnitOfWork unitOfWork) : base(repository, unitOfWork)
    {
        _students = students;
        _passwordService = passwordService;
        _sessionValidator = sessionValidator;
        _currentUser = currentUser;
        _clock = clock;
    }

    [GeneratedRegex("^[a-z0-9._@-]{3,64}$")]
    private static partial Regex LoginRegex();

    protected override SortMap<User> Sorts => SortMap;
    protected override IReadOnlyList<Expression<Func<User, string?>>> SearchFields => Search;
    protected override string NotFoundMessage => "Користувача не знайдено";

    public static string NormalizeLogin(string? login)
    {
        return (login ?? "").Trim().ToLowerInvariant();
    }

    public IReadOnlyList<PermissionDto> GetPermissions()
    {
        return Permissions.All.Select(x => new PermissionDto { Key = x.Key, Name = x.Name }).ToList();
    }

    protected override IQueryable<UserDto> Project(IQueryable<User> query)
    {
        return query.Select(x => new UserDto
        {
            Id = x.Id,
            Login = x.Login,
            DisplayName = x.DisplayName,
            StudentId = x.StudentId,
            Permissions = x.Permissions,
            IsActive = x.IsActive,
            LastLoginAt = x.LastLoginAt
        });
    }

    protected override void Apply(UserDto dto, User entity)
    {
        var isNew = entity.Id == 0;
        var deactivated = !isNew && entity.IsActive && !dto.IsActive;

        entity.Login = dto.Login;
        entity.DisplayName = dto.DisplayName;
        entity.StudentId = dto.StudentId;
        entity.Permissions = dto.Permissions;
        entity.IsActive = dto.IsActive;

        if (isNew)
            entity.CreatedAt = _clock.UtcNow;

        if (!string.IsNullOrEmpty(dto.Password))
        {
            entity.PasswordHash = _passwordService.Hash(entity, dto.Password);
            entity.LegacyPasswordSalt = null;

            if (!isNew)
                entity.TokenVersion++;
        }
        else if (deactivated)
        {
            entity.TokenVersion++;
        }
    }

    protected override void Normalize(UserDto dto)
    {
        dto.Login = NormalizeLogin(dto.Login);
        dto.DisplayName = TextNormalizer.Clean(dto.DisplayName);
        dto.Permissions = dto.Permissions.Select(x => x.Trim()).Distinct().ToList();

        if (string.IsNullOrEmpty(dto.Password))
            dto.Password = null;
    }

    protected override async Task ValidateAsync(UserDto dto, User? existing, CancellationToken cancellationToken)
    {
        var errors = new ValidationErrors();

        if (!LoginRegex().IsMatch(dto.Login))
            errors.Add("login", "Логін має містити 3-64 символи: латинські літери, цифри, . _ @ -");

        if (dto.DisplayName.Length == 0)
            errors.Add("displayName", ValidationMessages.Required);

        if (existing == null && dto.Password == null)
            errors.Add("password", ValidationMessages.Required);

        if (dto.Password != null && dto.Password.Length < AuthService.MinPasswordLength)
            errors.Add("password", $"Пароль має містити щонайменше {AuthService.MinPasswordLength} символів");

        if (dto.Permissions.Any(x => !Permissions.IsKnown(x)))
            errors.Add("permissions", ValidationMessages.InvalidValue);

        await EnsureExistsAsync(_students, dto.StudentId, "studentId", errors, cancellationToken);

        if (existing != null && existing.Id == _currentUser.UserId)
        {
            if (!dto.IsActive)
                errors.Add("isActive", "Не можна деактивувати власний обліковий запис");

            if (!dto.Permissions.Contains(Permissions.ManageUsers))
                errors.Add("permissions", "Не можна забрати у себе право керування користувачами");
        }

        errors.ThrowIfAny();

        var existingId = existing?.Id ?? 0;

        if (await Repository.Query().AnyAsync(x => x.Id != existingId && x.Login == dto.Login, cancellationToken))
            throw new ConflictException("Користувач з таким логіном уже існує");
    }

    public override async Task<UserDto> UpdateAsync(UserDto dto, CancellationToken cancellationToken)
    {
        var result = await base.UpdateAsync(dto, cancellationToken);
        _sessionValidator.Invalidate(result.Id);
        return result;
    }

    protected override async Task ValidateDeleteAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId != null && ids.Contains(_currentUser.UserId.Value))
            throw new ConflictException("Не можна видалити власний обліковий запис");

        var remainingManagers = await Repository.Query()
            .CountAsync(x => !ids.Contains(x.Id) && x.IsActive && x.Permissions.Contains(Permissions.ManageUsers), cancellationToken);

        if (remainingManagers == 0)
            throw new ConflictException("Має залишитися хоча б один користувач з правом керування користувачами");
    }

    public override async Task DeleteAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
    {
        await base.DeleteAsync(ids, cancellationToken);

        foreach (var id in ids)
            _sessionValidator.Invalidate(id);
    }
}
