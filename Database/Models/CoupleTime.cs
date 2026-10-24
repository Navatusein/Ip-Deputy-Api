namespace IpDeputyApi.Database.Models;

public class CoupleTime : IEntity
{
    public int Id { get; set; }
    public int Index { get; set; }
    public TimeOnly TimeStart { get; set; }
    public TimeOnly TimeEnd { get; set; }

    public ICollection<Couple> Couples { get; set; } = new List<Couple>();

    public string GetTimeFormatted()
    {
        return $"{TimeStart:HH:mm} - {TimeEnd:HH:mm}";
    }
}
