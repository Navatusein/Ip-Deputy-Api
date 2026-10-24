using IpDeputyApi.Paging;

namespace IpDeputyApi.Dto.Frontend;

public class StudentFilter : PageRequest
{
    public int? SubgroupId { get; set; }
}

public class CoupleFilter : PageRequest
{
    public int? DayOfWeekId { get; set; }
    public int? SubjectId { get; set; }
    public int? TeacherId { get; set; }
    public int? SubgroupId { get; set; }
}

public class AdditionalCoupleFilter : PageRequest
{
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public int? SubjectId { get; set; }
    public int? TeacherId { get; set; }
}

public class SubmissionsConfigFilter : PageRequest
{
    public int? SubgroupId { get; set; }
    public int? SubjectId { get; set; }
}

public class SubmissionStudentFilter : PageRequest
{
    public int? SubmissionsConfigId { get; set; }
    public int? StudentId { get; set; }
}
