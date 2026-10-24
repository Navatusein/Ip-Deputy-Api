using System.ComponentModel.DataAnnotations;
using IpDeputyApi.Exceptions;

namespace IpDeputyApi.Dto.Frontend;

public class CoupleDto : IHasId
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int SubjectTypeId { get; set; }
    public int DayOfWeekId { get; set; }
    public int CoupleTimeId { get; set; }
    public int? SubgroupId { get; set; }
    public int TeacherId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsRolling { get; set; }

    [MaxLength(256, ErrorMessage = ValidationMessages.TooLong)]
    public string? Cabinet { get; set; }

    [MaxLength(2048, ErrorMessage = ValidationMessages.TooLong)]
    public string? AdditionalInformation { get; set; }

    [MaxLength(2048, ErrorMessage = ValidationMessages.TooLong)]
    public string? Link { get; set; }

    public List<CoupleDateDto> AdditionalDates { get; set; } = new();
    public List<CoupleDateDto> RemovedDates { get; set; } = new();
    public int? WeeksCount { get; set; }
}
