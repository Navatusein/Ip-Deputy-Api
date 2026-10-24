namespace IpDeputyApi.Database.Models;

public class SubmissionsConfig : IEntity
{
    public int Id { get; set; }
    public int? SubgroupId { get; set; }
    public int? SubjectId { get; set; }
    public int? SubjectTypeId { get; set; }
    public string? CustomType { get; set; }
    public string? CustomName { get; set; }

    public Subgroup? Subgroup { get; set; }
    public Subject? Subject { get; set; }
    public SubjectType? SubjectType { get; set; }
    public ICollection<SubmissionWork> SubmissionWorks { get; set; } = new List<SubmissionWork>();
    public ICollection<SubmissionStudent> SubmissionStudents { get; set; } = new List<SubmissionStudent>();
}
