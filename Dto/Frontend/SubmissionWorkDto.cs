using System.ComponentModel.DataAnnotations;
using IpDeputyApi.Exceptions;

namespace IpDeputyApi.Dto.Frontend;

public class SubmissionWorkDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(256, ErrorMessage = ValidationMessages.TooLong)]
    public string Name { get; set; } = null!;

    public int Index { get; set; }
}
