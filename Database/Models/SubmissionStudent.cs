namespace IpDeputyApi.Database.Models;

public enum PreferredPosition
{
    InBegin,
    DoesNotMatter,
    InEnd
}

public class SubmissionStudent : IEntity
{
    public int Id { get; set; }
    public int SubmissionWorkId { get; set; }
    public int StudentId { get; set; }
    public int SubmissionsConfigId { get; set; }
    public PreferredPosition PreferredPosition { get; set; }
    public DateOnly SubmittedAt { get; set; }

    public Student Student { get; set; } = null!;
    public SubmissionWork SubmissionWork { get; set; } = null!;
    public SubmissionsConfig SubmissionsConfig { get; set; } = null!;
}
