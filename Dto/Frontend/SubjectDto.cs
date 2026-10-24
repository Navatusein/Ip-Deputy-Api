using System.ComponentModel.DataAnnotations;
using IpDeputyApi.Exceptions;

namespace IpDeputyApi.Dto.Frontend;

public class SubjectDto : IHasId
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(256, ErrorMessage = ValidationMessages.TooLong)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(64, ErrorMessage = ValidationMessages.TooLong)]
    public string ShortName { get; set; } = null!;

    [Range(0, 1000, ErrorMessage = ValidationMessages.InvalidValue)]
    public int LaboratoryCount { get; set; }

    [Range(0, 1000, ErrorMessage = ValidationMessages.InvalidValue)]
    public int PracticalCount { get; set; }
}
