using System.ComponentModel.DataAnnotations;
using IpDeputyApi.Exceptions;

namespace IpDeputyApi.Dto.Frontend;

public class TeacherDto : IHasId
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(128, ErrorMessage = ValidationMessages.TooLong)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(128, ErrorMessage = ValidationMessages.TooLong)]
    public string Surname { get; set; } = null!;

    [Required(AllowEmptyStrings = true, ErrorMessage = ValidationMessages.Required), MaxLength(128, ErrorMessage = ValidationMessages.TooLong)]
    public string Patronymic { get; set; } = null!;

    [MaxLength(32, ErrorMessage = ValidationMessages.TooLong)]
    public string? ContactPhone { get; set; }

    [MaxLength(256, ErrorMessage = ValidationMessages.TooLong)]
    public string? Email { get; set; }

    [MaxLength(256, ErrorMessage = ValidationMessages.TooLong)]
    public string? FitEmail { get; set; }

    [MaxLength(128, ErrorMessage = ValidationMessages.TooLong)]
    public string? TelegramNickname { get; set; }
}
