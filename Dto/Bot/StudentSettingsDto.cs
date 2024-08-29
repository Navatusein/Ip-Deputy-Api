namespace IpDeputyApi.Dto.Bot;

public class StudentSettingsDto
{
    public ulong TelegramId { get; set; }
    public string Language { get; set; } = null!;
    public bool ScheduleCompact { get; set; }
}