using System.ComponentModel.DataAnnotations;
using IpDeputyApi.Exceptions;

namespace IpDeputyApi.Dto.Frontend;

public class TelegramDto : IHasId
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public ulong TelegramId { get; set; }

    [MaxLength(8, ErrorMessage = ValidationMessages.TooLong)]
    public string? Language { get; set; }

    public bool? ScheduleCompact { get; set; }
    public DateTime? LastActivity { get; set; }
}
