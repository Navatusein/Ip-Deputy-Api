using System.ComponentModel.DataAnnotations;
using IpDeputyApi.Exceptions;

namespace IpDeputyApi.Dto.Frontend;

public class SubjectTypeDto : IHasId
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(128, ErrorMessage = ValidationMessages.TooLong)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(32, ErrorMessage = ValidationMessages.TooLong)]
    public string ShortName { get; set; } = null!;
}
