using System.ComponentModel.DataAnnotations;
using IpDeputyApi.Exceptions;

namespace IpDeputyApi.Dto.Frontend;

public class AdditionalCoupleDto : IHasId
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int SubjectTypeId { get; set; }
    public int DayOfWeekId { get; set; }
    public int? CoupleTimeId { get; set; }
    public int? SubgroupId { get; set; }
    public int TeacherId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly? Time { get; set; }

    [MaxLength(256, ErrorMessage = ValidationMessages.TooLong)]
    public string? Cabinet { get; set; }

    [MaxLength(2048, ErrorMessage = ValidationMessages.TooLong)]
    public string? AdditionalInformation { get; set; }

    [MaxLength(2048, ErrorMessage = ValidationMessages.TooLong)]
    public string? Link { get; set; }
}
