﻿namespace IpDeputyApi.Database.Models
{
    public class Couple
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

        public virtual Subject Subject { get; set; } = null!;
        public virtual SubjectType SubjectType { get; set; } = null!;
        public virtual DayOfWeek DayOfWeek { get; set; } = null!;
        public virtual CoupleTime CoupleTime { get; set; } = null!;
        public virtual Subgroup? Subgroup { get; set; }
        public virtual Teacher Teacher { get; set; } = null!;
        public virtual IEnumerable<CoupleDate> CoupleDates { get; } = new List<CoupleDate>();
    }
}
