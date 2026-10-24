namespace IpDeputyApi.Database.Models;

public class Telegram : IEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public ulong TelegramId { get; set; }
    public string Language { get; set; } = null!;
    public bool ScheduleCompact { get; set; }
    public DateOnly? LastCongratulations { get; set; }
    public DateTime? LastActivity { get; set; }

    public Student Student { get; set; } = null!;
}
