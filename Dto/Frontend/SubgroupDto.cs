using System.ComponentModel.DataAnnotations;
using IpDeputyApi.Exceptions;

namespace IpDeputyApi.Dto.Frontend;

public class SubgroupDto : IHasId
{
    public int Id { get; set; }
    public int Index { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(128, ErrorMessage = ValidationMessages.TooLong)]
    public string Name { get; set; } = null!;
}
