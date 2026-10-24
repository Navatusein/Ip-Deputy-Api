namespace IpDeputyApi.Database.Models;

public class Couple : IEntity
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
    public string? Cabinet { get; set; }
    public string? AdditionalInformation { get; set; }
    public string? Link { get; set; }

    public Subject Subject { get; set; } = null!;
    public SubjectType SubjectType { get; set; } = null!;
    public DayOfWeek DayOfWeek { get; set; } = null!;
    public CoupleTime CoupleTime { get; set; } = null!;
    public Subgroup? Subgroup { get; set; }
    public Teacher Teacher { get; set; } = null!;
    public ICollection<CoupleDate> CoupleDates { get; set; } = new List<CoupleDate>();
}
