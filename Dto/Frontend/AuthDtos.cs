using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using IpDeputyApi.Exceptions;

namespace IpDeputyApi.Dto.Frontend;

public class LoginDto
{
    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(64, ErrorMessage = ValidationMessages.TooLong)]
    public string Login { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(256, ErrorMessage = ValidationMessages.TooLong)]
    public string Password { get; set; } = null!;
}

public class LoginBotDto
{
    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(8192, ErrorMessage = ValidationMessages.TooLong)]
    public string InitData { get; set; } = null!;
}

public class ChangePasswordDto
{
    [Required(ErrorMessage = ValidationMessages.Required)]
    public string CurrentPassword { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(256, ErrorMessage = ValidationMessages.TooLong)]
    public string NewPassword { get; set; } = null!;
}

public class AuthUserDto
{
    public int? UserId { get; set; }
    public int? StudentId { get; set; }
    public string UserName { get; set; } = null!;
    public string Kind { get; set; } = null!;
    public IReadOnlyList<string> Permissions { get; set; } = [];

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? JwtToken { get; set; }
}

public class PermissionDto
{
    public string Key { get; set; } = null!;
    public string Name { get; set; } = null!;
}

public class UserDto : IHasId
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(64, ErrorMessage = ValidationMessages.TooLong)]
    public string Login { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(256, ErrorMessage = ValidationMessages.TooLong)]
    public string DisplayName { get; set; } = null!;

    public int? StudentId { get; set; }
    public List<string> Permissions { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    [MaxLength(256, ErrorMessage = ValidationMessages.TooLong)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Password { get; set; }
}
