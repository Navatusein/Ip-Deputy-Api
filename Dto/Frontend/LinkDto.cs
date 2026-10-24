using System.ComponentModel.DataAnnotations;
using IpDeputyApi.Exceptions;

namespace IpDeputyApi.Dto.Frontend;

public class LinkDto : IHasId
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(256, ErrorMessage = ValidationMessages.TooLong)]
    public string Name { get; set; } = null!;

    [Required(AllowEmptyStrings = true, ErrorMessage = ValidationMessages.Required), MaxLength(2048, ErrorMessage = ValidationMessages.TooLong)]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(2048, ErrorMessage = ValidationMessages.TooLong)]
    public string Url { get; set; } = null!;
}
