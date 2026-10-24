using System.ComponentModel.DataAnnotations;
using IpDeputyApi.Exceptions;

namespace IpDeputyApi.Dto.Frontend;

public class SubmissionsConfigDto : IHasId
{
    public int Id { get; set; }
    public int? SubgroupId { get; set; }
    public int? SubjectId { get; set; }
    public int? SubjectTypeId { get; set; }

    [MaxLength(128, ErrorMessage = ValidationMessages.TooLong)]
    public string? CustomType { get; set; }

    [MaxLength(256, ErrorMessage = ValidationMessages.TooLong)]
    public string? CustomName { get; set; }

    public List<SubmissionWorkDto> SubmissionWorks { get; set; } = new();
}
