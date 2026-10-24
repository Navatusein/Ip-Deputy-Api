namespace IpDeputyApi.Database.Models;

public class SubmissionWork : IEntity
{
    public int Id { get; set; }
    public int SubmissionsConfigId { get; set; }
    public string Name { get; set; } = null!;
    public int Index { get; set; }

    public SubmissionsConfig SubmissionsConfig { get; set; } = null!;
    public ICollection<SubmissionStudent> SubmissionStudents { get; set; } = new List<SubmissionStudent>();
}
